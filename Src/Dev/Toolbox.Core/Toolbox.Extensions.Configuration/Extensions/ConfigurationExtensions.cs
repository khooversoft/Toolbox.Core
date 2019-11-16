// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Khooversoft.Toolbox.Standard;

namespace Khooversoft.Toolbox.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
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

        /// <summary>
        /// Build option based on configuration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration">configuration built</param>
        /// <returns>option</returns>
        public static T BuildOption<T>(this IConfiguration configuration)
            where T : class, new()
        {
            configuration.Verify(nameof(configuration)).IsNotNull();

            T option = new T();
            configuration.Bind(option);

            return option;
        }
    }
}
