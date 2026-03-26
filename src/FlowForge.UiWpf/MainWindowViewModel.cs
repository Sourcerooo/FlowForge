using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace FlowForge.UiWpf;

public sealed class MainWindowViewModel : ObservableObject
{
    private OverviewScreenViewModel _overviewScreen;
    private TimelineScreenViewModel _timelineScreen;
    private ScenariosScreenViewModel _scenariosScreen;
    private AlertsScreenViewModel _alertsScreen;
    private SettingsScreenViewModel _settingsScreen;
    private RecommendationsScreenViewModel _recommendationsScreen;

    private bool _isDarkTheme;

    public MainWindowViewModel()
    {
        _overviewScreen = OverviewScreenViewModel.CreateSample();
        _timelineScreen = TimelineScreenViewModel.CreateSample(_overviewScreen);
        _scenariosScreen = ScenariosScreenViewModel.CreateSample();
        _alertsScreen = AlertsScreenViewModel.CreateSample(_overviewScreen.Alerts);
        _settingsScreen = SettingsScreenViewModel.CreateSample();
        _recommendationsScreen = RecommendationsScreenViewModel.CreateSample(_overviewScreen.Recommendations);
        CurrentScreen = _overviewScreen;

        NavigationItems.Add(new NavigationItemViewModel("Overview", "\uE80F", "Live process graph and KPI summary", true));
        NavigationItems.Add(new NavigationItemViewModel("Timeline", "\uE823", "Event playback and inspection controls", false));
        NavigationItems.Add(new NavigationItemViewModel("Scenarios", "\uE7C3", "Scenario presets and branch comparisons", false));
        NavigationItems.Add(new NavigationItemViewModel("Alerts", "\uE7BA", "Operational warnings and risk surfacing", false));
        NavigationItems.Add(new NavigationItemViewModel("Settings", "\uE713", "Runtime knobs for demo interaction", false));

        RunCommand = new DelegateCommand(_ => SetSimulationMode("Running", "Mock playback active", "Run pressed", "Signal", AppTheme.InfoSurfaceBrushKey, AppTheme.PrimaryAccentBrushKey));
        PauseCommand = new DelegateCommand(_ => SetSimulationMode("Paused", "Playback frozen for inspection", "Pause pressed", "Hold", AppTheme.WarningSurfaceBrushKey, AppTheme.WarningBrushKey));
        ResetCommand = new DelegateCommand(_ => SetSimulationMode("Reset", "Mock state returned to baseline", "Reset pressed", "Reset", AppTheme.DangerSurfaceBrushKey, AppTheme.DangerBrushKey));
        LoadScenarioCommand = new DelegateCommand(_ => _overviewScreen.AddEvent("Scenario panel opened", "Panel", AppTheme.SuccessSurfaceBrushKey, AppTheme.SuccessBrushKey));
        SelectNavigationCommand = new DelegateCommand(parameter => SelectNavigation(parameter as NavigationItemViewModel));
        SelectStageCommand = new DelegateCommand(parameter => _overviewScreen.SelectStage(parameter as StageSummaryViewModel));
        ToggleThemeCommand = new DelegateCommand(_ => ToggleTheme());
        OpenRecommendationsCommand = new DelegateCommand(_ => ShowRecommendationsScreen());
        OpenAlertsCommand = new DelegateCommand(_ => ShowAlertsScreen());
        ReturnToOverviewCommand = new DelegateCommand(_ => SelectNavigation(NavigationItems[0]));
        FocusStageInOverviewCommand = new DelegateCommand(parameter => FocusStageInOverview(parameter as string));

        CurrentWorkspaceTitle = NavigationItems[0].Label;
        CurrentWorkspaceDescription = NavigationItems[0].Description;
    }

    public ObservableCollection<NavigationItemViewModel> NavigationItems { get; } = new();

    public ICommand RunCommand { get; }

    public ICommand PauseCommand { get; }

    public ICommand ResetCommand { get; }

    public ICommand LoadScenarioCommand { get; }

    public ICommand SelectNavigationCommand { get; }

    public ICommand SelectStageCommand { get; }

    public ICommand ToggleThemeCommand { get; }

    public ICommand OpenRecommendationsCommand { get; }

    public ICommand OpenAlertsCommand { get; }

    public ICommand ReturnToOverviewCommand { get; }

