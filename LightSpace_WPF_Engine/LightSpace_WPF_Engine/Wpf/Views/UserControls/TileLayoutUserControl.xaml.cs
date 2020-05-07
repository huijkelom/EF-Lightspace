using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Wpf.Controls;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace LightSpace_WPF_Engine.Wpf.Views.UserControls
{
    public partial class TileLayoutUserControl : UserControl
    {
        private CustomImage selectedImage;
        private List<List<CustomImage>> imagesList;
        private Tile[,] tileDataList;

        public TileLayoutUserControl()
        {
            InitializeComponent();
            imagesList = new List<List<CustomImage>>();
        }

        public void SelectTile(CustomImage customImage)
        {
            if (selectedImage != null && selectedImage.TileData.TileId == customImage.TileData.TileId)
            {
                var customImg = ImageExtensions.GetIndividualSensorVisual(selectedImage.TileData, true);
                selectedImage.Source = customImg.Source;
                selectedImage = null;
            }
            else if(selectedImage != null)
            {
                var selectedX = Convert.ToInt32(selectedImage.InCanvasPosition.X);
                var selectedY = Convert.ToInt32(selectedImage.InCanvasPosition.Y);
                var selectedPos = selectedImage.TileData.Position;
                var customX = Convert.ToInt32(customImage.InCanvasPosition.X);
                var customY = Convert.ToInt32(customImage.InCanvasPosition.Y);
                var customPos = customImage.TileData.Position;

                imagesList[selectedX][selectedY] = customImage;
                imagesList[selectedX][selectedY].InCanvasPosition = new Point(selectedX,selectedY);
                tileDataList[selectedX, selectedY] = customImage.TileData;
                tileDataList[selectedX, selectedY].Position = customPos;

                imagesList[customX][customY] = selectedImage;
                imagesList[customX][customY].InCanvasPosition = new Point(customX, customY);
                tileDataList[customX, customY] = selectedImage.TileData;
                tileDataList[customX, customY].Position = selectedPos;

                var customImg = ImageExtensions.GetIndividualSensorVisual(selectedImage.TileData,true);
                imagesList[customX][customY].Source = customImg.Source;
                selectedImage = null;
            }
            else
            {
                selectedImage = customImage;
                var bmp = new Bitmap(12,12);
                bmp.DrawRectangle(Vector2.Zero(), 15, 15, true, 2, Color.Red);
                bmp.DrawText(new Vector2(0, 0), customImage.TileData.TileId.ToString(), 8, Color.Blue);
                selectedImage.Source = ImageExtensions.BitmapToImageSource(bmp);
            }
        }

        public void PopulateControl(Tile[,] tiles)
        {
            tileDataList = tiles;
            imagesList = new List<List<CustomImage>>();
            for (var tileX = 0; tileX < tiles.GetLength(0); tileX++)
            {
                imagesList.Add(new List<CustomImage>());
                for (var tileY = 0; tileY < tiles.GetLength(1); tileY++)
                {
                    var tile = tiles[tileX, tileY];
                    tile.TileId = (short)((tileX * tiles.GetLength(1)) + tileY);
                    var customImage = ImageExtensions.GetIndividualSensorVisual(tile,true);

                    customImage.TileData = tile;
                    customImage.InCanvasPosition = new Point(tileX, tileY);
                    customImage.MouseLeftButtonDown += (sender, args) => SelectTile(customImage);
                    imagesList[tileX].Add(customImage);
                }
            }

            foreach (var item in imagesList.SelectMany(list => list))
            {
                item.Margin = new Thickness(1);
            }
            PopulateCanvas(RightViewControl, imagesList);
        }

        private static void PopulateCanvas(Canvas canvas, List<List<CustomImage>> images)
        {
            canvas.Children.Clear();
            foreach (var graphic in images.SelectMany(imageList => imageList))
            {
                canvas.Children.Add(graphic);
            }

            foreach (UIElement child in canvas.Children)
            {
                if (child is CustomImage temp)
                {
                    Canvas.SetLeft(child, temp.InCanvasPosition.X*14);
                    Canvas.SetTop(child, temp.InCanvasPosition.Y*14);
                }
            }

            var contentBounds = Vector2.Zero();
            foreach (var list in images)
            {
                contentBounds.MapHighestValue( ImageExtensions.GetMaxSizeInList(list));
            }

            if (contentBounds.X > contentBounds.Y)
            {
                canvas.Width = contentBounds.X * images.Count;
                canvas.Height = contentBounds.X * images.Count;
            }
            else
            {
                canvas.Width = contentBounds.Y * images[0].Count;
                canvas.Height = contentBounds.Y * images[0].Count;
            }
            canvas.RenderSize = new System.Windows.Size(canvas.Width, canvas.Height);
            canvas.Background = (SolidColorBrush)Application.Current.Resources["LightSpaceGray"];
        }
    }
}
