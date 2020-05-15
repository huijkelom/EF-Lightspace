using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        public delegate void SetRenderChangedDelegate( bool renderChanged);

        public SetRenderChangedDelegate SetRenderChangedEvent;

        public delegate bool GetRenderChangedDelegate();

        public GetRenderChangedDelegate GetRenderChangedEvent;

        private Bitmap gameRender;
        public BaseHardwareController HardwareController;
        public bool UseSimulatedTiles = false;
        public Vector2 FieldSize = Vector2.Zero();
        private readonly object lockObject = new object();
        private bool renderChanged;


        public TileManager()
        {
            HardwareController = new UsbFloorController(this);
            SetRenderChangedEvent += SetRenderChanged;
            GetRenderChangedEvent += GetRenderChanged;
        }

        /// <summary>
        /// Generates a X*Y tile set with X*Y lights and all X*Y sensors inactive.
        /// </summary>
        public void GenerateDebugTiles(Vector2 size, bool noHardware)
        {
            Tiles = Tile.GetDebugTiles(size.X, size.Y, 8, 8, 4, 4,false);
            HardwareController.NoHardwareMode = noHardware;
        }

        public void SetRenderChanged(bool newRenderChanged)
        {
            lock (lockObject)
            {
                renderChanged = newRenderChanged;
            }
        }

        public bool GetRenderChanged()
        {
            lock (lockObject)
            {
                return renderChanged;
            }
        }

        public void SetRenderGraphic(Bitmap bitmap)
        {
            lock (lockObject)
            {
                gameRender = bitmap;
                SetRenderChanged(true);
                var bitmapImageSource = bitmap.BitmapToImageSource();
                var tempTiles = bitmapImageSource.MapImageToTiles(Tiles);
                Tiles = tempTiles;
            }
        }

        public Bitmap GetRenderGraphic()
        {
            lock (lockObject)
            {
                return gameRender;
            }
        }

        public int GetLightAmount()
        {
            if (Tiles.GetLength(0) == 0 || Tiles.GetLength(1) == 0)
            {
                return 8;
            }
            return Tiles[0, 0].Lights.GetLength(0);
        }

        public int GetSensorAmount()
        {
            if (Tiles.GetLength(0) == 0 || Tiles.GetLength(1) == 0)
            {
                return 8;
            }
            return Tiles[0, 0].Sensors.GetLength(0);
        }
    }
}
