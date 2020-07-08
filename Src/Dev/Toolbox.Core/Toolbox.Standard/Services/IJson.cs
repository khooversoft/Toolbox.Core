namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Interface for Json services
    /// </summary>
    public interface IJson
    {
        T Deserialize<T>(string subject);

        string Serialize<T>(T subject);
    }
}
