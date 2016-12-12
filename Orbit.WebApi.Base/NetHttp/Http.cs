using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Orbit.WebApi.Base.NetHttp.Serializer;
using Orbit.WebApi.Base.NetHttp.ServiceException;

namespace Orbit.WebApi.Base.NetHttp
{
    public class Http : HttpSynchronous
    {
        public static void SetCredentials(string username, string password)
        {
            _username = username;
            _password = password;
            _hasSentCredentials = false;
        }

        #region Asynchronous

        #region GET

        public static void GetAsync<TResult>(string uri, Action<TResult, Exception> callback,
            string acceptContentType = HttpContentType.Json)// where TResult: class
        {
            try
            {
                var state = new HttpResultContext<TResult>() { ResultCallback = callback };

                GetAsync<HttpResultContext<TResult>>(uri, DefaultCookieContainer, acceptContentType, ResultCallback<TResult>, state);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled Exception in GetAsync: " + ex.Message);
                callback(default(TResult), ex);
            }
        }

        public static void GetAsync<T>(string uri,
            CookieContainer cookieContainer, string contentType,
            Action<string, string, long, Stream, object, Exception> callback, T state)
        {
            MakeRequest(HttpMethod.Get, uri, cookieContainer, contentType, null, null, null, callback, state);
        }

        #endregion GET

        #region POST

        public static void PostAsync<TResult>(string uri, Action<TResult, Exception> callback,
            object content, string acceptContentType = HttpContentType.Json, string requestContentType = HttpContentType.Json) //where TResult: class
        {
            try
            {
                var buffer = SerializationHelper.Serialize(content, requestContentType);

                if (buffer != null)
                {
                    var contentStream = new MemoryStream(buffer); // Will be disposed of on callback

                    var state = new HttpResultContext<TResult>() { ResultCallback = callback };

                    HttpRequestContext<HttpResultContext<TResult>>.Current = new HttpRequestContext<HttpResultContext<TResult>>()
                    {
                        RequestContent = new HttpRequestData()
                        {
                            Content = buffer
                        }
                    };

                    MakeRequest<HttpResultContext<TResult>>(HttpMethod.Post, uri,
                                                                DefaultCookieContainer, acceptContentType,
                                                                requestContentType, null,
                                                                contentStream,
                                                                ResultCallback<TResult>, state);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled Exception in PostAsync: " + ex.Message);
                callback(default(TResult), ex);
            }
        }

        #endregion POST

        #region PUT

        public static void PutAsync<TResult>(string uri, Action<TResult, Exception> callback,
            object content, string acceptContentType = HttpContentType.Json, string requestContentType = HttpContentType.Json) //where TResult: class
        {
            try
            {
                var buffer = SerializationHelper.Serialize(content, requestContentType);

                if (buffer != null)
                {
                    var contentStream = new MemoryStream(buffer); // Will be disposed of on callback

                    var state = new HttpResultContext<TResult>() { ResultCallback = callback };

                    HttpRequestContext<HttpResultContext<TResult>>.Current = new HttpRequestContext<HttpResultContext<TResult>>()
                    {
                        RequestContent = new HttpRequestData()
                        {
                            Content = buffer
                        }
                    };

                    MakeRequest<HttpResultContext<TResult>>(HttpMethod.Put, uri,
                                                                DefaultCookieContainer, acceptContentType,
                                                                requestContentType, null,
                                                                contentStream,
                                                                ResultCallback<TResult>, state);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled Exception in PostAsync: " + ex.Message);
                callback(default(TResult), ex);
            }
        }

        #endregion PUT

        #region DELETE

        public static void DeleteAsync<TResult>(string uri, Action<TResult, Exception> callback,
            object content, string acceptContentType = HttpContentType.Json, string requestContentType = HttpContentType.Json) //where TResult: class
        {
            try
            {
                var buffer = SerializationHelper.Serialize(content, requestContentType);

                if (buffer != null)
                {
                    var contentStream = new MemoryStream(buffer); // Will be disposed of on callback

                    var state = new HttpResultContext<TResult>() { ResultCallback = callback };

                    HttpRequestContext<HttpResultContext<TResult>>.Current = new HttpRequestContext<HttpResultContext<TResult>>()
                    {
                        RequestContent = new HttpRequestData()
                        {
                            Content = buffer
                        }
                    };

                    MakeRequest<HttpResultContext<TResult>>(HttpMethod.Delete, uri,
                                                                DefaultCookieContainer, acceptContentType,
                                                                requestContentType, null,
                                                                contentStream,
                                                                ResultCallback<TResult>, state);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled Exception in PostAsync: " + ex.Message);
                callback(default(TResult), ex);
            }
        }

        #endregion DELETE

