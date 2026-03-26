using System.Windows;

namespace FlowForge.UiWpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = MainWindowViewModel.CreateSample();
    }
}
