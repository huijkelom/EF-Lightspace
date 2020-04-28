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
        public static List<OutputData> ByteArrayToOutputData(byte[] bytes)
        {
            var outputList = new List<OutputData>();
            //TODO: 00 Test if this method is functional
            var tileGroup = BitConverter.ToInt16(new[] { bytes[0], bytes[1] }, 0); //bytes[0] | bytes[1] << 8; bitwise code
            for (var tile = 2; tile < 33; tile += 2)
            {
                // Make a single binary list from the 2 bytes.
                var sensorData = ByteToBinary(bytes[tile]);
                sensorData += ByteToBinary(bytes[tile + 1]);
                // Separate the string value into separate integer values.
                var datalist = new int[sensorData.Length];
                for (var index = 0; index < sensorData.Length; index++)
                {
                    datalist[index] = Convert.ToInt32(sensorData.Substring(index, 1));
                }

                #region type 1 : reversing certain areas of the data, comment out if unneeded.
                // The first iteration reverses number 4 to 7 so 4&7 will swap and 5&6 will swap
                var min = 4;
                var max = 6;
                for (var i = min; i < max; i++)
                {
                    var temp = datalist[max];
                    datalist[max] = datalist[min];
                    datalist[min] = temp;
                }

                // The first iteration reverses number 12 to 15 so 12&15 will swap and 13&14 will swap
                min = 12;
                max = 15;
                for (var i = min; i < max; i++)
                {
                    var temp = datalist[max];
                    datalist[max] = datalist[min];
                    datalist[min] = temp;
                }
                #endregion

                // Loop through data and add an OutputData object to the list for every piece of data.
                var xIndex = 0;
                var yIndex = 0;
                foreach (var data in datalist)
                {
                    xIndex++;
                    if (xIndex > 3)
                    {
                        xIndex = 0;
                        yIndex++;
                    }
                    outputList.Add(new OutputData()
                    {
                        PressureDetected = (data != 0),
                        TileNumber = Convert.ToInt16(tileGroup + tile/2),
                        Position = new Vector2(xIndex,yIndex)
                    });
                }
            }
            return outputList;
        }

        /// <summary>
        /// Set DataArray value to an array of bytes created from the given objects.
        /// </summary>
        /// <param name="tileNumberBits"> Tile number as needed by hardware. </param>
        /// <param name="colors"> 64 separate bytes giving a color of every led on the tile. </param>
        /// <returns> Returns byte array containing the given parameters ready to send colors to specified hardware tile. </returns>
        public static byte[] InputDataToByteArray(int tileNumberBits, Color[,] colors)
        {
            //TODO: 00 Test if this method is functional
            // create a byte array sized 66 bytes long
            var bytes = new byte[2 + (colors.GetLength(0) * colors.GetLength(1))];
            #region First 2 bytes : Tile Number
            // Convert tile number to a binary string with Format 2 (Binary)
            var binaryTileNumberString = Convert.ToString(tileNumberBits, 2);
            // Pad the binary string to fit 2 bytes and convert it to byte array
            binaryTileNumberString = binaryTileNumberString.PadLeft(16, '0');
            bytes = Encoding.ASCII.GetBytes(binaryTileNumberString);
            #endregion

            #region Last 64 bytes : 1 byte per Light (8*8 lights)
            var index = 0;
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
            ConsoleLogger.WriteToConsole(s+t, 
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
        public static Tile[,] GetAlternatedTileList(Tile[,] tileInput)
        {
            var newTiles = tileInput;
            for (var i = 0; i < newTiles.GetLength(0); i+=2)
            {
                var length1 = newTiles.GetLength(1);
                for (var j = 0; j < length1 / 2; j++)
                {
                    var temp = newTiles[i, j];
                    newTiles[i, j] = newTiles[i, length1 - j - 1];
                    newTiles[i, length1 - j - 1] = temp;
                }
            }
            return newTiles;
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

            returnValue.X = ShrinkByteData(color.R, 5,8);
            returnValue.Y = ShrinkByteData(color.G, 6,8);
            returnValue.Z = ShrinkByteData(color.B, 5,8);

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

            returnValue.X = GrowByteData(colorVector.X, 8,5);
            returnValue.Y = GrowByteData(colorVector.Y, 8,6);
            returnValue.Z = GrowByteData(colorVector.Z, 8,5);

            return Color.FromArgb(returnValue.X, returnValue.Y, returnValue.Z);
        }

        public static int ShrinkByteData(int value, int bitSize , int currentSize)
        {
            var factorIndex = currentSize - bitSize;
            var factor = GetFactor(factorIndex);

            return (int)Math.Floor(Convert.ToDecimal(value / factor));
        }

        public static int GrowByteData(int value, int bitSize, int currentSize)
        {
            var factorIndex = bitSize - currentSize;
            var factor= GetFactor(factorIndex);

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
