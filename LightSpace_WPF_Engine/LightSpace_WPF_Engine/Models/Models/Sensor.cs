using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Models.Models
{
public class Sensor
    {
        public int TileId { get; set; }

        public bool PressureDetected { get; set; }

        public Vector2 Position { get; set; }

        public Sensor(int tileId, bool pressureDetected, Vector2 position)
        {
            TileId = tileId;
            PressureDetected = pressureDetected;
            Position = position;
        }
    }
}
