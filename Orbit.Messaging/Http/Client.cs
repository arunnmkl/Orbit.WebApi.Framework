using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Orbit.Messaging.Http
{
    /// <summary>
    /// Class to encapsulate the HTTP client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// The media type header
        /// </summary>
        private static readonly string mediaTypeHeader = "application/json";

        /// <summary>
        /// Gets the formatters.
        /// </summary>
        /// <value>
        /// The formatters.
        /// </value>
        public static IEnumerable<MediaTypeFormatter> Formatters
        {
            get
            {
                return new List<MediaTypeFormatter>()
                {
                    new JsonMediaTypeFormatter(),
                    new XmlMediaTypeFormatter()
                };
            }
        }

        /// <summary>
        /// Gets the specified request URI asynchronous.
        /// </summary>
        /// <typeparam name="T">type of the return object.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="bearerToken">The bearer token.</param>
        /// <param name="mediaType">Type of the media. default to "application/json"</param>
        /// <returns>
        /// the return object of type T
        /// </returns>
        /// <exception cref="System.NullReferenceException">requestUri</exception>
        public async static Task<T> GetAsync<T>(string requestUri, string bearerToken, string mediaType = null)
        {
            if (string.IsNullOrEmpty(requestUri))
            {
                throw new NullReferenceException("requestUri");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Security.Configuration.Current.ApiUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType ?? mediaTypeHeader));
                if (string.IsNullOrEmpty(bearerToken) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Security.Configuration.Current.AuthHeaderSchemaName, bearerToken);
                }

                HttpResponseMessage response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>(Formatters);
                }
            }

            return (T)Task.FromResult<object>(null).Result;
        }

        /// <summary>
        /// Gets the specified request URI asynchronous.
        /// </summary>
        /// <typeparam name="T">type of the return object.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="bearerToken">The bearer token.</param>
        /// <param name="mediaType">Type of the media. default to "application/json"</param>
        /// <returns>
        /// the return object of type T
        /// </returns>
        /// <exception cref="System.NullReferenceException">requestUri</exception>
        public async static Task<byte[]> GetAsync(string requestUri, string bearerToken, string mediaType = null)
        {
            if (string.IsNullOrEmpty(requestUri))
            {
                throw new NullReferenceException("requestUri");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Security.Configuration.Current.ApiUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType ?? mediaTypeHeader));
                if (string.IsNullOrEmpty(bearerToken) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Security.Configuration.Current.AuthHeaderSchemaName, bearerToken);
                }

                HttpResponseMessage response = await client.GetAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }

            return await Task.FromResult<byte[]>(null);
        }

        /// <summary>
        /// Posts the specified request URI asynchronous.
        /// </summary>
        /// <typeparam name="T">The type of the request</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="requestValue">The request value.</param>
        /// <param name="bearerToken">The bearer token.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns>
        /// result of the type TResult
        /// </returns>
        /// <exception cref="System.NullReferenceException">requestUri</exception>
        public async static Task<TResult> PostAsync<T, TResult>(string requestUri, T requestValue, string bearerToken, MediaTypeFormatter formatter = null, string mediaType = null)
        {
            if (string.IsNullOrEmpty(requestUri))
            {
                throw new NullReferenceException("requestUri");
            }

            if (requestValue == null)
            {
                throw new NullReferenceException("requestValue");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Security.Configuration.Current.ApiUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType ?? mediaTypeHeader));
                if (string.IsNullOrEmpty(bearerToken) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Security.Configuration.Current.AuthHeaderSchemaName, bearerToken);
                }

                HttpResponseMessage response = null;

                if (formatter == null)
                {
                    response = await client.PostAsJsonAsync(requestUri, requestValue);
                }
                else
                {
                    response = await client.PostAsync(requestUri, requestValue, formatter);
                }

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<TResult>(Formatters);
                }
            }

            return (TResult)Task.FromResult<object>(null).Result;
        }

        /// <summary>
        /// Puts the specified request URI asynchronous.
        /// </summary>
        /// <typeparam name="T">The type of the request</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="requestValue">The request value.</param>
        /// <param name="bearerToken">The bearer token.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns>
        /// result of the type TResult
        /// </returns>
        /// <exception cref="System.NullReferenceException">requestUri</exception>
        public async static Task<TResult> PutAsync<T, TResult>(string requestUri, T requestValue, string bearerToken, MediaTypeFormatter formatter = null, string mediaType = null)
        {
            if (string.IsNullOrEmpty(requestUri))
            {
                throw new NullReferenceException("requestUri");
            }

            if (requestValue == null)
            {
                throw new NullReferenceException("requestValue");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Security.Configuration.Current.ApiUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType ?? mediaTypeHeader));
                if (string.IsNullOrEmpty(bearerToken) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Security.Configuration.Current.AuthHeaderSchemaName, bearerToken);
                }

                HttpResponseMessage response = null;

                if (formatter == null)
                {
                    response = await client.PutAsJsonAsync(requestUri, requestValue);
                }
                else
                {
                    response = await client.PutAsync(requestUri, requestValue, formatter);
                }

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<TResult>(Formatters);
                }
            }

            return (TResult)Task.FromResult<object>(null).Result;
        }

        /// <summary>
        /// Deletes the specified request URI asynchronous.
        /// </summary>
        /// <typeparam name="T">the return type</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="bearerToken">The bearer token.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <returns>result.</returns>
        /// <exception cref="System.NullReferenceException">requestUri</exception>
        public async static Task<T> DeleteAsync<T>(string requestUri, string bearerToken, string mediaType = null)
        {
            if (string.IsNullOrEmpty(requestUri))
            {
                throw new NullReferenceException("requestUri");
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Security.Configuration.Current.ApiUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType ?? mediaTypeHeader));
                if (string.IsNullOrEmpty(bearerToken) == false)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Security.Configuration.Current.AuthHeaderSchemaName, bearerToken);
                }

                HttpResponseMessage response = await client.DeleteAsync(requestUri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<T>(Formatters);
                }
            }

            return (T)Task.FromResult<object>(null).Result;
        }
    }
}