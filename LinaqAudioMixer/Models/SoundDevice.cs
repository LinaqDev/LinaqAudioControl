using LinaqAudioControl.ViewModels;
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
            CurrentVolume = Device.AudioEndpointVolume.MasterVolumeLevelScalar;
            device.AudioEndpointVolume.OnVolumeNotification += OnUpdateVolume;
        }

        public string Name { get; set; }
        public MMDevice Device { get; set; }
        public float CurrentVolume { get; set; }
        public void VolumeUp()
        {
            Device.AudioEndpointVolume.VolumeStepUp();
        }

        public void VolumeDown()
        {
            Device.AudioEndpointVolume.VolumeStepDown();
        }
        private void OnUpdateVolume(AudioVolumeNotificationData data)
        {
            CurrentVolume = data.MasterVolume;
            RaisePropertyChanged(nameof(CurrentVolume));
        }
    }
}
