namespace Orbit.WebApi.Base.SqlSerializer
{
    public interface IMappingInfo
    {
        /// <summary>
        /// Gets the mapping attribute.
        /// </summary>
        /// <value>
        /// The mapping attribute.
        /// </value>
        MappingAttribute MappingAttribute { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        object GetValue(object obj);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        void SetValue(object obj, object value);
    }
}