    public ICommand FocusStageInOverviewCommand { get; }

    public object CurrentScreen
    {
        get;
        private set => SetProperty(ref field, value);
    }
    = null!;

    public string CurrentWorkspaceTitle
    {
        get;
        set => SetProperty(ref field, value);
    }
    = string.Empty;

    public string CurrentWorkspaceDescription
    {
        get;
        set => SetProperty(ref field, value);
    }
    = string.Empty;

    public string SimulationModeLabel
    {
        get;
        set => SetProperty(ref field, value);
    }
    = "Ready";

    public string SimulationModeDetail
    {
        get;
        set => SetProperty(ref field, value);
    }
    = "Demo controls available";

    public string ThemeButtonLabel
    {
        get;
        private set => SetProperty(ref field, value);
    }
    = "Dark Theme";

    public static MainWindowViewModel CreateSample() => new();

    private void SelectNavigation(NavigationItemViewModel? item)
    {
        if (item is null)
        {
            return;
        }

        foreach (var navigationItem in NavigationItems)
        {
            navigationItem.IsSelected = navigationItem == item;
        }

        CurrentWorkspaceTitle = item.Label;
        CurrentWorkspaceDescription = item.Description;
        CurrentScreen = item.Label switch
        {
            "Timeline" => _timelineScreen,
            "Scenarios" => _scenariosScreen,
            "Alerts" => _alertsScreen,
            "Settings" => _settingsScreen,
            _ => _overviewScreen,
        };

        _overviewScreen.AddEvent($"Workspace switched to {item.Label}", "Nav", AppTheme.InfoSurfaceBrushKey, AppTheme.PrimaryAccentBrushKey, false);
    }

    private void ShowAlertsScreen()
    {
        foreach (var navigationItem in NavigationItems)
        {
            navigationItem.IsSelected = navigationItem.Label == "Alerts";
        }

        CurrentWorkspaceTitle = "Alerts";
        CurrentWorkspaceDescription = "Operational warnings and mitigation context";
        CurrentScreen = _alertsScreen;
    }

    private void ShowRecommendationsScreen()
    {
        foreach (var navigationItem in NavigationItems)
        {
            navigationItem.IsSelected = false;
        }

        CurrentWorkspaceTitle = "Recommendations";
        CurrentWorkspaceDescription = "Guided next actions and mitigation suggestions";
        CurrentScreen = _recommendationsScreen;
    }

    private void SetSimulationMode(string label, string detail, string eventTitle, string badge, string badgeBackground, string badgeForeground)
    {
        SimulationModeLabel = label;
        SimulationModeDetail = detail;
        _overviewScreen.AddEvent(eventTitle, badge, badgeBackground, badgeForeground);
    }

    private void ToggleTheme()
    {
        _isDarkTheme = !_isDarkTheme;
        ThemeManager.ApplyTheme(_isDarkTheme ? AppThemeMode.Dark : AppThemeMode.Light);
        ThemeButtonLabel = _isDarkTheme ? "Light Theme" : "Dark Theme";

        var activeWorkspace = CurrentWorkspaceTitle;
        RebuildScreens();
        _overviewScreen.AddEvent($"Theme switched to {(_isDarkTheme ? "Dark" : "Light")}", "Theme", AppTheme.InfoSurfaceBrushKey, AppTheme.PrimaryAccentBrushKey, false);

        if (activeWorkspace == "Recommendations")
        {
            ShowRecommendationsScreen();
            return;
        }

        var navigationTarget = NavigationItems.FirstOrDefault(item => item.Label == activeWorkspace) ?? NavigationItems[0];
        SelectNavigation(navigationTarget);
    }

    private void RebuildScreens()
    {
        _overviewScreen = OverviewScreenViewModel.CreateSample();
        _timelineScreen = TimelineScreenViewModel.CreateSample(_overviewScreen);
        _scenariosScreen = ScenariosScreenViewModel.CreateSample();
        _alertsScreen = AlertsScreenViewModel.CreateSample(_overviewScreen.Alerts);
        _settingsScreen = SettingsScreenViewModel.CreateSample();
        _recommendationsScreen = RecommendationsScreenViewModel.CreateSample(_overviewScreen.Recommendations);
    }

