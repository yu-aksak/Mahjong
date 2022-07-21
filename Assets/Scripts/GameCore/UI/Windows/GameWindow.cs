using System;
using Core.StoreModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : Window<GameWindow>
{
    public static event Action HintButtonClicked;
    [SerializeField] private TextMeshProUGUI gameTimeText;
    [SerializeField] private Image gameTimerImage;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private Button bombButton;
    [SerializeField] private Button shuffleButton;
    [SerializeField] private Button undoButton;
    [SerializeField] private TileMap tileMapPrefab;
    [SerializeField] private ScoreMultiplier scoreMultiplier;
    private TileMap tileMap;
    private int score;
    private int place;
    private DateTime gameTime;

    private DateTime GameTime
    {
        get
        {
            return gameTime;
        }
        set
        {
            gameTime = value;
            gameTimeText.text = gameTime.ToString("mm:ss");
        }
    }

    public static int Score
    {
        get
        {
            return Instance.score;
        }
        private set
        {
            Instance.score = value;
            Instance.scoreText.text = $"Score: {value}";
        }
    }
    public static int Place => Instance.place;

    protected override void OnShow()
    {
        base.OnShow();
        StartGame();
        Advertiser.HideBanner();
    }

    protected override void OnHide()
    {
        base.OnHide();
        EndGame();
        Advertiser.ShowBanner();
    }

    private void StartGame()
    {
        tileMap = Instantiate(tileMapPrefab);
        tileMap.Matched += IncreaseScore;
        tileMap.Matched += scoreMultiplier.Increase;
        tileMap.Won += WinWindow.Show;
        tileMap.Lost += LoseWindow.Show;
        tileMap.OutOfMoves += OutOfMovesWindow.Show;
        
        pauseButton.onClick.AddListener(PauseWindow.Show);

        hintButton.onClick.AddListener(OnHintButtonClicked);
        bombButton.onClick.AddListener(tileMap.Bomb);
        shuffleButton.onClick.AddListener(OnShuffleButtonClicked);
        undoButton.onClick.AddListener(tileMap.Undo);
        
        scoreMultiplier.Init();
        Score = 0;
        gameTime = new DateTime();

        var countUpTimer = Timer.CreateCountUp(1, true);
        countUpTimer.Updated += value => GameTime = new DateTime().AddSeconds(Mathf.Lerp(0, tileMap.GameTimer.Interval, value));
        countUpTimer.Updated += UpdateGameTimeImage;
        countUpTimer.Stoped += () => GameTime = new DateTime().AddSeconds(tileMap.GameTimer.Interval);
        
        tileMap.GameTimer.NormalizeUpdated += UpdateGameTimeImage;
        tileMap.GameTimer.IntUpdated += CountDownTime;
    }
    
    private void EndGame()
    {
        BestScoresConfig.Config.TryAdd(ItemSelector.GetSelected(ItemType.Mahjong), score, out place);
        
        tileMap.Matched -= IncreaseScore;
        tileMap.Matched -= scoreMultiplier.Increase;
        tileMap.Won -= WinWindow.Show;
        tileMap.Lost -= LoseWindow.Show;
        tileMap.OutOfMoves -= OutOfMovesWindow.Show;
        tileMap.GameTimer.NormalizeUpdated -= UpdateGameTimeImage;
        tileMap.GameTimer.IntUpdated -= CountDownTime;

        hintButton.onClick.RemoveAllListeners();
        bombButton.onClick.RemoveAllListeners();
        shuffleButton.onClick.RemoveAllListeners();
        undoButton.onClick.RemoveAllListeners();

        scoreMultiplier.Reset();
        
        tileMap.Finish();
    }

    public static void Shuffle()
    {
        Instance.tileMap.Shuffle();
    }
    
    public static void Restart()
    {
        Instance.EndGame();
        Show();
    }

    public static void SetActiveHints(bool active)
    {
        var instance = Instance;
        
        instance.hintButton.interactable = active;
        instance.bombButton.interactable = active;
        instance.shuffleButton.interactable = active;
        instance.undoButton.interactable = active;
    }

    public static void UpdateTilesView()
    {
        Instance.tileMap.UpdateTilesView();
    }

    private void IncreaseScore()
    {
        Score += 10 * scoreMultiplier.Multiplier;
    }

    private static void OnHintButtonClicked()
    {
        HintButtonClicked?.Invoke();
    }

    private void OnShuffleButtonClicked()
    {
        SetActiveHints(false);
        tileMap.ShuffleAnimationEnded += SetActiveShuffleButton;
        tileMap.Shuffle();
    }
    
    private void SetActiveShuffleButton()
    {
        SetActiveHints(true);
        tileMap.ShuffleAnimationEnded -= SetActiveShuffleButton;
    }

    private void UpdateGameTimeImage(float value)
    {
        gameTimerImage.fillAmount = value;
    }
    
    private void CountDownTime(int _)
    {
        GameTime = GameTime.Subtract(TimeSpan.FromSeconds(1));
    }
}
