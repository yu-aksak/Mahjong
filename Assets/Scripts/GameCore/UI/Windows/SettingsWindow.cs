using Core.Extensions.Unity;
using Core.GameSettingsModule;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : Window<SettingsWindow>
{
    private const string States = "States";
    
    [SerializeField] private Button sound;
    [SerializeField] private Button music;
    [SerializeField] private Button close;

    private Transform soundStatesParent;
    private Transform musicStatesParent;
    
    protected override void Init()
    {
        base.Init();
        soundStatesParent = sound.Get<Transform>(States);
        musicStatesParent = music.Get<Transform>(States);
        
        sound.onClick.AddListener(OnSoundButtonClick);
        music.onClick.AddListener(OnMusicButtonClick);
        close.onClick.AddListener(OnCloseButtonClick);
    }

    protected override void OnShow()
    {
        base.OnShow();
        ChangeIcon(soundStatesParent, GameSettings.Config.AudioSettings.IsSoundEnabled);
        ChangeIcon(musicStatesParent, GameSettings.Config.AudioSettings.IsMusicEnabled);
    }

    private void ChangeIcon(Transform button, bool value)
    {
        if (value)
        {
            button.GetChild(0).gameObject.SetActive(true);
            button.GetChild(1).gameObject.SetActive(false);
        }
        else 
        {
            button.GetChild(0).gameObject.SetActive(false);
            button.GetChild(1).gameObject.SetActive(true);
        }
    }

    private void OnSoundButtonClick()
    {
        var audioSettings = GameSettings.Config.AudioSettings;
        bool isSoundEnabled = !audioSettings.IsSoundEnabled;
        audioSettings.IsSoundEnabled = isSoundEnabled;
        ChangeIcon(soundStatesParent, isSoundEnabled);
    }
    private void OnMusicButtonClick()
    {
        var audioSettings = GameSettings.Config.AudioSettings;
        bool isMusicEnabled = !audioSettings.IsMusicEnabled;
        audioSettings.IsMusicEnabled = isMusicEnabled;
        ChangeIcon(musicStatesParent, isMusicEnabled);
    }    
    
    private void OnCloseButtonClick()
    {
       Hide();
    }
}