    private void FocusStageInOverview(string? stageName)
    {
        if (string.IsNullOrWhiteSpace(stageName))
        {
            return;
        }

        SelectNavigation(NavigationItems[0]);
        _overviewScreen.SelectStageByName(stageName);
        _overviewScreen.AddEvent($"Focused {stageName} from side panel", "Focus", AppTheme.InfoSurfaceBrushKey, AppTheme.PrimaryAccentBrushKey, false);
    }
}

public sealed class OverviewScreenViewModel : ObservableObject
{
    private static readonly ReadOnlyCollection<double> PickingQueueSeries = new([18d, 22d, 19d, 21d, 24d, 17d, 16d, 19d, 18d, 26d, 25d, 25d, 25d, 32d]);
    private static readonly ReadOnlyCollection<double> PackingQueueSeries = new([9d, 10d, 12d, 11d, 15d, 18d, 16d, 19d, 24d, 23d, 22d, 27d, 29d, 31d]);
    private static readonly ReadOnlyCollection<double> ShippingThroughputSeries = new([10d, 10d, 11d, 18d, 17d, 10d, 9d, 8d, 9d, 14d, 12d, 11d, 19d, 13d]);
    private static readonly ReadOnlyCollection<double> DelayForecastSeries = new([8d, 9d, 11d, 10d, 14d, 12d, 9d, 13d, 12d, 16d, 19d, 17d, 15d, 21d]);

    private int _nextTimelineMinute = 43;
    public ObservableCollection<StageSummaryViewModel> Stages { get; } = new();

    public ObservableCollection<MetricViewModel> KpiMetrics { get; } = new();

    public ObservableCollection<HeroMetricViewModel> HeroMetrics { get; } = new();

    public ObservableCollection<TrendSeriesViewModel> TrendSeries { get; } = new();

    public ObservableCollection<TimelineEntryViewModel> TimelineEntries { get; } = new();

    public ObservableCollection<EventEntryViewModel> EventEntries { get; } = new();

    public ObservableCollection<AlertItemViewModel> Alerts { get; } = new();

    public ObservableCollection<RecommendationItemViewModel> Recommendations { get; } = new();

    public FlowNodeViewModel SourceNode { get; private set; } = null!;

    public StageSummaryViewModel SelectedStage
    {
        get;
        private set => SetProperty(ref field, value);
    }
    = null!;

