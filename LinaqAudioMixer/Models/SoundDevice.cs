using LinaqAudioControl.ViewModels;
using MaterialDesignThemes.Wpf;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinaqAudioMixer.Models
{
    public class SoundDevice : BaseModel
    {
        public SoundDevice(MMDevice device)
        {
            Device = device;
            this.Name = device.FriendlyName;
            CurrentVolume = Device.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
            device.AudioEndpointVolume.OnVolumeNotification += OnUpdateVolume;
            SetIconKind();
        }

        public string Name { get; set; }
        public MMDevice Device { get; set; }
        private float _currentVolume;
        public float CurrentVolume
        {
            get => _currentVolume;
            set
            {
                _currentVolume = value;
                Device.AudioEndpointVolume.MasterVolumeLevelScalar = value / 100;
            }
        }

        private PackIconKind _iconKind;
        public PackIconKind IconKind
        {
            get => _iconKind;
            set
            {
                _iconKind = value;
                RaisePropertyChanged(nameof(IconKind));
            }
        }

        public void VolumeUp()
        {
            Device.AudioEndpointVolume.VolumeStepUp();
        }

        public void VolumeDown()
        {
            Device.AudioEndpointVolume.VolumeStepDown();
        }

        public void Mute()
        {
            Device.AudioEndpointVolume.Mute = !Device.AudioEndpointVolume.Mute;
            SetIconKind();
        }

        private void OnUpdateVolume(AudioVolumeNotificationData data)
        {
            CurrentVolume = data.MasterVolume * 100;
            RaisePropertyChanged(nameof(CurrentVolume));
        }

        private void SetIconKind()
        {
            if (Device.AudioEndpointVolume.Mute)
                IconKind = PackIconKind.VolumeOff;
            else
                IconKind = PackIconKind.VolumeHigh;
        }
    
    }

}
