using System.Windows;
using System.Windows.Media;

namespace FlowForge.UiWpf;

public enum AppThemeMode
{
    Light,
    Dark,
}

public static class ThemeManager
{
    public static SolidColorBrush GetBrush(string key)
        => (SolidColorBrush)Application.Current.Resources[key];

    public static void ApplyTheme(AppThemeMode mode)
    {
        var targetSource = new Uri(mode == AppThemeMode.Dark ? "Themes/Brushes.Dark.xaml" : "Themes/Brushes.Light.xaml", UriKind.Relative);
        var dictionaries = Application.Current.Resources.MergedDictionaries;

        var themeDictionary = dictionaries.FirstOrDefault(dictionary => dictionary.Source is not null && dictionary.Source.OriginalString.Contains("Brushes.", StringComparison.OrdinalIgnoreCase));
        if (themeDictionary is null)
        {
            dictionaries.Insert(0, new ResourceDictionary { Source = targetSource });
            return;
        }

        themeDictionary.Source = targetSource;
    }
}
