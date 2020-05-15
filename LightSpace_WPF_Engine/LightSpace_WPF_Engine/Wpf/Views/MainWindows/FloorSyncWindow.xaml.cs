using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
using System.Windows.Shapes;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for FloorSyncWindow.xaml
    /// </summary>
    public partial class FloorSyncWindow : Window
    {
        private bool isClosing = false;
        private Vector2 fieldSize = Vector2.Zero();

        public FloorSyncWindow()
        {
            InitializeComponent();
            InitButtons();
        }

        private void GetFieldSize()
        {
            if (Game.Get.TileManager.FieldSize != Vector2.Zero())
            {
                fieldSize = Game.Get.TileManager.FieldSize;
            }
            else if (Game.Get.RunningGameBehavior != null)
            {
                fieldSize = Game.Get.RunningGameBehavior.GameFieldTileSize != null ? Game.Get.RunningGameBehavior.GameFieldTileSize : Vector2.Zero();
            }
        }

        public void InitButtons()
        {
            GetFieldSize();
            ToolbarStackPanel.Children.Clear();
            #region CloseBtn
            var closeBtn = new Button()
            {
                Name = "CloseButton",
                Margin = new Thickness(10, 9, 5, 9),
                Content = "X"
            };
            closeBtn.Click += (o, args) => CloseWindow();
            ToolbarStackPanel.Children.Add(closeBtn);
            #endregion

            #region TileDimensions

            var xLbl = new Label
            {
                Margin = new Thickness(10, 15, 1, 9),
                Content = "X tile length:"
            };
            var xTb = new TextBox
            {
                Name = "xTextBox",
                Margin = new Thickness(10, 9, 15, 9),
                Text = fieldSize.X.ToString(),
                Width = 15,
                Height = 25,
                MaxLength = 2
            };
            xTb.PreviewTextInput += NumberValidationTextBox;

            var yLbl = new Label
            {
                Margin = new Thickness(10, 15, 1, 9),
                Content = "Y tile length:"
            };
            var yTb = new TextBox
            {
                Name = "yTextBox",
                Margin = new Thickness(10, 9, 5, 9),
                Text = fieldSize.Y.ToString(),
                Width = 15,
                Height = 25,
                MaxLength = 2
            };
            yTb.PreviewTextInput += NumberValidationTextBox;

            ToolbarStackPanel.Children.Add(xLbl);
            ToolbarStackPanel.Children.Add(xTb);
            ToolbarStackPanel.Children.Add(yLbl);
            ToolbarStackPanel.Children.Add(yTb);

            #endregion

            #region SimulateHardwareButton
            var simulateHardwareBtn = new Button()
            {
                Name = "SimulateHardwareBtn",
                Margin = new Thickness(10, 9, 5, 9),
                Content = "Simulate Hardware"
            };
            simulateHardwareBtn.Click += SimulateHardwareBtn_OnClick;
            ToolbarStackPanel.Children.Add(simulateHardwareBtn);
            #endregion
        }

        private void CloseWindow()
        {
            SetFieldSize();
            Game.Get.TileManager.UseSimulatedTiles = false;
            Game.Get.TileManager.GenerateDebugTiles(fieldSize,false);
            MainWindow.Main.RefreshGame();
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!isClosing)
            {
                isClosing = true;
            }
        }

        private void SetFieldSize()
        {
            // Get the node, check if it or it's text is invalid and then set the value of FieldSize.*
            var tb = (TextBox)LogicalTreeHelper.FindLogicalNode(this,"xTextBox");
            fieldSize.X = (tb != null && tb.Text != "") ? Convert.ToInt32(tb.Text) : fieldSize.X;
            tb = (TextBox)LogicalTreeHelper.FindLogicalNode(this, "yTextBox");
            fieldSize.Y = (tb != null && tb.Text != "") ? Convert.ToInt32(tb.Text) : fieldSize.Y;
        }

        private void SimulateHardwareBtn_OnClick(object sender, RoutedEventArgs e)
        {
            SetFieldSize();
            Game.Get.TileManager.UseSimulatedTiles = true;
            Game.Get.TileManager.GenerateDebugTiles(fieldSize,true);
            MainWindow.Main.RefreshGame();
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
