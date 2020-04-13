using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Models.Models
{
    public class ActiveGameData
    {
        public GameName EnumValue { get; set; }
        public string GameName { get; set; }
        public string GameDescription { get; set; }
        public Bitmap GameIcon { get; set; }
        public UserControl GameCustomControl { get; set; }
        public Vector2 PreferredGameTileSize { get; set; }

        public ActiveGameData()
        {
            const GameName game = Enums.GameName.None;
            GameName = game.ToString();
            GameDescription = GameList.GetGameDescription(game);
            GameIcon = GameList.GetGameIcon(game);
            GameCustomControl = GameList.GetGameControls(game);
            PreferredGameTileSize = GameList.GetPreferredGameTileSize(game);
        }

        public ActiveGameData(string gameName, string gameDescription, Bitmap gameIcon, UserControl gameCustomControl)
        {
            GameName = gameName;
            GameDescription = gameDescription;
            GameIcon = gameIcon;
            GameCustomControl = gameCustomControl;
        }
    }
}
