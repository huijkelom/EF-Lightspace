using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LightSpace_WPF_Engine.Games.FullColorTest;
using LightSpace_WPF_Engine.Games.TestGame;
using LightSpace_WPF_Engine.Wpf.views.UserControls;
using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Models.Exceptions;
using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Wpf.Views.UserControls.CustomControls;

namespace LightSpace_WPF_Engine.Models.Models
{
    static class GameList
    {
        public static UserControl GetGameControls(GameName gameName)
        {
            switch (gameName)
            {
                case GameName.None:
                    return new NoneGameControl();
                case GameName.Template:
                    return new CustomGameControlTemplate();
                case GameName.TestGame0:
                    return new TestGameControl();
                case GameName.TestGame1:
                    return new NoneGameControl();
                default:
                    throw new InvalidGameException();
            }
        }

        public static Vector2 GetPreferredGameTileSize(GameName gameName)
        {
            switch (gameName)
            {
                case GameName.None:
                    return new Vector2(1, 1);
                case GameName.Template:
                    return new Vector2(1, 1);
                case GameName.TestGame0:
                    return new Vector2(2, 2);
                case GameName.TestGame1:
                    return new Vector2(1, 1);
                default:
                    return new Vector2(5,5);
            }
        }

        //TODO: 50 Possibly load these in from a file. Not necessary though since it would still require a new build with the added games.
        public static string GetGameDescription(GameName gameName)
        {
            switch (gameName)
            {
                case GameName.None:
                    return "No game is currently loaded.";
                case GameName.Template:
                    return "Currently loaded is the template game, with its own template controls.";
                case GameName.TestGame0:
                    return "Currently loaded is Test Game (0), which is used as a development tool to test functionality.";
                case GameName.TestGame1:
                    return "Sets all tiles to a single color to test if connection works.";
                default:
                    throw new InvalidGameException();
            }
        }

        public static Bitmap GetGameIcon(GameName gameName)
        {
            switch (gameName)
            {
                case GameName.None:
                    return Properties.Resources.EmptyIcon;

                case GameName.Template:
                    return Properties.Resources.TemplateIcon;

                case GameName.TestGame0:
                    return Properties.Resources.SkippingRopeIcon;

                case GameName.TestGame1:
                    return Properties.Resources.Testicon;

                default:
                    return Properties.Resources.EmptyIcon;
            }
        }

        public static RunningGameBehavior GetGameBehavior(GameName gameName)
        {
            switch (gameName)
            {
                case GameName.None:
                    return new RunningGameBehavior();
                case GameName.Template:
                    return new RunningGameBehavior();
                case GameName.TestGame0:
                    return new SkippingRopeGame();
                case GameName.TestGame1:
                    return new FullColorGame();
                default:
                    return new RunningGameBehavior();
            }
        }
    }
}
