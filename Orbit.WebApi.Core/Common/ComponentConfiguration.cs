using System;
using System.Configuration;
using System.IO;
using WebApiConfig = Orbit.WebApi.Core.Security;

namespace Orbit.WebApi.Core.Common
{
    /// <summary>
    /// This is a config settings for the component
    /// </summary>
    /// <typeparam name="T">Configuration section of the component</typeparam>
    public abstract class ComponentConfiguration<T> where T : ConfigurationSection, new()
    {
        /// <summary>
        /// Stores the component settings
        /// </summary>
        private static T _componentSettings;

        /// <summary>
        /// Gets the component settings.
        /// </summary>
        /// <value>
        /// The component settings.
        /// </value>
        public static T ComponentSettings
        {
            get
            {
                if (_componentSettings == null)
                {
                    SetComponentConfiguration();
                }

                return _componentSettings;
            }

            private set { _componentSettings = value; }
        }

        /// <summary>
        /// Initializes the <see cref="ComponentConfiguration{T}"/> class.
        /// </summary>
        static ComponentConfiguration()
        {
            SetComponentConfiguration();
        }

        /// <summary>
        /// Sets the component configuration.
        /// </summary>
        private static void SetComponentConfiguration()
        {
            try
            {
                var assemblyFileInfo = new FileInfo(typeof(T).Assembly.Location);
                var componentConfigFileName = string.Concat(AppDomain.CurrentDomain.BaseDirectory,
                                                            "\\", WebApiConfig.Configuration.Current.DeploymentPath, "\\",
                                                            assemblyFileInfo.Name.Substring(0, assemblyFileInfo.Name.Length - assemblyFileInfo.Extension.Length),
                                                            ".config");

                ConfigurationSection componentConfigSection = null;
                if (File.Exists(componentConfigFileName))
                {
                    componentConfigSection = ConfigurationManager.OpenMappedMachineConfiguration(new ConfigurationFileMap(componentConfigFileName)).Sections.Get(typeof(T).Name);
                }

                _componentSettings = componentConfigSection as T;

                if (_componentSettings == null)
                {
                    _componentSettings = new T();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}