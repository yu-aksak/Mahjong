using Core.CurrencyModule;
using Core.PlayerModule;
using TMPro;
using UnityEngine;

public class StatsWindow : Window<StatsWindow>
{
    [SerializeField] private TextMeshProUGUI coinsBalanceText;
    protected override void Init()
    {
        base.Init();
        var currency = PlayerProfile.Config.Wallet.GetCurrency(CurrencyType.Coins);
        currency.ValueChanged += OnCoinsBalanceChanged;
        OnCoinsBalanceChanged(currency.Value);
    }

    private void OnCoinsBalanceChanged(int balance)
    {
        coinsBalanceText.text = balance.ToString();
    }
}