        public static void MakeRequest<T>(string method, string uri,
            CookieContainer cookieContainer, string acceptTypes,
            string requestContentType, string requestContentDisposition,
            Stream requestContentStream,
            Action<string, string, long, Stream, object, Exception> callback, T state,
            bool deferCredentials = true)
        {
            var authorizationHeader = _username == null ? null : "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", _username, _password)));
            MakeRequest(method, uri, cookieContainer, acceptTypes, requestContentType, requestContentDisposition, requestContentStream, callback, state, authorizationHeader, deferCredentials);
        }

        public static void MakeRequest<T>(string method, string uri,
            CookieContainer cookieContainer, string acceptTypes,
            string requestContentType, string requestContentDisposition,
            Stream requestContentStream,
            Action<string, string, long, Stream, object, Exception> callback, T state,
            string authorizationHeader, bool deferCredentials = true)
        {
            var request = GenerateRequest(method, uri, cookieContainer, acceptTypes, requestContentType,
                                          requestContentDisposition, null, Timeout * 1000,  // Request Stream will be handled asynchronously below. Do not pass in for synchronous processing.
                                          authorizationHeader, deferCredentials);

            // Create the request context objects (if needed).  If a post was made, this should already be created.
            var requestContext = HttpRequestContext<T>.Current ?? new HttpRequestContext<T>();

            // make sure we have request content for the context.  If not, create one.
            var requestContent = requestContext.RequestContent = requestContext.RequestContent ?? new HttpRequestData();

            //store everything we need to make this request again...
            requestContent.Method = method;
            requestContent.Uri = uri;
            requestContent.AcceptTypes = acceptTypes;
            requestContent.RequestContentType = requestContentType;
            requestContent.RequestContentDisposition = requestContentDisposition;

            requestContext.Request = request;
            requestContext.RequestContentStream = requestContentStream;
            requestContext.Callback = callback;
            requestContext.State = state;
            requestContext.RetryWithLogin = deferCredentials;

            requestContext.HaveResponse = false;
            requestContext.TimedOut = false;

            requestContext.TimeoutTimer = new Timer(TimeoutCallback<T>, requestContext, Timeout * 1000, System.Threading.Timeout.Infinite);

            if (requestContentStream != null)
            {
                request.ContentLength = requestContentStream.Length;
                request.BeginGetRequestStream(RequestStreamCallback<T>, requestContext);
            }
            else
            {
                request.BeginGetResponse(ResponseCallback<T>, requestContext);
            }

            HttpRequestContext<T>.Current = null;
        }

        private static void RequestStreamCallback<T>(IAsyncResult ar)
        {
            HttpRequestContext<T> requestContext = null;
            Stream contentStream = null;
            Stream requestStream = null;

            try
            {
                requestContext = (HttpRequestContext<T>)ar.AsyncState;

                var request = requestContext.Request;
                contentStream = requestContext.RequestContentStream;

                requestStream = request.EndGetRequestStream(ar);

                if (contentStream != null)
                {
                    contentStream.CopyTo(requestStream);

                    contentStream.Close();
                }

                requestStream.Close();

                request.BeginGetResponse(ResponseCallback<T>, requestContext);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled Exception in RequestStreamCallback: " + ex.Message);

                if (requestContext != null && requestContext.Callback != null)
                {
                    requestContext.Callback(null, null, 0, null, requestContext.State, ex);
                }
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Dispose();
                }
                if (requestStream != null)
                {
                    requestStream.Dispose();
                }
            }
        }

