using Core.Extensions.Unity;
using Core.GameSettingsModule;
using UnityEngine;
using UnityEngine.UI;

public class PauseWindow : Window<PauseWindow>
{
    private const string States = "States";
    
    [SerializeField] private Button sound;
    [SerializeField] private Button music;
    [SerializeField] private Button close;
    [SerializeField] private Button retry;
    [SerializeField] private Button home;
    [SerializeField] private Button play;
    [SerializeField] private Toggle hardLevelToggle;
    [SerializeField] private Toggle easyLevelToggle;
    
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
        retry.onClick.AddListener(OnRetryButtonClick);
        home.onClick.AddListener(OnHomeButtonClick);
        play.onClick.AddListener(OnCloseButtonClick);

        hardLevelToggle.onValueChanged.AddListener(CheckIsHardDifficulty);
        easyLevelToggle.onValueChanged.AddListener(CheckIsEasyDifficulty);
        
        var gameSettings = GameSettings.Config;
        hardLevelToggle.isOn = gameSettings.LevelDifficulty == LevelDifficultyType.Hard;
        easyLevelToggle.isOn = gameSettings.LevelDifficulty == LevelDifficultyType.Easy;
    }

    protected override void OnShow()
    {
        base.OnShow();
        ChangeIcon(soundStatesParent, GameSettings.Config.AudioSettings.IsSoundEnabled);
        ChangeIcon(musicStatesParent, GameSettings.Config.AudioSettings.IsMusicEnabled);
        Timer.BaseTimer.PauseAll();
    }

    protected override void OnHide()
    {
        Timer.BaseTimer.ResumeAll();
    }

    private void ChangeIcon(Transform statesParent, bool value)
    {
        if (value)
        {
            statesParent.GetChild(0).gameObject.SetActive(true);
            statesParent.GetChild(1).gameObject.SetActive(false);
        }
        else 
        {
            statesParent.GetChild(0).gameObject.SetActive(false);
            statesParent.GetChild(1).gameObject.SetActive(true);
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
    
    private void OnRetryButtonClick()
    {
        GameWindow.Restart();
        Hide();
    }    
    
    private void OnHomeButtonClick()
    {
        StoreWindow.Show();
        GameWindow.Hide();
        Hide();
    }

    private static void CheckIsHardDifficulty(bool active)
    {
        if (active)
        {
            TileView.NeedBlock = false;
            GameSettings.Config.LevelDifficulty = LevelDifficultyType.Hard;
            GameWindow.UpdateTilesView();
        }
    }
    
    private static void CheckIsEasyDifficulty(bool active)
    {
        if (active)
        {
            TileView.NeedBlock = true;
            GameSettings.Config.LevelDifficulty = LevelDifficultyType.Easy;
            GameWindow.UpdateTilesView();
        }
    }
}
