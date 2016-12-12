using System.Reflection;

namespace Orbit.WebApi.Base.SqlSerializer
{
    internal class PropertyMappingInfo : IMappingInfo
    {
        /// <summary>
        /// Gets the property mapping attribute.
        /// </summary>
        /// <value>
        /// The property mapping attribute.
        /// </value>
        public PropertyMappingAttribute PropertyMappingAttribute { get; private set; }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMappingInfo"/> class.
        /// </summary>
        /// <param name="propertyMappingAttribute">The property mapping attribute.</param>
        /// <param name="propertyInfo">The property information.</param>
        public PropertyMappingInfo(PropertyMappingAttribute propertyMappingAttribute, PropertyInfo propertyInfo)
        {
            PropertyMappingAttribute = propertyMappingAttribute;
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Gets the mapping attribute.
        /// </summary>
        /// <value>
        /// The mapping attribute.
        /// </value>
        public MappingAttribute MappingAttribute
        {
            get { return PropertyMappingAttribute; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return PropertyInfo.Name; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public object GetValue(object obj)
        {
            return PropertyInfo.GetValue(obj, BindingFlags.GetProperty, null, null, null);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object obj, object value)
        {
            PropertyInfo.SetValue(obj, value, BindingFlags.SetProperty, null, null, null);
        }
    }
}