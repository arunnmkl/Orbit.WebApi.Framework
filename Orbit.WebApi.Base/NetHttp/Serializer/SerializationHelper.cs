using System;
using System.IO;
using System.Text;
using Orbit.WebApi.Base.NetHttp.ServiceException;

namespace Orbit.WebApi.Base.NetHttp.Serializer
{
    public static class SerializationHelper
    {
        private static readonly Encoding DefaultContentEncoding = Encoding.UTF8;

        public static byte[] Serialize(object content, string contentType, Encoding contentEncoding = null)
        {
            if (content == null)
            {
                return null;
            }

            byte[] buffer = null;

            if (contentType == HttpContentType.Json)
            {
                var jsonData = JsonSerializer.Serialize(content);

                buffer = (contentEncoding ?? DefaultContentEncoding).GetBytes(jsonData);
            }
            else if (contentType == HttpContentType.Xml || contentType == HttpContentType.XmlText)
            {
                var xmlData = XmlSerializer.Serialize(content);

                buffer = (contentEncoding ?? DefaultContentEncoding).GetBytes(xmlData);
            }
            else if (content is String)
            {
                buffer = (contentEncoding ?? DefaultContentEncoding).GetBytes((String)content);
            }
            else if (content.GetType() == typeof(byte[]))
            {
                var contentBytes = (byte[])content;
                buffer = new byte[contentBytes.Length];
                contentBytes.CopyTo(buffer, 0);
            }

            return buffer;
        }

        public static TResult Deserialize<TResult>(Stream contentStream, string contentTypeList = HttpContentType.Json)
        {
            var timedOut = false;
            return Deserialize<TResult>(contentStream, contentStream.Length, ref timedOut, contentTypeList);
        }

        public static TResult Deserialize<TResult>(Stream contentStream, long contentLength, string contentTypeList = HttpContentType.Json)
        {
            var timedOut = false;
            return Deserialize<TResult>(contentStream, contentLength, ref timedOut, contentTypeList);
        }

        public static TResult Deserialize<TResult>(Stream contentStream, ref bool timedOut, string contentTypeList = HttpContentType.Json)
        {
            return Deserialize<TResult>(contentStream, contentStream.Length, ref timedOut, contentTypeList);
        }

        public static TResult Deserialize<TResult>(Stream contentStream, long contentLength, ref bool timedOut, string contentTypeList = HttpContentType.Json)
        {
            var deserializedResult = default(TResult);

            var contentTypeToDeserialize = String.Empty;

            if (contentTypeList != null)
            {
                var contentTypes = contentTypeList.ToLower().Split(';');

                foreach (var contentType in contentTypes)
                {
                    var trimmedContentType = contentType.Trim();
                    if (trimmedContentType == HttpContentType.Json
                        || trimmedContentType == HttpContentType.Xml
                        || trimmedContentType == HttpContentType.XmlText)
                    {
                        contentTypeToDeserialize = trimmedContentType;
                        break;
                    }
                }
            }

            switch (contentTypeToDeserialize)
            {
                case HttpContentType.Json:
                    deserializedResult = JsonSerializer.Deserialize<TResult>(contentStream, Encoding.UTF8);
                    break;

                case HttpContentType.Xml:
                case HttpContentType.XmlText:
                    deserializedResult = XmlSerializer.Deserialize<TResult>(contentStream, null);
                    break;

                default:
                    var result = new byte[contentLength];
                    var bytesRead = 0;
                    var transferSize = contentLength < 8192 ? (int)contentLength : 8192;

                    while (bytesRead < contentLength && contentStream.CanRead && !timedOut)
                    {
                        bytesRead += contentStream.Read(result, bytesRead, transferSize);
                    }

                    contentStream.Close();

                    new WebServiceResponseException("Unknown ContentType", contentTypeList, result);

                    break;
            }

            return deserializedResult;
        }
    }
}