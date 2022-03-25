using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InvoiceAssessmentApi
{
    /// <summary>
    /// Resonsible for loading configuration settings
    /// </summary>
    static class SettingsManager
    {
        internal static IConfigurationRoot GetSettings()
        {
            var config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
             .AddEnvironmentVariables()
             .Build();

            return config;
        }
    }
}
