using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Orbit.WebApi.Base.NetHttp.Serializer
{
    /// <summary>
    /// Class Xml Serialize.
    /// </summary>
    public static class XmlSerializer
    {
        /// <summary>
        /// Serializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="formatted">if set to <c>true</c> [formatted].</param>
        /// <returns>System.String.</returns>
        public static string Serialize(object data, bool formatted = false)
        {
            var writer = new StringWriter();
            Serialize(data, writer);
            return writer.ToString();
        }

        /// <summary>
        /// Serializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="outputFileName">Name of the output file.</param>
        public static void Serialize(object data, string outputFileName)
        {
            var writer = XmlWriter.Create(outputFileName);
            Serialize(data, writer);
            writer.Close();
        }

        /// <summary>
        /// Serializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="formatted">if set to <c>true</c> [formatted].</param>
        public static void Serialize(object data, Stream stream, Encoding encoding, bool formatted = false)
        {
            Serialize(data, XmlWriter.Create(stream, new XmlWriterSettings() { Indent = formatted, Encoding = encoding }));
        }

        /// <summary>
        /// Serializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="writer">The writer.</param>
        public static void Serialize(object data, XmlWriter writer)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(data.GetType());
            xmlSerializer.Serialize(writer, data);
        }

        /// <summary>
        /// Serializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="writer">The writer.</param>
        public static void Serialize(object data, TextWriter writer)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(data.GetType());
            xmlSerializer.Serialize(writer, data);
        }

        /// <summary>
        /// DE serializes the specified input file name.
        /// </summary>
        /// <param name="inputFileName">Name of the input file.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public static object Deserialize(string inputFileName, Type type)
        {
            var reader = XmlReader.Create(inputFileName);
            var data = Deserialize(reader, type);
            reader.Close();

            return data;
        }

        /// <summary>
        /// DE serializes the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns>T.</returns>
        public static T Deserialize<T>(string data)
        {
            var dataReader = new StringReader(data);
            var xmlReader = XmlReader.Create(dataReader);

            return (T)Deserialize(xmlReader, typeof(T));
        }

        /// <summary>
        /// DE serializes the specified stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>T.</returns>
        public static T Deserialize<T>(Stream stream, Encoding encoding)
        {
            return (T)Deserialize(stream, encoding, typeof(T));
        }

        /// <summary>
        /// DE serializes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public static object Deserialize(Stream stream, Encoding encoding, Type type)
        {
            return Deserialize(XmlReader.Create(stream), type);
        }

        /// <summary>
        /// DE serialize the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public static object Deserialize(XmlReader reader, Type type)
        {
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(type);
            return xmlSerializer.Deserialize(reader);
        }
    }
}