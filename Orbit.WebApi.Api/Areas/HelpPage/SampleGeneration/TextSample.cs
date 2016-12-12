using System;

namespace Orbit.WebApi.Api.Areas.HelpPage
{
    /// <summary>
    /// This represents a preformatted text sample on the help page. There's a display template named TextSample associated with this class.
    /// </summary>
    public class TextSample
    {
        /// <summary>
        /// for Text Sample
        /// </summary>
        /// <param name="text"></param>
        public TextSample(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            Text = text;
        }
        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// for Equals method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            TextSample other = obj as TextSample;
            return other != null && Text == other.Text;
        }
        /// <summary>
        /// for GetHashCode method
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
        /// <summary>
        /// For ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }
    }
}