﻿namespace Khooversoft.Toolbox.Standard
{
    public interface IJson
    {
        T Deserialize<T>(string subject);

        string Serialize<T>(T subject);
    }
}
