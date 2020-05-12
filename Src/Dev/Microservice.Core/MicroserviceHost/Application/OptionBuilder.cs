using Khooversoft.MessageNet.Host;
using Khooversoft.Toolbox.Configuration;
using Khooversoft.Toolbox.Standard;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MicroserviceHost
{
    internal class OptionBuilder
    {
        public string[]? Args { get; set; }

        public Stream? JsonStream { get; set; }

        public string UserSecretsId { get; set; } = nameof(MicroserviceHost);

        public OptionBuilder SetArgs(params string[] args)
        {
            Args = args;
            return this;
        }

        public OptionBuilder SetJsonStream(Stream stream)
        {
            JsonStream = stream;
            return this;
        }

        public OptionBuilder SetUserSecretId(string userSecretId)
        {
            UserSecretsId = userSecretId.VerifyNotEmpty(nameof(userSecretId));
            return this;
        }

        public IOption Build()
        {
            string[] args = Args ?? Array.Empty<string>();

            Option option = new ConfigurationBuilder()
                .AddIncludeFiles(args, "ConfigFile")
                .Func(x => JsonStream != null ? x.AddJsonStream(JsonStream) : x)
                .AddCommandLine(args.ConflateKeyValue<Option>())
                .AddUserSecrets(UserSecretsId.VerifyNotEmpty(nameof(UserSecretsId)))
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
