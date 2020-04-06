using LightSpace_WPF_Engine.Models.Utility;
using System.Drawing;

namespace LightSpace_WPF_Engine.Models.Models
{
    public class Light 
    {
        public Vector2 Position { get; set; }

        public Vector3 Color { get; private set; }

        public Light(Vector2 position, Vector3 color)
        {
            Position = position;
            Color = color;
        }

        public void SetColor(Color color)
        {
            this.Color = new Vector3(color.R, color.G, color.B);
        }

        public void SetColor(int r, int g, int b)
        {
            this.Color = new Vector3(r, g, b);
        }
    }
}
