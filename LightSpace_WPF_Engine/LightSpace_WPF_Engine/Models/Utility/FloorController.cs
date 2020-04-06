using LightSpace_WPF_Engine.Models.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Models;

namespace LightSpace_WPF_Engine.Models.Utility
{
    //TODO: Implement hardware reading and writing
    public sealed class FloorController
    {
        private static FloorController _instance;

        public static FloorController Get => _instance ?? (_instance = new FloorController());

        public List<Tile> GameTiles { get; private set; }

        public static void ReadUsbData()
        {
            //TODO: Read USB Data
        }

        public static void WriteUsbData()
        {
            //Note: This code will throw an error and break the app currently. Once hardware is available to work with revise this to be done with delegates.
            //var imgSource = ImageExtensions.BitmapToImageSource(Game.Get.TileManager.GetRenderGraphic());
            //var tiles = imgSource.MapImageToTiles(Game.Get.TileManager.Tiles);
            //var alternatedTiles = DataTool.GetAlternatedTileList(tiles);

            //TODO: Write USB Data
        }

        public FloorController()
        {
            GameTiles = GetTiles();
        }

        public List<Tile> GetTiles()
        {
            //TODO: Get tiles and return them here
            return null;
        }

        public void SetFloorLights()
        {
            
        }
    }
}
