using System.Reflection;

namespace Orbit.WebApi.Base.SqlSerializer
{
    /// <summary>
    /// This is a field mapping info class which contains the property/field info.
    /// </summary>
    internal class FieldMappingInfo : IMappingInfo
    {
        /// <summary>
        /// Gets the field mapping attribute.
        /// </summary>
        /// <value>
        /// The field mapping attribute.
        /// </value>
        public FieldMappingAttribute FieldMappingAttribute { get; private set; }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <value>
        /// The field information.
        /// </value>
        public FieldInfo FieldInfo { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldMappingInfo"/> class.
        /// </summary>
        /// <param name="fieldMappingAttribute">The field mapping attribute.</param>
        /// <param name="fieldInfo">The field information.</param>
        public FieldMappingInfo(FieldMappingAttribute fieldMappingAttribute, FieldInfo fieldInfo)
        {
            FieldMappingAttribute = fieldMappingAttribute;
            FieldInfo = fieldInfo;
        }

        /// <summary>
        /// Gets the mapping attribute.
        /// </summary>
        /// <value>
        /// The mapping attribute.
        /// </value>
        public MappingAttribute MappingAttribute
        {
            get { return FieldMappingAttribute; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return FieldInfo.Name; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public object GetValue(object obj)
        {
            return FieldInfo.GetValue(obj);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object obj, object value)
        {
            FieldInfo.SetValue(obj, value);
        }
    }
}