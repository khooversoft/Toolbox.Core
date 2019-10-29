using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Toolbox.Standard;

namespace Toolbox.Core.Extensions.Configuration
{
    public static class ArgsExtensions
    {
        /// <summary>
        /// Search array of string for key-value "key=value" that matches required
        /// </summary>
        /// <param name="args">command line argument</param>
        /// <param name="searchForKeys">include argument names, will use for command line and json property searches</param>
        /// <returns>json files to include, first is the configuration</returns>
        public static IReadOnlyList<string> GetIncludeFiles(this IEnumerable<string> args, params string[] searchForKeys)
        {
            var files = new List<string>();
            var searchForKeyList = new List<string>(searchForKeys);

            if (args.Count() == 0 || searchForKeyList.Count == 0)
            {
                return files;
            }

            // Get argument key-value pairs
            var arguments = args
                .Select(x => x.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(x => x.Length == 2)
                .Select(x => new KeyValuePair<string, string>(x[0], x[1]))
                .ToList();

            // Add json file using the order of 'includeArgumentNames' as precedent
            searchForKeyList
                .ForEach(x => arguments
                    .Where(y => y.Key.Equals(x, StringComparison.OrdinalIgnoreCase))
                    .ForEach(y => files.Add(y.Value.Do(Path.GetFullPath)))
                    );

            var filesToSearch = files
                .Reverse<string>()
                .ToStack();

            while (filesToSearch.TryPop(out string file))
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .AddJsonFile(file, optional: false)
                    .Build();

                string configFolder = Path.GetDirectoryName(file);

                var newFiles = searchForKeyList
                    .SelectMany(x => configuration.GetSection(x).GetChildren())
                    .Select(x => Path.Combine(configFolder, x.Value).Do(Path.GetFullPath))
                    .Where(x => !files.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                newFiles
                    .ForEach(x => files.Add(x));

                newFiles
                    .ForEach(x => filesToSearch.Push(x));
            }

            return files;
        }

        /// <summary>
        /// Based on 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] ConflateKeyValue<T>(this IEnumerable<string> args, string value = "true")
            where T : class, new()
        {
            var switches = typeof(T).GetProperties()
                .Where(x => x.CanWrite && x.PropertyType == typeof(bool) && x.GetCustomAttribute<OptionAttribute>() != null)
                .SelectMany(x => x.Name.ToEnumerable().Concat(x.GetCustomAttribute<OptionAttribute>().ShortCuts ?? Enumerable.Empty<string>()))
                .ToList();

            return args.ConflateKeyValue(switches, value);
        }

        /// <summary>
        /// Conflate "value" in potential key value pairs where value is not specified.  args will be reordered
        ///   key1 key2=k2
        ///   -- to --
        ///   key2=k2 key1={value}
        /// </summary>
        /// <param name="args"></param>
        /// <param name="switches"></param>
        /// <param name="value">value to set</param>
        /// <returns>conflated args</returns>
        public static string[] ConflateKeyValue(this IEnumerable<string> args, IEnumerable<string> switches, string value = "true")
        {
            // Get argument key-value pairs
            var arguments = args
                .Select(x =>
                {
                    return new { Arg = x, Split = x.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries) };
                });

            var returnArguments = new List<string>(arguments.Where(x => x.Split.Length != 1).Select(x => x.Arg));

            var newSwitches = arguments
                .Where(x => x.Split.Length == 1)
                .Select(x => x.Split[0])
                .Join(switches, x => x, x => x, (o, i) => o, StringComparer.OrdinalIgnoreCase)
                .Select(x => x + "=" + value);

            returnArguments.AddRange(newSwitches);

            return returnArguments.ToArray();
        }
    }
}
