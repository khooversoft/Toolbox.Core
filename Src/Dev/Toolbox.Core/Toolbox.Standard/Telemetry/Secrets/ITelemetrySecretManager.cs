using System.Collections.Generic;

namespace Khooversoft.Toolbox.Standard
{
    public interface ITelemetrySecretManager : IEnumerable<string>
    {
        int Count { get; }

        TelemetrySecretManager Add(params string[] secrets);

        string? Mask(string? value);

        ITelemetrySecretManager With(ITelemetrySecretManager add);
    }
}