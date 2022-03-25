using InvoiceAssessmentApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.IO;
using System.Security.Authentication;
using InvoiceAssessmentApi.Services;

[assembly: FunctionsStartup(typeof(Startup))]
namespace InvoiceAssessmentApi
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = SettingsManager.GetSettings();

            builder.Services.AddSingleton<IConfiguration>(config);

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(config["ConnectionString"]));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            builder.Services.AddSingleton((s) => new MongoClient(settings));
            builder.Services.AddTransient<IInvoiceService, InvoiceService>();
        }
    }
}

