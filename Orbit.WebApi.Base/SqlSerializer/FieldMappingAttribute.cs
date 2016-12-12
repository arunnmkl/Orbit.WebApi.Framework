using System;

namespace Orbit.WebApi.Base.SqlSerializer
{
    /// <summary>
    /// That is for the field mapping from the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldMappingAttribute : MappingAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMappingAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of field.</param>
        public FieldMappingAttribute(string name)
            : base(name, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMappingAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of field.</param>
        /// <param name="id">if set to <c>true</c> [identifier].</param>
        /// <param name="identity">if set to <c>true</c> [identity].</param>
        public FieldMappingAttribute(string name, bool id = false, bool identity = false)
            : base(name, id, identity)
        {
        }
    }
}