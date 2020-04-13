using LightSpace_WPF_Engine.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
using System.Windows.Shapes;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for GameSelectWindow.xaml
    /// </summary>
    public partial class GameSelectWindow : Window
    {
        private ActiveGameData gameData = new ActiveGameData();
        private bool isClosing = false;
        public GameSelectWindow(ActiveGameData existingGameData)
        {
            gameData = existingGameData;
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            isClosing = false;
            MainDockPanel.Children.Clear();

            var gameNames = (GameName[]) Enum.GetValues(typeof(GameName));
            var games = new List<ActiveGameData>();
            foreach (var gameName in gameNames)
            {
                var activeGameData = new ActiveGameData
                {
                    EnumValue             = gameName, 
                    GameName              = gameName.ToString(),
                    GameDescription       = GameList.GetGameDescription(gameName),
                    GameIcon              = GameList.GetGameIcon(gameName),
                    GameCustomControl     = GameList.GetGameControls(gameName),
                    PreferredGameTileSize = GameList.GetPreferredGameTileSize(gameName)
                };
                games.Add(activeGameData);
            }

            foreach (var game in games)
            {
                #region Generate Elements

                var sp = new StackPanel
                {
                    Name = $"{game.GameName}_StackPanel",
                    Width = 80,
                    Height = 90
                };
                var lb = new Label
                {
                    Content = game.GameName,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                var imgBrush = new ImageBrush
                {
                    ImageSource = ImageExtensions.BitmapToImageSource(game.GameIcon)
                };
                var btn = new Button
                {
                    Height = 64,
                    Width = 64,
                    Background = imgBrush
                };

                #endregion

                btn.Click += (sender, args) => LoadGame(game);
                sp.Children.Add(lb);
                sp.Children.Add(btn);
                MainDockPanel.Children.Add(sp);
            }
        }

        private void LoadGame(ActiveGameData data)
        {
            gameData = data;
            MainWindow.Main.LoadGame(ref data);
            isClosing = true;
            Close();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!isClosing)
            {
                LoadGame(gameData);
            }
        }
    }
}
