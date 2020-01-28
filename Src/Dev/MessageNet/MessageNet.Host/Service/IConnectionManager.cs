namespace MessageNet.Host
{
    public interface IConnectionManager
    {
        ConnectionManager Add(params ConnectionRegistration[] connectionRegistrations);

        string GetConnection(string networkId);
    }
}