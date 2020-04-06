using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Wpf.Views.MainWindows;

namespace LightSpace_WPF_Engine.Models.Models
{
    /// <summary>
    /// <para>This class is an object in which the output data of the LightSpace floor can be stored.
    /// The data consists of 2 bytes.  </para>
    /// <para>    Byte 1: 1 bit detection, 7 bits tile number </para>
    /// <para>    Byte 2: 4 bits X position, 4 bits Y position </para>
    /// </summary>
    [Serializable]
    public class OutputData
    {
        public bool PressureDetected { get; set; }

        public int TileNumber { get; set; }

        public Vector2 Position { get; set; }

        /// <summary>
        /// Set data values from given byte array.
        /// </summary>
        /// <param name="bytes"> Byte array containing detection state, tile number data and X & Y position.</param>
        public void SetDataFromByteArray(byte[] bytes)
        {
            var pressureDetectedBinary = Convert.ToInt32(DataTool.SeparateBits(bytes[0], 7, 1));
            PressureDetected = Convert.ToBoolean(pressureDetectedBinary);

            var tileNumberBinary = DataTool.PadPartialByteToBinary(DataTool.SeparateBits(bytes[0], 1, 7));
            TileNumber = Convert.ToInt32(tileNumberBinary, 2);

            var xPosBinary = DataTool.PadPartialByteToBinary(DataTool.SeparateBits(bytes[1], 0, 4));
            var yPosBinary = DataTool.PadPartialByteToBinary(DataTool.SeparateBits(bytes[1], 4, 4));
            Position = new Vector2(Convert.ToInt32(xPosBinary, 2), Convert.ToInt32(yPosBinary, 2));
        }

        /// <summary>
        /// Pushes this OutputData onto the TileManager to properly set the input.
        /// </summary>
        public void PushToTileManager()
        {
            var tiles = Game.Get.TileManager.Tiles;
            foreach (var tile in tiles)
            {
                if (Tile.GetTrueListId(tiles, tile.Position) == TileNumber)
                {
                    Game.Get.TileManager.Tiles[tile.Position.X, tile.Position.Y].Sensors[Position.X, Position.Y]
                        .PressureDetected = PressureDetected;
                    return;
                }
            }
        }

        public static byte[] CreateTestBytes()
        {
            // Explanation for this process written in InputData.SetData() / InputData.CreateTestBytes().
            // This creates data able to be tested.
            var bytes = new byte[2];

            const bool detectionBit = true;
            const int tileNumberBits = 42;
            const int xPosition = 14;
            const int yPosition = 9;

            var tempByte = Convert.ToByte(detectionBit);
            var tempString1 = DataTool.SeparateBits(tempByte, 7, 1);
            tempByte = Convert.ToByte(tileNumberBits);
            var tempString2 = DataTool.SeparateBits(tempByte, 1, 7);
            var resultString = DataTool.CombineAndPadToBinary(tempString1, tempString2);
            bytes[0] = Convert.ToByte(resultString, 2);

            tempByte = Convert.ToByte(xPosition);
            tempString1 = DataTool.SeparateBits(tempByte, 4, 4);
            tempByte = Convert.ToByte(yPosition);
            tempString2 = DataTool.SeparateBits(tempByte, 4, 4);
            resultString = DataTool.CombineAndPadToBinary(tempString1, tempString2);
            bytes[1] = Convert.ToByte(resultString, 2);

            return bytes;
        }
    }
}