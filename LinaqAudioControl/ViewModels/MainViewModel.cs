using LinaqAudioControl.Helpers;
using LinaqAudioMixer.Models;
using LinaqAudioMixer.Providers;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Newtonsoft.Json;
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
        private readonly OutputDeviceProvider outputDeviceProvider;
        private HashSet<string> shownDevices;
        public MainViewModel()
        {
            outputDeviceProvider = new OutputDeviceProvider();

            shownDevices = JsonConvert.DeserializeObject<HashSet<string>>(Properties.Settings.Default.ShownDevices);

            SoundDevices = new ObservableCollection<SoundDevice>();
            VolumeUpCmd = new RelayCommand(VolumeUpExe);
            VolumeDownCmd = new RelayCommand(VolumeDownExe);
            MuteCmd = new RelayCommand(MuteExe);
            LoadDevicesAsync();
        }
        public ICommand VolumeUpCmd { get; set; }
        public ICommand VolumeDownCmd { get; set; }
        public ICommand MuteCmd { get; set; }

        private ObservableCollection<SoundDevice> _availableSoundDevices;
        public ObservableCollection<SoundDevice> AvailableSoundDevices
        {
            get => _availableSoundDevices;
            set
            {
                _availableSoundDevices = value;
                RaisePropertyChanged(nameof(AvailableSoundDevices));
            }
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
        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                RaisePropertyChanged(nameof(ProgressValue));
                Console.WriteLine($"Progress {value}");
            }
        }

        private int _progressMaxValue;
        public int ProgressMaxValue
        {
            get => _progressMaxValue;
            set
            {
                _progressMaxValue = value;
                RaisePropertyChanged(nameof(ProgressMaxValue));
            }
        }
        private async void LoadDevicesAsync()
        {
            AvailableSoundDevices = new ObservableCollection<SoundDevice>(await outputDeviceProvider.GetAllInputDevicesAsync());

            if (shownDevices == null)
            {
                shownDevices = new HashSet<string>();
                foreach (var item in AvailableSoundDevices)
                {
                    shownDevices.Add(item.Id);
                }
            }

            foreach (var id in shownDevices)
            {
                var dev = AvailableSoundDevices.FirstOrDefault(x => x.Id == id);
                dev.IsShown = true;
                SoundDevices.Add(dev);
            }

            foreach (var item in AvailableSoundDevices)
            {
                item.IsShownChanged += Item_IsShownChanged;
            }

            ProgressMaxValue = 0;
        }

        private void Item_IsShownChanged(object sender, EventArgs e)
        {
            if (sender is SoundDevice sd)
            {
                if (sd.IsShown)
                {
                    shownDevices.Add(sd.Id); 
                }
                else
                {
                    shownDevices.Remove(sd.Id); 
                }
               
            }

            SoundDevices.Clear();
            foreach (var id in shownDevices)
            {
                var dev = AvailableSoundDevices.FirstOrDefault(x => x.Id == id);
                SoundDevices.Add(dev);
            }
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

        private void MuteExe(object obj)
        {
            if (obj is SoundDevice sd)
            {
                sd.Mute();
            }
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ShownDevices = JsonConvert.SerializeObject(shownDevices);
            Properties.Settings.Default.Save();
        }
    }
}
