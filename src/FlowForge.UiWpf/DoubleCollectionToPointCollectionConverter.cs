using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FlowForge.UiWpf;

public sealed class DoubleCollectionToPointCollectionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IReadOnlyList<double> points || points.Count == 0)
        {
            return new PointCollection();
        }

        var (width, height) = ParseSize(parameter as string);
        var min = points.Min();
        var max = points.Max();
        var range = Math.Max(1d, max - min);
        var step = points.Count == 1 ? 0d : width / (points.Count - 1d);

        var collection = new PointCollection(points.Count);

        for (var index = 0; index < points.Count; index++)
        {
            var x = step * index;
            var normalized = (points[index] - min) / range;
            var y = height - (normalized * height);
            collection.Add(new Point(x, y));
        }

        return collection;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();

    private static (double Width, double Height) ParseSize(string? parameter)
    {
        if (string.IsNullOrWhiteSpace(parameter))
        {
            return (156d, 28d);
        }

        var parts = parameter.Split(',');
        if (parts.Length != 2)
        {
            return (156d, 28d);
        }

        var width = double.TryParse(parts[0], CultureInfo.InvariantCulture, out var parsedWidth)
            ? parsedWidth
            : 156d;

        var height = double.TryParse(parts[1], CultureInfo.InvariantCulture, out var parsedHeight)
            ? parsedHeight
            : 28d;

        return (width, height);
    }
}
