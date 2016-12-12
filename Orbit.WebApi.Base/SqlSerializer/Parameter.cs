using System;
using System.Data;

namespace Orbit.WebApi.Base.SqlSerializer
{
    /// <summary>
    /// The parameter class which is used to pass in the queries.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The Parameter name.</value>
        public string Name { get; set; }

        public object Value { get; set; }

        /// <summary>
        /// The table value parameter type
        /// </summary>
        public string TableValueParameterType { get; set; }

        /// <summary>
        /// The direction
        /// </summary>
        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// The type
        /// </summary>
        internal Type Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public Parameter(string name, object value)
            : this(name, value, ParameterDirection.Input, value == null ? null : value.GetType())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="tableValueParameterType">Type of the table value parameter.</param>
        public Parameter(string name, DataTable value, string tableValueParameterType)
            : this(name, value)
        {
            TableValueParameterType = tableValueParameterType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="type">The type.</param>
        internal Parameter(string name, object value, ParameterDirection direction, Type type)
        {
            Name = name;
            Value = value;
            Direction = direction;
            Type = type;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <remarks>Overridden to enable quick, debugging via intelligence</remarks>
        public override string ToString()
        {
            return string.Concat(Name, "=", Value ?? string.Empty, " [", Direction, "]");
        }
    }
}