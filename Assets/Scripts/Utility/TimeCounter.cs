using UnityEngine;

namespace Utility
{
    [System.Serializable]
    public class TimeCounter
    {
        [SerializeField] float _timeDuration;
        [SerializeField] float _timeDelayed;
        [SerializeField] bool _repeatable;
        [SerializeField] bool _counting;
        public bool isCounting { get => _counting; }
        public float delayedTime { get => _timeDelayed; }
        public float setDuration { set { _timeDuration = value; Reset(); } }
        public float getDuration { get => _timeDuration; }
        public TimeCounter(float duration, bool repeat = true)
        { _timeDuration = duration; _repeatable = repeat; _timeDelayed = 0f; _counting = true; }
        public void Reset() { _timeDelayed = 0f; }
        public void Stop() { _counting = false; Reset(); }
        public void Start() { _counting = true; }
        public void Pause() { _counting = false; }
        public void SetDelayedTime (float time) { _timeDelayed = time; }
        public bool IsTimeUp()
        {
            if (_counting)
            {
                if (_timeDelayed < _timeDuration)
                {
                    _timeDelayed += Time.deltaTime;
                    return false;
                }
                else if (_repeatable && _timeDelayed > _timeDuration)
                {
                    Reset();
                    return true;
                }
                else
                    return true;
            }
            else
                return false;
        }
    }
}