using System;
using System.Collections.Generic;
using System.Drawing;
using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Games.VLoerIsLava
{
    public class FloorIsLava : RunningGameBehavior
    {
        private Bitmap backgroundImage;
        private int lightAmount;
        private int sensorAmount;
        private int gameFieldWidth;
        private int gameFieldHeight;
        private List<Tile> GoodTiles = new List<Tile>();
        private List<Tile> resetTile = new List<Tile>();
        private int goodTilesAmount = 10;

        public override void Start()
        {
            base.Start();
            Game.Get.CoreLoop.TicksPerSecond = 10;
            GameName = GameName.FloorIsLava;
            Description = GameList.GetGameDescription(GameName);
            var tiles = Game.Get.TileManager.Tiles;
            GameFieldTileSize = new Vector2(tiles.GetLength(0), tiles.GetLength(1));

            lightAmount = Game.Get.TileManager.GetLightAmount();
            sensorAmount = Game.Get.TileManager.GetSensorAmount();

            gameFieldWidth = GameFieldTileSize.X * lightAmount;
            gameFieldHeight = GameFieldTileSize.Y * lightAmount;

            resetTile.Add(tiles[0, 0]);

            goodTilesAmount = GameFieldTileSize.X * GameFieldTileSize.Y / 2;

            backgroundImage = new Bitmap(gameFieldWidth, gameFieldHeight);
            backgroundImage.DrawRectangle(Vector2.Zero(), gameFieldWidth, gameFieldHeight, true, 2, Colors.Black());
            Game.Get.TileManager.SetRenderGraphic(backgroundImage);
        }
    }
}