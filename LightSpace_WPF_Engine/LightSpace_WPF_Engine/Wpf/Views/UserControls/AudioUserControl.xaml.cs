using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LightSpace_WPF_Engine.Wpf.Views.MainWindows;

namespace LightSpace_WPF_Engine.Wpf.Views.UserControls
{
    /// <summary>
    /// Interaction logic for AudioUserControl.xaml
    /// </summary>
    public partial class AudioUserControl : UserControl
    {
        public AudioUserControl()
        {
            InitializeComponent();
            //TODO: 41 Add control bars for volume. Needs to wait for audio update.
            
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainWindow.Main.SoundManager.SoundVolume = e.NewValue;
        }
    }
}
