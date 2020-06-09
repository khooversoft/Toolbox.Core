namespace Khooversoft.Toolbox.Standard
{
    public interface ISecretFilter
    {
        string? FilterSecrets(string? data, string replaceSecretWith = "***");
    }
}