    public static OverviewScreenViewModel CreateSample()
    {
        static SolidColorBrush B(string key) => ThemeManager.GetBrush(key);

        var viewModel = new OverviewScreenViewModel
        {
            SourceNode = new FlowNodeViewModel("Order Intake", "520 queued work items", "Generation window 15 min", B("SourceNodeBackgroundBrush"), B("SourceNodeBorderBrush"), B("PrimaryAccentBrush")),
        };

        viewModel.Stages.Add(new StageSummaryViewModel(
            "Stage 10",
            "Picking",
            "Pick-A",
            "Queue 42",
            "2 / 3 busy",
            "Avg 03:10",
            "67% utilization",
            B("StageNeutralBackgroundBrush"),
            B("PrimaryStageBorderBrush"),
            B("PrimaryAccentBrush"),
            true,
            [
                new WorkerViewModel("W1", "Busy", B("PrimaryAccentBrush"), B("PrimaryWorkerBackgroundBrush")),
                new WorkerViewModel("W2", "Busy", B("PrimaryAccentBrush"), B("PrimaryWorkerBackgroundBrush")),
                new WorkerViewModel("W3", "Idle", B("WorkerIdleAccentBrush"), B("WorkerIdleBackgroundBrush")),
            ]));

        viewModel.Stages.Add(new StageSummaryViewModel(
            "Stage 20",
            "Packing",
            "Pack-A",
            "Queue 27",
            "3 / 3 busy",
            "Avg 04:35",
            "91% utilization",
            B("StageWarmBackgroundBrush"),
            B("SecondaryStageBorderBrush"),
            B("SecondaryAccentBrush"),
            true,
            [
                new WorkerViewModel("W1", "Busy", B("SecondaryAccentBrush"), B("SecondaryWorkerBackgroundBrush")),
                new WorkerViewModel("W2", "Busy", B("SecondaryAccentBrush"), B("SecondaryWorkerBackgroundBrush")),
                new WorkerViewModel("W3", "Busy", B("SecondaryAccentBrush"), B("SecondaryWorkerBackgroundBrush")),
            ]));

        viewModel.Stages.Add(new StageSummaryViewModel(
            "Stage 30",
            "Shipping",
            "Ship-A",
            "Queue 11",
            "1 / 3 busy",
            "Avg 02:15",
            "34% utilization",
            B("SourceNodeBackgroundBrush"),
            B("PrimaryStageBorderBrush"),
            B("PanelHeaderBrush"),
            false,
            [
                new WorkerViewModel("W1", "Busy", B("PanelHeaderBrush"), B("PrimaryWorkerBackgroundBrush")),
                new WorkerViewModel("W2", "Idle", B("WorkerIdleAccentBrush"), B("WorkerIdleBackgroundBrush")),
                new WorkerViewModel("W3", "Idle", B("WorkerIdleAccentBrush"), B("WorkerIdleBackgroundBrush")),
            ]));

        viewModel.Stages.Add(new StageSummaryViewModel(
            "Stage 40",
            "Quality",
            "QA-A",
            "Queue 6",
            "1 / 3 busy",
            "Avg 01:40",
            "28% utilization",
            B("StageNeutralBackgroundBrush"),
            B("PrimaryStageBorderBrush"),
            B("SuccessBrush"),
            true,
            [
                new WorkerViewModel("W1", "Busy", B("SuccessBrush"), B("PrimaryWorkerBackgroundBrush")),
                new WorkerViewModel("W2", "Idle", B("WorkerIdleAccentBrush"), B("WorkerIdleBackgroundBrush")),
                new WorkerViewModel("W3", "Idle", B("WorkerIdleAccentBrush"), B("WorkerIdleBackgroundBrush")),
            ]));

        viewModel.Stages.Add(new StageSummaryViewModel(
            "Stage 50",
            "Dispatch",
            "Dispatch-A",
            "Queue 4",
            "2 / 3 busy",
            "Avg 01:10",
            "52% utilization",
            B("SourceNodeBackgroundBrush"),
            B("SourceNodeBorderBrush"),
            B("PanelHeaderBrush"),
            false,
            [
                new WorkerViewModel("W1", "Busy", B("PanelHeaderBrush"), B("PrimaryWorkerBackgroundBrush")),
                new WorkerViewModel("W2", "Busy", B("PanelHeaderBrush"), B("PrimaryWorkerBackgroundBrush")),
                new WorkerViewModel("W3", "Idle", B("WorkerIdleAccentBrush"), B("WorkerIdleBackgroundBrush")),
            ]));

        viewModel.SelectStage(viewModel.Stages[0]);

        viewModel.HeroMetrics.Add(new HeroMetricViewModel("Throughput", "1,284", "orders/h", "Completed work items per simulated hour", B("KpiThroughputBrush"), B("InfoSurfaceBrush")));
        viewModel.HeroMetrics.Add(new HeroMetricViewModel("Bottleneck", "Packing", "Stage 20", "Highest weighted queue pressure and utilization", B("WarningBrush"), B("WarningSurfaceBrush")));
        viewModel.HeroMetrics.Add(new HeroMetricViewModel("WIP", "842", "active items", "Current work in progress across the process", B("PrimaryAccentBrush"), B("TrendBlueBackgroundBrush")));

        viewModel.KpiMetrics.Add(new MetricViewModel("Avg Lead Time", "228 min", B("StrongTextBrush")));
        viewModel.KpiMetrics.Add(new MetricViewModel("Avg Queue Wait", "18.4 min", B("WarningBrush")));
        viewModel.KpiMetrics.Add(new MetricViewModel("Avg Processing Time", "03:54", B("PrimaryAccentBrush")));
        viewModel.KpiMetrics.Add(new MetricViewModel("SLA Breach Risk", "High", B("DangerBrush")));
        viewModel.KpiMetrics.Add(new MetricViewModel("Stage Utilization", "78%", B("SuccessBrush")));

        viewModel.TrendSeries.Add(new TrendSeriesViewModel("Picking Queue", PickingQueueSeries, B("PrimaryAccentBrush"), B("TrendBlueBackgroundBrush")));
        viewModel.TrendSeries.Add(new TrendSeriesViewModel("Packing Queue", PackingQueueSeries, B("SuccessBrush"), B("TrendGreenBackgroundBrush")));
        viewModel.TrendSeries.Add(new TrendSeriesViewModel("Shipping Throughput", ShippingThroughputSeries, B("TrendOrangeBrush"), B("TrendOrangeBackgroundBrush")));
        viewModel.TrendSeries.Add(new TrendSeriesViewModel("Delay Forecast", DelayForecastSeries, B("TrendRedBrush"), B("TrendRedBackgroundBrush")));

        viewModel.TimelineEntries.Add(new TimelineEntryViewModel(12, "12:30", B("TimelineBlueBrush")));
        viewModel.TimelineEntries.Add(new TimelineEntryViewModel(160, "12:34", B("TimelineBlueBrush")));
        viewModel.TimelineEntries.Add(new TimelineEntryViewModel(310, "12:38", B("TimelineAccentBrush")));
        viewModel.TimelineEntries.Add(new TimelineEntryViewModel(470, "12:42", B("PanelHeaderBrush")));
        viewModel.TimelineEntries.Add(new TimelineEntryViewModel(640, "12:50", B("DangerBrush")));

        viewModel.EventEntries.Add(new EventEntryViewModel("12:31", "Orders spike", "Signal", B("InfoSurfaceBrush"), B("PrimaryAccentBrush")));
        viewModel.EventEntries.Add(new EventEntryViewModel("12:34", "Picking queue grows", "Warning", B("WarningSurfaceBrush"), B("WarningBrush")));
        viewModel.EventEntries.Add(new EventEntryViewModel("12:38", "Packing station saturates", "Flow", B("SuccessSurfaceBrush"), B("SuccessBrush")));
        viewModel.EventEntries.Add(new EventEntryViewModel("12:42", "Alert raised", "Critical", B("DangerSurfaceBrush"), B("DangerBrush")));

        viewModel.Alerts.Add(new AlertItemViewModel("West capacity degraded 30%", "Capacity drift at Stage 20 blocks downstream throughput.", "Packing", "\uE814", B("DangerSurfaceBrush"), B("DangerBorderBrush"), B("DangerBrush")));
        viewModel.Alerts.Add(new AlertItemViewModel("Packing queue near SLA breach", "Queue length exceeded forecast threshold for the active scenario.", "Packing", "\uE7BA", B("WarningSurfaceBrush"), B("WarningBorderBrush"), B("WarningBrush")));
        viewModel.Alerts.Add(new AlertItemViewModel("Shipping freeze spreading upstream", "Low dispatch throughput begins to starve order completion.", "Shipping", "\uE814", B("DangerSurfaceBrush"), B("DangerBorderBrush"), B("DangerBrush")));

        viewModel.Recommendations.Add(new RecommendationItemViewModel("Predicted bottleneck in 9 min", "Picking and packing converge on the same constrained handoff lane.", "Picking", "Throughput +4% | Queue -8%", B("PanelHeaderBrush")));
        viewModel.Recommendations.Add(new RecommendationItemViewModel("Reroute 20% to Hub B", "Shift work mix to reduce queue pressure at Pack-A.", "Packing", "Queue -12% | SLA risk medium", B("SuccessBrush")));
        viewModel.Recommendations.Add(new RecommendationItemViewModel("Add temporary packing capacity", "Bring one reserve worker online for the active window.", "Packing", "Capacity +18% | Delay forecast stabilizes", B("PrimaryAccentBrush")));

        viewModel.AddEvent("Mockup initialized", "Info", AppTheme.SuccessSurfaceBrushKey, AppTheme.SuccessBrushKey, false);
        return viewModel;
    }

