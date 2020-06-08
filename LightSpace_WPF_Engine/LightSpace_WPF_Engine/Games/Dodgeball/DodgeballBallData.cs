using LightSpace_WPF_Engine.Models.Utility;
using System.Drawing;
using LightSpace_WPF_Engine.Models.Models;

namespace LightSpace_WPF_Engine.Games.Dodgeball
{
    /// <summary>
    /// An object containing all neccesary data for a dodgeball used within <see cref="DodgeballGameBehavior"/>.
    /// </summary>
    class DodgeballBallData
    {
        /// <summary> The ball position as a <see cref="float"/> to make more bouncing angles possible. </summary>
        public float BallPositionX = 0, BallPositionY = 0;
        /// <summary> The ball velocity as a <see cref="float"/> to make more bouncing angles possible. </summary>
        public float VelocityX = 1, VelocityY = 1;
        /// <summary> The ball size as an <see cref="int"/>. </summary>
        public int BallSize = 1;
        /// <summary> The ball <see cref="Color"/> as it's rendered normally. </summary>
        public Color BallColor;

        /// <summary> A <see cref="bool"/> checking Player / <see cref="Sensor"/> collision. </summary>
        public bool IsPlayerColliding = false;
        /// <summary> A <see cref="bool"/> checking game field border collision. </summary>
        public bool IsFieldColliding = false;
        /// <summary> A <see cref="bool"/> signifying wether hitting the game border should show collision <see cref="Color"/>. </summary>
        public bool ShowCollisionColorOnFieldHit = false;

        /// <summary>
        /// Constructor for DodgeBall ball data.
        /// </summary>
        /// <param name="spawnPosition"> Spawn position in whole numbers to determine spawn location.</param>
        /// <param name="startVelocityX"> Start velocity X as float so it can go on non-45 degree angles over longer distances. </param>
        /// <param name="startVelocityY"> Start velocity Y as float so it can go on non-45 degree angles over longer distances. </param>
        /// <param name="ballSize"> Size of ball used for collision and rendering. </param>
        /// <param name="ballColor"> Color of ball used for rendering. </param>
        public DodgeballBallData(Vector2 spawnPosition, float startVelocityX, float startVelocityY, int ballSize, Color ballColor)
        {
            BallPositionX = spawnPosition.X;
            BallPositionY = spawnPosition.Y;
            VelocityX = startVelocityX;
            VelocityY = startVelocityY;
            BallSize = ballSize;
            BallColor = ballColor;
        }

        /// <summary>
        /// Calculates new velocity based on existing velocity and inputs
        /// </summary>
        /// <param name="sidesBlocked"> A length 4 boolean array is expected, in order Left,Top,Right,Bottom with true on a border reached and false for open space.</param>
        public void CalculateNewVelocity(bool[] sidesBlocked)
        {
            var hit = false;
            // Left or Right is blocked, invert sideways velocity
            if(sidesBlocked[0] || sidesBlocked[2])
            {
                VelocityX = -VelocityX;
                hit = true;
            }

            // Top or Bottom is blocked, invert sideways velocity
            if (sidesBlocked[1] || sidesBlocked[3])
            {
                VelocityY = -VelocityY;
                hit = true;
            }
            if (ShowCollisionColorOnFieldHit)
            {
                IsFieldColliding = hit;
            }
            Step();
        }

        public void Step()
        {
            BallPositionX += VelocityX;
            BallPositionY += VelocityY;
        }

        public override string ToString()
        {
            return $"Ball (size:{BallSize} color:{BallColor}) @ X{BallPositionX.ToString("n2")} Y{BallPositionY.ToString("n2")} | Velocity @ X{VelocityX.ToString("n2")} Y{VelocityY.ToString("n2")}";
        }
    }
}
