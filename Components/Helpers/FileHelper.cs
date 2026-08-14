namespace TupleGeneratorGUI.Helpers {
    public static class FileHelper {
        private static readonly string UserDataPath = Path.Combine("UserData");

        public static void CreateUserDirectories(string username) {
            string root = Path.Combine(UserDataPath, username);
            Directory.CreateDirectory(root);
            Directory.CreateDirectory(Path.Combine(root, "ExportedCollections"));
            Directory.CreateDirectory(Path.Combine(root, "ImportedCollections"));
            Directory.CreateDirectory(Path.Combine(root, "Histograms"));
            Directory.CreateDirectory(Path.Combine(root, "Queries"));
            Directory.CreateDirectory(Path.Combine(root, "Selects"));
            Directory.CreateDirectory(Path.Combine(root, "Inserts"));
        }

        public static string GetUserExportCollectionPath(string username, string fileName)
            => Path.Combine(UserDataPath, username, "ExportedCollections", fileName);
        public static string GetUserImportCollectionPath(string username, string fileName)
            => Path.Combine(UserDataPath, username, "ImportedCollections", fileName);
        public static string GetUserHistogramPath(string username, string fileName)
            => Path.Combine(UserDataPath, username, "Histograms", fileName);
        public static string GetUserQueryExportPath(string username, string fileName)
            => Path.Combine(UserDataPath, username, "Queries", fileName);
        public static string GetUserSelectsPath(string username, string fileName)
            => Path.Combine(UserDataPath, username, "Selects", fileName);
        public static string GetUserInsertsPath(string username, string fileName)
            => Path.Combine(UserDataPath, username, "Inserts", fileName);

        public static async Task<string> ReadFromFile(string filePath, string prefix, string defaultValue) {
            using (StreamReader reader = new StreamReader(filePath)) {
                for (int i = 0; i < 3; i++) {
                    string? line = await reader.ReadLineAsync();

                    if (line != null && line.StartsWith(prefix))
                        return line.Substring(prefix.Length).Trim();
                }
            }

            return defaultValue;
        }
    }
}