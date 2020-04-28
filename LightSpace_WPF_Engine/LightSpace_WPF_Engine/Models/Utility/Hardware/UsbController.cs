using System.Collections.Generic;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Utility.Hardware;

namespace LightSpace_WPF_Engine.Models.Utility
{
    //TODO: 00 Implement hardware reading and writing
    public sealed class UsbFloorController : IHardwareController
    {
        public void ReadData()
        {
            //TODO: 00 Read USB Data
        }

        public void WriteData()
        {
            //Note: This code will throw an error and break the app currently. Once hardware is available to work with revise this to be done with delegates.
            //var imgSource = ImageExtensions.BitmapToImageSource(Game.Get.TileManager.GetRenderGraphic());
            //var tiles = imgSource.MapImageToTiles(Game.Get.TileManager.Tiles);
            //var alternatedTiles = DataTool.GetAlternatedTileList(tiles);

            //TODO: 00 Write USB Data
        }

        public List<Tile> GetTiles()
        {
            //TODO: 04 Get tiles and return them here
            return null;
        }
    }
}
