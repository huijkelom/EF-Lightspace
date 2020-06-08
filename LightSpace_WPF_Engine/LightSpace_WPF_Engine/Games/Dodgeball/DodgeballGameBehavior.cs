using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Models.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using LightSpace_WPF_Engine.Models.Models;

namespace LightSpace_WPF_Engine.Games.Dodgeball
{
    /// <summary>
    /// The complete <see cref="RunningGameBehavior"/> of the Dodgeball minigame for the capacitive tile floor.
    /// </summary>
    class DodgeballGameBehavior : RunningGameBehavior
    {
        #region Technical variables
        /// <summary> An <see cref="int"/> determining the amount of ticks per second the game runs at. </summary>
        private readonly int ticksPerSecond = 5;
        /// <summary> An <see cref="int"/> keeping track of the amount of <see cref="Light"/>s per row/column of a <see cref="Tile"/>. </summary>
        private int lightAmount = 1;
        /// <summary> An <see cref="int"/> keeping track of the game field width in pixels. </summary>
        private int gameFieldWidth = 1;
        /// <summary> An <see cref="int"/> keeping track of the game field height in pixels. </summary>
        private int gameFieldHeight = 1;
        /// <summary> A <see cref="Bitmap"/> that works as a blank canvas upon which you <see cref="Draw"/> your gameplay data. </summary>
        private Bitmap backgroundImage;
        #endregion

        #region Gameplay variables
        /// <summary> The amount of dodgeballs active in the game as an <see cref="int"/>. </summary>
        public int DodgeballCount = 3;
        /// <summary> A <see cref="bool"/> determining wether the game skips its <see cref="Update"/> and <see cref="LateUpdate"/>. </summary>
        public bool Paused = false;

        /// <summary> A <see cref="bool"/> used as a way to incorporate clearing the <see cref="dodgeBalls"/> into the core game loop. </summary>
        private bool clearBalls = false;
        /// <summary> A List of <see cref="DodgeballBallData"/> keeping track of all positions and velocities of active balls. </summary>
        private List<DodgeballBallData> dodgeBalls;
        /// <summary> A <see cref="float"/> signifying one end of a range of possible velocity values. </summary>
        private readonly float velocityRangeMin = .3f, velocityRangeMax = 1.7F;
        /// <summary> A <see cref="Vector2"/> signifying a range of possible ball sizes. </summary>
        private readonly Vector2 ballSizeRange = new Vector2(1, 4);
        /// <summary> A List of <see cref="Color"/> used as a color scheme of available ball colors. </summary>
        private readonly List<Color> availableBallColors = GenerateColorScheme();
        /// <summary> A <see cref="Color"/> used for when a <see cref="DodgeballBallData"/> has collision with a player / <see cref="Sensor"/>. </summary>
        private readonly Color ballHitColor = Colors.Red();
        #endregion

        /// <summary>
        /// Initialization method. Executed once upon creation.
        /// </summary>
        public override void Start()
        {
            // set ticks per second the core game loop makes, as well as retrieve the tiles measured
            Game.Get.CoreLoop.TicksPerSecond = ticksPerSecond;
            var tiles = Game.Get.TileManager.Tiles;
            GameFieldTileSize = new Vector2(tiles.GetLength(0), tiles.GetLength(1));

            lightAmount = Game.Get.TileManager.GetLightAmount();
            GameName = GameName.Dodgeball;

            gameFieldWidth = GameFieldTileSize.X * lightAmount;
            gameFieldHeight = GameFieldTileSize.Y * lightAmount;

            backgroundImage = new Bitmap(gameFieldWidth, gameFieldHeight);
            backgroundImage.DrawRectangle(Vector2.Zero(), gameFieldWidth, gameFieldHeight, true, 2, Colors.Black());
            Game.Get.TileManager.SetRenderGraphic(backgroundImage);

            dodgeBalls = new List<DodgeballBallData>();
        }

        /// <summary>
        /// Creates a list based color scheme for use when picking a ball color.
        /// </summary>
        /// <returns> Returns a List of colors used as a color scheme created within this method. </returns>
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

        /// <summary>
        /// Executes once every game tick.
        /// </summary>
        public override void Update()
        {
            if (Paused)
            {
                return;
            }

            if (clearBalls)
            {
                dodgeBalls.Clear();
                clearBalls = false;
            }

            // Spawn balls till desired amount is reached.
            var rand = new Random();
            while (dodgeBalls.Count < DodgeballCount)
            {
                SpawnBall(rand);
            }

            // If too many balls are active, remove the ball depending on which code is active.
            while(dodgeBalls.Count > DodgeballCount && dodgeBalls.Count != 0)
            {
                // Delete newest.
                //dodgeBalls.RemoveAt(dodgeBalls.Count - 1);
                // Delete oldest.
                dodgeBalls.RemoveAt(0);
            }
        }

        /// <summary>
        /// Executes once every game tick after the Update() method is finished.
        /// </summary>
        public override void LateUpdate()
        {
            if (Paused)
            {
                return;
            }

            foreach (var ball in dodgeBalls)
            {
                ball.CalculateNewVelocity(CheckFieldCollision(ball));
                ball.IsPlayerColliding = CheckPlayerCollision(ball);
            }
            Draw();
        }