    public void SelectStage(StageSummaryViewModel? stage)
    {
        if (stage is null)
        {
            return;
        }

        foreach (var item in Stages)
        {
            item.IsSelected = item == stage;
        }

        SelectedStage = stage;
    }

    public void SelectStageByName(string stageName)
        => SelectStage(Stages.FirstOrDefault(stage => string.Equals(stage.StageName, stageName, StringComparison.OrdinalIgnoreCase)));

    public void AddEvent(string title, string badgeText, string badgeBackgroundKey, string badgeForegroundKey, bool addTimelineMarker = true)
    {
        var timeLabel = $"12:{_nextTimelineMinute:00}";
        _nextTimelineMinute++;

        EventEntries.Insert(0, new EventEntryViewModel(timeLabel, title, badgeText, AppTheme.ResolveBrush(badgeBackgroundKey), AppTheme.ResolveBrush(badgeForegroundKey)));
        while (EventEntries.Count > 7)
        {
            EventEntries.RemoveAt(EventEntries.Count - 1);
        }

        if (!addTimelineMarker)
        {
            return;
        }

        var markerPosition = Math.Min(700d, 90d + (TimelineEntries.Count * 78d));
        TimelineEntries.Add(new TimelineEntryViewModel(markerPosition, timeLabel, AppTheme.ResolveBrush(badgeForegroundKey)));
    }
}

