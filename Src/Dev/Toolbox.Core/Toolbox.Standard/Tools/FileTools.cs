using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Khooversoft.Toolbox.Standard
{
    public static class FileTools
    {
        public static string WriteResourceToTempFile(string fileName, string folder, Type type, string resourceId)
        {
            fileName.VerifyNotEmpty(nameof(fileName));
            folder.VerifyNotEmpty(nameof(folder));

            string filePath = Path.Combine(Path.GetTempPath(), folder, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using var stream = GetResourceStream(type, resourceId);
            WriteStreamToFile(stream, filePath);

            return filePath;
        }

        public static byte[] GetFileHash(string file)
        {
            using Stream read = new FileStream(file, FileMode.Open);
            return MD5.Create().ComputeHash(read);
        }

        /// <summary>
        /// Get stream from assembly's resources
        /// </summary>
        /// <param name="type">type int the assembly that has the resource</param>
        /// <param name="streamId">resource id</param>
        /// <returns>stream</returns>
        public static Stream GetResourceStream(this Type type, string streamId) =>
                Assembly.GetAssembly(type.VerifyNotNull(nameof(type)))!
                    .GetManifestResourceStream(streamId.VerifyNotEmpty(nameof(streamId)))
                    .VerifyNotNull($"Cannot find {streamId} in assembly's resource");

        public static void WriteStreamToFile(this Stream stream, string file)
        {
            using Stream writeFile = new FileStream(file, FileMode.Create);
            stream.CopyTo(writeFile);
        }
    }
}
