using System.Windows.Media;

namespace FlowForge.UiWpf;

public static class AppTheme
{
    public const string PrimaryAccentBrushKey = nameof(PrimaryAccentBrushKey);
    public const string SecondaryAccentBrushKey = nameof(SecondaryAccentBrushKey);
    public const string SuccessBrushKey = nameof(SuccessBrushKey);
    public const string DangerBrushKey = nameof(DangerBrushKey);
    public const string WarningBrushKey = nameof(WarningBrushKey);
    public const string InfoSurfaceBrushKey = nameof(InfoSurfaceBrushKey);
    public const string SuccessSurfaceBrushKey = nameof(SuccessSurfaceBrushKey);
    public const string DangerSurfaceBrushKey = nameof(DangerSurfaceBrushKey);
    public const string WarningSurfaceBrushKey = nameof(WarningSurfaceBrushKey);

    public static SolidColorBrush StrongTextBrush { get; } = CreateBrush("#24364D");
    public static SolidColorBrush PanelHeaderBrush { get; } = CreateBrush("#4B73A9");
    public static SolidColorBrush PrimaryAccentBrush { get; } = CreateBrush("#2E7AD9");
    public static SolidColorBrush SecondaryAccentBrush { get; } = CreateBrush("#E89A3D");
    public static SolidColorBrush SuccessBrush { get; } = CreateBrush("#4FA85D");
    public static SolidColorBrush DangerBrush { get; } = CreateBrush("#D5544F");
    public static SolidColorBrush WarningBrush { get; } = CreateBrush("#E89A3D");
    public static SolidColorBrush KpiThroughputBrush { get; } = CreateBrush("#1E88E5");
    public static SolidColorBrush TimelineBlueBrush { get; } = CreateBrush("#7AB6F5");
    public static SolidColorBrush TimelineAccentBrush { get; } = CreateBrush("#4FA0F0");
    public static SolidColorBrush SourceNodeBackgroundBrush { get; } = CreateBrush("#FFF7FBFF");
    public static SolidColorBrush SourceNodeBorderBrush { get; } = CreateBrush("#BFD9F6");
    public static SolidColorBrush StageNeutralBackgroundBrush { get; } = CreateBrush("#FFFDFEFF");
    public static SolidColorBrush StageWarmBackgroundBrush { get; } = CreateBrush("#FFFFFAF5");
    public static SolidColorBrush PrimaryStageBorderBrush { get; } = CreateBrush("#CFE2F5");
    public static SolidColorBrush SecondaryStageBorderBrush { get; } = CreateBrush("#F1D7AE");
    public static SolidColorBrush PrimaryWorkerBackgroundBrush { get; } = CreateBrush("#D3E7FB");
    public static SolidColorBrush SecondaryWorkerBackgroundBrush { get; } = CreateBrush("#FBE8CE");
    public static SolidColorBrush WorkerIdleAccentBrush { get; } = CreateBrush("#7A8CA3");
    public static SolidColorBrush WorkerIdleBackgroundBrush { get; } = CreateBrush("#E8EEF5");
    public static SolidColorBrush TrendBlueBackgroundBrush { get; } = CreateBrush("#F2F7FE");
    public static SolidColorBrush TrendGreenBackgroundBrush { get; } = CreateBrush("#F2FBF3");
    public static SolidColorBrush TrendOrangeBrush { get; } = CreateBrush("#EB6A42");
    public static SolidColorBrush TrendOrangeBackgroundBrush { get; } = CreateBrush("#FFF4F0");
    public static SolidColorBrush TrendRedBrush { get; } = CreateBrush("#C95C52");
    public static SolidColorBrush TrendRedBackgroundBrush { get; } = CreateBrush("#FFF5F4");
    public static SolidColorBrush InfoSurfaceBrush { get; } = CreateBrush("#EAF4FF");
    public static SolidColorBrush SuccessSurfaceBrush { get; } = CreateBrush("#EEF8F0");
    public static SolidColorBrush WarningSurfaceBrush { get; } = CreateBrush("#FFF4E8");
    public static SolidColorBrush DangerSurfaceBrush { get; } = CreateBrush("#FDEEEE");
    public static SolidColorBrush WarningBorderBrush { get; } = CreateBrush("#F1D7AE");
    public static SolidColorBrush DangerBorderBrush { get; } = CreateBrush("#F0D4C2");

    public static SolidColorBrush ResolveBrush(string key) => key switch
    {
        PrimaryAccentBrushKey => PrimaryAccentBrush,
        SecondaryAccentBrushKey => SecondaryAccentBrush,
        SuccessBrushKey => SuccessBrush,
        DangerBrushKey => DangerBrush,
        WarningBrushKey => WarningBrush,
        InfoSurfaceBrushKey => InfoSurfaceBrush,
        SuccessSurfaceBrushKey => SuccessSurfaceBrush,
        DangerSurfaceBrushKey => DangerSurfaceBrush,
        WarningSurfaceBrushKey => WarningSurfaceBrush,
        _ => StrongTextBrush
    };

    private static SolidColorBrush CreateBrush(string color)
    {
        var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(color)!;
        brush.Freeze();
        return brush;
    }
}
