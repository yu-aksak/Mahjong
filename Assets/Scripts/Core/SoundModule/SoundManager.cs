using System.Collections.Generic;
using Core.GameSettingsModule;
using Core.SingleService;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.SoundModule
{
    public class SoundManager : SingleService<SoundManager>
    {
        private const string MusicMixerName = "Music";
        private const string SoundsMixerName = "Sounds";

        [SerializeField] private List<AudioClip> sounds;
        [SerializeField] private AudioMixerGroup soundGroup;
        [SerializeField] private AudioMixerGroup musicGroup;

        private MixerController soundMixer;
        private MixerController musicMixer;
        public static MixerController SoundMixer => Instance.soundMixer;
        public static MixerController MusicMixer => Instance.musicMixer;

        /*
    private Selectable selectable;
    private Canvas canvas;
    private float scaleFactor;
    private Vector3 rectPosition;
    private Rect rect;
    private bool hasSelectable;
    private List<RaycastResult> raycastList = new List<RaycastResult>();
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                var currentEventSystem = EventSystem.current;
                var pointerData = new PointerEventData(currentEventSystem);
                pointerData.position = Input.mousePosition;
                currentEventSystem.RaycastAll(pointerData, raycastList);

                hasSelectable = raycastList.Count > 0 && raycastList[0].gameObject.TryGetComponent(out selectable);

                if (hasSelectable)
                {
                    canvas = selectable.GetComponentInParent<Canvas>();
                    scaleFactor = canvas.scaleFactor;
                    var rectTransform = selectable.GetComponent<RectTransform>();
                    rectPosition = rectTransform.position;
                    rect = rectTransform.rect;
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            if (hasSelectable)
            {
                var canvasPos = canvas.ScreenToCanvasPosition(Input.mousePosition, scaleFactor);
                var position = canvasPos - rectPosition;

                if (rect.Contains(position / scaleFactor))
                {
                    PlaySoundByIndex(0);
                }
            }
        }
    }
    */

        protected override void Init()
        {
            var gameSettings = GameSettings.Config;

            soundMixer = new MixerController(soundGroup, SoundsMixerName);
            musicMixer = new MixerController(musicGroup, MusicMixerName);
            
            gameSettings.AudioSettings.SoundActiveChanged += soundMixer.SetEnable;
            gameSettings.AudioSettings.MusicActiveChanged += soundMixer.SetEnable;
        
            soundMixer.SetLoop(false);
            musicMixer.SetLoop(true);
        
            Timer.CreateCountDown(Time.deltaTime, true).Stoped += () =>
            {
                soundMixer.SetEnable(gameSettings.AudioSettings.IsSoundEnabled);
                musicMixer.SetEnable(gameSettings.AudioSettings.IsMusicEnabled);
            };
        }

        public static AudioClip GetClip(int index)
        {
            return Instance.sounds[index];
        }
    }
}




