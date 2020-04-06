using System.Drawing;
using System.Windows.Media;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Utility;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace LightSpace_WPF_Engine.Wpf.Controls
{
    public class CustomImage : Image
    {
        public CustomImage() : base()
        {

        }

        public CustomImage(ImageSource source) : base()
        {
            Source = source;
        }

        public CustomImage(Bitmap bitmap) : base()
        {
            Source = ImageExtensions.BitmapToImageSource(bitmap);
        }
        public Tile TileData { get; set; }

        public Point InCanvasPosition { get; set; } = new Point(0,0);
    }
}
