using Ionic.Zlib;
using System.Text;
using CompressionMode = Ionic.Zlib.CompressionMode;

namespace Psg.Common.Registrations.Helpers
{
    public static class GzipHelper
    {
        public static async Task<MemoryStream> DecompressBase64ToStreamAsync(string base64)
        {
            var fileBytes = Convert.FromBase64String(base64);

            MemoryStream outputStream = new MemoryStream();

            using var inputStream = new MemoryStream(fileBytes);

            using (ZlibStream zlibStream = new ZlibStream(inputStream, CompressionMode.Decompress))
            {
                await zlibStream.CopyToAsync(outputStream);
                outputStream.Position = 0; // Reset position to the beginning of the stream
                return outputStream;
            }
        }

        public static async ValueTask<MemoryStream?> DecompressBytesToStreamAsync(byte[]? fileBytes)
        {
            if (fileBytes == null)
                return null;

            MemoryStream outputStream = new MemoryStream();

            using var inputStream = new MemoryStream(fileBytes);

            using (ZlibStream zlibStream = new ZlibStream(inputStream, CompressionMode.Decompress))
            {
                await zlibStream.CopyToAsync(outputStream);
                outputStream.Position = 0; // Reset position to the beginning of the stream
                return outputStream;
            }
        }


        public static async Task<string> DecompressToStringAsync(byte[] compressedBytes)
        {
            using (MemoryStream inputStream = new MemoryStream(compressedBytes))
            using (MemoryStream outputStream = new MemoryStream())
            using (ZlibStream zlibStream = new ZlibStream(inputStream, CompressionMode.Decompress))
            {
                await zlibStream.CopyToAsync(outputStream);
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }
        }      
    }
}
