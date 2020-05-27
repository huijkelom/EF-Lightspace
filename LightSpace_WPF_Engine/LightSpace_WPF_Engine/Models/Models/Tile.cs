using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Models.Models
{
    [Serializable]
    //TODO: 03 Check out all cases to see if x & y of tile setting in 2D array are done right (x = column, y = row)
    public class Tile
    {
        // Tile ID
        public short TileId { get; set; }

        // Position of the tile on a grid.
        public Vector2 Position { get; set; } = Vector2.Zero();

        // Should be 64 in total. (8*8)
        public Light[,] Lights { get; set; } = new Light[0, 0];

        // Should be 16 in total. (4*4)
        public Sensor[,] Sensors { get; set; } = new Sensor[0, 0];

        private object lockObject = new object();

        public Tile(short tileId)
        {
            TileId = tileId;
            Init();
        }

        public void Init()
        {
            Position = new Vector2(0, 0);
            Lights = new Light[8, 8];
            Sensors = new Sensor[4, 4];
        }

        public void SetSensorDetectionByNumber(short number, bool value)
        {
            lock (lockObject)
            {
                var x = (int)Math.Floor((double)(number / Sensors.GetLength(0)));
                var y = number % Sensors.GetLength(1);
                Sensors[x, y].PressureDetected = value;
            }
        }

        public void SetTileColor(Color color)
        {
            lock (lockObject)
            {
                for (var i = 0; i < Lights.GetLength(0); i++)
                {
                    for (var j = 0; j < Lights.GetLength(1); j++)
                    {
                        Lights[i, j].SetColor(color);
                    }
                }
            }
        }

        public List<Light> GetLightsSurroundingSensor(Vector2 sensorPosition)
        {
            var positions = new List<Vector2>
            {
                new Vector2(sensorPosition.X * 2, sensorPosition.Y * 2),
                new Vector2((sensorPosition.X * 2) + 1, sensorPosition.Y * 2),
                new Vector2(sensorPosition.X * 2, (sensorPosition.Y * 2) + 1),
                new Vector2((sensorPosition.X * 2) + 1, (sensorPosition.Y * 2) + 1)
            };

            var lights = new List<Light>();

            for (var index = 0; index < 4; index++)
            {
                try
                {
                    lights.Add(Lights[positions[index].X, positions[index].Y]);
                }
                catch (IndexOutOfRangeException exception)
                {
                    ConsoleLogger.WriteToConsole(this,
                        $"IndexOutOfRangeException trying to find light positions surrounding sensor position",
                        exception);
                    lights.Add(new Light(Vector2.Zero(), Vector3.Zero()));
                }
            }

            return lights;
        }

        public Sensor GetSensorsNextToLight(Vector2 lightPosition)
        {
            var sensor1Pos = new Vector2(lightPosition.X / 2, lightPosition.Y / 2);

            try
            {
                return Sensors[sensor1Pos.X, sensor1Pos.Y];
            }
            catch (IndexOutOfRangeException exception)
            {
                ConsoleLogger.WriteToConsole(this,
                    $"IndexOutOfRangeException trying to find sensor position next to light position",
                    exception);
                return null;
            }
        }

        public override string ToString()
        {
            return $"pos:{Position},id:{TileId}";
        }
    }
}
