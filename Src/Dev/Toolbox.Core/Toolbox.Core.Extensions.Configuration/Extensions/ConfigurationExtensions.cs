using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Standard;

namespace Toolbox.Core.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static T BuildOption<T>(this IConfiguration configuration) where T : class, new()
        {
            return new OptionBuilder<T>(configuration).Build();
        }

        public static IConfigurationBuilder AddIncludeFiles(this IConfigurationBuilder builder, string[] args)
        {
            args.GetIncludeFiles("ConfigFile")
                .ForEach(x => builder.AddJsonFile(x, true));

            return builder;
        }
    }
}
