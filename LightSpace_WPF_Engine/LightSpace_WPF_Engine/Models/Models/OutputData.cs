﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Wpf.Views.MainWindows;

namespace LightSpace_WPF_Engine.Models.Models
{
    /// <summary>
    /// <para>This class is an object in which the output data of the LightSpace floor can be stored. </para>
    /// </summary>
    [Serializable]
    public class OutputData
    {
        public bool PressureDetected { get; set; }

        public short TileNumber { get; set; }

        public Vector2 Position { get; set; }

        /// <summary>
        /// Pushes this OutputData PressureDetected onto the TileManager to properly set the input.
        /// </summary>
        public void PushToTileManager()
        {
            var tiles = Game.Get.TileManager.Tiles;
            foreach (var tile in tiles)
            {
                if (tiles.GetTrueListId(Position) == TileNumber)
                {
                    try
                    {
                        Game.Get.TileManager.Tiles[tile.Position.X, tile.Position.Y].Sensors[Position.X, Position.Y]
                            .PressureDetected = PressureDetected;
                    }
                    catch(Exception exception)
                    {
                        ConsoleLogger.WriteToConsole(this,"Tile to modify out of index", exception);
                    }
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