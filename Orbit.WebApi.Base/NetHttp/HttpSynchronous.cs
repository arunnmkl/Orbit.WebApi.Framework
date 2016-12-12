using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Orbit.WebApi.Base.NetHttp.Serializer;

namespace Orbit.WebApi.Base.NetHttp
{
    /// <summary>
    /// Class HttpSynchronous.
    /// </summary>
    public class HttpSynchronous : HttpHelper
    {
        #region Get

        /// <summary>
        /// Gets the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <returns>System.String.</returns>
        public static string Get(string uri, CookieContainer cookieContainer = null)
        {
            string responseContent;
            string responseContentType;
            string responseContentDisposition;

            Get(uri, cookieContainer, out responseContentType, out responseContentDisposition, out responseContent);

            return responseContent;
        }

        /// <summary>
        /// Gets the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="responseContentType">Type of the response content.</param>
        /// <param name="responseContentDisposition">The response content disposition.</param>
        /// <param name="responseContent">Content of the response.</param>
        public static void Get(string uri, CookieContainer cookieContainer, out string responseContentType, out string responseContentDisposition, out string responseContent)
        {
            using (var mStream = new MemoryStream())
            {
                Get(uri, cookieContainer, mStream, out responseContentType, out responseContentDisposition);

                using (var reader = new StreamReader(mStream))
                {
                    responseContent = reader.ReadToEnd();
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Gets the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="responseContentStream">The response content stream.</param>
        /// <param name="responseContentType">Type of the response content.</param>
        /// <param name="responseContentDisposition">The response content disposition.</param>
        public static void Get(string uri, CookieContainer cookieContainer, Stream responseContentStream, out string responseContentType, out string responseContentDisposition)
        {
            MakeSynchronousRequest(HttpMethod.Get, uri, cookieContainer, null, null, null, null, responseContentStream, out responseContentType, out responseContentDisposition);
        }

        /// <summary>
        /// Gets the specified URI.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>TResult.</returns>
        public static TResult Get<TResult>(string uri, CookieContainer cookieContainer = null, string contentType = HttpContentType.Json)
        {
            return MakeSynchronousRequest<TResult>(HttpMethod.Get, uri, cookieContainer, null, contentType, null, null);
        }

        #endregion Get

        #region Post

        /// <summary>
        /// Posts the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptTypes">The accept types.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <param name="responseContentStream">The response content stream.</param>
        /// <param name="responseContentType">Type of the response content.</param>
        /// <param name="responseContentDisposition">The response content disposition.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <param name="deferCredentials">if set to <c>true</c> [defer credentials].</param>
        public static void Post(
            string uri,
            CookieContainer cookieContainer,
            string acceptTypes,
            string requestContentType,
            string requestContentDisposition,
            Stream requestContentStream,
            Stream responseContentStream,
            out string responseContentType,
            out string responseContentDisposition,
            int millisecondsTimeout = System.Threading.Timeout.Infinite,
            bool deferCredentials = true)
        {
            InnerMakeSynchronousRequest(HttpMethod.Post, uri, cookieContainer, acceptTypes, requestContentType, requestContentDisposition,
                                        requestContentStream, responseContentStream, millisecondsTimeout, null, deferCredentials,
                                        out responseContentDisposition, out responseContentType);
        }

        /// <summary>
        /// Posts the specified URI.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <returns>TResult.</returns>
        public static TResult Post<TResult>(string uri, CookieContainer cookieContainer, string acceptContentType, string requestContentType, string requestContentDisposition, Stream requestContentStream)
        {
            return MakeSynchronousRequest<TResult>(HttpMethod.Post, uri, cookieContainer, acceptContentType, requestContentType, requestContentDisposition, requestContentStream);
        }

        /// <summary>
        /// Posts the specified URI.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="content">The content.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <returns>TResult.</returns>
        public static TResult Post<TResult>(string uri, object content, string acceptContentType = HttpContentType.Json, string requestContentType = HttpContentType.Json, CookieContainer cookieContainer = null)
        {
            var buffer = SerializationHelper.Serialize(content, requestContentType);
            return MakeSynchronousRequest<TResult>(HttpMethod.Post, uri, cookieContainer, acceptContentType, requestContentType, null, buffer == null ? null : new MemoryStream(buffer));
        }

        #endregion Post

        #region PUT

        /// <summary>
        /// Puts the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptTypes">The accept types.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <param name="responseContentStream">The response content stream.</param>
        /// <param name="responseContentType">Type of the response content.</param>
        /// <param name="responseContentDisposition">The response content disposition.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <param name="deferCredentials">if set to <c>true</c> [defer credentials].</param>
        public static void Put(
            string uri,
            CookieContainer cookieContainer,
            string acceptTypes,
            string requestContentType,
            string requestContentDisposition,
            Stream requestContentStream,
            Stream responseContentStream,
            out string responseContentType,
            out string responseContentDisposition,
            int millisecondsTimeout = System.Threading.Timeout.Infinite,
            bool deferCredentials = true)
        {
            InnerMakeSynchronousRequest(HttpMethod.Put, uri, cookieContainer, acceptTypes, requestContentType, requestContentDisposition,
                                        requestContentStream, responseContentStream, millisecondsTimeout, null, deferCredentials,
                                        out responseContentDisposition, out responseContentType);
        }

        /// <summary>
        /// Puts the specified URI.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <returns>TResult.</returns>
        public static TResult Put<TResult>(string uri, CookieContainer cookieContainer, string acceptContentType, string requestContentType, string requestContentDisposition, Stream requestContentStream)
        {
            return MakeSynchronousRequest<TResult>(HttpMethod.Put, uri, cookieContainer, acceptContentType, requestContentType, requestContentDisposition, requestContentStream);
        }

        /// <summary>
        /// Puts the specified URI.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="content">The content.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <returns>TResult.</returns>
        public static TResult Put<TResult>(string uri, object content, string acceptContentType = HttpContentType.Json, string requestContentType = HttpContentType.Json, CookieContainer cookieContainer = null)
        {
            var buffer = SerializationHelper.Serialize(content, requestContentType);
            return MakeSynchronousRequest<TResult>(HttpMethod.Put, uri, cookieContainer, acceptContentType, requestContentType, null, buffer == null ? null : new MemoryStream(buffer));
        }

        #endregion PUT

        #region DELETE

        /// <summary>
        /// Deletes the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptTypes">The accept types.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <param name="responseContentStream">The response content stream.</param>
        /// <param name="responseContentType">Type of the response content.</param>
        /// <param name="responseContentDisposition">The response content disposition.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <param name="deferCredentials">if set to <c>true</c> [defer credentials].</param>
        public static void Delete(
            string uri,
            CookieContainer cookieContainer,
            string acceptTypes,
            string requestContentType,
            string requestContentDisposition,
            Stream requestContentStream,
            Stream responseContentStream,
            out string responseContentType,
            out string responseContentDisposition,
            int millisecondsTimeout = System.Threading.Timeout.Infinite,
            bool deferCredentials = true)
        {
            InnerMakeSynchronousRequest(HttpMethod.Delete, uri, cookieContainer, acceptTypes, requestContentType, requestContentDisposition,
                                        requestContentStream, responseContentStream, millisecondsTimeout, null, deferCredentials,
                                        out responseContentDisposition, out responseContentType);
        }

        /// <summary>
        /// Deletes the specified URI.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <returns>TResult.</returns>
        public static TResult Delete<TResult>(string uri, CookieContainer cookieContainer, string acceptContentType, string requestContentType, string requestContentDisposition, Stream requestContentStream)
        {
            return MakeSynchronousRequest<TResult>(HttpMethod.Delete, uri, cookieContainer, acceptContentType, requestContentType, requestContentDisposition, requestContentStream);
        }

        /// <summary>
        /// Deletes the specified URI.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="uri">The URI.</param>
        /// <param name="content">The content.</param>
        /// <param name="acceptContentType">Type of the accept content.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <returns>TResult.</returns>
        public static TResult Delete<TResult>(string uri, object content, string acceptContentType = HttpContentType.Json, string requestContentType = HttpContentType.Json, CookieContainer cookieContainer = null)
        {
            var buffer = SerializationHelper.Serialize(content, requestContentType);
            return MakeSynchronousRequest<TResult>(HttpMethod.Delete, uri, cookieContainer, acceptContentType, requestContentType, null, buffer == null ? null : new MemoryStream(buffer));
        }

        #endregion DELETE

        /// <summary>
        /// Makes the synchronous request.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptTypes">The accept types.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <param name="responseContentStream">The response content stream.</param>
        /// <param name="responseContentType">Type of the response content.</param>
        /// <param name="responseContentDisposition">The response content disposition.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <param name="deferCredentials">if set to <c>true</c> [defer credentials].</param>
        public static void MakeSynchronousRequest(
            string method,
            string uri,
            CookieContainer cookieContainer,
            string acceptTypes,
            string requestContentType,
            string requestContentDisposition,
            Stream requestContentStream,
            Stream responseContentStream,
            out string responseContentType,
            out string responseContentDisposition,
            int millisecondsTimeout = System.Threading.Timeout.Infinite,
            bool deferCredentials = true)
        {
            InnerMakeSynchronousRequest(method, uri, cookieContainer, acceptTypes, requestContentType, requestContentDisposition,
                                        requestContentStream, responseContentStream, millisecondsTimeout, null, deferCredentials,
                                        out responseContentDisposition, out responseContentType);
        }

        /// <summary>
        /// Makes the synchronous request.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="method">The method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptTypes">The accept types.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <param name="authorizationHeader">The authorization header.</param>
        /// <returns>TResult.</returns>
        public static TResult MakeSynchronousRequest<TResult>(
            string method, string uri,
            CookieContainer cookieContainer, string acceptTypes,
            string requestContentType, string requestContentDisposition, Stream requestContentStream,
            int millisecondsTimeout = System.Threading.Timeout.Infinite,
            string authorizationHeader = null)
        {
            return InnerMakeSynchronousRequest<TResult>(method, uri, cookieContainer, acceptTypes, requestContentType, requestContentDisposition, requestContentStream, millisecondsTimeout, authorizationHeader);
        }

        /// <summary>
        /// Inners the make synchronous request.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
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
        /// <returns>TResult.</returns>
        private static TResult InnerMakeSynchronousRequest<TResult>(string method,
            string uri,
            CookieContainer cookieContainer,
            string acceptTypes,
            string requestContentType,
            string requestContentDisposition,
            Stream requestContentStream,
            int millisecondsTimeout,
            string authorizationHeader,
            bool deferCredentials = true)
        {
            TResult result;

            var requestStartTickCount = Environment.TickCount;

            // Generate HTTP Request
            var request = GenerateRequest(method, uri, cookieContainer, acceptTypes, requestContentType,
                                          requestContentDisposition, requestContentStream, millisecondsTimeout,
                                          authorizationHeader, deferCredentials);

            try
            {
                var webResponse = request.GetResponse();
                var contentType = request.ContentType;
                using (var stream = webResponse.GetResponseStream())
                {
                    result = SerializationHelper.Deserialize<TResult>(stream, webResponse.ContentLength, contentType);
                }
                webResponse.Close();

                var currentTickCount = Environment.TickCount;
                if (currentTickCount > requestStartTickCount)
                {   // Record processing time for request
                    RecordProcessingTime(currentTickCount - requestStartTickCount);
                }
            }
            catch (WebException webEx)
            {
                Debug.WriteLine("Web exception received: " + webEx.Message);

                // ParseExceptionFromResponse "round trips" the webEx parameter if there's an error (i.e. if webEx.Response as HttpWebResponse is null),
                // so we get the raw exception back if parsing fails
                Exception exception = ParseExceptionFromResponse(webEx.Response as HttpWebResponse, webEx);

                throw exception;
            }
            catch
            {
                // Re-throw other exceptions
                throw;
            }

            return result;
        }

        /// <summary>
        /// Inners the make synchronous request.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="cookieContainer">The cookie container.</param>
        /// <param name="acceptTypes">The accept types.</param>
        /// <param name="requestContentType">Type of the request content.</param>
        /// <param name="requestContentDisposition">The request content disposition.</param>
        /// <param name="requestContentStream">The request content stream.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="millisecondsTimeout">The milliseconds timeout.</param>
        /// <param name="authorizationHeader">The authorization header.</param>
        /// <param name="deferCredentials">if set to <c>true</c> [defer credentials].</param>
        /// <param name="responseContentDisposition">The response content disposition.</param>
        /// <param name="responseContentType">Type of the response content.</param>
        private static void InnerMakeSynchronousRequest(string method,
            string uri,
            CookieContainer cookieContainer,
            string acceptTypes,
            string requestContentType,
            string requestContentDisposition,
            Stream requestContentStream,
            Stream outputStream,
            int millisecondsTimeout,
            string authorizationHeader,
            bool deferCredentials,
            out string responseContentDisposition,
            out string responseContentType)
        {
            var requestStartTickCount = Environment.TickCount;

            // Generate HTTP Request
            var request = GenerateRequest(method, uri, cookieContainer, acceptTypes, requestContentType,
                                          requestContentDisposition, requestContentStream, millisecondsTimeout,
                                          authorizationHeader, deferCredentials);

            try
            {
                var webResponse = request.GetResponse();
                using (var stream = webResponse.GetResponseStream())
                {
                    if (stream != null)
                    {   // Copy response to output stream
                        stream.CopyTo(outputStream);

                        if (outputStream.CanSeek)
                        {   // Seek to start of stream, if possible
                            outputStream.Seek(0, SeekOrigin.Begin);
                        }
                    }
                }

                responseContentDisposition = webResponse.Headers["content-disposition"];
                responseContentType = webResponse.ContentType;

                webResponse.Close();

                var currentTickCount = Environment.TickCount;
                if (currentTickCount > requestStartTickCount)
                {   // Record processing time for request
                    RecordProcessingTime(currentTickCount - requestStartTickCount);
                }
            }
            catch (WebException webEx)
            {
                Debug.WriteLine("Web exception received: " + webEx.Message);

                // ParseExceptionFromResponse "round trips" the webEx parameter if there's an error (i.e. if webEx.Response as HttpWebResponse is null),
                // so we get the raw exception back if parsing fails
                Exception exception = ParseExceptionFromResponse(webEx.Response as HttpWebResponse, webEx);

                throw exception;
            }
            catch
            {
                // Re-throw other exceptions
                throw;
            }
        }
    }
}
