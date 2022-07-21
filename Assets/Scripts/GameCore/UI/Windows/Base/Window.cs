using System;
using Core.SingleService;
using Core.SoundModule;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class Window<T> : SingleService<T> where T : Window<T>
{
    public static event Action<Window<T>> ShowedWindow;
    public static event Action<Window<T>> HidedWindow;
    public static event Action Showed;
    public static event Action Hided;

    [SerializeField] protected float fadeSpeed = 0.2f;
    protected bool needBindSounsAfterInit;
    private CanvasGroup canvasGroup;

    protected override void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        
        if (needBindSounsAfterInit == false)
        {
            BindButtonsSound();
            BindTogglesSound();
        }

        gameObject.SetActive(false);
    }

    protected void BindButtonsSound()
    {
        var buttons = GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.AddListener(OnButtonClick);
        }
    }

    private static void OnButtonClick()
    {
        SoundManager.SoundMixer.PlayOneShot(0);
    }
    
    protected void BindTogglesSound()
    {
        var toggles = GetComponentsInChildren<Toggle>();

        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].onValueChanged.AddListener(OnToggleClick);
        }
    }

    private static void OnToggleClick(bool _)
    {
        SoundManager.SoundMixer.PlayOneShot(0);
    }

    private void InternalShow()
    {
        gameObject.SetActive(true);
        OnShow();
        canvasGroup.DOFade(1, fadeSpeed).onComplete += OnCompleteShow;
    }
    
    private void InternalHide()
    {
        OnHide();
        canvasGroup.DOFade(0, fadeSpeed).onComplete += OnCompleteHide;
    }
    
    private void OnCompleteShow()
    {
        OnShowed();
        ShowedWindow?.Invoke(this);
        Showed?.Invoke();
    }

    private void OnCompleteHide()
    {
        gameObject.SetActive(false);
        OnHided();
        HidedWindow?.Invoke(this);
        Hided?.Invoke();
    }

    protected virtual void OnShow()
    {
        
    }
    
    protected virtual void OnHide()
    {
        
    }
    
    protected virtual void OnHided()
    {
        
    }
    
    protected virtual void OnShowed()
    {
        
    }

    public static void Show()
    {
        Instance.InternalShow();
    }
    
    public static void Hide()
    {
        Instance.InternalHide();
    }

    public static void AddChild(Transform child, bool worldPositionStays = false)
    {
        child.SetParent(Instance.transform, worldPositionStays);
    }
}