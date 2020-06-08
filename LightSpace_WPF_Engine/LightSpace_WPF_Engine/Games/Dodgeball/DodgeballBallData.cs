using LightSpace_WPF_Engine.Models.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Games.Dodgeball
{
    class DodgeballBallData
    {
        public float BallPositionX = 0, BallPositionY = 0;
        public float VelocityX = 1, VelocityY = 1;  
        public int BallSize = 1;
        public Color BallColor;

        public bool IsPlayerColliding = false;
        public bool IsFieldColliding = false;

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
