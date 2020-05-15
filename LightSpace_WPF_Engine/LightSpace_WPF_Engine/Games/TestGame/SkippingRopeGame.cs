using System;
using System.Drawing;
using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Models.Exceptions;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Games.TestGame
{
    /// <summary>
    /// Small test demo to show systems in the engine working.
    /// </summary>
    public class SkippingRopeGame : RunningGameBehavior
    {
        public bool Paused;

        // Game Settings
        public double ProgressDelay { get; set; } = .05;
        public double JumpDelayMin { get; set; } = .3;
        public double JumpDelayMax { get; set; } = 2;
        private const int TicksPerSecond = 10;

        #region Gameplay Variables

        private int lightAmount;
        private int sensorAmount;
        private Bitmap backgroundImage;
        private int gameFieldWidth;
        private int gameFieldHeight;

        private float jumpDelay = 3f;
        private int lastColumnIndex = 0;
        private int columnIndex = -1;
        private float jumpTimer = 0;
        private float progressTimer = 0;

        #endregion

        public override void Start()
        {
            // set ticks per second the core game loop makes, as well as retrieve the tiles measured
            Game.Get.CoreLoop.TicksPerSecond = TicksPerSecond;
            var tiles = Game.Get.TileManager.Tiles;
            GameFieldTileSize = Game.Get.TileManager.FieldSize;
            GameFieldTileSize =
                GameFieldTileSize.X == 0 || GameFieldTileSize.Y == 0 ? Vector2.One() : GameFieldTileSize;
            lightAmount = Game.Get.TileManager.GetLightAmount();
            sensorAmount = Game.Get.TileManager.GetSensorAmount();
            GameName = GameName.TestGame0;

            gameFieldWidth = GameFieldTileSize.X * lightAmount;
            gameFieldHeight = GameFieldTileSize.Y * lightAmount;

            backgroundImage = new Bitmap(gameFieldWidth, gameFieldHeight);
            backgroundImage.DrawRectangle(Vector2.Zero(), gameFieldWidth, gameFieldHeight, true, 2, Colors.Black());
            Game.Get.TileManager.SetRenderGraphic(backgroundImage);
        }

        public override void Update()
        {
            if (Paused)
            {
                return;
            }

            var deltaTime = Time.DeltaTime;
            jumpTimer += deltaTime;
            if (jumpTimer > jumpDelay)
            {
                progressTimer += deltaTime;
                if (progressTimer > ProgressDelay)
                {
                    columnIndex++;
                    progressTimer = 0;

                    if (columnIndex == GameFieldTileSize.X * lightAmount)
                    {
                        columnIndex = -1;
                        var rnd = new Random();
                        jumpDelay = (float)(rnd.NextDouble() * (JumpDelayMax - JumpDelayMin) + JumpDelayMin);
                        jumpTimer = 0;
                    }
                }
            }
        }

        public override void LateUpdate()
        {
            //ConsoleLogger.WriteToConsole(this,"SkippingRopeGame LateUpdate Draw");
            Draw(CheckCollision());
        }

        private Color CheckCollision()
        {
            if (columnIndex == -1)
            {
                return Colors.Blue();
            }

            var horizontalTilePos = columnIndex/lightAmount;

            var horizontalLightPos = columnIndex % lightAmount;
            var tiles = Game.Get.TileManager.Tiles;
            foreach (var tile in tiles)
            {
                if (tile.Position.X == horizontalTilePos)
                {
                    for (var index = 0; index < sensorAmount*2; index+=2)
                    {
                        if (tile.GetSensorsNextToLight(new Vector2(horizontalLightPos, index)).PressureDetected)
                        {
                            // collision detected, do whatever you want here in terms of game logic.
                            return Colors.Red();
                        }
                    }
                }
            }
            return Colors.Blue();
        }

        private void Draw(Color lineColor)
        {
            if (columnIndex == lastColumnIndex)
            {
                return;
            }
            // if the skipping rope is within the screen, display it, otherwise push the background to render
            if (columnIndex != -1)
            {
                var tempBitmap = backgroundImage.Clone() as Bitmap;
                tempBitmap.DrawLine(
                    new Vector2(columnIndex, 0), 
                    new Vector2(columnIndex, gameFieldHeight),
                    1,
                    lineColor);
                Game.Get.TileManager.SetRenderGraphic(tempBitmap);
            }
            else
            {
                var tempBitmap = backgroundImage.Clone() as Bitmap;
                Game.Get.TileManager.SetRenderGraphic(tempBitmap);
            }

            lastColumnIndex = columnIndex;
        }
    }
}