using System;
using System.Drawing;
using System.Text;
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
        public byte[] DataArray { get; set; }

        public byte[] CreateTestBytes()
        {
            // Explanation for this process written in SetData();
            // This method just used preset data to test with.
            byte[] bytes = new byte[4];

            bool unusedBit = false;
            int tileNumberBits = 24;
            Vector2 position = new Vector2(10,8);
            Color c = Color.FromArgb(200,150,100);

            #region First Byte
            byte tempByte = Convert.ToByte(unusedBit);
            string tempString1 = DataTool.SeparateBits(tempByte, 7, 1);
            tempByte = Convert.ToByte(tileNumberBits);
            string tempString2 = DataTool.SeparateBits(tempByte, 1, 7);
            string resultString = DataTool.CombineAndPadToBinary(tempString1, tempString2);
            bytes[0] = Convert.ToByte(resultString, 2);
            #endregion

            #region Second Byte
            tempByte = Convert.ToByte(position.X);
            tempString1 = DataTool.SeparateBits(tempByte, 4, 4);
            tempByte = Convert.ToByte(position.Y);
            tempString2 = DataTool.SeparateBits(tempByte, 4, 4);
            resultString = DataTool.CombineAndPadToBinary(tempString1, tempString2);
            bytes[1] = Convert.ToByte(resultString, 2);
            #endregion

            #region Third Byte
            var convertedColorVector = DataTool.Rgb888ToRgb555(c);
            tempByte = Convert.ToByte(convertedColorVector.X);
            tempString1 = DataTool.SeparateBits(tempByte, 3, 5);
            tempByte = Convert.ToByte(convertedColorVector.Y);
            tempString2 = DataTool.SeparateBits(tempByte, 3, 3);
            var tempString3 = DataTool.SeparateBits(tempByte, 6, 2);
            resultString = DataTool.CombineAndPadToBinary(tempString1, tempString2);
            bytes[2] = Convert.ToByte(resultString, 2);
            #endregion

            #region Fourth Byte
            tempByte = Convert.ToByte(convertedColorVector.Z);
            tempString1 = DataTool.SeparateBits(tempByte, 2, 6);
            resultString = DataTool.CombineAndPadToBinary(tempString3, tempString1);
            bytes[3] = Convert.ToByte(resultString, 2);
            #endregion

            return bytes;
        }

        /// <summary>
        /// Debug Check if byte array data contains the correct values.
        /// </summary>
        /// <param name="bytes">Byte array to test validity of.</param>
        public void TestDataValidity(byte[] bytes)
        {
            var tileNumber = 0;
            var positionVector = Vector2.Zero();
            var colorVector = Vector3.Zero();

            // Separate X bits [0-X-X-X-X-X-X-X] from 1st byte, pad binary on the left and convert value to int.
            var tileNumberBinary = DataTool.PadPartialByteToBinary(
                DataTool.SeparateBits(bytes[0], 1, 7)
                );
            tileNumber = Convert.ToInt32(tileNumberBinary, 2);

            // Separate X bits [X-X-X-X-0-0-0-0] from 2nd byte, pad binary on the left and convert value to int.
            var xPosBinary = DataTool.PadPartialByteToBinary(
                DataTool.SeparateBits(bytes[1], 0, 4)
                );
            // Separate X bits [0-0-0-0-X-X-X-X] from 2nd byte,
            // pad binary on the left and convert value to int. Combine into Vector2.
            var yPosBinary = DataTool.PadPartialByteToBinary(
                DataTool.SeparateBits(bytes[1], 4, 4)
                );
            positionVector = new Vector2(
                Convert.ToInt32(xPosBinary, 2), 
                Convert.ToInt32(yPosBinary, 2)
                );

            // Separate X bits [X-X-X-X-X-0-0-0] from third byte, pad binary on the left and convert value to int.
            colorVector.X = Convert.ToInt32(DataTool.PadPartialByteToBinary(
                DataTool.SeparateBits(bytes[2], 0, 5)),2);

            // Separate X bits [0-0-0-0-0-X-X-X] from third byte and [X-X-0-0-0-0-0-0] from fourth byte,
            // combine separate values, pad binary on the left and convert value to int.
            colorVector.Y = Convert.ToInt32(DataTool.PadPartialByteToBinary(
                $"{DataTool.SeparateBits(bytes[2], 5, 3)}" +
                $"{DataTool.SeparateBits(bytes[3], 0, 2)}"), 2);
            // Separate X bits [0-0-X-X-X-X-X-X] from fourth byte, pad binary on the left and convert value to int.
            colorVector.Z = Convert.ToInt32(DataTool.PadPartialByteToBinary(
                    DataTool.SeparateBits(bytes[3], 2, 6)), 2);
        }
    }
}