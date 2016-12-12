using System;

namespace Orbit.WebApi.Api.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    ///Parameter Annotation
    /// </summary>
    public class ParameterAnnotation
    {
        public Attribute AnnotationAttribute { get; set; }

        public string Documentation { get; set; }
    }
}