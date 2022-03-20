using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Generator.Waves;
using Utility;

namespace Generator
{
    public class WaveGenerator : MonoBehaviour
    {
        public static WaveGenerator instance { get; private set; }

        [SerializeField] Wave _waveData;
        [SerializeField] Wave _waveDataInstance;

        [SerializeField] int _waveStep;
        int _creatureAlive;

        [SerializeField] TimeCounter _waveDelayCounter;
        [SerializeField] TimeCounter _spawnDelayCounter;

        bool _waveOnGoing;

        [SerializeField] WaveState _waveState;
        public int creatureCount { get { return _creatureAlive; } }

        private void Awake() { instance = this; }


        void Start()
        {
            _waveStep = 0;
            _creatureAlive = 0;
            _waveDataInstance = _waveData.Clone();

            EventManager.instance.onGameStateChange += OnGameStateChanged;
            EventManager.instance.onEnemyKilled += OnEnemyKilled;
            EventManager.instance.onBaseGetHit += OnBaseGetHit;
        }

        void Update()
        {
            if(_waveOnGoing)
                WaveUpdate();
        }

        void OnEnemyKilled(int score, int gold)
        {
            _creatureAlive--;
            CheckIfCreaturesDefeated();
        }

        void OnBaseGetHit(int hp)
        {
            _creatureAlive--;
            CheckIfCreaturesDefeated();
        }

        void WaveUpdate()
        {
            switch (_waveState)
            {
                case WaveState.CountingDown:
                    if(_waveDelayCounter.IsTimeUp())
                    {
                        WaveStateChange(WaveState.WaveOnGoing);
                    }
                    EventManager.instance.onWaveUIUpdate?.Invoke(_waveStep + 1,
                            (int)(_waveDelayCounter.getDuration - _waveDelayCounter.delayedTime));
                    break;
                case WaveState.WaveOnGoing:
                    if (_spawnDelayCounter.IsTimeUp())
                    {
                        if (!SpawnCreature(_waveDataInstance.waves[_waveStep].waveCreatures))
                            WaveStateChange(WaveState.NextWave);
                    }
                    break;
                case WaveState.NextWave:
                    NextWave();
                    break;
                case WaveState.Finished:
                    _waveStep = 0;
                    break;
                case WaveState.CreaturesDefeated:
                    EventManager.instance.onCreaturesDefeated?.Invoke();
                    _waveOnGoing = false;
                    break;
            }
        }

        void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Play:
                    StartWaves();
                    break;
                case GameState.Win:
                    StopWave();
                    break;
                case GameState.Failure:
                    StopWave();
                    break;
            }
        }

        void StartWaves()
        {
            if (_waveData == null)
                return;
            if (_waveData.waves == null)
                return;
            if (_waveData.waves.Count <= _waveStep)
            {
                StopWave();
                return;
            }
            _waveDelayCounter = new TimeCounter(_waveData.waves[_waveStep].timeToWaveBegins, false);
            _spawnDelayCounter = new TimeCounter(_waveData.waves[_waveStep].timeBetweenSpawns, true);
            _waveOnGoing = true;
            WaveStateChange(WaveState.CountingDown);
        }

        void NextWave()
        {
            _waveStep++;
            StartWaves();
        }

        void StopWave()
        {
            WaveStateChange(WaveState.Finished);
        }

        void WaveStateChange(WaveState state)
        {
            _waveState = state;
            EventManager.instance.onWaveStateChange?.Invoke(_waveState);
        }

        void CheckIfCreaturesDefeated()
        {
            if(_waveState == WaveState.Finished && _creatureAlive <= 0)
                WaveStateChange(WaveState.CreaturesDefeated);
        }

        bool SpawnCreature(List<CreatureInfo> creatures)
        {
            foreach(CreatureInfo creatureInfo in creatures)
            {
                if (creatureInfo.count <= 0)
                {
                    creatures.Remove(creatureInfo);
                    break;
                }
            }

            if (creatures == null)
                return false;

            if (creatures.Count <= 0)
                return false;

            int selectRand = Random.Range(0, creatures.Count);

            if (creatures[selectRand].count > 0)
            {
                Instantiate(creatures[selectRand].prefab, GameManager.instance.SpawnPoint, Quaternion.identity);
                CreatureInfo creatureInfo = creatures[selectRand];
                creatureInfo.count--;
                creatures[selectRand] = creatureInfo;
                _creatureAlive++;
            }

            return true;
        }
    }

    public enum WaveState
    {
        CountingDown,
        WaveOnGoing,
        NextWave,
        Finished,
        CreaturesDefeated
    }
}