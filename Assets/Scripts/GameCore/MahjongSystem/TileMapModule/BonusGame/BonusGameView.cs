using System;
using Core.Extensions.Unity;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[Serializable]
public class BonusGameView
{
    [SerializeField] private GameObject progressPrefab;
    [SerializeField] private ParticleSystem bonusFxPrefab;
    [SerializeField] private ParticleSystem messageFxPrefab;

    private string progressImagePath = "Back";
    private string progressTextPath = "Text";
    
    private ParticleSystem bonusFx;
    private GameObject progress;
    private Image progressImage;
    private TextMeshProUGUI progressText;
    private Action<Timer.BaseTimer> startAction;
    private Transform camera;
    private Bounds cameraBounds;

    public bool IsStarted { get; private set; }

    public void Init()
    {
        startAction = FirstStart;
        var camera = Camera.main;
        cameraBounds = camera.GetBounds();
        this.camera = camera.transform;
    }
    
    public void Start(Timer.BaseTimer timer)
    {
        startAction(timer);
        IsStarted = true;
    }
    
    public void FirstStart(Timer.BaseTimer timer)
    {
        progress = Object.Instantiate(progressPrefab);
        bonusFx = Object.Instantiate(bonusFxPrefab);
        bonusFx.transform.position = new Vector3(camera.position.x, cameraBounds.min.y);
        GameWindow.AddChild(progress.transform);
        progressImage = progress.Get<Image>(progressImagePath);
        progressText = progress.Get<TextMeshProUGUI>(progressTextPath);
        DefaultStart(timer);
        startAction = DefaultStart;
    }

    private void DefaultStart(Timer.BaseTimer timer)
    {
        bonusFx.Play();
        progress.SetActive(true);
        progress.transform.DOScale(1.4f, 0.4f).SetLoops(-1, LoopType.Yoyo).SetId(this);
        progressText.text = ((int)timer.Interval).ToString();
        timer.NormalizeUpdated += value => progressImage.fillAmount = value;
        timer.IntUpdated += value => progressText.text = value.ToString();
    }

    public void Stop()
    {
        DOTween.Kill(this);
        progress.SetActive(false);
        bonusFx.Stop();
        IsStarted = false;
    }
    
    public void Destroy()
    {
        DOTween.Kill(this);
        Object.Destroy(progress);
        Object.Destroy(bonusFx);
        IsStarted = false;
    }
    
    public void PlayMessageFX()
    {
        var messagePrefab = Object.Instantiate(messageFxPrefab);
        MessageWindow.AddChild(messagePrefab.transform);
        MessageWindow.Hided += messagePrefab.Stop;
        messagePrefab.transform.SetSiblingIndex(1);
    }
}
