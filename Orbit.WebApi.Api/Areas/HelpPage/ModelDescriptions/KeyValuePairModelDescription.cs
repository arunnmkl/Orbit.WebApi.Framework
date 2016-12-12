namespace Orbit.WebApi.Api.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    ///Key Value Pair Model Description
    /// </summary>
    public class KeyValuePairModelDescription : ModelDescription
    {
        public ModelDescription KeyModelDescription { get; set; }

        public ModelDescription ValueModelDescription { get; set; }
    }
}