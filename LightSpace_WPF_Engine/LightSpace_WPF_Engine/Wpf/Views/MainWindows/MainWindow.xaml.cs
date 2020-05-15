using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Wpf.Controls;
using LightSpace_WPF_Engine.Wpf.ViewModels;
using LightSpace_WPF_Engine.Wpf.Views.UserControls;

namespace LightSpace_WPF_Engine.Wpf.Views.MainWindows
{
    public partial class MainWindow : Window
    {
        internal static MainWindow Main;

        public CustomConsole CustomConsole { get; private set; }

        public SoundManager SoundManager { get; private set; }

        public Dispatcher GetDispatcher;

        public delegate void PopulateCanvasControlEvent(Canvas canvas, List<CustomImage> images, bool needsExtraRoom);

        public PopulateCanvasControlEvent PopulateCanvasControlDelegate;

        public delegate void RenderEvent();

        public RenderEvent RenderDelegate;

        private ActiveGameData gameData;
        private bool renderTiles = true;
        private bool renderGame = true;

        public MainWindow()
        {
            ConsoleLogger.Init();
            // set main so it can be accessed from anywhere if needed
            Main = this;
            // set dispatcher so events can be routed to it
            GetDispatcher = Dispatcher;

            InitializeComponent();
            CustomInitialization();
        }

        public void CustomInitialization()
        {
            // init game
            Game.Get.Init();

            // set delegates
            RenderDelegate = Render;
            PopulateCanvasControlDelegate = PopulateCanvas;

            // set control settings and load new controls where needed
            RenderOptions.SetBitmapScalingMode(LeftViewContainer, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(MiddleViewContainer, BitmapScalingMode.NearestNeighbor);

            SoundManager = new SoundManager(.5d); //TODO: 50 volume control
            CustomConsole = new CustomConsole();
            ConsoleListBox.ItemsSource = CustomConsole.ConsoleMessages;

            DiagnosticsGroupBox.Content = new PerformanceUserControl();
            AudioGroupBox.Content = new AudioUserControl();
            SetCustomGameControls();
        }

        /// <summary>
        /// Used to load in custom controls for games. Games can have their own custom UserControls to load in here.
        /// </summary>
        /// <param name="gameName"> Enum specifying which game is active. </param>
        public void SetCustomGameControls(GameName gameName = GameName.None)
        {
            CustomControlArea.Content = GameList.GetGameControls(gameName);
            DescriptionTextBlock.Text = GameList.GetGameDescription(gameName);
            SetStartButton(gameName);
        }

        private bool SetStartButton(GameName gameName)
        {
            var value = true;
            if (gameName == GameName.None || gameName == GameName.Template)
            {
                value = false;
            }
            else if ((Game.Get.TileManager.FieldSize.X == 0 || Game.Get.TileManager.FieldSize.Y == 0) && Game.Get.TileManager.UseSimulatedTiles)
            {
                value = false;
            }

            LeftViewToggleButton.IsEnabled = value;
            MiddleViewToggleButton.IsEnabled = value;
            ToggleGameButton.IsEnabled = value;
            return value;
        }

        public void RefreshGame()
        {
            if (gameData == null)
            {
                return;
            }
            // Prevent LoadGame logic from double interacting with the same variable and clearing it.
            var copy = gameData;
            LoadGame(ref copy);
        }

        public void LoadGame(ref ActiveGameData newGameData)
        {
            gameData = newGameData;
            newGameData = null;
            this.Title = gameData.GameName;
            DescriptionTextBlock.Text = gameData.GameDescription;
            DescriptionTextBlock.Text += $"{Environment.NewLine}{Environment.NewLine}Preferred tile size: {gameData.PreferredGameTileSize} ";
            this.Icon = gameData.GameIcon.BitmapToImageSource();
            ClearCanvas(LeftViewContainer);
            ClearCanvas(MiddleViewContainer);

            // If Game is not allowed to start anyway, don't add the behavior or controls.
            if (!SetStartButton(gameData.EnumValue))
            {
                return;
            }
            CustomControlArea.Content = gameData.GameCustomControl;
            Game.Get.SetRunningGameBehavior(GameList.GetGameBehavior(gameData.EnumValue));
        }

        /// <summary>
        /// Start the process of loading or syncing tile data.
        /// </summary>
        private void SyncFloor_Click(object sender, RoutedEventArgs e)
        {
            var floorSyncWindow = new FloorSyncWindow();
            floorSyncWindow.ShowDialog();
        }

        private void ChooseGame_Click(object sender, RoutedEventArgs e)
        {
            var gameSelectWindow = new GameSelectWindow(gameData);
            gameSelectWindow.Init();
            gameSelectWindow.ShowDialog();
        }

        /// <summary>
        /// Toggles the core game loop of the game. TODO: Actually select a game instead of always starting SkippingRomeGame.
        /// </summary>
        private void ToggleGame_Click(object sender, RoutedEventArgs e)
        {
            Game.Get.ToggleLoop();
            var isRunning = Game.Get.CoreLoop.IsRunning;
            ChooseGameButton.IsEnabled = !isRunning;
            SyncFloorButton.IsEnabled = !isRunning;
            LeftViewToggleButton.IsEnabled = !isRunning;
            MiddleViewToggleButton.IsEnabled = !isRunning;
        }

        /// <summary>
        /// Shutdown functionality for the application.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Game.Get.ShutDown();
            Application.Current.Shutdown();
        }