        private static void ResponseCallback<T>(IAsyncResult ar)
        {
            Exception exception = null;

            HttpRequestContext<T> requestContext = null;
            HttpWebResponse response = null;
            string contentType = null;
            string contentDisposition = null;
            Stream responseStream = null;
            T state = default(T);
            long contentLength = 0;
            bool timedOut = false;
            bool needsauth = false;

            try
            {
                requestContext = HttpRequestContext<T>.Current = (HttpRequestContext<T>)ar.AsyncState;

                lock (requestContext)
                {
                    requestContext.HaveResponse = true;
                    timedOut = requestContext.TimedOut;
                    requestContext.TimeoutTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                }
                state = requestContext.State;
                response = (HttpWebResponse)requestContext.Request.EndGetResponse(ar);
                responseStream = response.GetResponseStream();
                contentLength = response.ContentLength;
                contentType = response.ContentType;
                contentDisposition = response.Headers["content-disposition"];
            }
            catch (WebException webEx)
            {
                Debug.WriteLine("Web exception received: " + webEx.Message);

                exception = webEx;

                var exResp = webEx.Response as HttpWebResponse;

                // There was an error from the remote server....
                //		we only care if it could potentially be an invalid username (so if username and password are not null and we are supposed to retry with credentials)
                if (requestContext.RetryWithLogin && _username != null && _password != null
                    && exResp != null && exResp.StatusCode == HttpStatusCode.Unauthorized)
                {
                    needsauth = true;
                }
                else if (exResp != null)
                {
                    exception = ParseExceptionFromResponse(exResp, webEx);
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                Debug.WriteLine("Unhandled Exception in ResponseCallback: " + ex.Message);
            }

            try
            {
                if (timedOut)
                {   // response timed out
                    Debug.WriteLine("Simplex Web-Service Request timed out. Time-out response handled by timeout wait thread.");
                }
                else if (needsauth)
                {
                    Debug.WriteLine("Authentication required. Re-submit with authentication credentials.");

                    MemoryStream contentStream = null;

                    try
                    {
                        if (requestContext.RequestContent.Content != null)
                        {
                            contentStream = new MemoryStream(requestContext.RequestContent.Content);
                        }

                        MakeRequest<T>(requestContext.RequestContent.Method,
                                       requestContext.RequestContent.Uri,
                                       requestContext.Request.CookieContainer,
                                       requestContext.RequestContent.AcceptTypes,
                                       requestContext.RequestContent.RequestContentType,
                                       requestContext.RequestContent.RequestContentDisposition,
                                       contentStream,
                                       requestContext.Callback,
                                       requestContext.State,
                                       false);
                    }
                    finally
                    {
                        if (contentStream != null)
                        {
                            contentStream.Close();
                            contentStream.Dispose();
                        }
                    }
                }
                else if (requestContext != null && requestContext.Callback != null)
                {   // Notify caller
                    requestContext.Callback(contentType, contentDisposition, contentLength, responseStream, state, exception);
                }
            }
            catch (Exception ex)
            {
                if (requestContext != null && requestContext.Callback != null)
                {
                    requestContext.Callback(contentType, contentDisposition, contentLength, responseStream, state, ex);
                }
            }

            try
            {
                if (responseStream != null)
                {   // Close response stream
                    responseStream.Close();
                    responseStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Simplex HTTP: Error disposing of responseStream: " + ex.ToString());
            }

            HttpRequestContext<T>.Current = null;
        }

        public static void TimeoutCallback<T>(object state)
        {
            bool haveResponse;

            var context = (HttpRequestContext<T>)state;

            lock (context)
            {
                context.TimedOut = context.HaveResponse ^ true;
                haveResponse = context.HaveResponse;
            }

            if (haveResponse)
            {
                return;
            }

            context.TimeoutTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            var ex = new WebServiceTimeoutException("Web Service call timed out", context.Request.Method, context.Request.RequestUri.ToString());

            context.Callback(null, null, 0, null, context.State, ex);
        }

        #region Private Memebers

        private class HttpRequestData
        {
            public byte[] Content;
            public string Method;
            public string Uri;
            public string AcceptTypes;
            public string RequestContentType;
            public string RequestContentDisposition;
        }

        private class HttpRequestContext<T>
        {
            public HttpWebRequest Request;
            public Stream RequestContentStream;
            public Action<string, string, long, Stream, object, Exception> Callback;
            public T State;
            public Timer TimeoutTimer;
            public bool TimedOut = false;
            public bool HaveResponse = false;
            public HttpRequestData RequestContent;
            public bool RetryWithLogin = true;
            public readonly int RequestStartTickCount = Environment.TickCount;

            [ThreadStatic]
            public static HttpRequestContext<T> Current;
        }

        private class HttpResultContext<TResult>
        {
            public Action<TResult, Exception> ResultCallback;
            public TResult Result;
            public Exception Exception;
        }

        private static void ResultCallback<TResult>(string contentTypeList, string contentDisposition, long contentLength, Stream contentStream, object context, Exception exception)
        {
            var currentRequestContext = HttpRequestContext<HttpResultContext<TResult>>.Current;

            var resultContext = context as HttpResultContext<TResult>;

            if (resultContext == null || resultContext.ResultCallback == null) return;

            resultContext.Exception = exception;

            try
            {
                if (resultContext.Exception == null)
                {
                    resultContext.Result = SerializationHelper.Deserialize<TResult>(contentStream, contentLength, contentTypeList);
                }
            }
            catch (WebServiceResponseException ex)
            {
                resultContext.Exception = ex;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unhandled Exception in ResultCallback: " + ex.Message);
                resultContext.Exception = ex;
            }
            finally
            {
                if (contentStream != null)
                {
                    contentStream.Close();
                    contentStream.Dispose();
                }

                if (currentRequestContext != null)
                {
                    if (!currentRequestContext.TimedOut)
                    {
                        currentRequestContext.TimeoutTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }

                    var currentTickCount = Environment.TickCount;
                    if (currentTickCount > currentRequestContext.RequestStartTickCount)
                    {
                        RecordProcessingTime(currentTickCount - currentRequestContext.RequestStartTickCount);
                    }
                }

                ThreadPool.QueueUserWorkItem(InvokeResultCallback<TResult>, context);
            }
        }

        private static void InvokeResultCallback<TResult>(object state)
        {
            var context = (HttpResultContext<TResult>)state;

            if (context.ResultCallback != null)
            {
                context.ResultCallback(context.Result, context.Exception);
            }
        }

        #endregion Private Memebers

        #endregion Asynchronous
    }
}