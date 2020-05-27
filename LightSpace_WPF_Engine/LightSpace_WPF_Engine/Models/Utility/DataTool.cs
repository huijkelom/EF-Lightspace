using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Models.Logging;

namespace LightSpace_WPF_Engine.Models.Utility
{
    /// <summary>
    /// Tool for converting data to and from binary, bits and bytes, or preparing data before converting.
    /// </summary>
    public static class DataTool
    {
        public static string SeparateBits(byte byteToSeparate, int startIndex, int length)
        {
            var byteString = ByteToBinary(byteToSeparate);
            if (startIndex + length <= byteString.Length)
            {
                return byteString.Substring(startIndex, length);
            }
            ConsoleLogger.WriteToConsole(byteString,
                $"Error grabbing substring from object with " +
                $"specified startIndex:{startIndex} and length:{length}");
            return byteString;
        }

        /// <summary>
        /// Set data values from given byte array.
        /// </summary>
        /// <param name="bytes"> Byte array containing tile group and 32 pairs of bytes, one pair for 32 tiles in a group.</param>
        public static Dictionary<short, bool[]> ToOutputDictionary(this byte[] bytes)
        {
            var dictionary = new Dictionary<short, bool[]>();
            for (var tile = 2; tile < 33; tile += 2)
            {
                // Make a single binary list from the 2 bytes.
                var sensorData = ByteToBinary(bytes[tile]);
                sensorData += ByteToBinary(bytes[tile + 1]);
                // Separate the string value into separate integer values.
                var datalist = new bool[sensorData.Length];
                for (var index = 0; index < sensorData.Length; index++)
                {
                    datalist[index] = sensorData.Substring(index, 1) == "1" ? true : false;
                }
                var tileNumber = ((tile / 2) - 1);
                dictionary.Add(Convert.ToInt16(tileNumber > 0 && tileNumber < 16 ? tileNumber : 0), datalist);
                continue;
            }
            return dictionary;
        }

        /// <summary>
        /// Set DataArray value to an array of bytes created from the given objects.
        /// </summary>
        /// <param name="tileNumberBits"> Tile number as needed by hardware. </param>
        /// <param name="colors"> 64 separate bytes giving a color of every led on the tile. </param>
        /// <returns> Returns byte array containing the given parameters ready to send colors to specified hardware tile. </returns>
        public static byte[] InputDataToByteArray(short tileNumberBits, Color[,] colors)
        {
            // create a byte array sized 66 bytes long
            var bytes = new byte[2 + (colors.GetLength(0) * colors.GetLength(1))];
            #region First 2 bytes : Tile Number

            bytes[0] = (byte)(0xFF & int.Parse(tileNumberBits.ToString()));
            bytes[1] = (byte)(0xFF & (int.Parse(tileNumberBits.ToString()) >> 8));
            #endregion

            #region Last 64 bytes : 1 byte per Light (8*8 lights)
            var index = 2;
            // Loop through the colors and add the color bytes to the byte array
            for (var x = 0; x < colors.GetLength(0); x++)
            {
                for (var y = 0; y < colors.GetLength(1); y++)
                {
                    bytes[index] = Colors.ColorX222ToByte(colors[x, y].R, colors[x, y].G, colors[x, y].B);
                    index++;
                }
            }
            #endregion

            return bytes;
        }

        public static string ByteToBinary(byte b)
        {
            return Convert.ToString(b, 2).PadLeft(8, '0');
        }

        public static string PadPartialByteToBinary(string s)
        {
            if (s.Length <= 8)
            {
                return Convert.ToString(s).PadLeft(8, '0');
            }
            ConsoleLogger.WriteToConsole(s, $"Error padding string specified :{s} to binary. Length > 8.");
            return s;
        }

        public static string CombineAndPadToBinary(string s, string t)
        {
            if (s.Length + t.Length <= 8)
            {
                return Convert.ToString(s + t).PadLeft(8, '0');
            }
            ConsoleLogger.WriteToConsole(s + t,
                $"Error combining strings specified values:{s} and :{t} to binary. Length > 8.");
            return s;
        }

        public static string Vector3ToByteString(Vector3 colorVector)
        {
            return $"{colorVector.X}|{colorVector.Y}|{colorVector.Z}";
        }

        //TODO: 02 Test if this is the right sequencing for the final result. If not, tweaking should be simply
        // changing i to 1 to offset the changing once. Tile Lights might also have to be completely rotated
        // but that's a whole new story.
        //TODO: 02 Change lights and sensors as well.
        public static Tile[,] GetAlternatedTileList(this Tile[,] tileInput)
        {
            var newTiles = tileInput;
            for (var i = 0; i < newTiles.GetLength(0); i += 2)
            {
                var length1 = newTiles.GetLength(1);
                for (var j = 0; j < length1 / 2; j++)
                {
                    var temp = newTiles[i, j];
                    newTiles[i, j] = newTiles[i, length1 - j - 1];
                    newTiles[i, length1 - j - 1] = temp;

                    // change the lights and sensors too
                    newTiles[i, j].FixInnerTile();
                    newTiles[i, length1 - j - 1].FixInnerTile();
                }
            }
            return newTiles;
        }

