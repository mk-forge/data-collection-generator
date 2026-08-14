namespace TupleGeneratorGUI.Services {
    public class LocalStorageSyncService {
        private readonly IJSRuntime JS;

        public LocalStorageSyncService(IJSRuntime js) {
            JS = js;
        }
    
        public async Task SetCollection(string tupleCount, string dimensionCount, string distribution, string fileName, string filePath, string originalFileName) {
            await JS.InvokeVoidAsync("SetCollection", "tupleCount", tupleCount);
            await JS.InvokeVoidAsync("SetCollection", "dimensionCount", dimensionCount);
            await JS.InvokeVoidAsync("SetCollection", "distribution", distribution);
            await JS.InvokeVoidAsync("SetCollection", "fileName", fileName);
            await JS.InvokeVoidAsync("SetCollection", "filePath", filePath);
            await JS.InvokeVoidAsync("SetCollection", "originalFileName", originalFileName);
        }

        public async Task<(string tupleCount, string dimensionCount, string distribution, string fileName, string filePath, string originalFileName)> GetCollection() {
            string tupleCount = await JS.InvokeAsync<string>("GetCollection", "tupleCount");
            string dimensionCount = await JS.InvokeAsync<string>("GetCollection", "dimensionCount");
            string distribution = await JS.InvokeAsync<string>("GetCollection", "distribution");
            string fileName = await JS.InvokeAsync<string>("GetCollection", "fileName");
            string filePath = await JS.InvokeAsync<string>("GetCollection", "filePath");
            string originalFileName = await JS.InvokeAsync<string>("GetCollection", "originalFileName");

            return (tupleCount, dimensionCount, distribution, fileName, filePath, originalFileName);
        }
    }
}