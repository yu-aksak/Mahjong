using Core.GameSettingsModule;
using Core.SoundModule;
using DG.Tweening;
using UnityEngine;

public class TileView
{
    private readonly SpriteRenderer sprite;
    private static readonly Color selectedColor = new Color(0.42f, 1f, 0.18f);
    private static readonly Color defaultColor = Color.white;
    private static readonly Color hintedColor = Color.cyan;
    private static readonly Color blockedColor = Color.gray;
    private static readonly Color onDestroyColor = Color.yellow;
    private static readonly Color wrongColor = new Color(1f, 0.18f, 0.16f);
    private readonly ParticleSystem destroyFx;

    public GameObject GameObject { get; }

    public Transform Transform { get; }

    public static bool NeedBlock { get; set; }

    public TileView(SpriteRenderer sprite, ParticleSystem destroyFx)
    {
        this.sprite = sprite;
        this.destroyFx = destroyFx;
        Transform = sprite.transform;
        GameObject = sprite.gameObject;
        NeedBlock = GameSettings.Config.LevelDifficulty == LevelDifficultyType.Easy;
    }

    public void SetSprite(Sprite sprite)
    {
        this.sprite.sprite = sprite;
    }
    
    public void SetBlockState(bool isBlocked)
    {
        if (NeedBlock && isBlocked)
        {
            ToColor(blockedColor);
        }
        else
        {
            ToColor(defaultColor);
        }
    }

    public void SetBySelectionState(bool isSelected)
    {
        if (isSelected)
        {
            Select();
        }
        else
        {
            Deselect();
        }
    }
    
    public void Select()
    {
        LoopColor(selectedColor, -1);
    }

    public void Deselect()
    {
        ToColor(defaultColor);
    }
    
    public void Hint(TweenCallback onComplete)
    {
        LoopColor(hintedColor, 3, onComplete);
    }

    public void WrongSelection()
    {
        LoopColor(wrongColor, 3);
    }
    
    public void OnBonus()
    {
        LoopColor(onDestroyColor, -1);
    }
    
    public void StartDestroyAnimation()
    {
        sprite.DOColor(onDestroyColor, 0.5f);
        Transform.DOScale(Vector3.one * 1.3f, 0.5f).SetEase(Ease.InOutCubic);

        var timer = Timer.CreateCountDown(0.3f, true);
        timer.SetId(this);
        
        timer.Stoped += () =>
        {
            SoundManager.SoundMixer.PlayOneShot(5);
            Object.Instantiate(destroyFx, sprite.transform.position, Quaternion.identity);
            var timer1 = Timer.CreateCountDown(0.1f, true);
            timer1.SetId(this);
            timer1.Stoped += () => GameObject.SetActive(false);
        };
    }

    private Tween LoopColor(Color color, int loops, TweenCallback onCompleteLoop = null)
    {
        var tween = ToColor(color);
        
        tween.onComplete += () =>
        {
            DOVirtual.Color(color, defaultColor, 0.6f, value => sprite.color = value).SetLoops(loops, LoopType.Yoyo).SetId(this).OnComplete(onCompleteLoop);
        };

        return tween;
    }
    
    private Tween ToColor(in Color color)
    {
        DOTween.Kill(this);
        return sprite.DOColor(color, 0.6f).SetId(this);
    }

    public void OnDestoy()
    {
        TimerExtensions.Kill(this);
        DOTween.Kill(this);
    }
}
