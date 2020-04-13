using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public class SoundManager
    {
        public double SoundVolume;

        //TODO: 40 Refine the entire sound playing system. 
        // > multiple sound volume settings in a library (string id+value)
        // > keep track of all threads/mediaplayers so their settings can be changed (volume, pausing, deletion)
        public SoundManager()
        {
            SoundVolume = 0.5d;
        }

        /// <summary>
        /// Plays a sound on a separate thread so it won't be interrupted.
        /// </summary>
        /// <param name="partialUrl"></param>
        public void PlaySound(string partialUrl = "Media/Testing/ShortBeep.wav")
        {
            new System.Threading.Thread(() => {
                var mp = new MediaPlayer();
                mp.Open(GetSoundUri(partialUrl));
                mp.Volume = SoundVolume;
                mp.Play();
            }).Start();
        }

        private static Uri GetSoundUri(string partialPath)
        {
            // Get full path from the given partial path
            var path = Path.GetFullPath(partialPath);
            // Remove 2 runtime folder names to get to actual file location and convert to Uri
            return new Uri(path.Replace("\\bin\\Debug", ""));
        }
    }
}
