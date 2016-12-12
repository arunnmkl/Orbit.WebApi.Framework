using System.Collections.ObjectModel;

namespace Orbit.WebApi.Api.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Complex Type Model Description
    /// </summary>
    public class ComplexTypeModelDescription : ModelDescription
    {
        public ComplexTypeModelDescription()
        {
            Properties = new Collection<ParameterDescription>();
        }

        public Collection<ParameterDescription> Properties { get; private set; }
    }
}