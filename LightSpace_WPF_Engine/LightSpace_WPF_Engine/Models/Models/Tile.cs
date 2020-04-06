using System;
using System.Collections.Generic;
using System.Drawing;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Models.Models
{
    [Serializable]
    //TODO: Check out all cases to see if x & y of tile setting in 2D array are done right (x = column, y = row)
    public class Tile
    {
        // Tile ID
        public int TileId { get; set; }

        // Position of the tile on a grid.
        public Vector2 Position { get; set; }

        // Should be 64 in total. (8*8)
        public Light[,] Lights { get; set; }

        // Should be 16 in total. (4*4)
        public Sensor[,] Sensors { get; set; }

        public Tile(int tileId)
        {
            TileId = tileId;
            Init();
        }

        public void Init()
        {
            Position = new Vector2(0, 0);
            Lights = new Light[8, 8];
            Sensors  = new Sensor[4,4];
        }

        public void SetTileColor(Color color)
        {
            for (var i = 0; i < Lights.GetLength(0); i++)
            {
                for (var j = 0; j < Lights.GetLength(1); j++)
                {
                    Lights[i,j].SetColor(color);
                }
            }
        }

        public List<Light> GetLightsSurroundingSensor(Vector2 sensorPosition)
        {
            var light1Pos = new Vector2(sensorPosition.X*2,sensorPosition.Y*2);
            var light2Pos = new Vector2((sensorPosition.X * 2)+1, sensorPosition.Y * 2);
            var light3Pos = new Vector2(sensorPosition.X * 2, (sensorPosition.Y * 2) + 1);
            var light4Pos = new Vector2((sensorPosition.X * 2) + 1, (sensorPosition.Y * 2) + 1);

            try
            {
                return new List<Light>()
                {
                    Lights[light1Pos.X,light1Pos.Y],
                    Lights[light2Pos.X,light2Pos.Y],
                    Lights[light3Pos.X,light3Pos.Y],
                    Lights[light4Pos.X,light4Pos.Y],

                };
            }
            catch (IndexOutOfRangeException exception)
            {
                ConsoleLogger.WriteToConsole(this,
                    $"IndexOutOfRangeException trying to find light positions surrounding sensor position",
                    exception);
                return new List<Light>();
            }
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

        public static int GetTrueListId(Tile[,] tiles, Vector2 pos)
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

        public static Tile[,] GetDebugTiles(
            int tileSizeX, int tileSizeY, 
            int lightSizeX, int lightSizeY, 
            int pressurePointSizeX, int pressurePointSizeY,
            bool colored)
        {
            var tiles = new Tile[tileSizeX, tileSizeY];
            for (var tileX = 0; tileX < tiles.GetLength(0); tileX++)
            {
                for (var tileY = 0; tileY < tiles.GetLength(1); tileY++)
                {
                    var tileId = (tileX * tileSizeX) + tileY;
                    tiles[tileX, tileY] = new Tile(tileX + tileY)
                    {
                        TileId = tileId,
                        Position = new Vector2(tileX,tileY),
                        Lights = GetDebugLights(tileX, tileSizeX, tileY, tileSizeY, lightSizeX, lightSizeY, colored),
                        Sensors = GetDebugPressurePoints(pressurePointSizeX, pressurePointSizeY,tileId)
                    };
                }
            }
            return tiles;
        }

        public static Light[,] GetDebugLights(int tileX, int tileSizeX, int tileY, int tileSizeY, int lightSizeX, int lightSizeY, bool colored)
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
                            new Vector3(0,0,0)
                            );
                    }

                }
            }
            return lights;
        }

        public static Sensor[,] GetDebugPressurePoints(int pointSizeX, int pointSizeY, int tileId)
        {
            var points = new Sensor[pointSizeX, pointSizeY];
            for (var pointX = 0; pointX < points.GetLength(0); pointX++)
            {
                for (var pointY = 0; pointY < points.GetLength(1); pointY++)
                {
                    points[pointX, pointY] = new Sensor(tileId,false,new Vector2(pointX,pointY));
                }
            }
            return points;
        }

        public override string ToString()
        {
            return $"pos:{Position},id:{TileId}";
        }
    }
}
