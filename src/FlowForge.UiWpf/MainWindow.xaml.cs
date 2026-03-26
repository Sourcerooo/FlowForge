using System.Windows;
using System.Windows.Media.Animation;

namespace FlowForge.UiWpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var viewModel = MainWindowViewModel.CreateSample();
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MainWindowViewModel.CurrentScreen))
            {
                AnimateScreenTransition();
            }
        };

        DataContext = viewModel;
    }

    private void AnimateScreenTransition()
    {
        var storyboard = new Storyboard();

        var opacityAnimation = new DoubleAnimation(0.15, 1, TimeSpan.FromMilliseconds(240));
        Storyboard.SetTarget(opacityAnimation, ScreenContentHost);
        Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

        var slideAnimation = new DoubleAnimation(16, 0, TimeSpan.FromMilliseconds(240))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        };
        Storyboard.SetTarget(slideAnimation, ScreenContentHost);
        Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

        storyboard.Children.Add(opacityAnimation);
        storyboard.Children.Add(slideAnimation);
        storyboard.Begin();
    }
}
