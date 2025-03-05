namespace psg_automated_nunit_common.Managers
{
    public static class ScriptManager
    {
        public static int FilesCount => _files.Count;

        private static Dictionary<string, string> _files = GetFiles();
       

        static Dictionary<string, string> GetFiles()
        {         
            var cssfolder = "Scripts";

            if (!Directory.Exists(cssfolder))
                return [];

            return Directory.GetFiles(cssfolder).ToDictionary(x => Path.GetFileName(x).ToLower(), x => x);
        }

        public static string GetScript(string fileName)
        {
            if(string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            if (_files.TryGetValue(fileName.ToLower(), out var filePath))
            {
                var file = File.ReadAllText(filePath);
                return file;
            }

            return "";
        }
       
    }
}
