using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.SoundModule
{
    public class MixerController
    {
        private const int VolumeOn = 0;
        private const int VolumeOf = -80;
    
        private readonly AudioMixerGroup group;
        private readonly AudioMixer mixer;
        private readonly AudioSource source;

        public MixerController(AudioMixerGroup group, string mixerName)
        {
            this.group = group;
            mixer = GetMixer(mixerName);
            source = new GameObject(mixerName).AddComponent<AudioSource>();
            source.outputAudioMixerGroup = group;
        }
    
        public void Play(int index, float volume = 1)
        {
            var clip = SoundManager.GetClip(index);
            source.volume = volume;
            source.clip = clip;
            source.Play();
        }
    
        public void PlayOneShot(int index, float volume = 1)
        {
            var clip = SoundManager.GetClip(index);
            source.volume = volume;
            source.clip = clip;
            source.PlayOneShot(clip);
        }

        public void PlayWithDuration(int index, float volume = 1, float duration = 0, float fadeSpeed = 0)
        {
            var soundSource = GetNewAudioSource(index);
            PlayWithDuration(soundSource, volume, duration, fadeSpeed);
        }
    
        public void PlayWithClipDuration(int index, float volume = 1, float fadeSpeed = 0)
        {
            var soundSource = GetNewAudioSource(index);
            PlayWithClipDuration(soundSource, volume, fadeSpeed);
        }

        public void SetEnable(bool value)
        {
            mixer.SetFloat("Volume", value ? VolumeOn : VolumeOf);
        }
    
        public void SetLoop(bool loop)
        {
            source.loop = loop;
        }
    
        private static void PlayWithClipDuration(AudioSource source, float volume = 1, float fadeSpeed = 0)
        {
            var duration = source.clip.length - fadeSpeed;
            PlayWithDuration(source, volume, duration, fadeSpeed);
        }
    
        private static void PlayWithDuration(AudioSource source, float volume = 1, float duration = 0, float fadeSpeed = 0)
        {
            source.DOFade(volume, fadeSpeed).onComplete += () =>
            {
                Timer.CreateCountDown(duration, true).Stoped += () =>
                {
                    source.DOFade(0, fadeSpeed).onComplete += () => Object.Destroy(source.gameObject);
                };
            };

            source.loop = true;
            source.Play();
        }
    
        private AudioSource GetNewAudioSource(int index)
        {
            var clip = SoundManager.GetClip(index);
            var soundSource = new GameObject($"{group.name} With Duration").AddComponent<AudioSource>();
            soundSource.outputAudioMixerGroup = group;
            soundSource.clip = clip;
            soundSource.volume = 0;
            return soundSource;
        }
    
        private static AudioMixer GetMixer(string mixerName)
        {
            var mixerPath = $"Mixer/{mixerName}";
            var mixer = Resources.Load<AudioMixer>(mixerPath);
        
            if (mixer == null)
            {
                Debug.LogWarning($"Can't load audio clip {mixerPath} from Resources");
                return null;
            }
    
            return mixer;
        }
    }
}