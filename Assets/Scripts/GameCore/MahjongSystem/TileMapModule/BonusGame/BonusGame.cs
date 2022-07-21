using System;
using Core.SoundModule;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BonusGame : ITileSelector
{
    [SerializeField] private BonusGameView view;
    [SerializeField] private float duration = 5;
    private int currentMatchNum;
    private int randomMatchNum;
    private bool isStarted;
    private Color messageColor = new Color(1f, 0.84f, 0f);
    private Timer.BaseTimer preStartTimer;
    private Timer.BaseTimer timer;

    public ITileSelector Selector { get; set; }
    private static int RandomMatchNum => Random.Range(3, 6);


    public void Init(ITileSelector tileSelector)
    {
        Selector = tileSelector;
        randomMatchNum = RandomMatchNum;
        view.Init();
    }

    public void TryStart()
    {
        if (isStarted == false)
        {
            currentMatchNum++;
            
            if (currentMatchNum == randomMatchNum)
            {
                currentMatchNum = 0;
                randomMatchNum = RandomMatchNum;
            
                if (Selector.TryGetTwoSameRandomTiles(out var tiles))
                {
                    var tile = tiles.Item1;
                    PreStart(tile);
                    
                    preStartTimer = Timer.CreateCountDown(duration, true);
                    preStartTimer.Stoped += () => OnPreStartTimeOut(tile);
                }
            }
        }
    }

    private void PreStart(Tile tile)
    {
        tile.view.OnBonus();
        tile.Removed += StartMessage;
        GameWindow.SetActiveHints(false);
        isStarted = true;
    }

    private void OnPreStartTimeOut(Tile tile)
    {
        isStarted = false;
        tile.view.SetBySelectionState(tile.IsSelected);
        tile.Removed -= StartMessage;
        GameWindow.SetActiveHints(true);
    }

    private void StartMessage()
    {
        MessageWindow.Show("<bounce>BONUS GAME!!!</bounce>", messageColor, 2, 100);
        view.PlayMessageFX();
        SoundManager.MusicMixer.PlayWithClipDuration(7);
        SoundManager.MusicMixer.PlayWithDuration(8, 0.6f, duration, 1);
        MessageWindow.Hided += Start;
    }

    private void Start()
    {
        preStartTimer.Kill();
        timer = Timer.CreateCountDown(duration, true);
        timer.Stoped += Stop;
        
        view.Start(timer);
        Selector.Selector = this;
    }

    public void TouchTile(in Tile tile)
    {
        if (tile.IsBlocked == false)
        {
            if (Selector.TryGetSameTile(tile, out var sameTile))
            {
                Selector.TouchTile(tile);
                Selector.TouchTile(sameTile);
            }
            else
            {
                tile.view.WrongSelection();
            }
        }
    }

    public bool TryGetSameTile(Tile tile, out Tile sameTile)
    {
        return Selector.TryGetSameTile(tile, out sameTile);
    }

    public bool TryGetTwoSameRandomTiles(out (Tile, Tile) tiles)
    {
        return Selector.TryGetTwoSameRandomTiles(out tiles);
    }

    private void Stop()
    {
        GameWindow.SetActiveHints(true);
        MessageWindow.Hided -= Start;
        view.Stop();
        timer.Kill();
        Selector.Selector = Selector;
        isStarted = false;
    }

    public void Destroy()
    {
        if (view.IsStarted)
        {
            Stop();
            view.Destroy();
        }
    }
}
