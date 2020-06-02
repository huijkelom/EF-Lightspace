using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using LightSpace_WPF_Engine.Games.TestGame;
using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Wpf.Controls;
using LightSpace_WPF_Engine.Wpf.ViewModels;
using LightSpace_WPF_Engine.Wpf.Views.UserControls;
using Colors = LightSpace_WPF_Engine.Models.Utility.Colors;
using Image = System.Windows.Controls.Image;

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
            //TODO: 11 Remove debug tiles (when hardware testing starts)
            Game.Get.TileManager.GenerateDebugTiles(Vector2.One());

            // set delegates
            RenderDelegate = Render;
            PopulateCanvasControlDelegate = PopulateCanvas;

            // set control settings and load new controls where needed
            RenderOptions.SetBitmapScalingMode(LeftViewContainer, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(MiddleViewContainer, BitmapScalingMode.NearestNeighbor);

            SoundManager = new SoundManager();
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

        private void SetStartButton(GameName gameName)
        {
            if (gameName == GameName.None || gameName == GameName.Template)
            {
                ToggleGameButton.IsEnabled = false;
                return;
            }

            ToggleGameButton.IsEnabled = true;
        }

        public void LoadGame(ref ActiveGameData newGameData)
        {
            Game.Get.TileManager.GenerateDebugTiles(newGameData.PreferredGameTileSize);
            gameData = newGameData;
            newGameData = null;
            this.Title = gameData.GameName;
            DescriptionTextBlock.Text = gameData.GameDescription;
            this.Icon =  ImageExtensions.BitmapToImageSource(gameData.GameIcon);
            CustomControlArea.Content = gameData.GameCustomControl;
            SetStartButton(gameData.EnumValue);

            Game.Get.SetRunningGameBehavior(GameList.GetGameBehavior(gameData.EnumValue));
        }

        /// <summary>
        /// Start the process of loading or syncing tile data.
        /// </summary>
        private void SyncFloor_Click(object sender, RoutedEventArgs e)
        {
            // Dev testing Method loading in test data into the registered inputs
            MessageBox.Show($"This button has no current use since hardware is not currently being tested or integrated. {Environment.NewLine}" +
                            $"To change how the tiles load go change the parameters in GenerateDebugTiles() in TileManager.", "Sync Floor Info");

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
            ChooseGameButton.IsEnabled = !Game.Get.CoreLoop.IsRunning;
            SyncFloorButton.IsEnabled = !Game.Get.CoreLoop.IsRunning;
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

        #region previously used debug code, currently unused
        private void LeftViewLoadButton_Click(object sender, RoutedEventArgs e)
        {
            #region drawing extension functions test

            var img = ImageExtensions.BuildImage(Game.Get.TileManager.Tiles);
            img.DrawLine(new Vector2(10, 10), new Vector2(20, 15), 1, Colors.Blue());

            img.DrawRectangle(new Vector2(18, 18), 5, 7, false, 1, Colors.MediumPurple());
            img.DrawRectangle(new Vector2(14, 13), 2, 4, true, 1, Colors.HotPink());

            img.DrawCircle(new Vector2(25, 25), 5, false, 1, Colors.Black());
            img.DrawCircle(new Vector2(25, 10), 5, true, 1, Colors.White());

            var polygonPoints1 = new List<Vector2>() { new Vector2(10, 1), new Vector2(10, 30), new Vector2(30, 3) };
            var polygonPoints2 = new List<Vector2>() { new Vector2(4, 4), new Vector2(4, 7), new Vector2(7, 7) };
            img.DrawPolygon(polygonPoints1, false, 1, Colors.BrightBlue());
            img.DrawPolygon(polygonPoints2, true, 1, Colors.Green());

            img.DrawText(new Vector2(12, 20), "1", 15, Colors.Red());

            var bitmapImage = ImageExtensions.GetBitmapFromPath("Media/Testing/TestDrawnImage2.png");
            img.DrawImage(new Vector2(3, 20), bitmapImage);

            #endregion

            //img.Save("image.png",ImageFormat.Png);
            //Variable used to load the image in online so it can be copied and checked using https://codebeautify.org/base64-to-image-converter.
            //var strval = Convert.ToBase64String(ImageExtensions.ToByteArray(img)); 
            //LeftViewContainer.Source = ImageExtensions.BitmapToImageSource(img);
            var controlImage = new CustomImage
            {
                Source = ImageExtensions.BitmapToImageSource(img)
            };
            Main.GetDispatcher.Invoke(PopulateCanvasControlDelegate, LeftViewContainer, new List<CustomImage> { controlImage }, false);
        }

        private void MiddleViewLoadButton_Click(object sender, RoutedEventArgs e)
        {
            var size = VisualTreeHelper.GetContentBounds(LeftViewContainer.Children[0]);
            ImageSource src = ImageExtensions.BitmapToImageSource(
                ImageExtensions.CanvasToBitmap(LeftViewContainer, true, Convert.ToInt32(size.Width), Convert.ToInt32(size.Height))
                );
            var newtiles = src.MapImageToTiles(Game.Get.TileManager.Tiles);

            #region first moveable test
            /*var image = ImageExtensions.ConstructSensorDataBackground(Tile.GetDebugTiles(2, 8, 8, 8, 4, 4));
            var img = new CustomImage
            {
                Source = ImageExtensions.BitmapToImageSource(image)
            };
            img.SetValue(DraggableExtender.CanDragProperty, true);

            var secondImage = ImageExtensions.BuildImage(Tile.GetDebugTiles(6, 6, 8, 8, 4, 4));
            var secondImg = new CustomImage
            {
                Source = ImageExtensions.BitmapToImageSource(secondImage)
            };
            secondImg.SetValue(DraggableExtender.CanDragProperty, true);*/
            #endregion

            var imgList = ImageExtensions.GetIndividualSensorVisuals(Game.Get.TileManager.Tiles);
            Main.GetDispatcher.Invoke(PopulateCanvasControlDelegate, MiddleViewContainer, imgList, true);
        }

        // For scaling the images within the middle canvas. Functionality  currently disabled.
        private void MiddleViewSizeChange(object sender, RoutedEventArgs e)
        {
            var expand = ((Button) sender).Tag;
            if (expand.ToString() == "true")
            {
                MiddleViewContainer.Width *= 1.2;
                MiddleViewContainer.Height *= 1.2;
            }
            else
            {
                // reducing scale by ~ same amount as the upscale 1.2 is
                MiddleViewContainer.Width *= 0.8335;
                MiddleViewContainer.Height *= 0.8335;
            }
            MiddleViewContainer.ClipToBounds = true;
        }

        private void RightViewLoadButton_Click(object sender, RoutedEventArgs e)
        {
            /*RightControlViewViewBox.Child = new TileLayoutUserControl();
            var tileLayoutControl = RightControlViewViewBox.Child as TileLayoutUserControl;
            tileLayoutControl.PopulateControl(Game.Get.TileManager.Tiles);*/
        }
        #endregion

        /// <summary>
        /// Forces a refresh on controls specified within the method.
        /// </summary>
        public void Render()
        {
            // only render if changes were found
            if (!Game.Get.TileManager.RenderChanged)
            {
                return;
            }

            var leftViewImage = new CustomImage(Game.Get.TileManager.GetRenderGraphic());
            Main.GetDispatcher.Invoke(PopulateCanvasControlDelegate, LeftViewContainer,
                new List<CustomImage> {leftViewImage}, false);

            //var rightViewImage = new List
           /* var sensorTiles = new List<CustomImage>();
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
            PopulateCanvas(MiddleViewContainer, updatedSensorImages, false);*/

            // Force Refresh elements
            LeftViewContainer.Refresh();
            MiddleViewContainer.Refresh();
            this.Refresh();
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
                // On application shutdown sometimes it will finish calling rendering functions while the application itself is already unavailable.
                // This results in a NullReferenceException.
            }
        }
    }
}
