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

        //TODO: Test if this is the right sequencing for the final result. If not, tweaking should be simply
        // changing i to 1 to offset the changing once. Tile Lights might also have to be completely rotated
        // but that's a whole new story.
        //TODO: Change lights and sensors as well.
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
