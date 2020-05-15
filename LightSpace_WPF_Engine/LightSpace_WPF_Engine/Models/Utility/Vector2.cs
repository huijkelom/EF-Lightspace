using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;

namespace LightSpace_WPF_Engine.Models.Utility
{
    [Serializable]
    public class Vector2
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Vector2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2()
        {

        }

        public static Point[] ToPointArray(List<Vector2> vectors)
        {
            var points = new Point[vectors.Count];
            for (var i = 0; i < vectors.Count; i++)
            {
                points[i] = new Point(vectors[i].X,vectors[i].Y);
            }
            return points;
        }

        public bool CompareTo(Vector2 otherVector2)
        {
            return X == otherVector2.X && Y == otherVector2.Y;
        }

        public void MapHighestValue(Vector2 otherVector2)
        {
            if (otherVector2.X > X)
            {
                X = otherVector2.X;
            }
            if (otherVector2.Y > Y)
            {
                Y = otherVector2.Y;
            }
        }

        public static System.Windows.Point ToPoint(Vector2 vector2)
        {
            return new System.Windows.Point(vector2.X,vector2.Y);
        }

        public static Vector2 FromPoint(System.Windows.Point point)
        {
            return new Vector2(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
        }

        public static Vector2 Zero()
        {
            return new Vector2(0,0);
        }

        public static Vector2 One()
        {
            return new Vector2(1, 1);
        }

        public override string ToString()
        {
            return $"{X}|{Y}";
        }
    }
}
