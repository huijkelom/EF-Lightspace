using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Models;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public static class Input
    {
        public static List<OutputData> OutputDataInputs { get; set; } = new List<OutputData>();

        public delegate void SetOutputDataInputEvent(byte[] bytes);

        /// <summary>
        /// Delegate used to create a set of OutputData from a byte array so it can be used as inputs for the games.
        /// </summary>
        public static SetOutputDataInputEvent SetOutputDataAsInputDelegate = SetOutputDataAsInput;

        public delegate void SetMockDataInputEvent(OutputData outputData);

        /// <summary>
        /// Delegate used to insert mock OutputData to simulate hardware functionality.
        /// </summary>
        public static SetMockDataInputEvent SetMockDataInputDelegate = SetMockInput;

        private static readonly object LockObject = new object();

        private static void SetOutputDataAsInput(byte[] bytes)
        {
            lock (LockObject)
            {
                var outputData = new OutputData();
                //outputData.SetDataFromByteArray(bytes);
                OutputDataInputs.Add(outputData);
            }
        }

        private static void SetMockInput(OutputData outputData)
        {
            lock (LockObject)
            {
                OutputDataInputs.Add(outputData);
                Game.Get.TileManager.RenderChanged = true;
            }
        }

        /// <summary>
        /// Get the hardware outputs to be used as game inputs.
        /// </summary>
        /// <returns> List of OutputData and clears the list afterwards. </returns>
        public static List<OutputData> GetOutputData(bool clear)
        {
            lock (LockObject)
            {
                if (OutputDataInputs.Count == 0)
                {
                    return new List<OutputData>();
                }
                var output = OutputDataInputs.ToList();
                output.ForEach(outputData => outputData.PushToTileManager());

                if (clear)
                {
                    OutputDataInputs.Clear();
                }
                return output;
            }
        }
    }
}
