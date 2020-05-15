using LightSpace_WPF_Engine.Models.Exceptions;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Models.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LightSpace_WPF_Engine.Wpf.Controls;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using FontFamily = System.Drawing.FontFamily;
using Image = System.Drawing.Image;
using Pen = System.Drawing.Pen;

namespace LightSpace_WPF_Engine.Models.Utility
{
    /// <summary>
    /// Tool for building images from and to data, or adding visuals to an image.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Returns bitmap based on partial path (Example: "Media/Testing/TestDrawnImage2.png")
        /// </summary>
        /// <param name="partialPath"> Partial path of the image.</param>
        /// <returns> Returns found Bitmap, or a 1*1 pixel size empty bitmap if nothing was found. </returns>
        public static Bitmap GetBitmapFromPath(string partialPath)
        {
            try
            {
                var path = Path.GetFullPath(partialPath);
                return new Bitmap(path.Replace("\\bin\\Debug", ""));
            }
            catch (Exception)
            {
                return new Bitmap(1,1);
            }
        }

        public static Bitmap SetPixel(this Bitmap image, Vector2 position, Color color)
        {
            image.SetPixel(position.X, position.Y, color);
            return image;
        }

        public static Bitmap DrawLine(this Bitmap image, Vector2 start, Vector2 stop, int width, Color color)
        {
            var pen = new Pen(color, width);
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.DrawLine(pen, start.X, start.Y, stop.X, stop.Y);
            }
            return image;
        }

        public static Bitmap DrawCircle(this Bitmap image, Vector2 position, int radius, bool fill, int lineWeight, Color color)
        {
            var pen = new Pen(color, lineWeight);
            using (var graphics = Graphics.FromImage(image))
            {
                if (fill)
                {
                    using (Brush brush = new SolidBrush(color))
                    {
                        graphics.FillEllipse(brush, position.X, position.Y, radius * 2, radius * 2);
                    }
                }
                graphics.DrawEllipse(pen, position.X, position.Y, radius * 2, radius * 2);
            }
            return image;
        }

        /// <summary>
        /// Draws a rectangle based on given parameters. Rectangle includes base position coordinates as first x/y row/column. (Width 2 = 3 wide drawn box)
        /// </summary>
        public static Bitmap DrawRectangle(this Bitmap image, Vector2 position, int width, int height, bool fill, int lineWeight, Color color)
        {
            var points = new List<Vector2>()
            {
                position,
                new Vector2(position.X + width, position.Y),
                new Vector2(position.X + width, position.Y + height),
                new Vector2(position.X, position.Y + height)
            };
            image.DrawPolygon(points, fill, lineWeight, color);

            return image;
        }

        public static Bitmap DrawPolygon(this Bitmap image, List<Vector2> positions, bool fill, int lineWeight, Color color)
        {
            var pen = new Pen(color, lineWeight);
            var points = Vector2.ToPointArray(positions);
            using (var graphics = Graphics.FromImage(image))
            {
                if (fill)
                {
                    using (Brush brush = new SolidBrush(color))
                    {
                        graphics.FillPolygon(brush, points);
                    }
                }
                graphics.DrawPolygon(pen, points);
            }
            return image;
        }

        public static Bitmap DrawText(this Bitmap image, Vector2 position, string text, int size, Color color)
        {
            using (var graphics = Graphics.FromImage(image))
            {
                using (Brush brush = new SolidBrush(color))
                {
                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                    graphics.DrawString(text, new Font(FontFamily.GenericMonospace, size), brush, position.X, position.Y);
                }
            }
            return image;
        }

        public static Bitmap DrawImage(this Bitmap image, Vector2 position, Bitmap imageToDraw)
        {
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.DrawImage(imageToDraw, position.X, position.Y, imageToDraw.Width, imageToDraw.Height);
            }
            return image;
        }

        public static Bitmap ConstructSensorDataBackground(Tile[,] tiles)
        {
            try
            {
                var lightAmount = tiles[0, 0].Lights.GetLength(0);
                var sensorAmount = tiles[0, 0].Sensors.GetLength(0);

                // Set size of all lights+sensor amounts combined * amount tiles.
                var imgSize = new Vector2(
                    tiles.GetLength(0) * (lightAmount + sensorAmount),
                    tiles.GetLength(1) * (lightAmount + sensorAmount)
                    );

                var img = new Bitmap(imgSize.X, imgSize.Y);
                img.DrawRectangle(Vector2.Zero(), imgSize.X, imgSize.Y, true, 5, Colors.DarkGray());

                // Generate horizontal lines.
                for (var lineX = 1; lineX < imgSize.Y - 1; lineX += 3)
                {
                    img.DrawLine(new Vector2(0, lineX), new Vector2(imgSize.X, lineX), 1, Colors.Black());
                }

                // Generate vertical lines.
                for (var lineY = 1; lineY < imgSize.X - 1; lineY += 3)
                {
                    img.DrawLine(new Vector2(lineY, 0), new Vector2(lineY, imgSize.Y), 1, Colors.Black());
                }

                // For every tile (X & Y) , go through every sensor (X & Y) and
                // draw every sensor point with the correct spacing.
                for (var tileX = 0; tileX < tiles.GetLength(0); tileX++)
                {
                    for (var tileY = 0; tileY < tiles.GetLength(1); tileY++)
                    {
                        for (var sensorX = 0; sensorX < tiles[tileX, tileY].Sensors.GetLength(0); sensorX++)
                        {
                            for (var sensorY = 0; sensorY < tiles[tileX, tileY].Sensors.GetLength(1); sensorY++)
                            {
                                var tile = tiles[tileX, tileY];
                                var sensor = tile.Sensors[sensorX, sensorY];
                                var color = sensor.PressureDetected ? Colors.DarkGreen() : Colors.MidnightRed();
                                img.SetPixel(
                                    (tileX * (lightAmount + sensorAmount) + (sensorX * 3)) + 1,
                                    (tileY * (lightAmount + sensorAmount) + (sensorY * 3)) + 1,
                                    color
                                );
                            }
                        }
                    }
                }
                return img;
            }
            catch (Exception exception)
            {
                ConsoleLogger.WriteToConsole(tiles, "Error building image from Tile[,] Array. " +
                                                    "(ImageExtensions)", exception);
                return new Bitmap(1, 1);
            }
        }

        public static List<CustomImage> GetIndividualSensorVisuals(Tile[,] tiles, bool draggable = true)
        {
            var imgList = new List<CustomImage>();
            for (var tileX = 0; tileX < tiles.GetLength(0); tileX++)
            {
                for (var tileY = 0; tileY < tiles.GetLength(1); tileY++)
                {
                    var tile = tiles[tileX, tileY];
                    var tempTileList = new Tile[1, 1];
                    tempTileList[0, 0] = tile;
                    var img = new CustomImage
                    {
                        Source = BitmapToImageSource(
                            ConstructSensorDataBackground(tempTileList)),
                    };
                    img.SetValue(DraggableExtender.CanDragProperty, draggable);
                    img.TileData = tile;
                    img.InCanvasPosition = (new System.Windows.Point(
                        (tileX * tile.Sensors.GetLength(0)) * 3,
                        (tileY * tile.Sensors.GetLength(1)) * 3));

                    RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
                    imgList.Add(img);
                }
            }

            return imgList;
        }

        public static CustomImage GetIndividualSensorVisual(Tile tile, bool showId = false)
        {
            var tempTileList = new Tile[1, 1];
            tempTileList[0, 0] = tile;
            var source = ConstructSensorDataBackground(tempTileList);
            if (showId)
            {
                source.DrawText(new Vector2(0, 0), tile.TileId.ToString(), 8, Color.Blue);
            }

            var img = new CustomImage
            {
                Source = BitmapToImageSource(source),
                TileData = tile,
                InCanvasPosition = new System.Windows.Point(0, 0),
            };
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.NearestNeighbor);
            return img;
        }

        public static Vector2 GetMaxSizeInList(List<CustomImage> images)
        {
            var vector = Vector2.One();
            foreach (var image in images)
            {
                if (image.Source == null)
                {
                    continue;
                }

                var x = image.Source.Width + image.InCanvasPosition.X;
                var y = image.Source.Height + image.InCanvasPosition.Y;

                if (vector.X < x)
                {
                    vector.X = Convert.ToInt32(Math.Round(x));
                }
                if (vector.Y < y)
                {
                    vector.Y = Convert.ToInt32(Math.Round(y));
                }
            }
            return vector;
        }

        /// <summary>
        /// Converts Canvas to Bitmap data.
        /// </summary>
        /// <param name="canvas"> The Canvas data used to create the image. </param>
        /// <param name="crop"> Optional bool signifying if the canvas needs to be cropped to a certain size. </param>
        /// <param name="width"> The width of the cropped image. </param>
        /// <param name="height"> The height of the cropped image. </param>
        /// <returns> The final image. Possibly cropped. </returns>
        public static Bitmap CanvasToBitmap(Canvas canvas, bool crop = false, int width = 40, int height = 40)
        {
            var rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
                (int)canvas.RenderSize.Height, 96d, 96d, PixelFormats.Default);
            rtb.Render(canvas);

            var pngEncoder = new PngBitmapEncoder();

            if (crop)
            {
                var croppedBmp = new CroppedBitmap(rtb, new Int32Rect(0, 0, width, height));
                pngEncoder.Frames.Add(BitmapFrame.Create(croppedBmp));
            }
            else
            {
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));
            }

            using (Stream s = new MemoryStream())
            {
                pngEncoder.Save(s);
                return new Bitmap(s);
            }
        }

        public static Tile[,] MapImageToTiles(this ImageSource image, Tile[,] tiles)
        {
            var lightAmount = tiles[0, 0].Lights.GetLength(0);
            var xTileSize = lightAmount * tiles.GetLength(0);
            var yTileSize = lightAmount * tiles.GetLength(1);
            
            // If the size doesn't match close enough to the tiles it won't match them and instead return the tiles as received.
            if (Math.Abs(image.Height - yTileSize) > .9 || Math.Abs(image.Width - xTileSize) > .9)
            {
                return tiles;
            }

            // Create a bmp from the image and check if it is not null before continuing.
            if (!(ImageWpfToGdi(image) is Bitmap bmp))
            {
                return tiles;
            }

            #region the -i dont get it- way
            /*// Lock the bitmaps bits.  

            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // Source: https://stackoverflow.com/questions/6020406/travel-through-pixels-in-bmp/6094092#6094092
            // Get the address of the first line.
            var ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            var bytes = bmpData.Stride * bmp.Height;
            var rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            var stride = bmpData.Stride;

            for (var column = 0; column < bmpData.Height; column++)
            {
                for (var row = 0; row < bmpData.Width; row++)
                {
                    var b = (byte)(rgbValues[(column * stride) + (row * 3)]);
                    var g = (byte)(rgbValues[(column * stride) + (row * 3) + 1]);
                    var r = (byte)(rgbValues[(column * stride) + (row * 3) + 2]);
                    tiles = SetColor(tiles, column, row, new Vector3(r, g, b));
                }
            }

            return tiles;*/
            #endregion

            #region the less performant way

            for (var xTile = 0; xTile < tiles.GetLength(0); xTile++)
            {
                for (var yTile = 0; yTile < tiles.GetLength(1); yTile++)
                {
                    for (var xLight = 0; xLight < lightAmount; xLight++)
                    {
                        for (var yLight = 0; yLight < lightAmount; yLight++)
                        {
                            var pixel = bmp.GetPixel((xTile * lightAmount) + xLight, (yTile * lightAmount) + yLight);
                            tiles[xTile, yTile].Lights[xLight, yLight].SetColor(pixel.R, pixel.G, pixel.B);
                        }
                    }
                }
            }

            return tiles;

            #endregion
        }

        public static Tile[,] SetColor(Tile[,] tiles, int column, int row, Vector3 color)
        {
            // Get amount of lights and coordinates of the tile to change.
            var lightAmount = tiles[0, 0].Lights.GetLength(0);
            var tileToChange = new Vector2(row/lightAmount, column / lightAmount);

            // Get the Column and Row location of the light to change.
            var lightPosC = (column % lightAmount);
            var lightPosR = (row % lightAmount);

            // Apply converted color to Light within Tile.
            tiles[tileToChange.X, tileToChange.Y].Lights[lightPosC, lightPosR].SetColor(Colors.VectorToColor(color,false));
            return tiles;
        }

        /// <summary>
        /// Converts ImageSource back into image.
        /// </summary>
        /// <param name="image"> ImageSource to be converted. </param>
        /// <returns> Converted Image. </returns>
        public static Image ImageWpfToGdi(ImageSource image)
        {
            var ms = new MemoryStream();
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image as BitmapSource));
            encoder.Save(ms);
            ms.Flush();
            return Image.FromStream(ms);
        }

        public static byte[] ToByteArray(this Image img)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    img.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
            catch (InvalidConversionException exception)
            {
                ConsoleLogger.WriteToUiConsole(img, "Error converting byte array to image. (ImageExtensions)", true, exception);
                return null;
            }
        }

        public static Image FromByteArray(this byte[] array)
        {
            try
            {
                using (var stream = new MemoryStream(array))
                {
                    using (var img = Image.FromStream(stream))
                    {
                        return img;
                    }
                }
            }
            catch (InvalidConversionException exception)
            {
                ConsoleLogger.WriteToUiConsole(array, "Error converting byte array to image. (ImageExtensions)", true, exception);
                return null;
            }
        }

        public static Bitmap BuildImage(Tile[,] tiles)
        {
            try
            {
                var lightAmount = tiles[0, 0].Lights.GetLength(0);

                var imgSize = new Vector2(tiles.GetLength(0) * lightAmount,
                               tiles.GetLength(1) * lightAmount);

                var img = new Bitmap(imgSize.X, imgSize.Y);

                for (var tileX = 0; tileX < tiles.GetLength(0); tileX++)
                {
                    for (var tileY = 0; tileY < tiles.GetLength(1); tileY++)
                    {
                        for (var lightX = 0; lightX < tiles[tileX, tileY].Lights.GetLength(0); lightX++)
                        {
                            for (var lightY = 0; lightY < tiles[tileX, tileY].Lights.GetLength(1); lightY++)
                            {
                                var tile = tiles[tileX, tileY];
                                var light = tile.Lights[lightX, lightY];

                                img.SetPixel(
                                    (tileX * lightAmount) + lightX,
                                    (tileY * lightAmount + lightY),
                                    Color.FromArgb(
                                        light.Color.X,
                                        light.Color.Y,
                                        light.Color.Z)
                                    );
                            }
                        }
                    }
                }
                return img;
            }
            catch (Exception exception)
            {
                ConsoleLogger.WriteToConsole(tiles, "Error building image from Tile[,] Array. " +
                                                    "(ImageExtensions)", exception);
                return new Bitmap(0, 0);
            }
        }

        public static BitmapImage BitmapToImageSource(this Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