        /// <summary>
        /// Creates an image of the current game screen and pushes it onto the TileManager.
        /// </summary>
        private void Draw()
        {
            if (dodgeBalls.Count < 1)
            {
                return;
            }

            var tempBitmap = backgroundImage.Clone() as Bitmap;
            foreach (var ball in dodgeBalls)
            {
                var color = ball.IsPlayerColliding || (ball.ShowCollisionColorOnFieldHit && ball.IsFieldColliding)
                    ? ballHitColor : ball.BallColor;

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

        /// <summary>
        /// Check <see cref="Sensor"/> based collision for specified ball.
        /// </summary>
        /// <param name="ball"> Ball to check around for collision. </param>
        /// <returns> True on collision found, False on no collision found.</returns>
        private bool CheckPlayerCollision(DodgeballBallData ball)
        {
            var baseX = (int)Math.Floor(ball.BallPositionX);
            var baseY = (int)Math.Floor(ball.BallPositionY);
            var tiles = Game.Get.TileManager.Tiles;

            for (int x = baseX; x < baseX + ball.BallSize; x++)
            {
                for (int y = baseY; y < baseY + ball.BallSize; y++)
                {
                    // Get the tile to check collision in.
                    var horizontalTilePos = x / lightAmount;
                    var verticalTilepos = y / lightAmount;
                    foreach (var tile in tiles)
                    {
                        // If tile at current index is at the right horizontal and vertical position.
                        if (tile.Position.X == horizontalTilePos && tile.Position.Y == verticalTilepos)
                        {
                            // Loop through its lights and check surrounding sensors for pressure.
                            for (var index = 0; index < lightAmount; index ++)
                            {
                                if (tile.GetSensorsNextToLight(new Vector2(Math.Abs(x%lightAmount), Math.Abs(y%lightAmount))).PressureDetected)
                                {
                                    // Collision detected, do whatever you want here in terms of game logic.
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            // No collision detected
            return false;
        }

        /// <summary>
        /// Check collision with edges of the game field for every side of the ball.
        /// </summary>
        /// <param name="ball"> Ball that has its 4 directions checked for field collision. </param>
        /// <returns> Boolean array with side order: left, top, right, bottom. where true == collision and false == no collision. </returns>
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

        /// <summary>
        /// Spawns a ball with randomized values within a certain range.
        /// </summary>
        /// <param name="rand"> The shared <see cref="Random"/> used to avoid repeating random generation. </param>
        private void SpawnBall(Random rand)
        {
            var xVelocity = GetNewVelocity(rand);
            var yVelocity = GetNewVelocity(rand);
            var ballSize = GetNewBallSize(rand);
            var spawnPos = GetNewSpawnPos(rand, ballSize);
            var ballColor = GetNewBallColor(rand);
            dodgeBalls.Add(new DodgeballBallData(spawnPos, xVelocity, yVelocity, ballSize, ballColor));
        }

        /// <summary>
        /// Gets a randomized <see cref="Vector2"/> spawn position within a certain range.
        /// </summary>
        /// <param name="rand"> The shared <see cref="Random"/> used to avoid repeating random generation. </param>
        /// <param name="ballSize"> The size of the ball to determine how close the origin can spawn to borders. </param>
        /// <returns> A randomized spawn position within a certain range. </returns>
        private Vector2 GetNewSpawnPos(Random rand, int ballSize)
        {
            var margin = 2;
            var minX = margin;
            var minY = margin;
            var maxX = gameFieldWidth - ballSize;
            var maxY = gameFieldHeight - ballSize;

            return new Vector2(rand.Next(minX, maxX - margin), rand.Next(minY, maxY - margin));
        }

        /// <summary>
        /// Gets a randomized <see cref="Color"/> from the color scheme list <see cref="availableBallColors"/>.
        /// </summary>
        /// <param name="rand"> The shared <see cref="Random"/> used to avoid repeating random generation. </param>
        /// <returns> A randomized color from the color scheme list <see cref="availableBallColors"/>. </returns>
        private Color GetNewBallColor(Random rand)
        {
            if (availableBallColors.Count < 1)
            {
                return Colors.Blue();
            }

            return availableBallColors[rand.Next(availableBallColors.Count)];
        }

        /// <summary>
        /// Gets a randomized <see cref="int"/> ball size within a certain range.
        /// </summary>
        /// <param name="rand"> The shared <see cref="Random"/> used to avoid repeating random generation. </param>
        /// <returns> A randomized ball size within a certain range. </returns>
        private int GetNewBallSize(Random rand)
        {
            return rand.Next(ballSizeRange.X, ballSizeRange.Y);
        }

        /// <summary>
        /// Gets a randomized <see cref="float"/> velocity value within a certain range.
        /// </summary>
        /// <param name="rand"> The shared <see cref="Random"/> used to avoid repeating random generation. </param>
        /// <returns> A randomized velocity value within a certain range. </returns>
        private float GetNewVelocity(Random rand)
        {
            float roll = rand.Next((int)(velocityRangeMin * 100), (int)(velocityRangeMax * 100));
            var modifier = (rand.Next(0, 2) != 0) ? 0 : roll * 2;
            return (roll-modifier) / 100;
        }

        /// <summary>
        /// Sets a bool to clear ball list during the start of the next update. Doing it instantly may cause errors due to asynchronous callbacks.
        /// </summary>
        public void ClearBallList()
        {
            clearBalls = true;
        }
    }
}
