using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Models.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LightSpace_WPF_Engine.Games.Dodgeball
{
    class DodgeballGameBehavior : RunningGameBehavior
    {
        #region Technical variables
        private readonly int ticksPerSecond = 5;
        private int lightAmount = 1;
        private int sensorAmount = 1;
        private int gameFieldWidth = 1;
        private int gameFieldHeight = 1;
        private Bitmap backgroundImage;
        #endregion

        #region Gameplay variables
        private List<DodgeballBallData> dodgeBalls;
        private int dodgeballCount = 3;
        private readonly float velocityRangeMin = .3f, velocityRangeMax = 1.7F;
        private readonly Vector2 ballSizeRange = new Vector2(1, 4);
        private readonly List<Color> availableBallColors = GenerateColorScheme();
        private readonly Color ballHitColor = Colors.Red();
        #endregion

        public override void Start()
        {
            // set ticks per second the core game loop makes, as well as retrieve the tiles measured
            Game.Get.CoreLoop.TicksPerSecond = ticksPerSecond;
            var tiles = Game.Get.TileManager.Tiles;
            GameFieldTileSize = new Vector2(tiles.GetLength(0), tiles.GetLength(1));

            lightAmount = Game.Get.TileManager.GetLightAmount();
            sensorAmount = Game.Get.TileManager.GetSensorAmount();
            GameName = GameName.Dodgeball;

            gameFieldWidth = GameFieldTileSize.X * lightAmount;
            gameFieldHeight = GameFieldTileSize.Y * lightAmount;

            backgroundImage = new Bitmap(gameFieldWidth, gameFieldHeight);
            backgroundImage.DrawRectangle(Vector2.Zero(), gameFieldWidth, gameFieldHeight, true, 2, Colors.Black());
            Game.Get.TileManager.SetRenderGraphic(backgroundImage);

            dodgeBalls = new List<DodgeballBallData>();
        }

        private static List<Color> GenerateColorScheme()
        {
            var tempColorList = new List<Color>();
            tempColorList.Add(Colors.Blue());
            tempColorList.Add(Colors.Green());
            tempColorList.Add(Colors.BrightBlue());
            tempColorList.Add(Colors.DarkGreen());
            tempColorList.Add(Colors.DarkBlue());
            tempColorList.Add(Colors.SeaGreen());
            tempColorList.Add(Colors.ForestGreen());
            return tempColorList;
        }

        public override void Update()
        {
            var rand = new Random();
            while (dodgeBalls.Count < dodgeballCount)
            {
                SpawnBall(rand);
            }
        }

        public override void LateUpdate()
        {
            foreach (var ball in dodgeBalls)
            {
                ball.CalculateNewVelocity(CheckFieldCollision(ball));
                ball.IsPlayerColliding = CheckPlayerCollision(ball);
            }
            Draw();
        }

        private void Draw()
        {
            if (dodgeBalls.Count < 1)
            {
                return;
            }

            var tempBitmap = backgroundImage.Clone() as Bitmap;
            foreach (var ball in dodgeBalls)
            {
                var color = !ball.IsPlayerColliding || !(ball.ShowCollisionColorOnFieldHit && ball.IsFieldColliding)
                    ? ball.BallColor : ballHitColor;

                tempBitmap.DrawRectangle(
                    new Vector2((int)ball.BallPositionX, (int)ball.BallPositionY),
                    ball.BallSize,
                    ball.BallSize,
                    true,
                    1,
                    color
                );
            }

            Game.Get.TileManager.SetRenderGraphic(tempBitmap);
        }

        private bool CheckPlayerCollision(DodgeballBallData ball)
        {
            var baseX = (int)Math.Floor(ball.BallPositionX);
            var baseY = (int)Math.Floor(ball.BallPositionY);
            var tiles = Game.Get.TileManager.Tiles;

            for (int x = baseX; x < baseX + ball.BallSize; x++)
            {
                for (int y = baseY; y < baseY + ball.BallSize; y++)
                {
                    var horizontalTilePos = x / lightAmount;
                    var horizontalLightPos = y / lightAmount;
                    foreach (var tile in tiles)
                    {
                        if (tile.Position.X == horizontalTilePos)
                        {
                            for (var index = 0; index < sensorAmount * 2; index += 2)
                            {
                                if (tile.GetSensorsNextToLight(new Vector2(horizontalLightPos, index)).PressureDetected)
                                {
                                    // collision detected, do whatever you want here in terms of game logic.
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool[] CheckFieldCollision(DodgeballBallData ball)
        {
            // Boolean array with side order:   left,  top,  right, bottom
            var collisionArray = new bool[4] { false, false, false, false };
            // ball params
            var minBallX = (int)Math.Floor(ball.BallPositionX);
            var minBallY = (int)Math.Floor(ball.BallPositionY);
            var maxBallX = (int)Math.Floor(ball.BallPositionX + ball.BallSize);
            var maxBallY = (int)Math.Floor(ball.BallPositionY + ball.BallSize);

            // field params
            var minX = 0;
            var minY = 0;
            var maxX = gameFieldWidth-1;
            var maxY = gameFieldHeight-1;

            if (minBallX <= minX)
            {
                collisionArray[0] = true;
            }
            if (minBallY <= minY)
            {
                collisionArray[1] = true;
            }
            if (maxBallX >= maxX)
            {
                collisionArray[2] = true;
            }
            if (maxBallY >= maxY)
            {
                collisionArray[3] = true;
            }

            return collisionArray;
        }

        private void SpawnBall(Random rand)
        {
            var xVelocity = GetNewVelocity(rand);
            var yVelocity = GetNewVelocity(rand);
            var ballSize = GetNewBallSize(rand);
            var spawnPos = GetNewSpawnPos(rand, ballSize);
            var ballColor = GetNewBallColor(rand);
            dodgeBalls.Add(new DodgeballBallData(spawnPos, xVelocity, yVelocity, ballSize, ballColor));
        }

        private Vector2 GetNewSpawnPos(Random rand, int ballSize)
        {
            var minX = 0;
            var minY = 0;
            var maxX = gameFieldWidth - ballSize;
            var maxY = gameFieldHeight - ballSize;

            return new Vector2(rand.Next(minX, maxX-1), rand.Next(minY, maxY-1));
        }

        private Color GetNewBallColor(Random rand)
        {
            if (availableBallColors.Count < 1)
            {
                return Colors.Blue();
            }

            return availableBallColors[rand.Next(availableBallColors.Count)];
        }

        private int GetNewBallSize(Random rand)
        {
            return rand.Next(ballSizeRange.X, ballSizeRange.Y);
        }

        private float GetNewVelocity(Random rand)
        {
            float roll = rand.Next((int)(velocityRangeMin * 100), (int)(velocityRangeMax * 100));
            return roll / 100;
        }
    }
}
