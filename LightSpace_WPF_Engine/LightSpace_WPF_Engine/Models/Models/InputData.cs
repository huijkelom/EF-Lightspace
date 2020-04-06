using System;
using System.Drawing;
using LightSpace_WPF_Engine.Models.Utility;

namespace LightSpace_WPF_Engine.Models.Models
{
    /// <summary>
    /// <para>This class is an object in which the Input data for the LightSpace floor can be stored.
    /// The data consists of 4 bytes.  </para>
    /// <para>    Byte 1: 1 bit unused        , 7 bits tile number </para>
    /// <para>    Byte 2: 4 bits X position   , 4 bits Y position </para>
    /// <para>    Byte 3: 5 bits red color    , 3 bits green color </para>
    /// <para>    Byte 4: 2 bits green color  , 6 bits blue color </para>
    /// </summary>
    [Serializable]
    public class InputData
    {
        public byte[] DataArray { get; set; }

        /// <summary>
        /// Set DataArray value to an array of bytes created from the given objects.
        /// </summary>
        /// <param name="tileNumberBits"> Tile number as received from hardware. </param>
        /// <param name="position"> Position of lights as received from hardware. </param>
        /// <param name="color"> Color(in RGB) the light should shift to. </param>
        /// <returns> Returns byte data containing the given parameters ready to send to the hardware</returns>
        public byte[] SetData(int tileNumberBits, Vector2 position, Color color)
        {
            // Initialise used variables.
            byte[] bytes = new byte[4];
            bool unusedBit = false;

            #region First Byte
            // Convert unusedBit (false/0) to a byte and then into string containing "0"
            byte tempByte = Convert.ToByte(unusedBit);
            string tempString1 = DataTool.SeparateBits(tempByte, 7, 1);
            // Convert tile number to a byte and then into a 7 bit length (max 127) string. 
            tempByte = Convert.ToByte(tileNumberBits);
            string tempString2 = DataTool.SeparateBits(tempByte, 1, 7);
            // Combine with the unused bit string to make the first byte.
            string resultString = DataTool.CombineAndPadToBinary(tempString1, tempString2);
            bytes[0] = Convert.ToByte(resultString, 2);
            #endregion

            #region Second Byte
            // Convert position.x to a byte and then separate the 4 bit long string.
            tempByte = Convert.ToByte(position.X);
            tempString1 = DataTool.SeparateBits(tempByte, 4, 4);
            // Convert position.y to a byte and then separate the 4 bit long string.
            tempByte = Convert.ToByte(position.Y);
            tempString2 = DataTool.SeparateBits(tempByte, 4, 4);
            // Combine both the position bit strings to make the second byte.
            resultString = DataTool.CombineAndPadToBinary(tempString1, tempString2);
            bytes[1] = Convert.ToByte(resultString, 2);
            #endregion

            #region Third Byte
            // Convert color to a color useable by the hardware (15/16 bit).
            // The colors will be spread over the last 2 bits like below (R = red, B = blue, G = green)
            // [R-R-R-R-R-G-G-G] [G-G-B-B-B-B-B-B]
            var convertedColorVector = DataTool.Rgb888ToRgb555(color);
            // Convert Color.R/Vector3.X to a byte and separate the 5 bit long string.
            tempByte = Convert.ToByte(convertedColorVector.X);
            tempString1 = DataTool.SeparateBits(tempByte, 3, 5);
            // Convert Color.G/Vector3.Y to a byte and separate the 3 bit long string from the third byte.
            tempByte = Convert.ToByte(convertedColorVector.Y);
            tempString2 = DataTool.SeparateBits(tempByte, 3, 3);
            // Get the last 2 bits but keep them separate for later.
            var tempString3 = DataTool.SeparateBits(tempByte, 6, 2);
            // Combine the Red / X values and the first 3 Green / Y values to make the third byte.
            resultString = DataTool.CombineAndPadToBinary(tempString1, tempString2);
            bytes[2] = Convert.ToByte(resultString, 2);
            #endregion

            #region Fourth Byte
            // Convert Color.B/Vector3.Z to a byte and then separate the 6 bit long string.
            tempByte = Convert.ToByte(convertedColorVector.Z);
            tempString1 = DataTool.SeparateBits(tempByte, 2, 6);
            // Combine the last 2 Green / Y values and the 6 Blue / Z values to make the fourth byte.
            resultString = DataTool.CombineAndPadToBinary(tempString3, tempString1);
            bytes[3] = Convert.ToByte(resultString, 2);
            #endregion

            // Set array to created bytes and return the bytes afterwards so they can be checked if deemed necessary.
            DataArray = bytes;
            return bytes;
        }

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