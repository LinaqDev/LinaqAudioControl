using LinaqAudioMixer.Models;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinaqAudioMixer.Providers
{
    public class OutputDeviceProvider
    {
        public OutputDeviceProvider()
        {

        }

        private HashSet<string> idsCache;

        public async Task<IEnumerable<SoundDevice>> GetAllInputDevicesAsync()
        {
            idsCache = new HashSet<string>();
            var result = new List<SoundDevice>();
            await Task.Run(() =>
            {
                MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
                int waveOutDevices = WaveOut.DeviceCount;
                for (int waveOutDevice = 0; waveOutDevice < waveOutDevices; waveOutDevice++)
                {
                    WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveOutDevice);
                    foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                    {
                        if (idsCache.Contains(device.ID))
                            continue;

                        if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                        {
                            idsCache.Add(device.ID);
                            result.Add(new SoundDevice(device));
                        }
                    }
                }
            });

            return result;
        }
    }
}
