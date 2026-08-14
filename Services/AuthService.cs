namespace TupleGeneratorGUI.Services {
    public class AuthService {
        private readonly ProtectedSessionStorage storage;
        public bool IsLoggedIn { get; private set; }
        public string? Username { get; private set; }
        public event Action? OnChange;

        public AuthService(ProtectedSessionStorage storage) {
            this.storage = storage;
        }

        public async Task InitializeAsync() {
            ProtectedBrowserStorageResult<string> result = await storage.GetAsync<string>("username");

            if (result.Success && result.Value != null) {
                Username = result.Value;
                IsLoggedIn = true;
            }

            OnChange?.Invoke();
        }

        public async Task SetLoggedIn(string username) {
            Username = username;
            IsLoggedIn = true;
            await storage.SetAsync("username", username);
            OnChange?.Invoke();
        }

        public async Task SetLoggedOut() {
            Username = null;
            IsLoggedIn = false;
            await storage.DeleteAsync("username");
            OnChange?.Invoke();
        }
    }
}