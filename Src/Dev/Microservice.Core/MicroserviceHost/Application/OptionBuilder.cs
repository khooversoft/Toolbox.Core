using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    internal static class OptionBuilder
    {
        public static IOption Build(this string[] args)
        {
            Option option = new ConfigurationBuilder()
                .AddIncludeFiles(args, "ConfigFile")
                .AddCommandLine(args.ConflateKeyValue<Option>())
                .AddUserSecrets(nameof(MicroserviceHost))
                .Build()
                .BuildOption<Option>();

            if (option.Help) { return option; }

            option.VerifyNotNull(nameof(option));
            (option.Run || option.UnRegister).VerifyAssert(x => x, "Run or UnRegister must be specified");
            option.NamespaceConnections
                .VerifyNotNull(nameof(option.NamespaceConnections))
                .ForEach(x => x.Verify());

            option.AssemblyPath
                .VerifyNotEmpty($"{option.AssemblyPath} is required")
                .VerifyAssert(x => File.Exists(x), $"{option.AssemblyPath} does not exist");

            option.Properties = option.BuildResolver();
            option.SecretManager = option.BuildSecretManager();

            option.MessageNetConfig = new MessageNetConfig(option.NamespaceConnections.Select(x => new NamespaceRegistration(x.Namespace, x.ConnectionString)).ToArray());

            return option;
        }
    }
}
