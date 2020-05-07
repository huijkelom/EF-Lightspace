using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Models;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public static class TileExtensions
    {
        public static bool IsPressureDetectedWithinArea(this Tile[,] tiles, Vector2 startPoint, Vector2 endPoint,
            int lightAmount = 0, int sensorAmount = 0)
        {
            // Get all sensors within specified area
            var sensors = GetSensorsWithinArea(tiles, startPoint, endPoint, lightAmount, sensorAmount);
            // If any of the sensors have PressureDetected true, return true, otherwise return false.
            return sensors.Any(sensor => sensor.PressureDetected = true);
        }

        public static List<Tile> ToList(this Tile[,] tiles)
        {
            var list = new List<Tile>();
            for (var x = 0; x < tiles.GetLength(0); x++)
            {
                for (var y = 0; y < tiles.GetLength(1); y++)
                {
                    list.Add(tiles[x,y]);
                }
            }
            return list;
        }

        public static bool AnyActiveSensorsInTile(this Tile tile)
        {
            for (var x = 0; x < tile.Sensors.GetLength(0); x++)
            {
                for (var y = 0; y < tile.Sensors.GetLength(1); y++)
                {
                    if (tile.Sensors[x, y].PressureDetected)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the sensors within the specified area of <see cref="Light"/>s / pixels.
        /// </summary>
        /// <param name="tiles"> 2Dimensional array of tiles </param>
        /// <param name="startPoint"> start point in <see cref="Light"/>s / pixels </param>
        /// <param name="endPoint"> End point in <see cref="Light"/>s / pixels </param>
        /// <returns></returns>
        public static List<Sensor> GetSensorsWithinArea(this Tile[,] tiles, Vector2 startPoint, Vector2 endPoint, int lightAmount = 0, int sensorAmount = 0)
        {
            if (lightAmount == 0)
            {                                                                                           
                lightAmount = Game.Get.TileManager.GetLightAmount();
            }

            if (sensorAmount == 0)
            {
                sensorAmount = Game.Get.TileManager.GetSensorAmount();
            }

            var lightSensorIndexChanger = lightAmount / sensorAmount;

            var sensors = new List<Sensor>();

            var startTileX = startPoint.X / lightAmount;
            var startTileY = startPoint.Y / lightAmount;
            var endTileX = endPoint.X / lightAmount;
            var endTileY = endPoint.Y / lightAmount;

            var startSensorX = (startPoint.X % lightAmount)/2;
            var startSensorY = (startPoint.Y % lightAmount)/2;
            var endSensorX = (endPoint.X % lightAmount)/2;
            var endSensorY = (endPoint.Y % lightAmount)/2;

            // loop through X&Y of tiles
            for (var tileX = 0; tileX < tiles.GetLength(0); tileX++)
            {
                for (var tileY = 0; tileY < tiles.GetLength(1); tileY++)
                {
                    // if not within the tiles specifies, continue
                    if ((tileX < startTileX || tileX > endTileX) && (tileY < startTileY || tileY > endTileY))
                    {
                        continue;
                    }

                    var tile = tiles[tileX, tileY];
                    // if it falls completely within (and not on a partial tile), we dont need to check for some lights but all lights
                    var fullTile = (tileX > startTileX && tileX < endTileX && tileY > startTileY && tileY < endTileY);
                    // loop through X&Y of lights
                    for (var sensorX = 0; sensorX < tile.Sensors.GetLength(0); sensorX++)
                    {
                        for (var sensorY = 0; sensorY < tile.Sensors.GetLength(1); sensorY++)
                        {
                            // if not within the range of lights
                            if (((sensorX < startSensorX || sensorX > endSensorX) && (sensorY < startSensorY || sensorY > endSensorY)) && !fullTile)
                            {
                                continue;
                            }
                            sensors.Add(tile.Sensors[sensorX, sensorY]);
                        }
                    }
                }
            }
            return sensors;
        }
    }
}
