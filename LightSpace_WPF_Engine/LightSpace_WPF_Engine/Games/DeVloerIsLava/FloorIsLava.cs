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
        private int counterStart = 0;
        private bool startGame = true;

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

        // Gives the start signal of the game.
        public void StartGame(Bitmap tempBG)
        {
            var allTiles = Game.Get.TileManager.Tiles;
            var color = Colors.Red();

            // Goes through 4 different colors to make the startsignal
            switch (counterStart)
            {
                case 0:
                    color = Colors.Red();
                    break;
                case 1:
                    color = Colors.Black();
                    break;
                case 2:
                    color = Colors.Orange();
                    break;
                case 3:
                    color = Colors.Black();
                    break;
                case 4:
                    color = Colors.Green();
                    break;
                case 5:
                    color = Colors.Black();
                    break;
                default:
                    break;
            }

            // Draws all tiles one color on the background
            foreach (var tile in allTiles)
            {
                var pos = new Vector2(tile.Position.X * lightAmount, tile.Position.Y * lightAmount);

                tempBG.DrawRectangle(pos, lightAmount - 1, lightAmount - 1, true, 0, color);
            }

            if (counterStart > 4)
            {
                startGame = false;
            }

            counterStart++;
        }
    }
}