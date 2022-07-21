using UnityEngine;
using UnityEngine.UI;

public class LoseWindow :  Window<LoseWindow>
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button homeButton;

    protected override void Init()
    {
        base.Init();
        retryButton.onClick.AddListener(GameWindow.Restart);
        retryButton.onClick.AddListener(Hide);
        homeButton.onClick.AddListener(StoreWindow.Show);
        homeButton.onClick.AddListener(GameWindow.Hide);
        homeButton.onClick.AddListener(Hide);
    }
}
