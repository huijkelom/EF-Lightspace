using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Models.Logging;

namespace LightSpace_WPF_Engine.Models.Utility.Hardware
{
    //TODO: 00 Implement hardware reading and writing
    public sealed class UsbFloorController : BaseHardwareController
    {
        // Hardware handshake information. (Vendor ID & Product ID)
        private static ushort vid = 0x04D8;
        private static ushort pid = 0x000A;

        // Base Constructor
        public UsbFloorController(TileManager tileManager) : base(tileManager)
        {
        }

        protected override async void ReadData()
        {
            if (NoHardwareMode)
            {
                return;
            }
            //TODO: 00 Read USB Data
            try
            {
                var filter = SerialDevice.GetDeviceSelectorFromUsbVidPid(vid, pid);
                var devices = await DeviceInformation.FindAllAsync(filter, null);
                if (devices.Count > 0)
                {
                    using (var device = await SerialDevice.FromIdAsync(devices[0].Id))
                    {
                        if (device == null) return;
                        using (var comReader = new DataReader(device.InputStream))
                        {
                            var receivedBytes = await comReader.LoadAsync(66);

                            if (receivedBytes > 0)
                            {
                                var usbReceived = new byte[receivedBytes];
                                comReader.ReadBytes(usbReceived);
                                /*CapPannelTextBox.Text = "0x" + (usbReceived[0] + (usbReceived[1] << 8)).ToString("X");
                                CapByteATextBox.Text = "0x" + usbReceived[2].ToString("X");
                                CapByteBTextBox.Text = "0x" + usbReceived[3].ToString("X");*/
                                // construct output data from byte array and push data to TileManager
                                DataTool.ByteArrayToOutputData(usbReceived).ForEach(data => data.PushToTileManager());
                            }
                        }

                        await Task.Delay(0); //500);
                    }
                }
            }
            catch (Exception exception)
            {
                ConsoleLogger.WriteToConsole(this, "Error reading from USB hardware.", exception);
            }
        }

