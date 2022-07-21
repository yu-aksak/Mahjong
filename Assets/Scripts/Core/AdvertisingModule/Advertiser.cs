using System;
using GoogleMobileAds.Api;
using UnityEngine;

public static class Advertiser
{
    private static RewardedAd rewardedAd;
    private static BannerView banner;
    private static Timer.BaseTimer timer;
    private static Action<RewardResult> rewardCallback;
    private static Action lockRewardCallback;
    private static bool isInitialize;

    public static bool IsRewardReady => rewardedAd.IsLoaded();

    static Advertiser()
    {
        Initialize();
    }

    public static void Initialize()
    {
        if (isInitialize)
        {
            return;
        }

        MobileAds.Initialize(null);
        
        banner = new BannerView("ca-app-pub-3940256099942544/6300978111", AdSize.SmartBanner, AdPosition.Bottom);
        
        rewardedAd = new RewardedAd("ca-app-pub-3940256099942544/5224354917");

        rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
        rewardedAd.OnAdFailedToShow += OnAdFailedToShow;
        
        LoadReward();

        isInitialize = true;
    }
    
    public static void ShowReward(Action<RewardResult> callback = null)
    {
        Debug.Log($"[{nameof(Advertiser)}] ShowReward");
        if (rewardedAd.IsLoaded())
        {
            Debug.Log($"[{nameof(Advertiser)}] IsLoaded");

            timer = Timer.CreateCountDown(1, true, true);
            timer.SetId(timer);
            timer.Updated += Update;
            
            rewardCallback = callback;
            rewardedAd.Show();
            LoadReward();
        }
    }
    
    private static void OnUserEarnedReward(object sender, Reward e)
    {
        lockRewardCallback = () =>
        {
            Debug.Log($"[{nameof(Advertiser)}] OnUserEarnedReward");
            rewardCallback?.Invoke(RewardResult.Rewarded);
        };
    }

    private static void OnAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        lockRewardCallback = () =>
        {
            Debug.Log($"[{nameof(Advertiser)}] OnAdFailedToShow");
            rewardCallback?.Invoke(RewardResult.Failed);
        };
    }

    public static void ShowBanner()
    {
        Debug.Log($"[{nameof(Advertiser)}] ShowBanner");
        banner.Show();
    }

    public static void HideBanner()
    {
        Debug.Log($"[{nameof(Advertiser)}] HideBanner");
        banner.Hide();
    }

    private static void LoadReward()
    {
        var request = new AdRequest.Builder().Build();

        rewardedAd.LoadAd(request);
    }

    private static void Update(float _)
    {
        if (lockRewardCallback != null)
        {
            lockRewardCallback();
            lockRewardCallback = null;
            TimerExtensions.Kill(timer);
        }
    }
}
