using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Games.BasePressureTest
{
    class BasePressureGame : RunningGameBehavior
    {
        public int TicksPerSecond = 2;

        private int gameFieldWidth;
        private int gameFieldHeight;
        private Bitmap backgroundImage;
        private int lightAmount;
        private List<Vector2> activeLights;

        public override void Start()
        {
            activeLights = new List<Vector2>();
            Game.Get.CoreLoop.TicksPerSecond = TicksPerSecond;
            GameFieldTileSize = Game.Get.TileManager.FieldSize;

            if (GameFieldTileSize.X == 0 || GameFieldTileSize.Y == 0)
            {
                GameFieldTileSize = new Vector2(2, 2);
            }
            lightAmount = Game.Get.TileManager.GetLightAmount();
            GameName = GameName.TestGame1;

            gameFieldWidth = GameFieldTileSize.X * lightAmount;
            gameFieldHeight = GameFieldTileSize.Y * lightAmount;

            backgroundImage = new Bitmap(gameFieldWidth, gameFieldHeight);
            backgroundImage.DrawRectangle(Vector2.Zero(), gameFieldWidth, gameFieldHeight, true, 2, Colors.Black());
            Draw(new List<Vector2>());
        }

        public override void Update()
        {
            var lightsToBeActive = new List<Vector2>();
            var tiles = Game.Get.TileManager.Tiles;
            for (var tileX = 0; tileX < tiles.GetLength(0); tileX++)
            {
                for (var tileY = 0; tileY < tiles.GetLength(1); tileY++)
                {
                    var sensors = tiles[tileX, tileY].Sensors;
                    for (var sensX = 0; sensX < sensors.GetLength(0); sensX++)
                    {
                        for (var sensY = 0; sensY < sensors.GetLength(1); sensY++)
                        {
                            if (sensors[sensX, sensY].PressureDetected)
                            {
                                var tempPositions = new List<Vector2>();
                                tiles[tileX, tileY].GetLightsSurroundingSensor(sensors[sensX, sensY].Position).ForEach(light => tempPositions.Add(
                                    new Vector2(light.Position.X+(tileX*8),light.Position.Y+(tileY * 8)))
                                );
                                // add all lights that arent already in the list
                                lightsToBeActive.AddRange(tempPositions.Except(lightsToBeActive));
                            }
                        }
                    }
                }
            }
            // check if its any different from last lights, if not dont redraw
            var activeExceptCurrent = new List<Vector2>();

            activeExceptCurrent.AddRange(lightsToBeActive.Except(activeLights).ToList());

            if (activeExceptCurrent.Count == 0)
            {
                return;
            }
            activeLights = activeExceptCurrent;
            Draw(lightsToBeActive);
        }

        public void Draw(List<Vector2> lightsToDraw)
        {
            var tempBitmap = backgroundImage.Clone() as Bitmap;
            foreach (var lightPos in lightsToDraw)
            {
                tempBitmap.SetPixel(lightPos, Color.Red);
            }
            Game.Get.TileManager.SetRenderGraphic(tempBitmap);
        }
    }
}
