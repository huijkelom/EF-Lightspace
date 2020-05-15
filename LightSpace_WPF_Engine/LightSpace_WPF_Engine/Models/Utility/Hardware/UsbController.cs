using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Wpf.Views.MainWindows;

namespace LightSpace_WPF_Engine.Models.Utility.Hardware
{
    //TODO: 00 Implement hardware reading and writing
    public sealed class UsbFloorController : BaseHardwareController
    {
        // Hardware handshake information. (Vendor ID & Product ID)
        private static readonly ushort vid = 0x04D8;
        private static readonly ushort pid = 0x000A;

        // Base Constructor
        public UsbFloorController(TileManager tileManager) : base(tileManager)
        {
            NoHardwareMode = false;
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
                var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(filter, null);
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
            if (NoHardwareMode || !Game.Get.TileManager.GetRenderChanged())
            {
                return;
            }

            try
            {
                var filter = SerialDevice.GetDeviceSelectorFromUsbVidPid(vid, pid);
                var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(filter, null);
                var usbTransmitData = new byte[66];

                using (var device = await SerialDevice.FromIdAsync(devices[0].Id))
                {
                    if (device == null) 
                        return;
                    using (var comWriter = new DataWriter(device.OutputStream))
                    {
                        var tiles = TileManager.Tiles.ToList();

                        var inputDataList = new List<InputData>();
                        foreach (var tile in tiles)
                        {
                            var data = new InputData(tile);
                            inputDataList.Add(data);
                        }

                        // Load tiles into inputData and write via ComWriter
                        foreach (var inputData in inputDataList)
                        {
                            // Create a converted input data array and copy it over to the usbTransmitData, then write it.
                            var convertedDataArray =
                                DataTool.InputDataToByteArray(inputData.TileNumber, inputData.ColorArray);
                            Array.Copy(convertedDataArray,usbTransmitData,convertedDataArray.Length);

                            ConsoleLogger.WriteToConsole(inputData,"Writing input data to hardware");

                            comWriter.WriteBytes(usbTransmitData);
                            await comWriter.StoreAsync();
                        }
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
