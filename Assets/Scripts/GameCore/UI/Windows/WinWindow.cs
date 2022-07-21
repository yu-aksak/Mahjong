using Core.CurrencyModule;
using Core.PlayerModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinWindow : Window<WinWindow>
{
    [SerializeField] private TextMeshProUGUI bestScoreLabelText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Image x2TimerImage;
    [SerializeField] private Button x2Button;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private int rewardValue = 30;

    protected override void Init()
    {
        base.Init();
        
        retryButton.onClick.AddListener(OnRetryButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
    }

    protected override void OnShow()
    {
        base.OnShow();
        Reset();
    }

    protected override void OnHided()
    {
        base.OnHided();
        Reward();
    }

    private void OnX2ButtonClick()
    {
        Advertiser.ShowReward(result =>
        {
            if (result.Equals(RewardResult.Rewarded))
            {
                rewardValue *= 2;
                rewardText.text = $"+{rewardValue}";
            }
            
            SetActiveRetryHomeButtons(true);
        });
        
        TimerExtensions.Kill(this);
    }

    private void InitX2Button()
    {
        if (Advertiser.IsRewardReady)
        {
            var timer = Timer.CreateCountDown(5, true);
            timer.NormalizeUpdated += value => x2TimerImage.fillAmount = value;
            timer.Stoped += () => SetActiveRetryHomeButtons(true);
            timer.SetId(this);
            
            x2Button.onClick.AddListener(OnX2ButtonClick);
            
            SetActiveRetryHomeButtons(false);
        }
        else
        {
            SetActiveRetryHomeButtons(true);
        }
    }

    private void SetActiveRetryHomeButtons(bool active)
    {
        retryButton.gameObject.SetActive(active);
        homeButton.gameObject.SetActive(active);
        x2Button.gameObject.SetActive(!active);
    }

    private void Reset()
    {
        Debug.Log("UpdateView");
        x2Button.onClick.RemoveAllListeners();
        InitX2Button();
        rewardValue = 20;
        bestScoreLabelText.gameObject.SetActive(GameWindow.Place == 0);
        scoreText.text = GameWindow.Score.ToString();
        rewardText.text = $"+{rewardValue}";
    }

    private void Reward()
    {
        PlayerProfile.Config.Wallet.Earn(CurrencyType.Coins, rewardValue);
    }

    private static void OnHomeButtonClick()
    {
        StoreWindow.Show();
        GameWindow.Hide();
        Hide();
    }
    
    private static void OnRetryButtonClick()
    {
        GameWindow.Restart();
        Hide();
    }
}
