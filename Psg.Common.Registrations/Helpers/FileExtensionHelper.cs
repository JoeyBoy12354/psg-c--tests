namespace Psg.Common.Registrations.Helpers
{

    public static class FileExtensionHelper
    {
        public static bool IsZlib(string? base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return false;

            byte[] bytes = Convert.FromBase64String(base64String);

            return IsZlib(bytes);
        }

        public static bool IsZlib(byte[]? bytes)
        {
            if (bytes == null)
                return false;

            // Check the first few bytes to determine the file type
            if (bytes.Length > 1 && bytes[0] == 0x78 && (bytes[1] == 0x01 || bytes[1] == 0x9C || bytes[1] == 0xDA))
            {
                return true; // Zlib compressed data
            }

            return false;
        }

        public static string DetectFileType(string? base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return "No Data!";

            byte[] bytes = Convert.FromBase64String(base64String);

            var result = DetectFileType(bytes);

            return result;
        }

        public static string DetectFileType(byte[]? bytes)
        {
            if (bytes == null)
                return "No Data!";

            // Check the first few bytes to determine the file type
            if (bytes.Length > 1 && bytes[0] == 0x78 && (bytes[1] == 0x01 || bytes[1] == 0x9C || bytes[1] == 0xDA))
            {
                return ".zlib"; // Zlib compressed data
            }

            if (bytes.Length > 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            {
                return ".jpg"; // JPEG image
            }

            if (bytes.Length > 3 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            {
                return ".png"; // PNG image
            }

            if (bytes.Length > 2 && bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
            {
                return ".gif"; // GIF image
            }

            if (bytes.Length > 1 && bytes[0] == 0x42 && bytes[1] == 0x4D)
            {
                return ".bmp"; // BMP image
            }

            if (bytes.Length > 3 && bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46)
            {
                return ".pdf"; // PDF document
            }

            if (bytes.Length > 3 && bytes[0] == 0x50 && bytes[1] == 0x4B && bytes[2] == 0x03 && bytes[3] == 0x04)
            {
                return ".zip"; // ZIP archive
            }

            if (bytes.Length > 1 && bytes[0] == 0xD0 && bytes[1] == 0xCF && bytes[2] == 0x11 && bytes[3] == 0xE0)
            {
                return ".doc"; // Microsoft Word document
            }

            if (bytes.Length > 7 && bytes[0] == 0x09 && bytes[1] == 0x08 && bytes[2] == 0x10 && bytes[3] == 0x00 &&
                     (bytes[6] == 0x00 && bytes[7] == 0x00 || bytes[6] == 0x02 && bytes[7] == 0x00))
            {
                return ".xls"; // Microsoft Excel spreadsheet
            }

            if (bytes.Length > 3 && bytes[0] == 0x50 && bytes[1] == 0x4B && bytes[2] == 0x07 && bytes[3] == 0x08)
            {
                return ".zipx"; // ZIPX archive
            }

            if (bytes.Length > 1 && bytes[0] == 0x37 && bytes[1] == 0x7A && bytes[2] == 0xBC && bytes[3] == 0xAF)
            {
                return ".7z"; // 7z archive
            }

            if (bytes.Length > 6 && bytes[0] == 0x52 && bytes[1] == 0x61 && bytes[2] == 0x72 && bytes[3] == 0x21 &&
                     bytes[4] == 0x1A && bytes[5] == 0x07 && (bytes[6] == 0x00 || bytes[6] == 0x01 || bytes[6] == 0x02 || bytes[6] == 0x03))
            {
                return ".rar"; // RAR archive
            }

            if (bytes.Length > 1 && bytes[0] == 0x78 && (bytes[1] == 0x01 || bytes[1] == 0x9C || bytes[1] == 0xDA))
            {
                return ".zlib"; // Zlib compressed data
            }
            // Add more checks for other file types as needed...


            return ".dat"; // Default to generic binary data
        }

    }

}
