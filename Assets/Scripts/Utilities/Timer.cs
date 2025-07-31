using System;

namespace WaveSystem.Utilities
{
    public class Timer
    {
        private float _time;
        private float _duration;

        private bool _isRunning;


        public event Action OnTimerStarted = delegate { };
        public event Action<float> OnTimeUpdated = delegate { };
        public event Action OnTimerPaused = delegate { };
        public event Action OnTimerCompleted = delegate { };

        public float Time => _time;
        public float Duration => _duration;
        public bool IsRunning => _isRunning;

        public Timer(float duration)
        {
            _duration = duration;
            _time = 0f;
        }

        public void Update(float deltaTime)
        {
            if (!_isRunning) return;

            if (_time < _duration)
            {
                _time += deltaTime;
                OnTimeUpdated?.Invoke(_time);
            }
            else
            {
                _isRunning = false;
                _time = 0;
    
                OnTimerCompleted?.Invoke();
            }

        }

        public void SetDuration(float duration)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Cannot set duration while the timer is running.");
            }

            _duration = duration;

            if (_time > _duration)
            {
                _time = _duration;
            }
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _time = 0f;
            OnTimerStarted?.Invoke();
        }

        public void Pause()
        {
            if (!_isRunning) return;

            _isRunning = false;
            OnTimerPaused?.Invoke();
        }

        public void Reset()
        {
            _time = 0f;
            _isRunning = false;
            OnTimerCompleted?.Invoke();
        }
    }
}

