namespace TupleGeneratorGUI.Components.Shared {
    public partial class ProgressBar : ComponentBase {
        public bool Visible { get; private set; } = false;
        public int Value { get; private set; } = 0;
        private CancellationTokenSource? cts;
        private bool finished = false;

        public void SetProgress(int value) {
            if (finished && value == 100)
                return;

            int previous = Value;
            Value = Math.Clamp(value, 0, 100);

            if (Value > 0 && Value < 100) {
                Visible = true;
                finished = false;
            }

            cts?.Cancel();
            cts = null;

            if (Value == 100 && previous < 100) {
                finished = true;
                cts = new CancellationTokenSource();
                CancellationToken token = cts.Token;
                Task task = HideAfterDelay(token);
            }

            InvokeAsync(StateHasChanged);
        }

        private async Task HideAfterDelay(CancellationToken token) {
            try {
                await Task.Delay(300, token);
                if (token.IsCancellationRequested)
                    return;

                Visible = false;
                Value = 0;
                await InvokeAsync(StateHasChanged);
            } catch (TaskCanceledException) {
            }
        }
    }
}