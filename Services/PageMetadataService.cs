namespace TupleGeneratorGUI.Services {
    public class PageMetadataService : IDisposable {
        private string title = "";
        private string favicon = "";
        public string Favicon => favicon;
        public event Action? OnChange;

        public void SetTitle(string text) {
            if (title != text) {
                title = text;
                favicon = text switch {
                    "Generátor datových kolekcí" => "favicon_data_collections.png?v=2",
                    "Generátor dotazů" => "favicon_queries.png?v=2",
                    "Přihlášení" => "favicon_login.png?v=2",
                    "Neexistující stránka" => "favicon_error.png?v=2",
                    _ => "favicon.png?v=2"
                };

                OnChange?.Invoke();
            }
        }

        public void Dispose() {
            OnChange = null;
        }
    }
}