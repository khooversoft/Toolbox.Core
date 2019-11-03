// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.Toolbox.Core.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Build option from configuration collection
        /// </summary>
        /// <typeparam name="T">type of option</typeparam>
        /// <param name="configuration">instance of the configuration collection</param>
        /// <returns>option</returns>
        public static T BuildOption<T>(this IConfiguration configuration) where T : class, new()
        {
            return new OptionBuilder<T>(configuration).Build();
        }

        /// <summary>
        /// Search for include configuration files on the command line.
        /// </summary>
        /// <param name="builder">configuration builder reference</param>
        /// <param name="args">command line args</param>
        /// <param name="includeFileArguments">list of include file arguments, at least one is required</param>
        /// <returns>configuration builder</returns>
        public static IConfigurationBuilder AddIncludeFiles(this IConfigurationBuilder builder, string[] args, params string[] includeFileArguments)
        {
            includeFileArguments.Length.Verify().Assert(x => x > 0, "Include file arguments");

            args.GetIncludeFiles(includeFileArguments)
                .ForEach(x => builder.AddJsonFile(x, true));

            return builder;
        }
    }
}
