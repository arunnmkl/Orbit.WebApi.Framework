using System;
using System.Reflection;

namespace Orbit.WebApi.Api.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// IModel Documentation Provider
    /// </summary>
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}