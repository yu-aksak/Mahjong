using System;

namespace Core.GameSettingsModule
{
    [Serializable]
    public class AudioSettings
    {
        private bool isSoundEnabled = true;
        private bool isMusicEnabled = true;

        public event Action<bool> SoundActiveChanged; 
        public event Action<bool> MusicActiveChanged; 

        public bool IsSoundEnabled
        {
            get
            {
                return isSoundEnabled;
            }
            set
            {
                isSoundEnabled = value;
                SoundActiveChanged?.Invoke(value);
            }
        }
    
        public bool IsMusicEnabled
        {
            get
            {
                return isMusicEnabled;
            }
            set
            {
                isMusicEnabled = value;
                MusicActiveChanged?.Invoke(value);
            }
        }
    }
}
