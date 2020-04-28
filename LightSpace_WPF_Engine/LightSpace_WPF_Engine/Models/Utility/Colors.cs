using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using LightSpace_WPF_Engine.Models.Models.Logging;

namespace LightSpace_WPF_Engine.Models.Utility
{
    /// <summary>
    /// Color Library used to access all available colors, and fetch equivalent or convert between colors.
    /// <para>All Color methods return data of type System.Drawing.<see cref="System.Drawing.Color"/>.</para>
    /// </summary>
    public static class Colors
    {
        // 255 / 3 = 85
        // 85*2 = 170
        // Combination of every 0,85,170 & 255 value = (r*g*b) 4*4*4 = 64 colors
        public static Vector3 ColorToVector3(Color color)
        {
            return new Vector3(ConvertValues(color.R),ConvertValues(color.G),ConvertValues(color.B));
        }

        public static Color VectorToColor(Vector3 vector, bool convert = true)
        {
            if (convert)
            {
                return Color.FromArgb(ConvertValues(vector.X), ConvertValues(vector.Y), ConvertValues(vector.Z));
            }
            else
            {
                return Color.FromArgb(vector.X, vector.Y, vector.Z);
            }
        }

        public static byte ColorX222ToByte(int r,int g, int b)
        {
            var byteString = "00";
            byteString += ConvertValues(GetClosestColorValue(r)).ToString().PadLeft(2,'0');
            byteString += ConvertValues(GetClosestColorValue(g)).ToString().PadLeft(2, '0');
            byteString += ConvertValues(GetClosestColorValue(b)).ToString().PadLeft(2, '0');
            return Convert.ToByte(byteString, 2);
        }

        private static int ConvertValues(int input)
        {
            switch (input)
            {
                case 0:
                    return 0;
                case 1:
                    return 85;
                case 2:
                    return 170;
                case 3:
                    return 255;
                case 85:
                    return 1;
                case 170:
                    return 2;
                case 255:
                    return 3;
                default: 
                    return 0;
            }
        }

        private static int GetClosestColorValue(int input)
        {
            var array = new int[4] {0, 85, 170, 255};
            try
            {
                return Convert.ToInt32(array.Min(x => Math.Abs((long) x - input)));
            }
            catch (Exception exception)
            {
                ConsoleLogger.WriteToConsole(input,$"Error converting input to closest color value.",exception);
            }
            return 0;
        }

        public static BitmapPalette GetWindowsMediaPalette()
        {
            var colors = GetAllColors();
            var windowsMediaColors = new List<System.Windows.Media.Color>();
            colors.ForEach(color => windowsMediaColors.Add(ColorToSwmColor(color)));
            return new BitmapPalette(windowsMediaColors);
        }

        public static ColorPalette GetPalette()
        {
            var colors = GetAllColors();
            var bmp = new Bitmap(1,1,PixelFormat.Format8bppIndexed);
            var colorPalette = bmp.Palette;
            for (var i = 0; i < colorPalette.Entries.Length-1; i++)
            {
                if (i <= colors.Count-1)
                {
                    colorPalette.Entries[i] = colors[i];
                }
                else
                {
                    colorPalette.Entries[i] = new Color();
                }
            }
            return colorPalette;
        }

        public static List<Color> GetAllColors()
        {
            return new List<Color>()
            {
                Black(),
                DarkGray(),
                LightGray(),
                White(),
                Red(),
                OrangeRed(),
                Salmon(),
                Orange(),
                LightSalmon(),
                LightPink(),
                Yellow(),
                LightYellow(),
                PaleYellow(),
                Fuchsia(),
                HotPink(),
                Plum(),
                Cyan(),
                LightCyan(),
                Azure(),
                Blue(),
                Indigo(),
                DarkMagenta(),
                MediumPurple(),
                MediumOrchid(),
                Lavender(),
                Green(),
                Lime(),
                LawnGreen(),
                BrightLawnGreen(),
                MediumSpringGreen(),
                LightGreen(),
                PaleSpringGreen(),
                GreenYellow(),
                PaleGreen(),
                DarkGreen(),
                MidnightRed(),
                MidnightBlue(),
                ForestGreen(),
                DarkRed(),
                DarkBlue(),
                Olive(),
                SeaGreen(),
                DarkPurple(),
                MidnightOrange(),
                DarkDeepPink(),
                LightOlive(),
                DullGreen(),
                LightForestGreen(),
                Gold(),
                LightSeaGreen(),
                DarkViolet(),
                Orchid(),
                MediumAquamarine(),
                DarkKhaki(),
                RoyalBlue(),
                DeepPurple(),
                DeepPink(),
                BubbleGum(),
                CrimsonPink(),
                BrightRoyalBlue(),
                BrightBlue(),
                SteelBlue(),
                DarkSalmon(),
                SlatePurple()
            };
        }

