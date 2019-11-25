using System;
using System.Collections.Generic;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public interface IPropertyResolver
    {
        bool Strict { get; }

        IReadOnlyDictionary<string, string> Properties { get; }

        IReadOnlyDictionary<string, string> SourceProperties { get; }

        string Resolve(string value);

        IPropertyResolver With(string key, string value, PropertyUpdate propertyUpdate);

        IPropertyResolver With(IEnumerable<KeyValuePair<string, string>> values, PropertyUpdate propertyUpdate);

        IPropertyResolver WithStrict(bool strict = true);
    }
}
