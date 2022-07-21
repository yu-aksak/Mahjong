using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutOfMovesWindow :  Window<OutOfMovesWindow>
{
    [SerializeField] private GameObject forAdsMode;
    [SerializeField] private GameObject forMoneyMode;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Image timerImage;
    [SerializeField] private TextMeshProUGUI timeText;

    protected override void Init()
    {
        base.Init();
        
        closeButton.onClick.AddListener(OnCloseButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
    }

    protected override void OnShow()
    {
        base.OnShow();

        ConfigureContinueButton(Advertiser.IsRewardReady);
        var timer = Timer.CreateCountDown(5, true);
        timer.NormalizeUpdated += value => timerImage.fillAmount = value;
        timer.IntUpdated += value => timeText.text = value.ToString();
        timer.Stoped += OnCloseButtonClick;
        
        timer.SetId(this);
    }

    protected override void OnHided()
    {
        base.OnHided();
        
        continueButton.onClick.RemoveAllListeners();
    }

    private void ConfigureContinueButton(bool isForAds)
    {
        forAdsMode.SetActive(isForAds);
        forMoneyMode.SetActive(!isForAds);
        continueButton.onClick.AddListener(() => TimerExtensions.Kill(this));
        
        if (isForAds)
        {
            continueButton.onClick.AddListener(OnContinueForAds);
        }
        else
        {
            continueButton.onClick.AddListener(OnContinueForMoney);
        }
    }

    private static void OnCloseButtonClick()
    {
        LoseWindow.Show();
        Hide();
    }
    
    private static void OnHomeButtonClick()
    {
        StoreWindow.Show();
        GameWindow.Hide();
        Hide();
    }
    
    private static void OnContinueForMoney()
    {
        GameWindow.Shuffle();
        Hide();
    }
    
    private static void OnContinueForAds()
    {
        Advertiser.ShowReward(result =>
        {
            switch (result)
            {
                case RewardResult.Failed:
                    OnCloseButtonClick();
                    break;
                case RewardResult.Rewarded:
                    GameWindow.Shuffle();
                    Hide();
                    break;
            }
        });
    }
}
