using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainWindow : Window<MainWindow>
{
    [SerializeField] private Button startGame;

    private Tweener tweenAnim;

    protected override void Awake()
    {
        base.Awake();
        startGame.onClick.AddListener(OnStartGameButtonClick);

        tweenAnim = startGame.transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnEnable()
    {
        tweenAnim.Play();
    }

    private void OnDisable()
    {
        tweenAnim.Pause();
    }

    private void OnDestroy()
    {
        tweenAnim.Kill();
    }

    private void OnStartGameButtonClick()
    {
        Hide();
        StoreWindow.Show();
    }
}