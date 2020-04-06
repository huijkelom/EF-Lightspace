using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LightSpace_WPF_Engine.Wpf.views.UserControls
{
    /// <summary>
    /// Interaction logic for ToolbarUserControl.xaml
    /// ToolbarUserControl is used as an alternative for the ThreeDBorderWindow WindowStyle, though it is not complete/fully functional.
    /// If a custom WindowStyle is deemed neccessary, this can be added.
    /// </summary>
    public partial class ToolbarUserControl : UserControl
    {
        private Window mainWindow;

        public ToolbarUserControl()
        {
            InitializeComponent();
            mainWindow = Application.Current.MainWindow;
        }

        public static string ToolbarTitle { get; private set; } = "LightSpace Play Floor Engine";

        private void Toolbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mainWindow.DragMove();
            }
        }

        private void Toolbar_CloseButton(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Toolbar_MinimizeButton(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Minimized;
        }

        private void Toolbar_MaximizeButton(object sender, RoutedEventArgs e)
        {
            mainWindow.WindowState = WindowState.Maximized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
