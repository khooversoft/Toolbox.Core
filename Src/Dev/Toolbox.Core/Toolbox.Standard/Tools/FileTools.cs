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
        /// <summary>
        /// Write assembly resource to temp file
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="folder">folder name under temp directory</param>
        /// <param name="type">assembly containing the resource</param>
        /// <param name="resourceId">resource id</param>
        /// <returns>output file</returns>
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

        /// <summary>
        /// Get file's hash (MD5)
        /// </summary>
        /// <param name="file">file to hash</param>
        /// <returns>MD5 hash byte array</returns>
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
        public static Stream GetResourceStream(this Type type, string streamId)
        {
            type.VerifyNotNull(nameof(type));

            return Assembly.GetAssembly(type)!
                    .GetManifestResourceStream(streamId.VerifyNotEmpty(nameof(streamId)))
                    .VerifyNotNull($"Cannot find {streamId} in assembly's resource");
        }

        /// <summary>
        /// Write stream to file
        /// </summary>
        /// <param name="stream">stream to write</param>
        /// <param name="file">full file</param>
        public static void WriteStreamToFile(this Stream stream, string file)
        {
            using Stream writeFile = new FileStream(file, FileMode.Create);
            stream.CopyTo(writeFile);
        }
    }
}