public sealed class TimelineScreenViewModel(IReadOnlyList<TimelineEntryViewModel> markers, IReadOnlyList<EventEntryViewModel> events)
{
    private static SolidColorBrush B(string key) => ThemeManager.GetBrush(key);

    public ObservableCollection<TimelineEntryViewModel> TimelineEntries { get; } = new(markers);

    public ObservableCollection<EventEntryViewModel> EventEntries { get; } = new(events);

    public ObservableCollection<InfoCardViewModel> Milestones { get; } = new(
        [
            new InfoCardViewModel("Orders Spike", "12:31", "Incoming demand rises above scenario baseline.", B("InfoSurfaceBrush"), B("PrimaryAccentBrush")),
            new InfoCardViewModel("Queue Inflection", "12:38", "Packing queue overtakes forecast guardrail.", B("WarningSurfaceBrush"), B("WarningBrush")),
            new InfoCardViewModel("Alert Window", "12:42", "Critical risk enters operator response threshold.", B("DangerSurfaceBrush"), B("DangerBrush")),
        ]);

    public static TimelineScreenViewModel CreateSample(OverviewScreenViewModel overview)
        => new(overview.TimelineEntries, overview.EventEntries);
}

public sealed class ScenariosScreenViewModel
{
    public ObservableCollection<ScenarioCardViewModel> ScenarioCards { get; } = new(
    [
        new ScenarioCardViewModel("Demand Spike West", "Active", "Strong order burst in the west region with elevated packing pressure.", ThemeManager.GetBrush("InfoSurfaceBrush"), ThemeManager.GetBrush("PrimaryAccentBrush")),
        new ScenarioCardViewModel("Carrier Delay South", "Draft", "Reduced dispatch throughput introduces shipping backlog and longer lead time.", ThemeManager.GetBrush("WarningSurfaceBrush"), ThemeManager.GetBrush("WarningBrush")),
        new ScenarioCardViewModel("Staff Shortage Central", "Paused", "One worker removed from Stage 20 shifts the bottleneck upstream.", ThemeManager.GetBrush("DangerSurfaceBrush"), ThemeManager.GetBrush("DangerBrush")),
    ]);

    public static ScenariosScreenViewModel CreateSample() => new();
}

public sealed class AlertsScreenViewModel : ObservableObject
{
    public AlertsScreenViewModel(IEnumerable<AlertItemViewModel> alerts)
    {
        Alerts = new ObservableCollection<AlertItemViewModel>(alerts);
        SelectedAlert = Alerts[0];
        SelectAlertCommand = new DelegateCommand(parameter =>
        {
            if (parameter is AlertItemViewModel alert)
            {
                SelectedAlert = alert;
            }
        });
    }

    public ObservableCollection<AlertItemViewModel> Alerts { get; }

    public ICommand SelectAlertCommand { get; }

    public AlertItemViewModel SelectedAlert
    {
        get;
        set => SetProperty(ref field, value);
    }
    = null!;
    public static AlertsScreenViewModel CreateSample(IEnumerable<AlertItemViewModel> alerts) => new(alerts);
}

public sealed class RecommendationsScreenViewModel : ObservableObject
{
    public RecommendationsScreenViewModel(IEnumerable<RecommendationItemViewModel> recommendations)
    {
        Recommendations = new ObservableCollection<RecommendationItemViewModel>(recommendations);
        SelectedRecommendation = Recommendations[0];
        SelectRecommendationCommand = new DelegateCommand(parameter =>
        {
            if (parameter is RecommendationItemViewModel recommendation)
            {
                SelectedRecommendation = recommendation;
            }
        });
    }

    public ObservableCollection<RecommendationItemViewModel> Recommendations { get; }

    public ICommand SelectRecommendationCommand { get; }

