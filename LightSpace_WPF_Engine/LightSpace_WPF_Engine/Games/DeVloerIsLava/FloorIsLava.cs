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
        private int counterEnd = 0;
        private int colorNumber = 0;
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

        // Gets random tiles where the player can stand on
        public void PickRandomTiles()
        {
            GoodTiles = new List<Tile>();
            var tiles = Game.Get.TileManager.Tiles;
            var rng = new Random();

            // Picks goodTilesAmount goodTiles where the player can stand on
            for (var index = 0; index < goodTilesAmount; index++)
            {
                GoodTiles.Add(tiles[
                    rng.Next(0, tiles.GetLength(0)),
                    rng.Next(0, tiles.GetLength(1))
                ]);
            }

            // Every round there is one less goodTile untill it gets to 1
            if (goodTilesAmount > 1)
            {
                goodTilesAmount--;
            }
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

        // Gives the end signal of the game
        public void GameOver(Bitmap tempBG)
        {
            var allTiles = Game.Get.TileManager.Tiles;
            var color = Colors.Red();
            startGame = true;

            // Sets the color of the floor
            switch (colorNumber)
            {
                case 0:
                    color = Colors.Red();
                    colorNumber = 1;
                    break;
                case 1:
                    color = Colors.Black();
                    colorNumber = 0;
                    break;
            }

            //Every tile in allTiles is being drawn on de tempBG with the assigned color
            foreach (var tile in allTiles)
            {
                var pos = new Vector2(tile.Position.X * lightAmount, tile.Position.Y * lightAmount);

                tempBG.DrawRectangle(pos, lightAmount - 1, lightAmount - 1, true, 0, color);
            }

            counterEnd++;
        }
    }
}