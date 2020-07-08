namespace Khooversoft.Toolbox.Standard
{
    /// <summary>
    /// Secret filter services interface
    /// </summary>
    public interface ISecretFilter
    {
        string? FilterSecrets(string? data, string replaceSecretWith = "***");
    }
}