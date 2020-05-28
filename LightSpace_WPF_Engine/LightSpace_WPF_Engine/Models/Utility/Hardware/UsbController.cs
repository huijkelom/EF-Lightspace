using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Wpf.Views.MainWindows;
using Microsoft.Win32;

namespace LightSpace_WPF_Engine.Models.Utility.Hardware
{
    public sealed class UsbFloorController : BaseHardwareController
    {
        // Hardware handshake information. (Vendor ID & Product ID)
        private static readonly string vid = "0x04D8";
        private static readonly string pid = "0x000A";
        private SerialPort _serialPort;
        private bool handshakeSuccessful = false;

        // Base Constructor
        public UsbFloorController(TileManager tileManager) : base(tileManager)
        {

        }

        public override void Start()
        {
            if (handshakeSuccessful)
            {
                Stop();
                handshakeSuccessful = false;
            }
            NoHardwareMode = false;
            var port = Handshake(vid.Replace("0x", ""), pid.Replace("0x", ""));
            if (string.IsNullOrEmpty(port))
            {
                return;
            }
            _serialPort = new SerialPort
            {
                PortName = port,
                BaudRate = 9600,
                Parity = Parity.Odd,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            _serialPort.Open();
            handshakeSuccessful = true;
        }

        /// <summary>
        /// Get a string based COM port associated with given VID and PID
        /// </summary>
        /// <param name="VID">string representing the vendor id of the USB/Serial convertor</param>
        /// <param name="PID">string representing the product id of the USB/Serial convertor</param>
        /// <returns></returns>
        private static string Handshake(string VID, string PID)
        {
            // Get a list of all available ports.
            var portNames = SerialPort.GetPortNames();
            // Create the Device ID string to compare Vendor ID and Product ID with the hardware on the port.
            var deviceIdString = $"USB\\VID_{VID}&PID_{PID}";
            // Search through entities with a caption containing a COM ID.
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                // Cast them to managementObjects so data can be read from them, and iterate through them.
                var a = searcher.Get().Cast<ManagementBaseObject>().ToList();
                foreach (var comObject in a)
                {
                    // Get DeviceID from properties.
                    var deviceInfo = comObject.Properties["DeviceId"];

                    #region Finding other properties.
                    // Comment block below is for iterating through the properties and listing them, so other properties can be found.
                    // Properties generally somewhat match properties found in DeviceManager>Device>Properties>Details.

                    /*
                    var properties = new List<string>();
                    foreach (var propertyData in comObject.Properties)
                    {
                        properties.Add($"name:{propertyData.Name} , val:{propertyData.Value}");
                    }
                    */
                    #endregion
                    
                    // Check if the current device matches the Device ID string created earlier.
                    if (deviceInfo.Value.ToString().Contains(deviceIdString))
                    {
                        // Safety Check if the current device is present. (some devices get remembered and are able to show up while not being available)
                        var present = comObject["Present"].ToString().ToLower();
                        if (present != "true")
                        {
                            continue;
                        }
                        // Extract the assigned COM port from the device caption and return it.
                        var caption = comObject["Caption"].ToString();
                        var subStringIndex = caption.IndexOf("COM", StringComparison.OrdinalIgnoreCase);
                        var port = $"COM{caption[subStringIndex + 3]}";
                        if (portNames.Contains(port))
                        {
                            return port;
                        }
                    }
                }
                return "";
            }
        }

        public override void Stop()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                NoHardwareMode = true;
                _serialPort.Close();
            }
        }

        void PortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string stringdata = _serialPort.ReadExisting();

            try
            {
                var buffer = new byte[66];
                int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);

                // construct output data from byte array and push data to TileManager
                //DataTool.ByteArrayToOutputData(buffer).ForEach(data => data.PushToTileManager());
            }
            catch (Exception exception)
            {
                Stop();
                ConsoleLogger.WriteToConsole(this, "Error reading from USB hardware.", exception);
            }
        }

        protected async override void ReadData()
        {
            if (NoHardwareMode)
            {
                return;
            }
            try
            {
                var buffer = new byte[66];
                int bytesRead = await _serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);
                
                // construct output data from byte array and push data to TileManager
                var tiles = Game.Get.TileManager.Tiles.ToList();

                var outputData = buffer.ToOutputDictionary();
                
                foreach (var data in outputData)
                {
                    // TODO: 00 this for loop breaks writing and performance
                    for (short index = 0; index < data.Value.Length; index++)
                    {
                        tiles[data.Key].SetSensorDetectionByNumber(index, data.Value[index]);
                    }
                }
            }
            catch (Exception exception)
            {
                Stop();
                ConsoleLogger.WriteToConsole(this, "Error reading from USB hardware.", exception);
            }
        }

        protected async override void WriteData()
        {
            if (NoHardwareMode || !TileManager.GetRenderChanged())
            {
                return;
            }

            try
            {
                var tiles = TileManager.Tiles;
                tiles.GetAlternatedTileList();

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

                    await _serialPort.BaseStream.WriteAsync(convertedDataArray, 0, 66);
                }
            }
            catch (Exception exception)
            {
                Stop();
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
