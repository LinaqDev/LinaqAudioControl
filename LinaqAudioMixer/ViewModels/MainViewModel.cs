using LinaqAudioControl.Helpers;
using LinaqAudioMixer.Models;
using LinaqAudioMixer.Providers;
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
using System.Windows.Input;

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

        private readonly OutputDeviceProvider outputDeviceProvider;
        public MainViewModel()
        {
            outputDeviceProvider = new OutputDeviceProvider();
            SoundDevices = new ObservableCollection<SoundDevice>();
            VolumeUpCmd = new RelayCommand(VolumeUpExe);
            VolumeDownCmd = new RelayCommand(VolumeDownExe);
            LoadDevicesAsync();
        }

        public ICommand VolumeUpCmd { get; set; }
        public ICommand VolumeDownCmd { get; set; }

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

        private async void LoadDevicesAsync()
        {
            SoundDevices = new ObservableCollection<SoundDevice>(await outputDeviceProvider.GetAllInputDevicesAsync());
        }

        private void VolumeDownExe(object obj)
        {
            if (obj is SoundDevice sd)
            {
                sd.VolumeDown();
            }
        }

        private void VolumeUpExe(object obj)
        {
            if (obj is SoundDevice sd)
            {
                sd.VolumeUp();
            }
        }
    }
}
