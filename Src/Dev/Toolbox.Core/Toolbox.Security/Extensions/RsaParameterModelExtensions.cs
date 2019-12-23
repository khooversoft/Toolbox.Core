﻿using Khooversoft.Toolbox.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Khooversoft.Toolbox.Security
{
    public static class RsaParameterModelExtensions
    {
        private static readonly IReadOnlyList<byte> _empty = Enumerable.Empty<byte>().ToList();

        public static RsaParameterModel ConvertTo(this RSAParameters rSAParameters)
        {
            return new RsaParameterModel
            {
                D = rSAParameters.D?.ToList() ?? _empty,
                DP = rSAParameters.DP?.ToList() ?? _empty,
                DQ = rSAParameters.DQ?.ToList() ?? _empty,
                Exponent = rSAParameters.Exponent?.ToList() ?? _empty,
                InverseQ = rSAParameters.InverseQ?.ToList() ?? _empty,
                Modulus = rSAParameters.Modulus?.ToList() ?? _empty,
                P = rSAParameters.P?.ToList() ?? _empty,
                Q = rSAParameters.Q?.ToList() ?? _empty,
            };
        }

        public static RSAParameters ConvertTo(this RsaParameterModel rSAParametersModel)
        {
            return new RSAParameters
            {
                D = rSAParametersModel.D?.ToArray(),
                DP = rSAParametersModel.DP?.ToArray(),
                DQ = rSAParametersModel.DQ?.ToArray(),
                Exponent = rSAParametersModel.Exponent?.ToArray(),
                InverseQ = rSAParametersModel.InverseQ?.ToArray(),
                Modulus = rSAParametersModel.Modulus?.ToArray(),
                P = rSAParametersModel.P?.ToArray(),
                Q = rSAParametersModel.Q?.ToArray(),
            };
        }

        public static string ToJson(this RsaParameterModel subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            return JsonSerializer.Serialize(subject);
        }

        public static RsaParameterModel ToRasParameterModel(this string json)
        {
            json.Verify(nameof(json)).IsNotEmpty();

            return JsonSerializer.Deserialize<RsaParameterModel>(json);
        }

        public static string ToBinaryString(this RSAParameters subject)
        {
            subject.Verify(nameof(subject)).IsNotNull();

            RsaParameterModel model = subject.ConvertTo();

            var formatter = new BinaryFormatter();
            using var memory = new MemoryStream();
            formatter.Serialize(memory, model);
            memory.Position = 0;

            return Convert.ToBase64String(memory.ToArray());
        }

        public static RSAParameters ToRSAParameters(this string subject)
        {
            subject.Verify(nameof(subject)).IsNotEmpty();

            byte[] array = Convert.FromBase64String(subject);

            using var memory = new MemoryStream(array);
            var formatter = new BinaryFormatter();
            RsaParameterModel model = (RsaParameterModel)formatter.Deserialize(memory);

            return model.ConvertTo();
        }
    }
}