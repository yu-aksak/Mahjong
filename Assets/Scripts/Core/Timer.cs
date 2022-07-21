using System;
using System.Collections;
using System.Collections.Generic;
using Core.SingleService;
using UnityEngine;

public class Timer : SingleService<Timer>
{
    public abstract class BaseTimer
    {
        public event Action Stoped;
        public event Action<float> NormalizeUpdated;
        public event Action<float> Updated;
        public event Action<int> IntUpdated;
        public event Action Elapsed;
        public event Action Killed;
        private static Action pauseAll;
        private static Action resumeAll;
        public bool isLoop;
        
        protected float time;
        protected int intTime = 1;
        protected float interval;
        protected bool isStarted;
        protected bool isPaused;

        public float Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
            }
        }

        protected abstract bool IsIntUpdated { get; }
        protected abstract bool IsTimeOver { get; }

        protected BaseTimer(float interval, bool needStartImmediately = false, bool isLoop = false)
        {
            Interval = interval;
            this.isLoop = isLoop;

            if (needStartImmediately)
            {
                Restart();
            }
        }
        
        public void Start()
        {
            if (isStarted == false)
            {
                ResetTime();
                UnityUpdated += Update;
                pauseAll += Pause;
                resumeAll += Resume;
                isStarted = true;
            }
        }
        
        public void Restart()
        {
            if (isStarted == false)
            {
                Start();
            }
            else
            {
                ResetTime();
            }
        }

        public void Pause()
        {
            if (isStarted && isPaused == false)
            {
                UnityUpdated -= Update;
                isPaused = true;
            }
        }

        public static void PauseAll()
        {
            pauseAll?.Invoke();
        }

        public static void ResumeAll()
        {
            resumeAll?.Invoke();
        }

        public void Resume()
        {
            if (isStarted && isPaused)
            {
                UnityUpdated += Update;
                isPaused = false;
            }
        }
    
        public void Stop()
        {
            if (isStarted)
            {
                isStarted = false;
                ResetTime();
                UnityUpdated -= Update;
                Stoped?.Invoke();
                pauseAll -= Pause;
                resumeAll -= Resume;
            }
        }

        public void Kill()
        {
            UnityUpdated -= Update;
            Updated = null;
            NormalizeUpdated = null;
            IntUpdated = null;
            Stoped = null;
            pauseAll -= Pause;
            resumeAll -= Resume;
            Killed?.Invoke();
        }
        
        private void Update()
        {
            Updated?.Invoke(time);
            NormalizeUpdated?.Invoke(Mathf.InverseLerp(0, interval, time));

            UpdateTime();

            if (IsTimeOver)
            {
                OnTimeOver();
                return;
            }
            
            if (IsIntUpdated)
            {
                IntUpdated?.Invoke(intTime);
                UpdateIntTime();
            }
        }
        
        private void OnTimeOver()
        {
            if (isLoop)
            {
                ResetTime();
                Elapsed?.Invoke();
            }
            else
            {
                Stop();
            }
        }

        protected abstract void ResetTime();
        protected abstract void UpdateTime();
        protected abstract void UpdateIntTime();
    }
    
    public class CountUpTimer : BaseTimer
    {
        protected override bool IsIntUpdated => time > intTime;
        protected override bool IsTimeOver => time > Interval;
        
        protected override void ResetTime()
        {
            time = 0;
            intTime = 0;
        }

        protected override void UpdateTime()
        {
            time += Time.deltaTime;
        }
        
        protected override void UpdateIntTime()
        {
            intTime++;
        }

        public CountUpTimer(float interval, bool needStartImmediately = false, bool isLoop = false) : base(interval, needStartImmediately, isLoop)
        {
        }
    }
    
    public class CountDownTimer : BaseTimer
    {
        protected override bool IsIntUpdated => time < intTime;
        protected override bool IsTimeOver => time < 0;

        protected override void ResetTime()
        {
            time = interval;
            intTime = Mathf.CeilToInt(interval);
        }

        protected override void UpdateTime()
        {
            time -= Time.deltaTime;
        }
        
        protected override void UpdateIntTime()
        {
            intTime--;
        }

        public CountDownTimer(float interval, bool needStartImmediately = false, bool isLoop = false) : base(interval, needStartImmediately, isLoop)
        {
        }
    }

    private static event Action UnityUpdated;
    private static Func<float, bool, bool, BaseTimer> createCountDown;
    private static Func<float, bool, bool, BaseTimer> createCountUp;

    static Timer()
    {
        createCountDown = (interval, isBeginImmediately, isLoop) =>
        {
            StaticConstuctor();
            return new CountDownTimer(interval, isBeginImmediately, isLoop);
        };
        
        createCountUp = (interval, isBeginImmediately, isLoop) =>
        {
            StaticConstuctor();
            return new CountUpTimer(interval, isBeginImmediately, isLoop);
        };
    }

    private static void StaticConstuctor()
    {
        Instance = new GameObject("Timer").AddComponent<Timer>();
        createCountDown = InternalCreateCountDown;
        createCountUp = InternalCreateCountUp;
    }

    public static BaseTimer CreateCountDown(float interval, bool isBeginImmediately = false, bool isLoop = false)
    {
        return createCountDown(interval, isBeginImmediately, isLoop);
    }
    
    public static BaseTimer CreateCountUp(float interval, bool isBeginImmediately = false, bool isLoop = false)
    {
        return createCountUp(interval, isBeginImmediately, isLoop);
    }

    public static void StartCoroutinee(IEnumerator enumerator)
    { 
        Instance.StartCoroutine(enumerator);
    }
    
    private static BaseTimer InternalCreateCountDown(float interval, bool isBeginImmediately = false, bool isLoop = false)
    {
        return new CountDownTimer(interval, isBeginImmediately, isLoop);
    }
    
    private static BaseTimer InternalCreateCountUp(float interval, bool isBeginImmediately = false, bool isLoop = false)
    {
        return new CountUpTimer(interval, isBeginImmediately, isLoop);
    }

    private void Update()
    {
        UnityUpdated?.Invoke();
    }
}

public static class TimerExtensions
{
    private static readonly Dictionary<int, Timer.BaseTimer> timers = new Dictionary<int, Timer.BaseTimer>();
    public static Timer.BaseTimer SetId(this Timer.BaseTimer timer, object id)
    {
        timers[id.GetHashCode()] = timer;

        return timer;
    }

    public static void Kill(object id)
    {
        var key = id.GetHashCode();

        if (timers.TryGetValue(key, out var timer))
        {
            timer.Kill();
            timers.Remove(key);
        }
    }
}
