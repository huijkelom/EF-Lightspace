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
        private float counter = 0;
        private int counterStart = 0;
        private int counterEnd = 0;
        private int colorNumber = 0;
        private double timeProgression = 10;
        private int goodTilesAmount = 10;
        private bool firstTime = true;
        private bool startGame = true;
        private bool setReset = false;
        private bool gameOver = false;
        private bool playerInGoodTile = false;

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

        // The first update, used to select the goodTiles before being drawn
        public override void Update()
        {
            if (!startGame && !gameOver)
            {
                counter += Time.DeltaTime;

                // After a specified time the goodTiles are selected
                if (counter > timeProgression || firstTime)
                {
                    PickRandomTiles();
                    counter = 0;
                    firstTime = false;

                    // Decreases the time in between rounds, every round
                    if (timeProgression > 3)
                    {
                        timeProgression = timeProgression - (timeProgression * 0.05);
                    }
                }
            }
        }

        // Is the second update, the main part of the game is handled here.
        public override void LateUpdate()
        {
            var tempBG = backgroundImage.Clone() as Bitmap;

            // Performs the StartGame method multiple times
            if (startGame && !gameOver)
            {
                counter += Time.DeltaTime;
                if (counter > 1)
                {
                    counter = 0;
                    StartGame(tempBG);
                }
            }
            // Performs the GameOver method multiple times
            else if (gameOver && !setReset)
            {
                counter += Time.DeltaTime;

                if (counter > 0.1)
                {
                    counter = 0;
                    if (counterEnd < 16)
                    {
                        GameOver(tempBG);
                    }
                    else
                    {
                        setReset = true;
                    }
                }
            }
            // This if draws the goodTiles and the red background, it also tracks the player on the floor
            else if (!setReset)
            {
                counter += Time.DeltaTime;
                // After a period of time this if statement will be executed
                if (counter > timeProgression - 1)
                {
                    var allTiles = Game.Get.TileManager.Tiles;
                    playerInGoodTile = false;

                    // Draws all tiles Red on the Background
                    foreach (var tile in allTiles)
                    {
                        var pos = new Vector2(tile.Position.X * lightAmount, tile.Position.Y * lightAmount);

                        tempBG.DrawRectangle(pos, lightAmount - 1, lightAmount - 1, true, 0, Colors.Red());
                    }

                    // Checks every goodTile if player is on it
                    foreach (var tile in GoodTiles)
                    {
                        if (tile.AnyActiveSensorsInTile())
                        {
                            playerInGoodTile = true;
                        }
                    }

                    // When Player isnt on a goodTile the game is over
                    if (!playerInGoodTile)
                    {
                        gameOver = true;
                    }
                }

                DrawGoodTiles(tempBG);
                TrackPlayer(tempBG);
            }
            // When the game is over a there will be a yellow tile that marks the reset button
            else
            {
                var pos = new Vector2(0 * lightAmount, 0 * lightAmount);
                tempBG.DrawRectangle(pos, lightAmount - 1, lightAmount - 1, true, 0, Colors.Yellow());
                foreach (var tile in resetTile)
                {
                    if (tile.AnyActiveSensorsInTile())
                    {
                        Reset();
                    }
                }
            }

            Draw(tempBG);
        }

        // Draws all the goodtiles on the background
        private void DrawGoodTiles(Bitmap tempBG)
        {
            if (GoodTiles.Count > 0)
            {
                foreach (var tile in GoodTiles)
                {
                    var pos = new Vector2(tile.Position.X * lightAmount, tile.Position.Y * lightAmount);
                    var color = Colors.Green();

                    // Draw them at height&width = light amount. first x&y position is automatically added.
                    tempBG.DrawRectangle(pos, lightAmount - 1, lightAmount - 1, true, 0, color);
                }
            }
        }

        // Draws the background on the floor and shows this in the program
        public void Draw(Bitmap bmp)
        {
            Game.Get.TileManager.SetRenderGraphic(bmp);
        }

        // Check collision within tile, if collision found draw outline of the tile yellow
        public void TrackPlayer(Bitmap tempBG)
        {
            // Gets all tiles on the floor and stores them in allTiles
            var allTiles = Game.Get.TileManager.Tiles;

            // Goes through all the tiles in allTiles
            foreach (var tile in allTiles)
            {
                // If a sensor is activated in the tile it will outline the tile yellow
                if (tile.AnyActiveSensorsInTile())
                {
                    var pos = new Vector2(tile.Position.X * lightAmount, tile.Position.Y * lightAmount);

                    tempBG.DrawRectangle(pos, lightAmount - 1, lightAmount - 1, false, 0, Colors.Yellow());
                }
            }
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