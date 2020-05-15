using System;
using System.Drawing;
using System.Text;
using System.Windows.Documents;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Models.Models
{
    /// <summary>
    /// <para>This class is an object in which the Input data for the LightSpace floor can be stored.
    /// The data consists of 66 bytes.  </para>
    /// <para>    Byte 1 & 2: 16 bits tileNumber. </para>
    /// <para>    Byte 3-66 : 8 bits (1 byte) for each led on a tile. </para>
    /// </summary>
    [Serializable]
    public class InputData
    {
        public short TileNumber { get; private set; }
        public Color[,] ColorArray { get; private set; }

        public InputData(Tile tile)
        {
            TileNumber = tile.TileId;
            ColorArray = new Color[tile.Lights.GetLength(0), tile.Lights.GetLength(1)];

            for (var x = 0; x < tile.Lights.GetLength(0); x++)
            {
                for (var y = 0; y < tile.Lights.GetLength(1); y++)
                {
                    ColorArray[x, y] = Colors.VectorToColor(tile.Lights[x, y].Color);
                }
            }
        }
    }
}