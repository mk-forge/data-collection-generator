namespace TupleGeneratorGUI.Helpers {
    public static class UiInteractionHelper {
        public static void SetStatus(string[] statuses, string[] types, int index, string status, string type) {
            statuses[index] = status;
            types[index] = type;
        }

        public static void ResetOtherTabs(string[] statuses, int index) {
            for (int i = 0; i < statuses.Length; i++) {
                if (i != index)
                    statuses[i] = "";
            }
        }

        public static string FormatResults(int count) {
            if (count == 1)
                return "1 výsledek";

            if (count >= 2 && count <= 4)
                return $"{count} výsledky";

            return $"{count} výsledků";
        }
    
        public static string FormatValue(double value) {
            if (value == 0.0)
                return "0";
    
            string formatted = value.ToString("F8", CultureInfo.InvariantCulture);
    
            formatted = formatted.TrimEnd('0');
            if (formatted.EndsWith('.'))
                formatted = formatted.TrimEnd('.');
    
            return formatted;
        }

        public static string FormatNumber(uint value) {
            CultureInfo cultureInfo = new CultureInfo("cs-CZ");

            if (value >= 1_000_000)
                return (Math.Round(value / 1_000_000.0, 1)).ToString("0.#", cultureInfo) + "M";

            if (value >= 1_000)
                return (Math.Round(value / 1_000.0)).ToString("0", cultureInfo) + "K";

            return value.ToString(cultureInfo);
        }

        public static string GetDistributionName(DistributionType type) {
            return type switch {
                DistributionType.Uniform => "uniformní",
                DistributionType.Normal => "normální",
                DistributionType.Logonormal => "logonormální",
                DistributionType.Diagonal => "diagonální",
                DistributionType.Sierpinski => "Sierpińského",
                DistributionType.Bit => "bitová",
                _ => ""
            };
        }

        public static string CreateMask(uint dimensionCount) {
            string mask = "0";

            for (uint i = 1; i < dimensionCount; i++)
                mask += ",0";

            return mask;
        }

        public static string CreateRandomMask(uint dimensionCount) {
            Random random = new Random();
            List<string> values = new List<string>();
    
            for (uint i = 0; i < dimensionCount; i++)
                values.Add(random.Next(2).ToString());
    
            return string.Join(",", values);
        }

        public static int GetPreviousPage(int page, int pageCount) {
            return page > 1 ? page - 1 : pageCount;
        }

        public static int GetNextPage(int page, int pageCount) {
            return page < pageCount ? page + 1 : 1;
        }

        public static int ClampPage(int page, int max) {
            return page < 1 ? 1 : (page > max ? max : page);
        }

        public static async Task HandleEnter(KeyboardEventArgs e, bool isBusy, Func<Task> function) {
            if (e.Key == "Enter" && !isBusy)
                await function();
        }

        public static void CleanupTempFiles() {
            foreach (string tempFile in Directory.GetFiles(Environment.CurrentDirectory, "*.bin")) {
                try {
                    File.Delete(tempFile);
                } catch {
                }
            }
        }
    }
}