using Core.ObjectPool;
using Core.StoreModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelWindow :  Window<LevelWindow>
{
    [SerializeField] private RectTransform bestScoreLayout;
    [SerializeField] private RectTransform bestScoreSlotParent;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI bestScoreSlotPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button playButton;
    
    private readonly ObjectPoolEnableDisable<TextMeshProUGUI> textPool = new ObjectPoolEnableDisable<TextMeshProUGUI>(Create);
    private BestScores bestScores;

    protected override void Init()
    {
        base.Init();
        closeButton.onClick.AddListener(Hide);
        playButton.onClick.AddListener(Play);
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateView();
    }

    private void UpdateView()
    {
        var selected = ItemSelector.GetSelected(ItemType.Mahjong);
        bestScores = BestScoresConfig.Config.GetScores(selected);
        var item = Store.Config.GetItem(ItemType.Mahjong, ItemSelector.GetSelected(ItemType.Mahjong));
        levelNameText.text = item.name;
            
        textPool.Fill(bestScores.Count);
        
        for (int i = 0; i < bestScores.Count; i++)
        {
            OnCreateText(i);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(bestScoreLayout);
    }
    
    private void OnCreateText(int index)
    {
        textPool[index].text = $"{index + 1}. {bestScores[index]}";
    }

    private void Play()
    {
        Hide();
        StoreWindow.Hide();
        StatsWindow.Hide();
        GameWindow.Show();
    }
    
    private static TextMeshProUGUI Create()
    {
        var instance = Instance;
        return Instantiate(instance.bestScoreSlotPrefab, instance.bestScoreSlotParent);
    }
}
