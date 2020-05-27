using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Games.FullColorTest
{
    public class FullColorGame : RunningGameBehavior
    {
        public int TicksPerSecond = 2;
        public Color TileColor = Colors.Blue();

        private int gameFieldWidth;
        private int gameFieldHeight;
        private Bitmap backgroundImage;
        private int lightAmount;

        public override void Start()
        {
            Game.Get.CoreLoop.TicksPerSecond = TicksPerSecond;
            var tiles = Game.Get.TileManager.Tiles;
            GameFieldTileSize = Game.Get.TileManager.FieldSize;

            if (GameFieldTileSize.X == 0 || GameFieldTileSize.Y == 0)
            {
                GameFieldTileSize = new Vector2(2,2);
            }
            lightAmount = Game.Get.TileManager.GetLightAmount();
            GameName = GameName.TestGame1;

            gameFieldWidth = GameFieldTileSize.X * lightAmount;
            gameFieldHeight = GameFieldTileSize.Y * lightAmount;

            backgroundImage = new Bitmap(gameFieldWidth, gameFieldHeight);
            backgroundImage.DrawRectangle(Vector2.Zero(), gameFieldWidth, gameFieldHeight, true, 2, TileColor);
            Draw();
        }

        public override void Update()
        {
            backgroundImage.DrawRectangle(Vector2.Zero(), gameFieldWidth, gameFieldHeight, true, 2, TileColor);
            Draw();
        }

        public override void LateUpdate()
        {
            Draw();

        }

        public void Draw()
        {
            Game.Get.TileManager.SetRenderGraphic(backgroundImage);
        }
    }
}