        public static System.Windows.Media.Color ColorToSwmColor(Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color SwmColorToColor(System.Windows.Media.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        #region Colors
        public static Color Red() // 255 0 0
        {
            return Color.FromArgb(255, 0, 0);
        }

        public static Color OrangeRed() // 255 85 0
        {
            return Color.FromArgb(255, 85, 0); 
        }

        public static Color Orange() // 255 170 0
        {
            return Color.FromArgb(255, 170, 0);
        }

        public static Color Yellow() // 255 255 0
        {
            return Color.FromArgb(255, 255, 0);
        }

        public static Color LawnGreen() // 170 255 0
        {
            return Color.FromArgb(170, 255, 0);
        }

        public static Color Lime() // 85 255 0
        {
            return Color.FromArgb(85, 255, 0);
        }

        public static Color CrimsonPink() // 255 0 85
        {
            return Color.FromArgb(255, 0, 85);
        }

        public static Color DeepPink() // 255 0 170
        {
            return Color.FromArgb(255, 0, 170);
        }

        public static Color Fuchsia() // 255 0 255
        {
            return Color.FromArgb(255, 0, 255);
        }

        public static Color Salmon() // 255 85 85
        {
            return Color.FromArgb(255, 85, 85);
        }

        public static Color LightSalmon() // 255 170 85
        {
            return Color.FromArgb(255, 170, 85);
        }

        public static Color LightYellow() // 255 255 85
        {
            return Color.FromArgb(255, 255, 85);
        }

        public static Color PaleYellow() // 255 255 170
        {
            return Color.FromArgb(255, 255, 170);
        }

        public static Color White() // 255 255 255
        {
            return Color.FromArgb(255, 255, 255);
        }

        public static Color BubbleGum() // 255 85 170 
        {
            return Color.FromArgb(255, 85, 170);
        }

        public static Color LightPink() // 255 170 170
        {
            return Color.FromArgb(255, 170, 170);
        }

        public static Color Plum() // 255 170 255
        {
            return Color.FromArgb(255, 170, 255);
        }

        public static Color Green() // 0 255 0
        {
            return Color.FromArgb(0, 255, 0);
        }

        public static Color LightGreen() // 85 255 85
        {
            return Color.FromArgb(85, 255, 85);
        }

        public static Color GreenYellow() // 170 255 85
        {
            return Color.FromArgb(170, 255, 85);
        }

        public static Color HotPink() // 255 85 255
        {
            return Color.FromArgb(255, 85, 255);
        }

        public static Color Cyan() // 0 255 255
        {
            return Color.FromArgb(0, 255, 255);
        }

        public static Color LightCyan() // 85 255 255
        {
            return Color.FromArgb(85, 255, 255);
        }

        public static Color Azure() // 170 255 255
        {
            return Color.FromArgb(170, 255, 255);
        }

        public static Color Indigo() // 85 0 255
        {
            return Color.FromArgb(85, 0, 255);
        }

        public static Color DarkMagenta() // 170 0 255
        {
            return Color.FromArgb(170, 0, 255);
        }

        public static Color MediumPurple() // 85 85 255
        {
            return Color.FromArgb(85, 85, 255);
        }

        public static Color MediumOrchid() // 170 85 255
        {
            return Color.FromArgb(170, 85, 255);
        }

        public static Color Lavender() // 170 170 255
        {
            return Color.FromArgb(170, 170, 255);
        }

        public static Color BrightLawnGreen() // 0 255 85
        {
            return Color.FromArgb(0, 255, 85);
        }

        public static Color MediumSpringGreen() // 0 255 170
        {
            return Color.FromArgb(0, 255, 170);
        }

        public static Color PaleSpringGreen() // 85 255 170
        {
            return Color.FromArgb(85, 255, 170);
        }

        public static Color PaleGreen() // 170 255 170
        {
            return Color.FromArgb(170, 255, 170);
        }

        public static Color DarkGreen() // 0 85 0
        {
            return Color.FromArgb(0, 85, 0);
        }

        public static Color MidnightRed() // 85 0 0
        {
            return Color.FromArgb(85, 0, 0);
        }

        public static Color MidnightBlue() // 0 0 85
        {
            return Color.FromArgb(0, 0, 85);
        }

        public static Color ForestGreen() // 0 170 0
        {
            return Color.FromArgb(0, 170, 0);
        }

        public static Color DarkRed() // 170 0 0
        {
            return Color.FromArgb(170, 0, 0);
        }

        public static Color DarkBlue() // 0 0 170
        {
            return Color.FromArgb(0, 0, 170);
        }

        public static Color Olive() // 85 85 0
        {
            return Color.FromArgb(85, 85, 0);
        }

        public static Color SeaGreen() // 0 85 85
        {
            return Color.FromArgb(0, 85, 85);
        }

        public static Color DarkPurple() // 85 0 85
        {
            return Color.FromArgb(85, 0, 85);
        }

        public static Color MidnightOrange() // 170 85 0
        {
            return Color.FromArgb(170, 85, 0);
        }

        public static Color DarkDeepPink() // 170 0 85
        {
            return Color.FromArgb(170, 0, 85);
        }

        public static Color LightOlive() // 85 170 0
        {
            return Color.FromArgb(85, 170, 0);
        }

        public static Color DullGreen() // 85 170 85
        {
            return Color.FromArgb(85, 170, 85);
        }

        public static Color LightForestGreen() // 0 170 85
        {
            return Color.FromArgb(0, 170, 85);
        }

        public static Color Gold() // 170 170 0
        {
            return Color.FromArgb(170, 170, 0);
        }

        public static Color LightSeaGreen() // 0 170 170
        {
            return Color.FromArgb(0, 170, 170);
        }

        public static Color DarkViolet() // 170 0 170
        {
            return Color.FromArgb(170, 0, 170);
        }

        public static Color Orchid() // 170 85 170
        {
            return Color.FromArgb(170, 85, 170);
        }

        public static Color MediumAquamarine() // 85 170 170
        {
            return Color.FromArgb(85, 170, 170);
        }

        public static Color DarkKhaki() // 0 85 0
        {
            return Color.FromArgb(170, 170, 85);
        }

        public static Color RoyalBlue() // 0 85 170
        {
            return Color.FromArgb(0, 85, 170);
        }

        public static Color DeepPurple() // 0 85 170
        {
            return Color.FromArgb(85, 0, 170);
        }

        public static Color BrightRoyalBlue() // 0 85 255
        {
            return Color.FromArgb(0, 85, 255);
        }

        public static Color BrightBlue() // 0 170 255
        {
            return Color.FromArgb(0, 170, 255);
        }

        public static Color SteelBlue() // 85 170 255
        {
            return Color.FromArgb(85, 170, 255);
        }

        public static Color DarkSalmon() // 0 85 0
        {
            return Color.FromArgb(170, 85, 85);
        }

        public static Color SlatePurple() // 85 85 170
        {
            return Color.FromArgb(85, 85, 170);
        }

        public static Color Blue() // 0 0 255
        {
            return Color.FromArgb(0, 0, 255);
        }

        public static Color DarkGray() // 85 85 85
        {
            return Color.FromArgb(85, 85, 85);
        }

        public static Color LightGray() // 170 170 170
        {
            return Color.FromArgb(170, 170, 170);
        }

        public static Color Black() // 0 0 0
        {
            return Color.FromArgb(0, 0, 0);
        }
        #endregion
    }
}
