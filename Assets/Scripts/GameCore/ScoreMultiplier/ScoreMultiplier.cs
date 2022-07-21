using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ScoreMultiplier
{
    [SerializeField] private GameObject progressParent;
    [SerializeField] private Image progress;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private ScoreMultiplierConfig config;

    private Timer.BaseTimer timer;
    private int currentStepLevel = -1;
    private MultiplyStep currentStep = new MultiplyStep {multiplier = 1};

    public int Multiplier => currentStep.multiplier;

    public void Init()
    {
        timer = new Timer.CountDownTimer(0, isLoop: true);
        timer.NormalizeUpdated += UpdateView;
        timer.Elapsed += Reset;
        Reset();
    }
    
    public void Increase()
    {
        if (progressParent.activeSelf == false)
        {
            progressParent.transform.DOScale(Vector3.one * 1.25f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetId(this);
        }

        currentStepLevel++;
        OnStepLevelChanged();
        SetActive(true);
    }

    public void Reset()
    {
        Debug.Log("Reset");
        currentStepLevel = 0;
        OnStepLevelChanged();
        progressParent.transform.localScale = Vector3.one;
        DOTween.Kill(this);
        SetActive(false);
    }

    private void OnStepLevelChanged()
    {
        if (currentStepLevel < config.steps.Count)
        {
            currentStep = config.steps[currentStepLevel];
        }

        timer.Interval = currentStep.time;
        timer.Restart();
        text.text = $"X{currentStep.multiplier}";
    }

    private void SetActive(bool active)
    {
        if (active)
        {
            timer.Start();
        }
        else
        {
            timer.Stop();
        }
        
        progressParent.SetActive(active);
    }

    private void UpdateView(float value)
    {
        progress.fillAmount = value;
    }
}
