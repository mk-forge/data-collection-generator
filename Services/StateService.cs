namespace TupleGeneratorGUI.Services {
    public class StateService : IDisposable {
        public TupleGeneratorWrapper.CollectionGeneratorBase? Collection { get; set; }
        public event Action<double>? ProgressChanged;
        public double CurrentProgress { get; private set; }
        private PeriodicTimer? _progressTimer;
        private CancellationTokenSource? _cts;
        private readonly BusyStateService _busyStateService;
        private readonly LocalStorageSyncService _localStorageSyncService;
        public event Action? OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public uint CollectionTupleCount { get; set; }
        public uint CollectionDimensionCount { get; set; }
        public string? CollectionFilePath { get; set; }
        public string? CollectionFileName { get; set; }
        public string? OriginalCollectionFileName { get; set; }
        public DistributionType? CollectionTab { get; set; } = DistributionType.Uniform;
        public string[] CollectionStatuses { get; set; } = new string[6];
        public string[] CollectionStatusTypes { get; set; } = new string[6] { "alert-info", "alert-info", "alert-info", "alert-info", "alert-info", "alert-info" };
        public string[] CollectionValidationStatuses { get; set; } = new string[6];
        public DistributionType? GeneratedDistributionType { get; set; }
        public uint CollectionHistogramIntervals { get; set; } = 10;
        public List<HistogramItem> CollectionHistogramItems = new List<HistogramItem>();
        public ObservableCollection<string> Tuples { get; set; } = new ObservableCollection<string>();

        public bool IsCollectionLoaded { get; set; }
        public bool IsCollectionImported { get; set; } = false;
        public bool IsCollectionChanged { get; set; } = false;
        public bool AreCollectionInsertsChanged { get; set; } = false;
        public bool ShouldRefreshCollection { get; set; } = false;
        public bool IsCollectionViewed { get; set; } = false;
        public bool IsCollectionActive => Collection != null && GeneratedDistributionType == CollectionTab;
        public bool AreQueriesActive => Queries.Count > 0 && GeneratedQueryType == QueryTab;
        public bool IsCopyingFile = false;
        public bool NeedsZOrder() => !Collection!.IsZOrdered() && !Collection.IsZOrderSortCacheValid();

        public bool UseHistogramIntervals { get; set; } = true;
        public int LastHistogramIntervals { get; set; } = -1;
        public int CollectionPage { get; set; } = 1;
        public int CollectionHistogramPage { get; set; } = 1;
        public int CollectionPageCount => (int)Math.Ceiling(CollectionTupleCount / (double)CollectionPageSize);
        public int CollectionHistogramPageSize { get; set; } = 1;
        public int CollectionIndex => (int)CollectionTab!;

        public QueryType? QueryTab { get; set; } = QueryType.Point;
        public string[] QueryStatuses { get; set; } = new string[6];
        public string[] QueryStatusTypes { get; set; } = new string[6] { "alert-info", "alert-info", "alert-info", "alert-info", "alert-info", "alert-info" };
        public string[] QueryValidationStatuses { get; set; } = new string[6];
        public List<string> Queries { get; set; } = new List<string>();
        public QueryType? GeneratedQueryType { get; set; }
        public bool ShowQueries { get; set; } = false;

        public uint GeneratedQueryCount { get; set; }
        public uint GeneratedIntervalsCount { get; set; }
        public int QueryPage { get; set; } = 1;
        public int QueryPageSize => (int)(QueryTab == QueryType.CartesianRange ? 500 / Math.Max(1, GeneratedIntervalsCount) : 500);
        public int QueryPageCount => (int)Math.Ceiling(GeneratedQueryCount / (double)QueryPageSize);
        public int QueryIndex => (int)QueryTab!;

        public int CollectionPageSize {
            get {
                double scale = 5000.0 / CollectionDimensionCount;
                int size = (int)Math.Max(100, scale);
                return (int)Math.Min(size, CollectionTupleCount);
            }
        }

        public int HistogramPageCount {
            get {
                if (UseHistogramIntervals) {
                    int pageCount = (int)Math.Ceiling(CollectionDimensionCount / (double)CollectionHistogramPageSize);
                    return Math.Max(1, pageCount);
                } else {
                    int size = 16;
                    int pageCount = (int)Math.Ceiling(CollectionDimensionCount / (double)size);
                    return Math.Max(1, pageCount);
                }
            }
        }

        public IEnumerable<HistogramItem> HistogramPageItems {
            get {
                if (UseHistogramIntervals) {
                    int size = CollectionHistogramPageSize;
                    int startDimension = (CollectionHistogramPage - 1) * size + 1;
                    int endDimension = startDimension + size - 1;
                    return CollectionHistogramItems.Where(x => x.Dimension >= startDimension && x.Dimension <= endDimension);
                } else {
                    int size = 16;
                    int startDimension = (CollectionHistogramPage - 1) * size;
                    return CollectionHistogramItems.Skip(startDimension).Take(size);
                }
            }
        }

        public IEnumerable<string> QueryPageItems {
            get {
                int startQuery = (int)((QueryPage - 1) * QueryPageSize);
                int endQuery = Math.Min(startQuery + (int)QueryPageSize, Queries.Count);
                return Queries.Skip(startQuery).Take(endQuery - startQuery);
            }
        }

        public uint TupleCount { get; set; } = 1000000;
        public uint DimensionCount { get; set; } = 16;
        public uint MinValue { get; set; } = 1;
        public uint MaxValue { get; set; } = 50000;
        public uint Mean { get; set; } = 25000;
        public uint StdDeviation { get; set; } = 5000;
        public double Buffer { get; set; } = 0.5;
        public double Percentage { get; set; } = 0.5;
        public double Probability { get; set; } = 0.5;
        public uint Digits { get; set; } = 2;

        public uint PointQueryCount { get; set; } = 100;
        public double PointQueryPercentage { get; set; } = 0.5;
        public uint PartialMatchQueryCount { get; set; } = 100;
        public double PartialMatchQueryPercentage { get; set; } = 0.5;
        public uint NarrowRangeQueryCount { get; set; } = 100;
        public uint NarrowRangeQueryDispersion { get; set; } = 50;
        public string NarrowRangeQueryMask { get; set; } = "";
        public double NarrowRangeQueryPercentage { get; set; } = 0.5;
        public double NarrowRangeQueryIntervalProbability { get; set; } = 0.7;
        public uint RangeQueryCount { get; set; } = 100;
        public uint RangeQuerySetSizeStart { get; set; } = 2;
        public uint RangeQuerySetSizeEnd { get; set; } = 6;
        public uint CartesianRangeQueryCount { get; set; } = 100;
        public uint CartesianRangeQuerySetSizeStart { get; set; } = 2;
        public uint CartesianRangeQuerySetSizeEnd { get; set; } = 10;
        public uint CartesianRangeQueryIntervals { get; set; } = 2;

        public StateService(BusyStateService busyStateService, LocalStorageSyncService localStorageSyncService) {
            _busyStateService = busyStateService;
            _localStorageSyncService = localStorageSyncService;
        }

        public void StartProgress() {
            StopProgress();
            if (IsCopyingFile) return;

            _cts = new CancellationTokenSource();
            _progressTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));

            Task.Run(async () => {
                try {
                    while (await _progressTimer.WaitForNextTickAsync(_cts.Token)) {
                        if (Collection == null) continue;
                        double progress = Collection.GetProgress();
                        CurrentProgress = progress;
                        ProgressChanged?.Invoke(progress);
                    }
                } catch (OperationCanceledException) {}
            });
        }

        public void StopProgress() {
            _cts?.Cancel();
            _progressTimer = null;
            _cts = null;
        }

        public void FinishProgress() {
            StopProgress();
    
            if (!IsCopyingFile)
                ProgressChanged?.Invoke(100);
        }

        public uint GetGeneratedQueryCount() =>
            QueryTab switch {
                QueryType.Point => PointQueryCount,
                QueryType.PartialMatch => PartialMatchQueryCount,
                QueryType.NarrowRange => NarrowRangeQueryCount,
                QueryType.Range => RangeQueryCount,
                QueryType.CartesianRange => CartesianRangeQueryCount,
                _ => 0
            };

        public void ResetCollectionState() {
            CollectionFileName = null;
            IsCollectionImported = false;
            IsCollectionLoaded = true;
            IsCollectionChanged = true;
            AreCollectionInsertsChanged = true;
            CollectionHistogramPage = 1;
            LastHistogramIntervals = -1;
            Tuples.Clear();
            CollectionHistogramItems.Clear();
        }

        public void ResetQueryState() {
            for (int i = 0; i < QueryStatuses.Length; i++) {
                QueryStatuses[i] = "";
                QueryValidationStatuses[i] = "";
            }
            
            ShowQueries = false;
            Queries.Clear();
        }

        public void ReplaceCollection(TupleGeneratorWrapper.CollectionGeneratorBase collection) {
            if (Collection != null)
                UiInteractionHelper.CleanupTempFiles();

            Collection = collection;
        }
        
        public void GenerateHistogram(uint intervalCount) {
            CollectionHistogramItems.Clear();
            Tuples.Clear();

            string histogram = Collection!.PrintHistogram(intervalCount);
            string[] lines = histogram.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<HistogramItem> histogramItems = new List<HistogramItem>();
            int index = 0;

            if (intervalCount == 0) {
                while (index < lines.Length) {
                    string[] parts = lines[index].Split(';');
                    histogramItems.Add(new HistogramItem {
                        Dimension = int.Parse(parts[0], CultureInfo.InvariantCulture),
                        Interval = 1,
                        Min = double.Parse(parts[1], CultureInfo.InvariantCulture),
                        Max = double.Parse(parts[2], CultureInfo.InvariantCulture),
                        UniqueCount = int.Parse(parts[3], CultureInfo.InvariantCulture)
                    });
                    index++;
                }
            } else {
                while (index < lines.Length) {
                    if (string.IsNullOrWhiteSpace(lines[index])) {
                        index++;
                        continue;
                    }

                    string[] dimensionParts = lines[index].Split(';');
                    int dimension = int.Parse(dimensionParts[0], CultureInfo.InvariantCulture);
                    index++;

                    for (uint i = 0; i < intervalCount && index < lines.Length; i++) {
                        if (string.IsNullOrWhiteSpace(lines[index])) {
                            index++;
                            i--;
                            continue;
                        }

                        string[] intervalParts = lines[index].Split(';');
                        histogramItems.Add(new HistogramItem {
                            Dimension = dimension,
                            Interval = int.Parse(intervalParts[0], CultureInfo.InvariantCulture),
                            Min = double.Parse(intervalParts[1], CultureInfo.InvariantCulture),
                            Max = double.Parse(intervalParts[2], CultureInfo.InvariantCulture),
                            UniqueCount = int.Parse(intervalParts[3], CultureInfo.InvariantCulture)
                        });

                        index++;
                    }
                }
            }

            CollectionHistogramItems = histogramItems;
        }

        public async Task PreviousCollectionPage() {
            CollectionPage = UiInteractionHelper.GetPreviousPage(CollectionPage, CollectionPageCount);
            await LoadTuples();
            NotifyStateChanged();
        }

        public async Task NextCollectionPage() {
            CollectionPage = UiInteractionHelper.GetNextPage(CollectionPage, CollectionPageCount);
            await LoadTuples();
            NotifyStateChanged();
        }

        public void PreviousHistogramTablePage() {
            CollectionHistogramPage = UiInteractionHelper.GetPreviousPage(CollectionHistogramPage, HistogramPageCount);
        }

        public void NextHistogramTablePage() {
            CollectionHistogramPage = UiInteractionHelper.GetNextPage(CollectionHistogramPage, HistogramPageCount);
        }

        public async Task PreviousQueryPage() {
            QueryPage = UiInteractionHelper.GetPreviousPage(QueryPage, QueryPageCount);
            await LoadQueries();
        }

        public async Task NextQueryPage() {
            QueryPage = UiInteractionHelper.GetNextPage(QueryPage, QueryPageCount);
            await LoadQueries();
        }

        public async Task ValidateCollectionPage(int page) {
            CollectionPage = UiInteractionHelper.ClampPage(page, CollectionPageCount);
            await LoadTuples();
        }

        public void ValidateHistogramPage(int page) {
            CollectionHistogramPage = UiInteractionHelper.ClampPage(page, HistogramPageCount);
        }

        public async Task ValidateQueryPage(int page) {
            QueryPage = UiInteractionHelper.ClampPage(page, QueryPageCount);
            await LoadQueries();
        }

        public void OnHistogramModeChanged(ChangeEventArgs e) {
            UseHistogramIntervals = (bool)e.Value!;
            CollectionHistogramItems.Clear();
            LastHistogramIntervals = -1;
            CollectionHistogramPage = 1;
        }

        private double GetValue(TupleGeneratorWrapper.cTuple tuple, uint index, TupleGeneratorWrapper.cSpaceDescriptor spaceDescriptor, string dataType) {
            return dataType switch {
                "UInt" => tuple.GetUInt(index, spaceDescriptor),
                "Int" => tuple.GetInt(index, spaceDescriptor),
                "Float" => tuple.GetFloat(index, spaceDescriptor),
                "Double" => tuple.GetDouble(index, spaceDescriptor),
                _ => tuple.GetInt(index, spaceDescriptor)
            };
        }

        private async Task<string> GetDataType() {
            if (IsCollectionImported && !string.IsNullOrEmpty(CollectionFilePath))
                return await FileHelper.ReadFromFile(CollectionFilePath!, "Data Type:", "");

            return GeneratedDistributionType switch {
                DistributionType.Uniform => "Int",
                DistributionType.Normal => "UInt",
                DistributionType.Logonormal => "Int",
                DistributionType.Diagonal => "Double",
                DistributionType.Sierpinski => "Double",
                DistributionType.Bit => "Float",
                _ => "Int"
            };
        }

        private string FormatTuple(TupleGeneratorWrapper.cTuple tuple, TupleGeneratorWrapper.cSpaceDescriptor spaceDescriptor, string dataType) {
            string[] parts = new string[CollectionDimensionCount];

            for (uint i = 0; i < CollectionDimensionCount; i++) {
                double value = GetValue(tuple, i, spaceDescriptor, dataType);
                parts[i] = UiInteractionHelper.FormatValue(value);
            }

            return "(" + string.Join(", ", parts) + ")";
        }

        public async Task LoadTuples() {
            TupleGeneratorWrapper.cSpaceDescriptor spaceDescriptor = Collection!.GetSpaceDescriptor();
            string dataType = await GetDataType();

            int startTuple = (CollectionPage - 1) * CollectionPageSize;
            int endTuple = Math.Min(startTuple + CollectionPageSize, (int)CollectionTupleCount);
            ObservableCollection<string> tuples = new ObservableCollection<string>();

            for (int i = startTuple; i < endTuple; i++) {
                TupleGeneratorWrapper.cTuple tuple = Collection.GetTuple((uint)i);
                tuples.Add(FormatTuple(tuple, spaceDescriptor, dataType));
            }

            Tuples.Clear();
            foreach (string tuple in tuples) {
                Tuples.Add(tuple);
            }
        }

        public async Task LoadQueries() {
            TupleGeneratorWrapper.cSpaceDescriptor spaceDescriptor = Collection!.GetSpaceDescriptor();
            string dataType = await GetDataType();
            int startQuery = (QueryPage - 1) * QueryPageSize;
            int endQuery = Math.Min(startQuery + QueryPageSize, (int)GetGeneratedQueryCount());
            List<string> queries = new List<string>();

            for (uint queryIndex = (uint)startQuery; queryIndex < endQuery; queryIndex++) {
                bool isLastQuery = (queryIndex + 1 == endQuery);

                if (QueryTab == QueryType.Range) {
                    TupleGeneratorWrapper.RangeQuery rq = Collection.GetRangeQuery(queryIndex);
                    int queryCount = rq.tuples.Count;
                    string ql = FormatTuple(rq.ql, spaceDescriptor, dataType);
                    string qh = FormatTuple(rq.qh, spaceDescriptor, dataType);

                    queries.Add(
                        $"<div class=\"main-title\"><strong>{queryIndex + 1}. dotaz</strong> ({UiInteractionHelper.FormatResults(queryCount)}):</div>" +
                        $"QL = {ql}<br>QH = {qh}" + (isLastQuery ? "" : "<br><br>")
                    );
                } else if (QueryTab == QueryType.CartesianRange) {
                    TupleGeneratorWrapper.CartesianRangeQuery crq = Collection.GetCartesianRangeQuery(queryIndex);
                    StringBuilder sb = new StringBuilder();

                    int queryCount = 0;
                    for (int intervalIndex = 0; intervalIndex < crq.intervals.Count; intervalIndex++)
                        queryCount += crq.intervals[intervalIndex].tuples.Count;

                    sb.Append($"<div class=\"main-title\"><strong>{queryIndex + 1}. dotaz</strong> ({UiInteractionHelper.FormatResults(queryCount)}):</div>");

                    for (int intervalIndex = 0; intervalIndex < crq.intervals.Count; intervalIndex++) {
                        TupleGeneratorWrapper.CartesianRangeInterval interval = crq.intervals[intervalIndex];
                        int intervalCount = interval.tuples.Count;
                        bool isLastInterval = (intervalIndex + 1 == crq.intervals.Count);
                        string ql = FormatTuple(interval.ql, spaceDescriptor, dataType);
                        string qh = FormatTuple(interval.qh, spaceDescriptor, dataType);

                        sb.Append(
                            $"<div class=\"interval-title\"><strong>{intervalIndex + 1}. interval</strong> ({UiInteractionHelper.FormatResults(intervalCount)}):</div>" +
                            $"QL<sub>{intervalIndex}</sub> = {ql}<br>" + $"QH<sub>{intervalIndex}</sub> = {qh}" + (isLastInterval ? "" : "<br><br>")
                        );
                    }

                    if (!isLastQuery) sb.Append("<br><br>");
                    queries.Add(sb.ToString());
                } else if (QueryTab == QueryType.PartialMatch) {
                    TupleGeneratorWrapper.cTuple qlTuple = Collection.GetQuery(queryIndex * 2);
                    TupleGeneratorWrapper.cTuple qhTuple = Collection.GetQuery(queryIndex * 2 + 1);
                    int firstMismatchIndex = -1;

                    for (uint i = 0; i < CollectionDimensionCount; i++) {
                        double qlValue = GetValue(qlTuple, i, spaceDescriptor, dataType);
                        double qhValue = GetValue(qhTuple, i, spaceDescriptor, dataType);

                        if (qlValue != qhValue) {
                            firstMismatchIndex = (int)i;
                            break;
                        }
                    }

                    string[] qlParts = new string[CollectionDimensionCount];
                    string[] qhParts = new string[CollectionDimensionCount];

                    for (uint i = 0; i < CollectionDimensionCount; i++) {
                        if (firstMismatchIndex != -1 && i > firstMismatchIndex) {
                            qlParts[i] = "min";
                            qhParts[i] = "max";
                            continue;
                        }

                        double qlValue = GetValue(qlTuple, i, spaceDescriptor, dataType);
                        double qhValue = GetValue(qhTuple, i, spaceDescriptor, dataType);
                        qlParts[i] = UiInteractionHelper.FormatValue(qlValue);
                        qhParts[i] = UiInteractionHelper.FormatValue(qhValue);
                    }

                    string ql = "(" + string.Join(", ", qlParts) + ")";
                    string qh = "(" + string.Join(", ", qhParts) + ")";

                    queries.Add(
                        $"<div class=\"main-title\"><strong>{queryIndex + 1}. dotaz:</strong></div>" +
                        $"QL = {ql}<br/>QH = {qh}" + (isLastQuery ? "" : "<br/><br/>")
                    );
                } else {
                    TupleGeneratorWrapper.cTuple qlTuple = Collection.GetQuery(queryIndex * 2);
                    TupleGeneratorWrapper.cTuple qhTuple = Collection.GetQuery(queryIndex * 2 + 1);
                    string ql = FormatTuple(qlTuple, spaceDescriptor, dataType);
                    string qh = FormatTuple(qhTuple, spaceDescriptor, dataType);

                    queries.Add(
                        $"<div class=\"main-title\"><strong>{queryIndex + 1}. dotaz:</strong></div>" +
                        $"QL = {ql}<br/>QH = {qh}" + (isLastQuery ? "" : "<br/><br/>")
                    );
                }
            }

            Queries.Clear();
            foreach (string query in queries) {
                Queries.Add(query);
            }
        }

        public async Task ImportCollection(string filePath, string fileName, Action<string, string> setStatus) {
            string dataType = await FileHelper.ReadFromFile(filePath, "Data Type:", "");
            uint tupleCount = uint.Parse(await FileHelper.ReadFromFile(filePath, "Tuples Count:", "2"));
            uint dimensionCount = uint.Parse(await FileHelper.ReadFromFile(filePath, "Dimension:", "2"));

            if (dataType != "UInt" && dataType != "Int" && dataType != "Float" && dataType != "Double") {
                setStatus("Neznámý datový typ v souboru.", "alert-danger");
                return;
            }

            Collection?.Cleanup();

            ReplaceCollection(dataType switch {
                "UInt" => new TupleGeneratorWrapper.CollectionGenerator_cUInt(filePath),
                "Int" => new TupleGeneratorWrapper.CollectionGenerator_cInt(filePath),
                "Float" => new TupleGeneratorWrapper.CollectionGenerator_cFloat(filePath),
                "Double" => new TupleGeneratorWrapper.CollectionGenerator_cDouble(filePath),
                _ => null!
            });

            await Task.Run(() => Collection?.Load());

            CollectionTupleCount = tupleCount;
            CollectionDimensionCount = dimensionCount;
            CollectionFilePath = filePath;
            CollectionFileName = fileName;
            IsCollectionLoaded = true;
            ResetCollectionState();
            ResetQueryState();
            GeneratedDistributionType = CollectionTab;
            IsCollectionImported = true;
            await _localStorageSyncService.SetCollection(tupleCount.ToString(), dimensionCount.ToString(), "", fileName, filePath, OriginalCollectionFileName ?? fileName);
        }

        public async Task RunWithProgressCollection(Func<Task> action, string successMessage, string errorPrefix, Func<Task>? onFinally = null) {
            _busyStateService.SetBusy(true);
            Tuples.Clear();
            NotifyStateChanged();
            if (!IsCopyingFile)
                StartProgress();

            string originalStatus = CollectionStatuses[CollectionIndex];

            try {
                await action();

                if (!string.IsNullOrEmpty(successMessage) && CollectionStatuses[CollectionIndex] == originalStatus) {
                    CollectionStatuses[CollectionIndex] = successMessage;
                    CollectionStatusTypes[CollectionIndex] = "alert-success";
                }
            } catch (Exception ex) {
                CollectionStatuses[CollectionIndex] = $"{errorPrefix}: {ex.Message}";
                CollectionStatusTypes[CollectionIndex] = "alert-danger";
            } finally {
                FinishProgress();
                _busyStateService.SetBusy(false);

                if (onFinally != null)
                    await onFinally();
            }
        }

        public async Task RunWithProgressQuery(Func<Task> action, string successMessage, string errorPrefix, Func<Task>? onFinally = null) {
            _busyStateService.SetBusy(true);
            if (!IsCopyingFile)
                StartProgress();

            string originalStatus = QueryStatuses[QueryIndex];

            try {
                await action();

                if (!string.IsNullOrEmpty(successMessage)) {
                    if (QueryStatuses[QueryIndex] == originalStatus || QueryStatuses[QueryIndex] == "Probíhá seřazení datové kolekce podle Z-Order...") {
                        QueryStatuses[QueryIndex] = successMessage;
                        QueryStatusTypes[QueryIndex] = "alert-success";
                    }
                }
            } catch (Exception ex) {
                QueryStatuses[QueryIndex] = $"{errorPrefix}: {ex.Message}";
                QueryStatusTypes[QueryIndex] = "alert-danger";
            } finally {
                FinishProgress();
                _busyStateService.SetBusy(false);

                if (onFinally != null)
                    await onFinally();
            }
        }

        public void Dispose() {
            StopProgress();
            _cts?.Dispose();
            _progressTimer?.Dispose();
            Collection = null;
        }
    }
}