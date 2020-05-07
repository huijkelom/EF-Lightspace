using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Utility.Hardware;

namespace LightSpace_WPF_Engine.Models.Utility
{
    //TODO: 01 Make sure all instances of tiles being used use this specific instance
    public sealed class TileManager
    {
        public Tile[,] Tiles { get; private set; } = new Tile[0,0];
        private Bitmap gameRender;
        public bool RenderChanged { get; set; }
        public BaseHardwareController BaseHardwareController;
        public bool UseSimulatedTiles = false;
        public Vector2 FieldSize = Vector2.Zero();
        private readonly object lockObject = new object();

        public TileManager()
        {
            BaseHardwareController = new UsbFloorController(this);
        }

        /// <summary>
        /// Generates a X*Y tile set with X*Y lights and all X*Y sensors inactive.
        /// </summary>
        public void GenerateDebugTiles(Vector2 size)
        {
            //TODO: 10 Make something to set this per game (preferred floor size in game?)
            Tiles = Tile.GetDebugTiles(size.X, size.Y, 8, 8, 4, 4,false);
            BaseHardwareController.NoHardwareMode = true;
        }

        public void SetRenderGraphic(Bitmap bitmap)
        {
            lock (lockObject)
            {
                gameRender = bitmap;
                RenderChanged = true;
                Tiles = bitmap.BitmapToImageSource().MapImageToTiles(Tiles);
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
            if (Tiles.GetLength(0) == 0 || Tiles.GetLength(1) == 0)
            {
                return 0;
            }
            return Tiles[0, 0].Lights.GetLength(0);
        }

        public int GetSensorAmount()
        {
            if (Tiles.GetLength(0) == 0 || Tiles.GetLength(1) == 0)
            {
                return 0;
            }
            return Tiles[0, 0].Sensors.GetLength(0);
        }
    }
}