        private void ConsoleListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ConsoleListBox.Items.MoveCurrentToLast();
            ConsoleListBox.ScrollIntoView(ConsoleListBox.SelectedItem);
            ConsoleGroupBox.Header = $"Console ({ConsoleListBox.Items.Count})";
        }

        private void ToggleRenderTiles_Click(object sender, RoutedEventArgs e)
        {
            renderTiles = !renderTiles;
            MiddleViewToggleButton.Content = renderTiles ? "Hide" : "Show";
        }

        private void ToggleRenderGame_Click(object sender, RoutedEventArgs e)
        {
            renderGame = !renderGame;
            LeftViewToggleButton.Content = renderGame ? "Hide" : "Show";
        }

        /// <summary>
        /// Forces a refresh on controls specified within the method.
        /// </summary>
        public void Render()
        {
            // only render if changes were found
            if (!Game.Get.TileManager.GetRenderChanged())
            {
                return;
            }

            if (renderGame)
            {
                var leftViewImage = new CustomImage(Game.Get.TileManager.GetRenderGraphic());
                Main.GetDispatcher.Invoke(PopulateCanvasControlDelegate, LeftViewContainer,
                    new List<CustomImage> { leftViewImage }, false);
            }
            else
            {
                ClearCanvas(LeftViewContainer);
            }

            if (renderTiles)
            {
                var sensorTiles = new List<CustomImage>();
                foreach (UIElement child in MiddleViewContainer.Children)
                {
                    if (child is CustomImage temp)
                    {
                        sensorTiles.Add(temp);
                    }
                }

                foreach (var sensorTile in sensorTiles)
                {
                    var tileData = sensorTile.TileData;
                    sensorTile.TileData.Sensors =
                        Game.Get.TileManager.Tiles[tileData.Position.X, tileData.Position.Y].Sensors;
                }

                var updatedSensorImages = ImageExtensions.GetIndividualSensorVisuals(Game.Get.TileManager.Tiles);
                PopulateCanvas(MiddleViewContainer, updatedSensorImages, false);
            }
            else
            {
                ClearCanvas(MiddleViewContainer);
            }

            // Force Refresh elements
            LeftViewContainer.Refresh();
            MiddleViewContainer.Refresh();
            this.Refresh();
        }

        private static void ClearCanvas(Canvas canvas)
        {
            canvas.Children.Clear();
            canvas.RenderSize = new System.Windows.Size(canvas.Width, canvas.Height);
            canvas.Background = (SolidColorBrush)Application.Current.Resources["LightSpaceGray"];
        }

        private static void PopulateCanvas(Canvas canvas, List<CustomImage> images, bool needsExtraRoom)
        {
            var extraRoom = needsExtraRoom ? 10 : 0;
            canvas.Children.Clear();
            foreach (var graphic in images)
            {
                canvas.Children.Add(graphic);
            }

            foreach (UIElement child in canvas.Children)
            {
                if (child is CustomImage temp)
                {
                    Canvas.SetLeft(child,temp.InCanvasPosition.X);
                    Canvas.SetTop(child, temp.InCanvasPosition.Y);
                }
            }

            var contentBounds = ImageExtensions.GetMaxSizeInList(images);
            if (contentBounds.X > contentBounds.Y)
            {
                canvas.Width = contentBounds.X + extraRoom;
                canvas.Height = contentBounds.X + extraRoom;
            }
            else
            {
                canvas.Width = contentBounds.Y + extraRoom;
                canvas.Height = contentBounds.Y + extraRoom;
            }

            try
            {
                canvas.RenderSize = new System.Windows.Size(canvas.Width, canvas.Height);
                canvas.Background = (SolidColorBrush) Application.Current.Resources["LightSpaceGray"];
            }
            catch (Exception)
            {
                // On application shutdown sometimes it will finish calling rendering functions from the Loop thread while the application itself is already unavailable.
                // This results in a NullReferenceException caught at this point.
            }
        }
    }
}
