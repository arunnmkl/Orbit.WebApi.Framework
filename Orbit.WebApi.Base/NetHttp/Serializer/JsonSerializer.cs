using System;
using System.IO;
using System.Text;

namespace Orbit.WebApi.Base.NetHttp.Serializer
{
    public static class JsonSerializer
    {
        private static readonly Newtonsoft.Json.JsonSerializer _jsonSerializer = new Newtonsoft.Json.JsonSerializer();

        #region Serialize

        public static string Serialize(object data, bool formatted = false)
        {
            using (var writer = new StringWriter())
            {
                Serialize(data, writer, formatted);
                return writer.ToString();
            }
        }

        public static void Serialize(object data, Stream stream, Encoding encoding, bool formatted = false)
        {
            var writer = new StreamWriter(stream, encoding);
            writer.AutoFlush = true;
            Serialize(data, writer, formatted);
        }

        public static void Serialize(object data, TextWriter writer, bool formatted = false)
        {
            _jsonSerializer.Serialize(writer, data);
        }

        #endregion Serialize

        #region Deserialize

        public static T Deserialize<T>(Stream stream, Encoding encoding)
        {
            return (T)Deserialize(stream, encoding, typeof(T));
        }

        public static object Deserialize(Stream stream, Encoding encoding, Type type)
        {
            var reader = new StreamReader(stream, encoding);
            return Deserialize(reader, type);
        }

        public static T Deserialize<T>(string data)
        {
            return (T)Deserialize(data, typeof(T));
        }

        public static object Deserialize(string data, Type type)
        {
            using (var reader = new StringReader(data))
            {
                return Deserialize(reader, type);
            }
        }

        public static object Deserialize(TextReader reader, Type type)
        {
            return _jsonSerializer.Deserialize(reader, type);
        }

        #endregion Deserialize
    }
}
