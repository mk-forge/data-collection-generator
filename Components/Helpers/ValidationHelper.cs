namespace TupleGeneratorGUI.Helpers {
    public static class ValidationHelper {
        public static void ValidateCount(uint count, StringBuilder errors) {
            if (count < 1)
                errors.Append("Počet dotazů musí být alespoň 1.<br/>");
        }

        public static void ValidatePercentage(double value, StringBuilder errors) {
            if (value < 0.0 || value > 1.0)
                errors.Append("Podíl prázdných dotazů musí být v rozsahu 0 – 100 %.<br/>");
        }

        public static void ValidateIntervalProbability(double value, StringBuilder errors) {
            if (value < 0.01 || value > 1.0)
                errors.Append("Pravděpodobnost intervalu musí být v rozsahu 1 – 100 %.<br/>");
        }

        public static void ValidateMinAtLeast(uint value, uint min, string label, StringBuilder errors) {
            if (value < min)
                errors.Append($"{label} musí být alespoň {min}.<br/>");
        }

        public static void ValidateRange(double value, double min, double max, string label, StringBuilder errors) {
            if (value < min || value > max)
                errors.Append($"{label} musí být v rozsahu {min}–{max}.<br/>");
        }

        public static void ValidateMinMax(uint min, uint max, StringBuilder errors) {
            if (max <= min)
                errors.Append("Maximální hodnota musí být větší než minimální hodnota.<br/>");
        }

        public static void ValidateRangeSizes(uint start, uint end, uint tupleCount, StringBuilder errors) {
            if (start >= tupleCount)
                errors.Append("Minimální počet n‑tic v dotazu nesmí být větší nebo roven počtu n‑tic datové kolekce.<br/>");

            if (end < 1)
                errors.Append("Maximální počet n‑tic v dotazu musí být alespoň 1.<br/>");

            if (end <= start)
                errors.Append("Maximální počet n‑tic v dotazu nesmí být menší než minimální počet.<br/>");

            if (end >= tupleCount)
                errors.Append("Maximální počet n‑tic v dotazu nesmí být větší nebo roven počtu n‑tic datové kolekce.<br/>");
        }

        public static bool ValidateMask(string mask, uint dimensionCount) {
            if (string.IsNullOrWhiteSpace(mask))
                return false;

            string[] parts = mask.Split(',');

            if (parts.Length != dimensionCount)
                return false;

            for (int i = 0; i < parts.Length; i++) {
                string part = parts[i];
                if (string.IsNullOrWhiteSpace(part))
                    return false;

                if (part.Length != 1)
                    return false;

                if (!uint.TryParse(part, out uint value))
                    return false;

                if (value > 1)
                    return false;
            }

            return true;
        }

        public static void ValidateMask(string mask, uint dimensionCount, StringBuilder errors) {
            if (!ValidateMask(mask, dimensionCount))
                errors.Append("Maska není platná.<br/>");
        }

        public static void ValidateIntervals(uint end, uint intervalCount, StringBuilder errors) {
            if (end < 2 * intervalCount) {
                errors.Append($"Maximální počet n‑tic v dotazu musí být alespoň {2 * intervalCount}, protože každý interval má QL a QH.<br/>");
            }
        }

        public static void ValidateEven(uint value, string label, StringBuilder errors) {
            if (value % 2 != 0)
                errors.Append($"{label} musí být sudý.<br/>");
        }

        public static void ValidateHistogramIntervals(uint intervalCount, uint tupleCount, StringBuilder errors) {
            if (intervalCount < 2)
                errors.Append("Počet intervalů musí být alespoň 2.<br/>");

            if (intervalCount > tupleCount)
                errors.Append($"Počet intervalů nesmí být větší než počet n-tic.<br/>");
        }
    }
}