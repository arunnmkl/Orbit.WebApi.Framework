using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Xml.XPath;
using Orbit.WebApi.Api.Areas.HelpPage.ModelDescriptions;

namespace Orbit.WebApi.Api.Areas.HelpPage
{
    /// <summary>
    /// A custom <see cref="IDocumentationProvider"/> that reads the API documentation from an XML documentation file.
    /// </summary>
    public class XmlDocumentationProvider : IDocumentationProvider, IModelDocumentationProvider
    {
        private XPathNavigator _documentNavigator;
        private const string TypeExpression = "/doc/members/member[@name='T:{0}']";
        private const string MethodExpression = "/doc/members/member[@name='M:{0}']";
        private const string PropertyExpression = "/doc/members/member[@name='P:{0}']";
        private const string FieldExpression = "/doc/members/member[@name='F:{0}']";
        private const string ParameterExpression = "param[@name='{0}']";

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDocumentationProvider"/> class.
        /// </summary>
        /// <param name="documentPath">The physical path to XML document.</param>
        public XmlDocumentationProvider(string documentPath)
        {
            if (documentPath == null)
            {
                throw new ArgumentNullException("documentPath");
            }
            XPathDocument xpath = new XPathDocument(documentPath);
            _documentNavigator = xpath.CreateNavigator();
        }
        /// <summary>
        /// Describes the Http Controller
        /// </summary>
        /// <param name="controllerDescriptor"></param>
        /// <returns></returns>
        public string GetDocumentation(HttpControllerDescriptor controllerDescriptor)
        {
            XPathNavigator typeNode = GetTypeNode(controllerDescriptor.ControllerType);
            return GetTagValue(typeNode, "summary");
        }
        /// <summary>
        /// Provides information about the action methods
        /// </summary>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public virtual string GetDocumentation(HttpActionDescriptor actionDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "summary");
        }
        /// <summary>
        /// Represents HTTP parameter descriptor
        /// </summary>
        /// <param name="parameterDescriptor"></param>
        /// <returns></returns>
        public virtual string GetDocumentation(HttpParameterDescriptor parameterDescriptor)
        {
            ReflectedHttpParameterDescriptor reflectedParameterDescriptor = parameterDescriptor as ReflectedHttpParameterDescriptor;
            if (reflectedParameterDescriptor != null)
            {
                if (reflectedParameterDescriptor.ParameterInfo is CustomParameterInfo)
                {
                    const string PropertyExpression = "/doc/members/member[@name='P:{0}']";
                    var pi = (CustomParameterInfo)reflectedParameterDescriptor.ParameterInfo;

                    string selectExpression = String.Format(CultureInfo.InvariantCulture, PropertyExpression, pi.Prop.DeclaringType.FullName + "." + pi.Prop.Name);
                    XPathNavigator methodNode = _documentNavigator.SelectSingleNode(selectExpression);
                    if (methodNode != null)
                    {
                        return methodNode.Value.Trim();
                    }
                }
                else
                {
                    XPathNavigator methodNode = GetMethodNode(reflectedParameterDescriptor.ActionDescriptor);
                    if (methodNode != null)
                    {
                        string parameterName = reflectedParameterDescriptor.ParameterInfo.Name;
                        XPathNavigator parameterNode = methodNode.SelectSingleNode(string.Format(CultureInfo.InvariantCulture, ParameterExpression, parameterName));
                        if (parameterNode != null)
                        {
                            return parameterNode.Value.Trim();
                        }
                    }
                }
            }

            return null;
        }
        /// <summary>
        ///  Provides information about the action methods
        /// </summary>
        /// <param name="actionDescriptor"></param>
        /// <returns></returns>
        public string GetResponseDocumentation(HttpActionDescriptor actionDescriptor)
        {
            XPathNavigator methodNode = GetMethodNode(actionDescriptor);
            return GetTagValue(methodNode, "returns");
        }
        /// <summary>
        ///Obtains information about the attributes of a member and provides access for metadata
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public string GetDocumentation(MemberInfo member)
        {
            string memberName = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetTypeName(member.DeclaringType), member.Name);
            string expression = member.MemberType == MemberTypes.Field ? FieldExpression : PropertyExpression;
            string selectExpression = String.Format(CultureInfo.InvariantCulture, expression, memberName);
            XPathNavigator propertyNode = _documentNavigator.SelectSingleNode(selectExpression);
            return GetTagValue(propertyNode, "summary");
        }
        /// <summary>
        /// Represents Type Declarations
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetDocumentation(Type type)
        {
            XPathNavigator typeNode = GetTypeNode(type);
            return GetTagValue(typeNode, "summary");
        }

        private XPathNavigator GetMethodNode(HttpActionDescriptor actionDescriptor)
        {
            ReflectedHttpActionDescriptor reflectedActionDescriptor = actionDescriptor as ReflectedHttpActionDescriptor;
            if (reflectedActionDescriptor != null)
            {
                string selectExpression = String.Format(CultureInfo.InvariantCulture, MethodExpression, GetMemberName(reflectedActionDescriptor.MethodInfo));
                return _documentNavigator.SelectSingleNode(selectExpression);
            }

            return null;
        }

        private static string GetMemberName(MethodInfo method)
        {
            string name = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", GetTypeName(method.DeclaringType), method.Name);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                string[] parameterTypeNames = parameters.Select(param => GetTypeName(param.ParameterType)).ToArray();
                name += String.Format(CultureInfo.InvariantCulture, "({0})", String.Join(",", parameterTypeNames));
            }

            return name;
        }

        private static string GetTagValue(XPathNavigator parentNode, string tagName)
        {
            if (parentNode != null)
            {
                XPathNavigator node = parentNode.SelectSingleNode(tagName);
                if (node != null)
                {
                    return node.Value.Trim();
                }
            }

            return null;
        }

        private XPathNavigator GetTypeNode(Type type)
        {
            string controllerTypeName = GetTypeName(type);
            string selectExpression = String.Format(CultureInfo.InvariantCulture, TypeExpression, controllerTypeName);
            return _documentNavigator.SelectSingleNode(selectExpression);
        }

        private static string GetTypeName(Type type)
        {
            string name = type.FullName;
            if (type.IsGenericType)
            {
                // Format the generic type name to something like: Generic{System.Int32,System.String}
                Type genericType = type.GetGenericTypeDefinition();
                Type[] genericArguments = type.GetGenericArguments();
                string genericTypeName = genericType.FullName;

                // Trim the generic parameter counts from the name
                genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
                string[] argumentTypeNames = genericArguments.Select(t => GetTypeName(t)).ToArray();
                name = String.Format(CultureInfo.InvariantCulture, "{0}{{{1}}}", genericTypeName, String.Join(",", argumentTypeNames));
            }
            if (type.IsNested)
            {
                // Changing the nested type name from OuterType+InnerType to OuterType.InnerType to match the XML documentation syntax.
                name = name.Replace("+", ".");
            }

            return name;
        }

        /// <summary>
        /// CustomParameterInfo
        /// </summary>
        /// <seealso cref="System.Reflection.ParameterInfo" />
        private class CustomParameterInfo : ParameterInfo
        {
            /// <summary>
            /// Gets or sets the property.
            /// </summary>
            /// <value>
            /// The property.
            /// </value>
            public PropertyInfo Prop { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CustomParameterInfo"/> class.
            /// </summary>
            /// <param name="prop">The property.</param>
            public CustomParameterInfo(PropertyInfo prop)
            {
                Prop = prop;
                base.NameImpl = prop.Name;
            }
        }
    }
}
