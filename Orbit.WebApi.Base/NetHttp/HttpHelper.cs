using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Orbit.WebApi.Base.NetHttp.ServiceException;

namespace Orbit.WebApi.Base.NetHttp
{
    /// <summary>
    /// Class HttpHelper.
    /// </summary>
    public class HttpHelper : HttpBase
    {
        /// <summary>
        /// Gets the full URI.
        /// </summary>
        /// <param name="partialUri">The partial URI.</param>
        /// <returns>Uri.</returns>
        public static Uri GetFullUri(string partialUri)
        {
            Uri fullUri;

            if (!Uri.TryCreate(partialUri, UriKind.Absolute, out fullUri))
            {
                fullUri = new Uri(string.Concat(UriBase ?? string.Empty, partialUri));
            }

            return fullUri;
        }

        /// <summary>
        /// Gets the basic authorization header.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>System.String.</returns>
        public static string GetBasicAuthorizationHeader(string userName, string password)
        {
            return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", userName, password)));
        }

        /// <summary>
        /// Gets the cookie value.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieName">Name of the cookie.</param>
        /// <returns>System.String.</returns>
        public static string GetCookieValue(string uri, string cookieName)
        {
            return DefaultCookieContainer.GetCookies(new Uri(uri))[cookieName].Value;
        }

        /// <summary>
        /// Generates the request.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptTypes">The accept types.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <param name="authorizationHeader">The authorization header.</param>
        /// <param name="deferCredentials">if set to <c>true</c> [defer credentials].</param>
        /// <returns>HttpWebRequest.</returns>
        protected static HttpWebRequest GenerateRequest(
            string method,
            string uri,
            CookieContainer cookieContainer,
            string acceptTypes,
            string requestContentType,
            string requestContentDisposition,
            Stream requestContentStream,
            int millisecondsTimeout,
            string authorizationHeader,
            bool deferCredentials)
        {
            var requestUri = GetFullUri(uri);
            var request = (HttpWebRequest)WebRequest.Create(requestUri);

            const bool canSetHeader = true;

            request.KeepAlive = !DisableHttpKeepAlive;
            request.KeepAlive = false;
            request.AllowWriteStreamBuffering = false;  // Prevent excessive use of Memory resulting in OutOfMemoryExceptions during heavy loads

            request.Method = method;

            // only send credentials if allow retry with credentials is false.  This means this is our only chance to send credentials.
            // make sure the username/password exist being attempting credential login.
            if (IsSendingCredentials(deferCredentials, requestUri, authorizationHeader))
            {
                if (!deferCredentials)
                {
                    request.Credentials = new NetworkCredential(_username, _password);
                }
                else
                {
                    if (canSetHeader)
                    {
                        request.Headers[HttpRequestHeader.Authorization] = authorizationHeader ?? GetBasicAuthorizationHeader(_username, _password);
                    }
                }
            }

            request.Accept = acceptTypes;
            request.ContentType = requestContentType;

            request.Timeout = millisecondsTimeout;
            request.ContentLength = requestContentStream == null ? 0 : requestContentStream.Length;

            if (canSetHeader) request.CookieContainer = cookieContainer ?? DefaultCookieContainer;

            if (requestContentDisposition != null)
            {
                request.Headers["content-disposition"] = requestContentDisposition;
            }

            if (_processingRecordCount > 0)
            {
                // Do not send statistics if there are none
                request.Headers[AvgClientProcessTimeHttpHeader] = Math.Round(_processingAverageTime, 0).ToString(CultureInfo.InvariantCulture);
            }

            if (requestContentStream != null)
            {
                using (Stream s = request.GetRequestStream())
                {
                    requestContentStream.CopyTo(s);
                }
            }

            return request;
        }

        /// <summary>
        /// Determines whether [is sending credentials] [the specified defer credentials].
        /// </summary>
        /// <param name="deferCredentials">if set to <c>true</c> [defer credentials].</param>
        /// <param name="uri">The URI.</param>
        /// <param name="authorizationCredentials">The authorization credentials.</param>
        /// <returns><c>true</c> if [is sending credentials] [the specified defer credentials]; otherwise, <c>false</c>.</returns>
        protected static bool IsSendingCredentials(bool deferCredentials, Uri uri, string authorizationCredentials)
        {
            if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_password) && Preauthenticate && !_hasSentCredentials && uri.AbsolutePath != "/locator")
            {
                _hasSentCredentials = true;
                return true;
            }
            return (!deferCredentials && !string.IsNullOrEmpty(authorizationCredentials));
        }

        /// <summary>
        /// Records the processing time.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        protected static void RecordProcessingTime(int milliseconds)
        {
            if (DisableProcessingStatisitcs)
            {   // Do perform statistic processing, if disabled
                _processingRecordCount = 0;
                return;
            }

            lock (ProcessingLockObject)
            {
                if (_processingRecordCount < 9)
                {
                    _processingAverageTime = (_processingAverageTime * _processingRecordCount + milliseconds) / ++_processingRecordCount;
                }
                else
                {
                    _processingAverageTime = (_processingAverageTime * 9 + milliseconds) / 10;
                }
            }
        }

        /// <summary>
        /// Parses the exception from response.
        /// </summary>
        /// <param name="webResponse">The web response.</param>
        /// <param name="webException">The web exception.</param>
        /// <returns>Exception.</returns>
        protected static Exception ParseExceptionFromResponse(HttpWebResponse webResponse, Exception webException)
        {
            try
            {
                var exRespStream = webResponse.GetResponseStream();

                var errorCode = webResponse.Headers["Error-Code"];
                if (errorCode != null)
                {
                    string content = string.Empty;

                    if (exRespStream != null)
                    {
                        long len = webResponse.ContentLength;

                        byte[] result = new byte[len];
                        int bytesRead = 0;
                        int blockSize = 8192;

                        while (bytesRead < len && exRespStream.CanRead)
                        {
                            blockSize = blockSize > len - bytesRead ? (int)(len - bytesRead) : blockSize;
                            bytesRead += exRespStream.Read(result, bytesRead, blockSize);
                        }

                        exRespStream.Close();

                        content = Encoding.UTF8.GetString(result, 0, result.Length);
                    }

                    return new WebServiceServerException(errorCode, content, webException);
                }
            }
            catch (Exception)
            {
            }

            return webException;
        }
    }
}