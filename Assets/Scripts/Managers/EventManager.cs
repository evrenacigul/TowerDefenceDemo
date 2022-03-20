using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generator;

namespace Managers
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager instance { get; private set; }
        public delegate void EventBasic();
        public delegate void EventTemplate<T>(T value);
        public delegate void EventKill<T1, T2>(T1 score, T2 gold);
        public delegate void EventWave<T1, T2>(T1 wave, T2 seconds);
        public delegate void EventTemplateDelayed<T1, T2>(T1 value, T2 delaySeconds);

        public EventTemplate<GameState> onGameStateChange;
        public EventTemplate<WaveState> onWaveStateChange;
        public EventTemplate<int> onBuildingDone;

        public EventKill<int, int> onEnemyKilled;

        public EventWave<int, int> onWaveUIUpdate;

        public EventTemplate<int> onBaseGetHit;

        public EventBasic onUIUpdate;
        public EventBasic onCreaturesDefeated;

        void Awake() { instance = this; }
    }
}