using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FlowForge.UiWpf.Screens;

public partial class OverviewScreen : UserControl
{
    public OverviewScreen()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        SelectedStagePanel.RenderTransform = new TranslateTransform();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.OldValue is INotifyPropertyChanged oldObservable)
        {
            oldObservable.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (eventArgs.NewValue is INotifyPropertyChanged newObservable)
        {
            newObservable.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName != nameof(OverviewScreenViewModel.SelectedStage))
        {
            return;
        }

        AnimateSelectedStagePanel();
    }

    private void AnimateSelectedStagePanel()
    {
        var storyboard = new Storyboard();

        var opacityAnimation = new DoubleAnimation(0.55, 1, TimeSpan.FromMilliseconds(220));
        Storyboard.SetTarget(opacityAnimation, SelectedStagePanel);
        Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));

        var slideAnimation = new DoubleAnimation(18, 0, TimeSpan.FromMilliseconds(240))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
        };
        Storyboard.SetTarget(slideAnimation, SelectedStagePanel);
        Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

        storyboard.Children.Add(opacityAnimation);
        storyboard.Children.Add(slideAnimation);
        storyboard.Begin();
    }
}