        /// <summary>
        /// Swap the rows of the lights and sensors of the tile. 
        /// </summary>
        /// <param name="tile"> Tile that will have its array rows swapped.</param>
        private static void FixInnerTile(this Tile tile)
        {
            // swap all the rows (X)
            for (var i = 0; i < tile.Lights.GetLength(0) / 2; i++)
            {
                // make the array
                var tempLightList = new Light[tile.Lights.GetLength(0)];
                for (var j = 0; j < tile.Lights.GetLength(0); j++)
                {
                    // fill the array
                    tempLightList[j] = tile.Lights[i, j];
                }

                for (var j = 0; j < tile.Lights.GetLength(0); j++)
                {
                    // fill the second array taken from the back
                    var tempLight = tile.Lights[tile.Lights.GetLength(0) - i - 1, j];
                    // overwrite 
                    tile.Lights[tile.Lights.GetLength(0) - i - 1, j] = tempLightList[j];
                    tile.Lights[i, j] = tempLight;
                }
            }

            for (var i = 0; i < tile.Sensors.GetLength(0) / 2; i++)
            {
                // make the array
                var tempSensorList = new Sensor[tile.Sensors.GetLength(0)];
                for (var j = 0; j < tile.Sensors.GetLength(0); j++)
                {
                    // fill the array
                    tempSensorList[j] = tile.Sensors[i, j];
                }

                for (var j = 0; j < tile.Sensors.GetLength(0); j++)
                {
                    // swap the value from the first array with the second value
                    var tempSensor = tile.Sensors[tile.Sensors.GetLength(0) - i - 1, j];
                    // overwrite 
                    tile.Sensors[tile.Sensors.GetLength(0) - i - 1, j] = tempSensorList[j];
                    tile.Sensors[i, j] = tempSensor;
                }
            }
        }

        public static Bitmap Rgb888To4Bpp(Bitmap image)
        {
            var newImage = image.Clone(
                new Rectangle(0, 0, image.Width, image.Height),
                PixelFormat.Format4bppIndexed
                );
            //new Bitmap(image.Width,image.Height, PixelFormat.Format4bppIndexed);
            return newImage;
        }

        public static Vector3 Rgb888ToRgb565(Color color)
        {
            var returnValue = Vector3.Zero();

            returnValue.X = ShrinkByteData(color.R, 5, 8);
            returnValue.Y = ShrinkByteData(color.G, 6, 8);
            returnValue.Z = ShrinkByteData(color.B, 5, 8);

            return returnValue;
        }

        public static Vector3 Rgb888ToRgb555(Color color)
        {
            var returnValue = Vector3.Zero();

            returnValue.X = ShrinkByteData(color.R, 5, 8);
            returnValue.Y = ShrinkByteData(color.G, 5, 8);
            returnValue.Z = ShrinkByteData(color.B, 5, 8);

            return returnValue;
        }

        public static Color Rgb565ToRgb888(Vector3 colorVector)
        {
            var returnValue = Vector3.Zero();

            returnValue.X = GrowByteData(colorVector.X, 8, 5);
            returnValue.Y = GrowByteData(colorVector.Y, 8, 6);
            returnValue.Z = GrowByteData(colorVector.Z, 8, 5);

            return Color.FromArgb(returnValue.X, returnValue.Y, returnValue.Z);
        }

        public static int ShrinkByteData(int value, int bitSize, int currentSize)
        {
            var factorIndex = currentSize - bitSize;
            var factor = GetFactor(factorIndex);

            return (int)Math.Floor(Convert.ToDecimal(value / factor));
        }

        public static int GrowByteData(int value, int bitSize, int currentSize)
        {
            var factorIndex = bitSize - currentSize;
            var factor = GetFactor(factorIndex);

            return (int)Math.Floor(Convert.ToDecimal(value * factor));
        }

        private static int GetFactor(int factorIndex)
        {
            var factor = 1;
            for (var i = 0; i < factorIndex; i++)
            {
                factor *= 2;
            }
            return factor;
        }

        /// <summary>
        /// Compares the two given arrays to check if they differ at all.
        /// </summary>
        /// <param name="firstArray"></param>
        /// <param name="secondArray"></param>
        /// <returns> Returns True if the array is equal.
        /// Returns False if either is null or array is not equal.</returns>
        public static bool CompareByteArrays(byte[] firstArray, byte[] secondArray)
        {
            if (firstArray != null && secondArray != null)
            {
                return firstArray.SequenceEqual(secondArray);
            }
            return false;
        }
    }
}
