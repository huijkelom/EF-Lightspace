using LightSpace_WPF_Engine.Games.Dodgeball;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Wpf.Views.MainWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace LightSpace_WPF_Engine.Wpf.Views.UserControls.CustomControls
{
    /// <summary>
    /// Interaction logic for DodgeballGameControl.xaml
    /// </summary>
    public partial class DodgeballGameControl : UserControl
    {
        public DodgeballGameControl()
        {
            InitializeComponent();
        }

        // Toggle a sensor ON as if it were input by the hardware
        private void Test0Button_OnClick(object sender, RoutedEventArgs e)
        {
            var sensor = new Vector2(1, 2);
            var outputData = new OutputData
            {
                PressureDetected = true,
                TileNumber = 0,
                Position = sensor
            };
            Input.SetMockDataInputDelegate(outputData);
        }

        // Toggle a sensor OFF as if it were input by the hardware
        private void Test1Button_OnClick(object sender, RoutedEventArgs e)
        {
            var sensor = new Vector2(1, 2);
            var outputData = new OutputData
            {
                PressureDetected = false,
                TileNumber = 0,
                Position = sensor
            };
            Input.SetMockDataInputDelegate(outputData);
        }

        // pause
        private void Test2Button_OnClick(object sender, RoutedEventArgs e)
        {
            var game = Game.Get.RunningGameBehavior as DodgeballGameBehavior;
            game.Paused = true;
        }

        // resume
        private void Test3Button_OnClick(object sender, RoutedEventArgs e)
        {
            var game = Game.Get.RunningGameBehavior as DodgeballGameBehavior;
            game.Paused = false;
        }

        private void Test4Button_OnClick(object sender, RoutedEventArgs e)
        {
            var coreLoopIsRunning = Game.Get.CoreLoop.IsRunning;
            if (coreLoopIsRunning)
            {
                Game.Get.CoreLoop.StopLoop();
            }
            // Settings: Framerate, speed of jump rope? min&max delay? 
            var game = Game.Get.RunningGameBehavior as DodgeballGameBehavior;

            var window = new SizeToContentWindow
            {
                Title = "Settings",
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize
            };

            #region framerate settings

            var lbl1 = new Label
            {
                Margin = new Thickness(0, 0, 10, 0),
                Content = "Frame Rate:"
            };
            var tb1 = new TextBox
            {
                Name = "FrameRateTextBox",
                Text = Game.Get.CoreLoop.TicksPerSecond.ToString(),
                MaxLength = 2
            };
            tb1.PreviewTextInput += NumberValidationTextBox;
            var sp1 = new StackPanel
            {
                Margin = new Thickness(5),
                Background = (SolidColorBrush)Application.Current.Resources["LightSpaceGray"],
                Orientation = Orientation.Horizontal
            };
            sp1.Children.Add(lbl1);
            sp1.Children.Add(tb1);
            window.MainStackPanel.Children.Add(sp1);

            #endregion

            #region skipping rope progress settings

            var lbl2 = new Label
            {
                Margin = new Thickness(0, 0, 10, 0),
                Content = "Ball count:"
            };
            var tb2 = new TextBox
            {
                Name = "BallCountTextBox",
                Text = game.DodgeballCount.ToString(),
                MaxLength = 2
            };
            tb2.PreviewTextInput += NumberValidationTextBox;
            var sp2 = new StackPanel
            {
                Margin = new Thickness(5),
                Background = (SolidColorBrush)Application.Current.Resources["LightSpaceGray"],
                Orientation = Orientation.Horizontal
            };
            sp2.Children.Add(lbl2);
            sp2.Children.Add(tb2);
            window.MainStackPanel.Children.Add(sp2);
            #endregion


            var ApplyButton = new Button
            {
                Content = "Apply",
                HorizontalAlignment = HorizontalAlignment.Center
            };
            ApplyButton.Click += (o, args) => ApplySettings(window, coreLoopIsRunning);
            window.MainStackPanel.Children.Add(ApplyButton);
            window.ApplyTemplate();
            window.ShowDialog();
        }

        private void Test5Button_OnClick(object sender, RoutedEventArgs e)
        {
            var game = Game.Get.RunningGameBehavior as DodgeballGameBehavior;
            game.ClearBallList();
        }

        public void ApplySettings(SizeToContentWindow window, bool coreLoopIsRunning)
        {
            var game = Game.Get.RunningGameBehavior as DodgeballGameBehavior;
            var frameRateTextBox = (TextBox)LogicalTreeHelper.FindLogicalNode(window, "FrameRateTextBox");
            if (frameRateTextBox != null)
            {
                Game.Get.CoreLoop.TicksPerSecond = Convert.ToInt32(frameRateTextBox.Text);
            }
            var jumpProgressTextBox = (TextBox)LogicalTreeHelper.FindLogicalNode(window, "BallCountTextBox");
            if (jumpProgressTextBox != null)
            {
                game.DodgeballCount = Convert.ToInt32(jumpProgressTextBox.Text);
            }
            
            window.Close();
            if (coreLoopIsRunning)
            {
                Game.Get.CoreLoop.StartLoop();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