        protected override async void WriteData()
        {
            if (NoHardwareMode)
            {
                return;
            }
            //Note: This code will throw an error and break the app currently. Once hardware is available to work with revise this to be done with delegates.
            //var imgSource = ImageExtensions.BitmapToImageSource(Game.Get.TileManager.GetRenderGraphic());
            //var tiles = imgSource.MapImageToTiles(Game.Get.TileManager.Tiles);
            //var alternatedTiles = DataTool.GetAlternatedTileList(tiles);

            //TODO: 00 Write USB Data
            try
            {
                var filter = SerialDevice.GetDeviceSelectorFromUsbVidPid(vid, pid);
                var devices = await DeviceInformation.FindAllAsync(filter, null);
                var usbTransmitData = new byte[66];

                using (var device = await SerialDevice.FromIdAsync(devices[0].Id))
                {
                    if (device == null) return;
                    using (var comWriter = new DataWriter(device.OutputStream))
                    {
                        var tiles = TileManager.Tiles.ToList();
                        if (tiles.Count == 0)
                        {
                            return;
                        }

                        // Load tiles into inputData and write via ComWriter
                        foreach (var inputData in tiles.Select(tile => new InputData(tile)))
                        {
                            // Create a converted input data array and copy it over to the usbTransmitData, then write it.
                            var convertedDataArray =
                                DataTool.InputDataToByteArray(inputData.TileNumber, inputData.ColorArray);
                            Array.Copy(convertedDataArray,usbTransmitData,convertedDataArray.Length);

                            comWriter.WriteBytes(usbTransmitData);
                            await comWriter.StoreAsync();
                        }

                        #region tileNumber

                        /*USBTransmitData[0] = (byte)(0xFF & int.Parse(LedPannelTextBox.Text));
                        USBTransmitData[1] = (byte)(0xFF & (int.Parse(LedPannelTextBox.Text)) >> 8);*/

                        #endregion

                        #region Colors

                        /*USBTransmitData[2] = byte.Parse(LedA1TextBox.Text);
                        USBTransmitData[3] = byte.Parse(LedB1TextBox.Text);
                        USBTransmitData[4] = byte.Parse(LedC1TextBox.Text);
                        USBTransmitData[5] = byte.Parse(LedD1TextBox.Text);
                        USBTransmitData[6] = byte.Parse(LedE1TextBox.Text);
                        USBTransmitData[7] = byte.Parse(LedF1TextBox.Text);
                        USBTransmitData[8] = byte.Parse(LedG1TextBox.Text);
                        USBTransmitData[9] = byte.Parse(LedH1TextBox.Text);

                        USBTransmitData[10] = byte.Parse(LedA2TextBox.Text);
                        USBTransmitData[11] = byte.Parse(LedB2TextBox.Text);
                        USBTransmitData[12] = byte.Parse(LedC2TextBox.Text);
                        USBTransmitData[13] = byte.Parse(LedD2TextBox.Text);
                        USBTransmitData[14] = byte.Parse(LedE2TextBox.Text);
                        USBTransmitData[15] = byte.Parse(LedF2TextBox.Text);
                        USBTransmitData[16] = byte.Parse(LedG2TextBox.Text);
                        USBTransmitData[17] = byte.Parse(LedH2TextBox.Text);

                        USBTransmitData[18] = byte.Parse(LedA3TextBox.Text);
                        USBTransmitData[19] = byte.Parse(LedB3TextBox.Text);
                        USBTransmitData[20] = byte.Parse(LedC3TextBox.Text);
                        USBTransmitData[21] = byte.Parse(LedD3TextBox.Text);
                        USBTransmitData[22] = byte.Parse(LedE3TextBox.Text);
                        USBTransmitData[23] = byte.Parse(LedF3TextBox.Text);
                        USBTransmitData[24] = byte.Parse(LedG3TextBox.Text);
                        USBTransmitData[25] = byte.Parse(LedH3TextBox.Text);

                        USBTransmitData[26] = byte.Parse(LedA4TextBox.Text);
                        USBTransmitData[27] = byte.Parse(LedB4TextBox.Text);
                        USBTransmitData[28] = byte.Parse(LedC4TextBox.Text);
                        USBTransmitData[29] = byte.Parse(LedD4TextBox.Text);
                        USBTransmitData[30] = byte.Parse(LedE4TextBox.Text);
                        USBTransmitData[31] = byte.Parse(LedF4TextBox.Text);
                        USBTransmitData[32] = byte.Parse(LedG4TextBox.Text);
                        USBTransmitData[33] = byte.Parse(LedH4TextBox.Text);

                        USBTransmitData[34] = byte.Parse(LedA5TextBox.Text);
                        USBTransmitData[35] = byte.Parse(LedB5TextBox.Text);
                        USBTransmitData[36] = byte.Parse(LedC5TextBox.Text);
                        USBTransmitData[37] = byte.Parse(LedD5TextBox.Text);
                        USBTransmitData[38] = byte.Parse(LedE5TextBox.Text);
                        USBTransmitData[39] = byte.Parse(LedF5TextBox.Text);
                        USBTransmitData[40] = byte.Parse(LedG5TextBox.Text);
                        USBTransmitData[41] = byte.Parse(LedH5TextBox.Text);

                        USBTransmitData[42] = byte.Parse(LedA6TextBox.Text);
                        USBTransmitData[43] = byte.Parse(LedB6TextBox.Text);
                        USBTransmitData[44] = byte.Parse(LedC6TextBox.Text);
                        USBTransmitData[45] = byte.Parse(LedD6TextBox.Text);
                        USBTransmitData[46] = byte.Parse(LedE6TextBox.Text);
                        USBTransmitData[47] = byte.Parse(LedF6TextBox.Text);
                        USBTransmitData[48] = byte.Parse(LedG6TextBox.Text);
                        USBTransmitData[49] = byte.Parse(LedH6TextBox.Text);

                        USBTransmitData[50] = byte.Parse(LedA7TextBox.Text);
                        USBTransmitData[51] = byte.Parse(LedB7TextBox.Text);
                        USBTransmitData[52] = byte.Parse(LedC7TextBox.Text);
                        USBTransmitData[53] = byte.Parse(LedD7TextBox.Text);
                        USBTransmitData[54] = byte.Parse(LedE7TextBox.Text);
                        USBTransmitData[55] = byte.Parse(LedF7TextBox.Text);
                        USBTransmitData[56] = byte.Parse(LedG7TextBox.Text);
                        USBTransmitData[57] = byte.Parse(LedH7TextBox.Text);

                        USBTransmitData[58] = byte.Parse(LedA8TextBox.Text);
                        USBTransmitData[59] = byte.Parse(LedB8TextBox.Text);
                        USBTransmitData[60] = byte.Parse(LedC8TextBox.Text);
                        USBTransmitData[61] = byte.Parse(LedD8TextBox.Text);
                        USBTransmitData[62] = byte.Parse(LedE8TextBox.Text);
                        USBTransmitData[63] = byte.Parse(LedF8TextBox.Text);
                        USBTransmitData[64] = byte.Parse(LedG8TextBox.Text);
                        USBTransmitData[65] = byte.Parse(LedH8TextBox.Text);*/

                        #endregion
                    }
                }
            }
            catch (Exception exception)
            {
                ConsoleLogger.WriteToConsole(this, "Error writing to USB hardware.", exception);
            }
        }

        public List<Tile> GetTiles()
        {
            //TODO: 04 Get tiles and return them here
            return null;
        }
    }
}
