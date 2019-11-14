using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Khooversoft.Toolbox.Extensions.Configuration
{
    public static class OptionExtensions
    {
        /// <summary>
        /// Format help for an option
        /// </summary>
        /// <returns>arr or string</returns>
        public static IReadOnlyList<string> FormatHelp<T>(this T option)
            where T : class
        {
            option.Verify(nameof(option)).IsNotNull();

            return GetHelpData(option.GetType())
                .FormatHelpOptionData();
        }

        /// <summary>
        /// Get help data from class's option attributes
        /// </summary>
        /// <param name="type">option class's type</param>
        /// <param name="onlyProperties">return only properties, no subclasses</param>
        /// <returns>help details</returns>
        public static IReadOnlyList<OptionHelp> GetHelpData(this Type type, bool onlyProperties = false)
        {
            type.Verify(nameof(type)).IsNotNull();

            var propertyHelpText = type.GetProperties()
                .SelectMany(x => x.GetCustomAttributes<OptionAttribute>(), (o, i) => new { PropertyInfo = o, Attr = i })
                .Select(x => x.Attr.GetHelpForAttribute(x.PropertyInfo))
                .ToList();

            if (onlyProperties)
            {
                return propertyHelpText;
            }

            var classHelpText = type.GetCustomAttributes<OptionHelpAttribute>()
                .Where(x => x.Area == HelpArea.Header)
                .Select(x => new OptionHelp(x.Area, x.HelpText))
                .ToList();

            var subclassHelpText = type.GetProperties()
                .Where(x => x.PropertyType.IsClass && x.PropertyType != typeof(string))
                .SelectMany(x =>
                    GetHelpData(x.PropertyType, onlyProperties: true)
                        .Select(y => new OptionHelp(HelpArea.Detail, $"{x.Name}:{y.Command}", y.Text))
                    )
                .ToList();

            var exampleHelpText = type.GetCustomAttributes<OptionHelpAttribute>()
                .Where(x => x.Area == HelpArea.Example)
                .Select(x => new OptionHelp(x.Area, x.HelpText))
                .ToList();

            var helpText = propertyHelpText
                .Concat(classHelpText)
                .Concat(subclassHelpText.OrderBy(x => x.Command))
                .ToList();

            return helpText;
        }

        /// <summary>
        /// Get and format settings
        /// </summary>
        /// <typeparam name="T">option type</typeparam>
        /// <param name="option">option</param>
        /// <returns>sorted string list of formated options</returns>
        public static IReadOnlyList<string> FormatSettings<T>(this T option)
            where T : class
        {
            option.Verify(nameof(option)).IsNotNull();

            IReadOnlyList<KeyValuePair<string, object>> properties = option.GetPropertyValuesWithPath(x => x.GetCustomAttribute<OptionAttribute>() != null)
                .OrderBy(x => x.Key)
                .ToList();

            string fmt = $"{{0,-{properties.Max(x => x.Key.Length)}}} : {{1}}";

            return properties
                .Select(x => string.Format(fmt, x.Key, x.Value))
                .ToList();
        }


        /// <summary>
        /// Get option data for property attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        /// <param name="parameterType">parameter type</param>
        /// <returns>new option attribute used for formatting</returns>
        private static OptionHelp GetHelpForAttribute(this OptionAttribute attribute, PropertyInfo propertyInfo)
        {
            attribute.Verify(nameof(attribute)).IsNotNull();
            propertyInfo.Verify(nameof(propertyInfo)).IsNotNull();

            bool isClassProperty = propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string);

            string name = attribute.Name ?? propertyInfo.Name;
            string syntax = attribute.Syntax ?? (isClassProperty || propertyInfo.PropertyType == typeof(bool) ? string.Empty : "={value}");

            return new OptionHelp(HelpArea.Detail, name + syntax, attribute.HelpText);
        }

        /// <summary>
        /// Format help for an option
        /// </summary>
        /// <param name="helpData">help data from 'GetHelpData'</param>
        /// <returns>arr or string</returns>
        private static IReadOnlyList<string> FormatHelpOptionData(this IEnumerable<OptionHelp> helpData)
        {
            helpData.Verify(nameof(helpData)).IsNotNull();

            int MaxColumn1 = helpData
                .Max(x => x.Command?.Length ?? 0);

            string fmt = $"{{0,-{MaxColumn1}}} : {{1}}";

            var list = new List<string>();

            helpData
                .Where(x => x.Area == HelpArea.Header)
                .SelectMany(x => x.Text)
                .ForEach(x => list.Add(x));

            if (list.Count > 0)
            {
                list.Add(string.Empty);
            }

            var detailHelp = helpData
                .Where(x => x.Area == HelpArea.Detail)
                .OrderBy(x => x.Command)
                .ToList();

            string? subClassName = null;
            foreach (var line in detailHelp)
            {
                string[]? commandParts = line.Command?.Split(new char[] { ':' });
                if (commandParts?.Length > 0)
                {
                    string tempClassName = commandParts[0];
                    if (subClassName != tempClassName)
                    {
                        list.Add(string.Empty);
                        subClassName = tempClassName;
                    }
                }

                list.Add(string.Format(fmt, line.Command, line.Text?.FirstOrDefault()));

                if (line.Text?.Length > 1)
                {
                    line.Text
                        .Skip(1)
                        .ForEach(x => list.Add(string.Format(fmt, string.Empty, x)));
                }

            }

            if (list.Count > 0)
            {
                list.Add(string.Empty);
            }

            helpData
                .Where(x => x.Area == HelpArea.Example)
                .SelectMany(x => x.Text)
                .ForEach(x => list.Add(x));

            return list;
        }
    }
}
