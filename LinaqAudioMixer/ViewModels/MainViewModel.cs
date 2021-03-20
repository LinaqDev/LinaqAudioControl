using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LinaqAudioControl.ViewModels
{
    public class MainViewModel : BaseModel
    {
        #region test
        //[DllImport("winmm.dll", SetLastError = true)]
        //static extern uint waveInGetNumDevs();

        //[DllImport("winmm.dll", SetLastError = true, CharSet = CharSet.Auto)]
        //public static extern uint waveInGetDevCaps(uint hwo, ref WAVEOUTCAPS pwoc, uint cbwoc);

        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        //public struct WAVEOUTCAPS
        //{
        //    public ushort wMid;
        //    public ushort wPid;
        //    public uint vDriverVersion;
        //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        //    public string szPname;
        //    public uint dwFormats;
        //    public ushort wChannels;
        //    public ushort wReserved1;
        //    public uint dwSupport;
        //}
        //public void GetSoundDevices()
        //{
        //    uint devices = waveInGetNumDevs();
        //    string[] result = new string[devices];
        //    WAVEOUTCAPS caps = new WAVEOUTCAPS();
        //    for (uint i = 0; i < devices; i++)
        //    {
        //        waveInGetDevCaps(i, ref caps, (uint)Marshal.SizeOf(caps));
        //        result[i] = caps.szPname;
        //        SoundDevices.Add(new SoundDevice() { Name = caps.szPname });
        //    }
        //}
        #endregion test

        private HashSet<string> idsCache;

        public MainViewModel()
        {
            SoundDevices = new ObservableCollection<SoundDevice>();
            GetAllInputDevices();
        }


        public void GetAllInputDevices()
        {
            idsCache = new HashSet<string>();

            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                Console.WriteLine($"{n}: {caps.ProductName}");
            }

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
                        SoundDevices.Add(new SoundDevice(device));
                    }
                }
            }
        }

        public Dictionary<string, MMDevice> GetInputAudioDevices()
        {
            Dictionary<string, MMDevice> retVal = new Dictionary<string, MMDevice>();
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All))
                {
                    if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                    {
                        retVal.Add(device.FriendlyName, device);
                        break;
                    }
                }
            }
      
            return retVal;
        }



        private ObservableCollection<SoundDevice> _soundDevices;
        public ObservableCollection<SoundDevice> SoundDevices
        {
            get => _soundDevices;
            set
            {
                _soundDevices = value;
                RaisePropertyChanged(nameof(SoundDevices));
            }
        }

    }


    public class SoundDevice
    {
        public SoundDevice(MMDevice device)
        {
            Device = device;
            this.Name = device.FriendlyName;
        }

        public string Name { get; set; }
        public MMDevice Device { get; set; }
    }
}
