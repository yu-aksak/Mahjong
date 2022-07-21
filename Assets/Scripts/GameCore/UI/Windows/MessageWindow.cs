using Core.PlayerModule;
using TMPro;
using UnityEngine;

public class MessageWindow :  Window<MessageWindow>
{
    [SerializeField] private Color wrongColor = new Color(1f, 0.19f, 0.22f);
    [SerializeField] private TextMeshProUGUI message;
    
    public static void Initialize()
    {
        PlayerProfile.Config.Wallet.Purchased += OnPurchased;
    }
    
    public static void Show(string message, Color color, float duration = 1, float fontSize = 70)
    {
        Show();
        var instance = Instance;
        instance.message.text = message;
        instance.message.fontSize = fontSize;
        instance.message.color = color;
        
        Timer.CreateCountDown(duration, true).Stoped += Hide;
    }

    private static void OnPurchased(bool isPurchased)
    {
        if (isPurchased == false)
        {
            Show("No Money", Instance.wrongColor, 0.5f);
        }
    }
}