using System.Text.Json;

namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Provides json services using .net core JSON
    /// </summary>
    public class Json : IJson
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public string Serialize<T>(T subject) => JsonSerializer.Serialize(subject, _options);

        public T Deserialize<T>(string subject) => JsonSerializer.Deserialize<T>(subject, _options);
    }
}
