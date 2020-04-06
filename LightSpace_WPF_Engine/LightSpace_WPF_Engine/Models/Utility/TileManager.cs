using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LightSpace_WPF_Engine.Models.Models;

namespace LightSpace_WPF_Engine.Models.Utility
{
    //TODO: Make sure all instances of tiles being used use this specific instance
    public sealed class TileManager
    {
        public Tile[,] Tiles { get; private set; } = new Tile[0,0];
        private Bitmap gameRender;
        public bool RenderChanged { get; set; }

        private readonly object lockObject = new object();

        /// <summary>
        /// Generates a X*Y tile set with X*Y lights and all X*Y sensors inactive.
        /// </summary>
        public void GenerateDebugTiles()
        {
            Tiles = Tile.GetDebugTiles(5, 5, 8, 8, 4, 4,false);
        }

        public void SetRenderGraphic(Bitmap bitmap)
        {
            lock (lockObject)
            {
                gameRender = bitmap;
                RenderChanged = true;
            }
        }

        public Bitmap GetRenderGraphic()
        {
            lock (lockObject)
            {
                RenderChanged = false;
                return gameRender;
            }
        }

        public int GetLightAmount()
        {
            return Tiles[0, 0].Lights.GetLength(0);
        }

        public int GetSensorAmount()
        {
            return Tiles[0, 0].Sensors.GetLength(0);
        }

        public void ReloadTiles(int width, int height)
        {
            var tempTileList = FloorController.Get.GetTiles();
            var tileArray = new Tile[height, width];
            var columnIndex = 0;
            var rowIndex = 0;
            foreach (var tile in tempTileList)
            {
                tileArray[columnIndex, rowIndex] = tile;
                rowIndex++;

                if (rowIndex > height - 1)
                {
                    rowIndex = 0;
                    columnIndex++;
                }
            }

            // Alternate tiles so the grid follows the snaked line of the hardware
            Tiles = DataTool.GetAlternatedTileList(tileArray);
        }
    }
}
