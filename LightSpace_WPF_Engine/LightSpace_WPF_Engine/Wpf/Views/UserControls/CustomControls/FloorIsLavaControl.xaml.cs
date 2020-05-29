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
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Wpf.Views.UserControls.CustomControls
{
    /// <summary>
    /// Interaction logic for FloorIsLavaControl.xaml
    /// </summary>
    public partial class FloorIsLavaControl : UserControl
    {
        public FloorIsLavaControl()
        {
            InitializeComponent();
        }

        private void Test0Button_OnClick(object sender, RoutedEventArgs e)
        {
            SetSensor(0, 0, 0);
        }

        private void Test1Button_OnClick(object sender, RoutedEventArgs e)
        {
            SetSensor(4, 0, 20);
        }

        private void Test2Button_OnClick(object sender, RoutedEventArgs e)
        {
            SetSensor(2, 2, 12);
        }

        private void Test3Button_OnClick(object sender, RoutedEventArgs e)
        {
            SetSensor(0, 4, 4);
        }

        private void Test4Button_OnClick(object sender, RoutedEventArgs e)
        {
            SetSensor(4, 4, 24);
        }

        // Toggle a sensor ON/OFF as if it were input by the hardware
        private static void SetSensor(int tileWidth, int tileHeight, int sensorTile)
        {
            List<Tile> SensorTile = new List<Tile>();
            var tiles = Game.Get.TileManager.Tiles;
            SensorTile.Add(tiles[tileWidth, tileHeight]);
            foreach (var tile in SensorTile)
            {
                if (tile.AnyActiveSensorsInTile())
                {
                    var sensor = new Vector2(1, 2);
                    var outputData = new OutputData
                    {
                        PressureDetected = false,
                        TileNumber = sensorTile,
                        Position = sensor
                    };

                    Input.SetMockDataInputDelegate(outputData);
                }
                else
                {
                    var sensor = new Vector2(1, 2);
                    var outputData = new OutputData
                    {
                        PressureDetected = true,
                        TileNumber = sensorTile,
                        Position = sensor
                    };

                    Input.SetMockDataInputDelegate(outputData);
                }
            }
        }
    }
}
