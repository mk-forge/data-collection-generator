namespace TupleGeneratorGUI.Services {
    public class BusyStateService {
        public bool IsBusy;
        public event Action? OnChange;

        public void SetBusy(bool isBusy) {
            if (IsBusy != isBusy) {
                IsBusy = isBusy;
                OnChange?.Invoke();
            }
        }
    }
}