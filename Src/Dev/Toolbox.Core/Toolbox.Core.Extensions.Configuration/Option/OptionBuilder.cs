using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Toolbox.Standard;

namespace Toolbox.Core.Extensions.Configuration
{
    /// <summary>
    /// Option builder uses the .NET Configuration to parse command line, json, memory, and secret values
    /// and build option class with flags, property, arrays, and property resolver.
    /// 
    /// Also support the ability to specify additional json include files for collecting configurations.
    /// </summary>
    /// <typeparam name="T">option type class, must have parameterless constructor</typeparam>
    public class OptionBuilder<T>
        where T : class, new()
    {
        private readonly IConfiguration _configuration;
        private readonly IDictionary<string, Action<T, string>> _singleMap = new Dictionary<string, Action<T, string>>(StringComparer.OrdinalIgnoreCase);
        private readonly IDictionary<string, Action<T, IReadOnlyList<string>>> _listMap = new Dictionary<string, Action<T, IReadOnlyList<string>>>(StringComparer.OrdinalIgnoreCase);
        private readonly IDictionary<string, Action<T, IReadOnlyDictionary<string, string>>> _dictMap = new Dictionary<string, Action<T, IReadOnlyDictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);

        public OptionBuilder(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            BuildMap();
        }

        public T Build()
        {
            T option = new T();
            ApplyOption(option);

            return option;
        }

        private void ApplyOption(T option)
        {
            foreach (var decode in _singleMap)
            {
                string value = _configuration[decode.Key];
                if (!value.IsEmpty())
                {
                    decode.Value(option, value);
                }
            }

            IConfigurationSection configurationSection;

            foreach (var decode in _listMap)
            {
                if (!_configuration.GetSection(decode.Key).Exists())
                {
                    continue;
                }

                configurationSection = _configuration.GetSection(decode.Key);
                if (configurationSection != null)
                {
                    IReadOnlyList<string> items = configurationSection.GetChildren()
                        .Select(y => y.Value.Trim())
                        .ToList();

                    decode.Value(option, items);
                }
            }

            foreach (var decode in _dictMap)
            {
                if (!_configuration.GetSection(decode.Key).Exists())
                {
                    continue;
                }

                configurationSection = _configuration.GetSection(decode.Key);
                if (configurationSection != null)
                {
                    IReadOnlyDictionary<string, string> dictItems = configurationSection.GetChildren()
                        .ToDictionary(y => y.Key, y => y.Value, StringComparer.OrdinalIgnoreCase);

                    decode.Value(option, dictItems);
                }
            }
        }

        private void BuildMap()
        {
            var stack = new Stack<(Type Type, string? ClassPath)>(new (Type Type, string? ClassPath)[] { (typeof(T), null) });

            while (stack.Count > 0)
            {
                (Type Type, string? ClassPath) current = stack.Pop();

                var attributes = current.Type.GetProperties()
                    .Where(x => x.CanWrite)
                    .Where(x => !x.PropertyType.IsClass || x.PropertyType == typeof(string))
                    .Select(x => new { PropertyInfo = x, Attribute = x.GetCustomAttribute<OptionAttribute>() })
                    .ToList();

                attributes
                    .ForEach(x => AddInstruction(x.PropertyInfo, x.Attribute, current.ClassPath));

                var classProperties = current.Type.GetProperties()
                    .Where(x => x.CanWrite)
                    .Where(x => x.PropertyType.IsClass && x.PropertyType != typeof(string) && x.GetCustomAttribute<OptionAttribute>() != null)
                    .ToList();

                classProperties
                    .Reverse<PropertyInfo>()
                    .ForEach(x => stack.Push((x.PropertyType, (current.ClassPath != null ? current.ClassPath + ":" : string.Empty) + x.Name)));
            }
        }

        private void AddInstruction(PropertyInfo propertyInfo, OptionAttribute attribute, string? classPath)
        {
            string configurationName = (classPath != null ? classPath + ":" : string.Empty) + (attribute.Name ?? propertyInfo.Name);
            string variablePath = classPath != null ? classPath + ":" + propertyInfo.Name : propertyInfo.Name;

            var configurationNames = configurationName.ToEnumerable()
                .Concat(attribute.ShortCuts ?? Enumerable.Empty<string>());

            switch (propertyInfo.PropertyType)
            {
                case Type boolType when boolType == typeof(bool) || boolType == typeof(bool?):
                    configurationNames
                        .ForEach(x => _singleMap.Add(x, (x, v) => x.SetPropertyValueByPath(variablePath, bool.Parse(v))));
                    break;

                case Type listType when listType == typeof(List<string>) || listType == typeof(IReadOnlyList<string>):
                    configurationNames
                        .ForEach(x => _listMap.Add(x, (x, v) => x.SetPropertyValueByPath(variablePath, v)));
                    break;

                case Type dictType when dictType == typeof(Dictionary<string, string>) || dictType == typeof(IDictionary<string, string>) || dictType == typeof(IReadOnlyDictionary<string, string>):
                    configurationNames
                        .ForEach(x => _dictMap.Add(x, (x, v) => x.SetPropertyValueByPath(variablePath, v)));
                    break;

                default:
                    configurationNames
                        .ForEach(x => _singleMap.Add(x, (x, v) => x.SetPropertyValueByPath(variablePath, v)));
                    break;
            }
        }
    }
}