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

        public static int GetTrueListId(this Tile[,] tiles, Vector2 pos)
        {
            var id = -1;
            for (var tileX = 0; tileX < tiles.GetLength(0); tileX++)
            {
                for (var tileY = 0; tileY < tiles.GetLength(1); tileY++)
                {
                    if (tiles[tileX, tileY].Position.CompareTo(pos))
                    {
                        id = (tileX * tiles.GetLength(1)) + tileY;
                        return id;
                    }
                }
            }
            return id;
        }

        public static Tile[,] GenerateTiles(
            this Tile[,] tiles,
            int tileSizeX, int tileSizeY,
            int lightSizeX, int lightSizeY,
            int sensorSizeX, int sensorSizeY,
            bool colored)
        {
            for (var tileX = 0; tileX < tiles.GetLength(0); tileX++)
            {
                for (var tileY = 0; tileY < tiles.GetLength(1); tileY++)
                {
                    var tileId = (short)((tileX * tileSizeX) + tileY);
                    tiles[tileX, tileY] = new Tile(tileId)
                    {
                        TileId = tileId,
                        Position = new Vector2(tileX, tileY),
                        Lights = GenerateLights(tileX, tileSizeX, tileY, tileSizeY, lightSizeX, lightSizeY, colored),
                        Sensors = GenerateSensors(sensorSizeX, sensorSizeY, tileId)
                    };
                }
            }
            return tiles;
        }

        public static Light[,] GenerateLights(int tileX, int tileSizeX, int tileY, int tileSizeY, int lightSizeX, int lightSizeY, bool colored)
        {
            var xColorMultiplier = 255 / (tileSizeX * lightSizeX);
            var yColorMultiplier = 255 / (tileSizeY * lightSizeY);

            var lights = new Light[lightSizeX, lightSizeY];
            for (var lightX = 0; lightX < lights.GetLength(0); lightX++)
            {
                for (var lightY = 0; lightY < lights.GetLength(1); lightY++)
                {
                    if (colored)
                    {
                        lights[lightX, lightY] = new Light(
                            new Vector2(lightX, lightY),
                            new Vector3(
                                (tileX * lightSizeX + lightX) * xColorMultiplier,
                                (tileY * lightSizeY + lightY) * yColorMultiplier,
                                0)
                        );
                    }
                    else
                    {
                        lights[lightX, lightY] = new Light(
                            new Vector2(lightX, lightY),
                            new Vector3(0, 0, 0)
                            );
                    }

                }
            }
            return lights;
        }

        public static Sensor[,] GenerateSensors(int pointSizeX, int pointSizeY, int tileId)
        {
            var points = new Sensor[pointSizeX, pointSizeY];
            for (var pointX = 0; pointX < points.GetLength(0); pointX++)
            {
                for (var pointY = 0; pointY < points.GetLength(1); pointY++)
                {
                    points[pointX, pointY] = new Sensor(tileId, false, new Vector2(pointX, pointY));
                }
            }
            return points;
        }
    }
}
