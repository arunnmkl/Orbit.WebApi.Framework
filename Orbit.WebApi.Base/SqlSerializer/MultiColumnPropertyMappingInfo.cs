using System;
using System.Collections.Generic;
using System.Reflection;

namespace Orbit.WebApi.Base.SqlSerializer
{
    public class MultiColumnPropertyMappingInfo : IMappingInfo
    {
        /// <summary>
        /// Gets the multi column property mapping attribute.
        /// </summary>
        /// <value>
        /// The multi column property mapping attribute.
        /// </value>
        public MultiColumnPropertyMappingAttribute MultiColumnPropertyMappingAttribute { get; private set; }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiColumnPropertyMappingInfo"/> class.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="info">The information.</param>
        public MultiColumnPropertyMappingInfo(MultiColumnPropertyMappingAttribute attribute, PropertyInfo info)
        {
            MultiColumnPropertyMappingAttribute = attribute;
            PropertyInfo = info;
        }

        /// <summary>
        /// Gets the mapping attribute.
        /// </summary>
        /// <value>
        /// The mapping attribute.
        /// </value>
        public MappingAttribute MappingAttribute
        {
            get { return MultiColumnPropertyMappingAttribute; }
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
        /// <exception cref="System.NotImplementedException"></exception>
        public object GetValue(object obj)
        {
            throw new NotImplementedException();
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

        /// <summary>
        /// Constructs the value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyBag">The property bag.</param>
        /// <returns></returns>
        public object ConstructValue(object obj, Dictionary<string, object> propertyBag)
        {
            var value = MultiColumnPropertyMappingAttribute.ConstructValue(obj, propertyBag);
            return value;
        }
    }
}