    public RecommendationItemViewModel SelectedRecommendation
    {
        get;
        set => SetProperty(ref field, value);
    }
    = null!;
    public static RecommendationsScreenViewModel CreateSample(IEnumerable<RecommendationItemViewModel> recommendations) => new(recommendations);
}

public sealed class SettingsScreenViewModel
{
    public ObservableCollection<InfoCardViewModel> SettingCards { get; } = new(
    [
        new InfoCardViewModel("Playback Controls", "Mock", "Control buttons remain dummy-only until simulation integration arrives.", ThemeManager.GetBrush("InfoSurfaceBrush"), ThemeManager.GetBrush("PrimaryAccentBrush")),
        new InfoCardViewModel("Theme", "Interactive", "Use the header toggle to switch between light and dark presentation modes.", ThemeManager.GetBrush("SuccessSurfaceBrush"), ThemeManager.GetBrush("SuccessBrush")),
        new InfoCardViewModel("Graph Detail", "Adaptive", "Stages stay compact until selected so 3-5 stages remain readable.", ThemeManager.GetBrush("WarningSurfaceBrush"), ThemeManager.GetBrush("WarningBrush")),
    ]);

    public static SettingsScreenViewModel CreateSample() => new();
}

public sealed class NavigationItemViewModel(string label, string iconGlyph, string description, bool isSelected) : ObservableObject
{
    public string Label { get; } = label;

    public string IconGlyph { get; } = iconGlyph;

    public string Description { get; } = description;

    public bool IsSelected
    {
        get;
        set => SetProperty(ref field, value);
    } = isSelected;
}

public sealed class StageSummaryViewModel(
    string sequenceLabel,
    string stageName,
    string stationName,
    string queueText,
    string workerSummary,
    string processingTimeText,
    string utilizationText,
    Brush background,
    Brush borderBrush,
    Brush accentBrush,
    bool hasNextStage,
    IReadOnlyList<WorkerViewModel> workers) : ObservableObject
{
    public string SequenceLabel { get; } = sequenceLabel;
    public string StageName { get; } = stageName;
    public string StationName { get; } = stationName;
    public string QueueText { get; } = queueText;
    public string WorkerSummary { get; } = workerSummary;
    public string ProcessingTimeText { get; } = processingTimeText;
    public string UtilizationText { get; } = utilizationText;
    public Brush Background { get; } = background;
    public Brush BorderBrush { get; } = borderBrush;
    public Brush AccentBrush { get; } = accentBrush;
    public bool HasNextStage { get; } = hasNextStage;
    public IReadOnlyList<WorkerViewModel> Workers { get; } = workers;

    public bool IsSelected
    {
        get;
        set => SetProperty(ref field, value);
    }
}

public sealed record FlowNodeViewModel(string Title, string Value, string Subtitle, Brush Background, Brush BorderBrush, Brush AccentBrush);
public sealed record WorkerViewModel(string Label, string Status, Brush AccentBrush, Brush BackgroundBrush);
public sealed record HeroMetricViewModel(string Label, string Value, string Unit, string Context, Brush AccentBrush, Brush SurfaceBrush);
public sealed record MetricViewModel(string Label, string Value, Brush ValueBrush);
public sealed record TrendSeriesViewModel(string Title, IReadOnlyList<double> Points, Brush StrokeBrush, Brush BackgroundBrush);
public sealed record TimelineEntryViewModel(double MarkerPosition, string TimeLabel, Brush MarkerBrush);
public sealed record EventEntryViewModel(string TimeLabel, string Title, string BadgeText, Brush BadgeBackground, Brush BadgeForeground);
public sealed record AlertItemViewModel(string Title, string Description, string TargetStageName, string IconGlyph, Brush Background, Brush BorderBrush, Brush IconBrush);
public sealed record RecommendationItemViewModel(string Title, string Description, string TargetStageName, string ImpactPreview, Brush BadgeBrush);
public sealed record ScenarioCardViewModel(string Title, string Status, string Description, Brush Background, Brush AccentBrush);
public sealed record InfoCardViewModel(string Title, string Subtitle, string Description, Brush Background, Brush AccentBrush);

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}

public sealed class DelegateCommand(Action<object?> execute, Predicate<object?>? canExecute = null) : ICommand
{
    private readonly Action<object?> _execute = execute;
    private readonly Predicate<object?>? _canExecute = canExecute;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
