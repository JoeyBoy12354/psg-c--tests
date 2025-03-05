using System.Reflection;

namespace psg_automated_nunit_common.Helpers
{
    /// <summary>
    /// Helps with file operations.
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Based on the <c>path</c>, it will return the full folder path relative to the executing directory.
        /// <br/> If the <c>path</c> is a foldername, ex. 'screenshots', the the <c>path</c> will be D:\\executingDirectory\\screenshots.
        /// <br/> If the <c>path</c> is an absolute path, ex. 'D:\\app\\screenshots', then that will be used instead.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFolderPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return Directory.GetCurrentDirectory();

            // check for absolute path ex. C:\\something\\folderName
            if(path.Contains(':'))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                return path;
            }
          
            var bin = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location) ?? "";

            if (string.IsNullOrWhiteSpace(bin))
                throw new DirectoryNotFoundException("Cannot find entry Assembly!");

            var baseFolder = Path.Combine(bin, path);

            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            return baseFolder;
        }
    }
}
