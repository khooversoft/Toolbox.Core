namespace MessageNet.Host
{
    internal interface IConnectionManager
    {
        ConnectionManager Add(params ConnectionRegistration[] connectionRegistrations);

        string GetConnection(string networkId);
    }
}