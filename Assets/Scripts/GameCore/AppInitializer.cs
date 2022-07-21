using Core.SoundModule;
using UnityEngine;

public class AppInitializer : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        MainWindow.Show();
    
        Timer.CreateCountDown(0.1f, true).Stoped += () =>
        {
            SoundManager.MusicMixer.Play(2);
        };
        
        Advertiser.Initialize();
        Advertiser.ShowBanner();
        
        MessageWindow.Initialize();
    }
}
