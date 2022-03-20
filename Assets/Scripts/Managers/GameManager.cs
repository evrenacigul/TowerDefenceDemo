using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generator;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }
        public GameState gameState { get; private set; }

        [SerializeField] private int _gold;
        [SerializeField] private int _score;
        [SerializeField] private int _baseHealth = 100;

        public int Gold { get { return _gold; } }
        public int ScorePoints { get { return _score; } }
        public int BaseHealth { get { return _baseHealth; } }

        public Vector3 SpawnPoint { get; private set; }
        public List<Vector3> PathPoints { get; private set; }

        private void Awake() { instance = this; }

        void Start()
        {
            SetGameState(GameState.WaitToStart);

            Vector2Int spawn = MapGenerator.instance.GetSpawnPoint;
            SpawnPoint = new Vector3(spawn.x, 0, spawn.y);

            if (PlayerPrefs.HasKey("Score"))
                _score = PlayerPrefs.GetInt("Score");
            else
            {
                _score = 0;
                PlayerPrefs.SetInt("Score", _score);
            }

            EventManager.instance.onBuildingDone += OnBuildingDone;
            EventManager.instance.onEnemyKilled += OnEnemyKilled;
            EventManager.instance.onBaseGetHit += OnBaseGetHit;
            EventManager.instance.onWaveStateChange += OnWaveStateChange;
        }

        public void SetGameState(GameState state)
        {
            switch(state)
            {
                case GameState.WaitToStart:
                    PathPoints = MapGenerator.instance.Path;
                    break;
                case GameState.Play:
                    EventManager.instance.onUIUpdate?.Invoke();
                    Time.timeScale = 1f;
                    break;
                case GameState.Pause:
                    Time.timeScale = 0.00001f;
                    break;
                case GameState.Failure:
                    _score = 0;
                    break;
                case GameState.Win:
                    int score = PlayerPrefs.GetInt("Score");
                    PlayerPrefs.SetInt("Score", _score + score);
                    break;
            }
            gameState = state;
            EventManager.instance.onGameStateChange?.Invoke(state);
        }

        void OnEnemyKilled(int gold, int score)
        {
            _gold += gold;
            _score += score;
            EventManager.instance.onUIUpdate?.Invoke();
        }

        void OnBuildingDone(int gold)
        {
            _gold -= gold;
            EventManager.instance.onUIUpdate?.Invoke();
        }

        void OnBaseGetHit(int damage)
        {
            _baseHealth -= damage;
            if (_baseHealth <= 0)
                SetGameState(GameState.Failure);

            EventManager.instance.onUIUpdate?.Invoke();
        }

        void OnWaveStateChange(WaveState state)
        {
            if(state == WaveState.CreaturesDefeated)
            {
                SetGameState(GameState.Win);
            }
        }
    }

    public enum GameState
    {
        WaitToStart,
        Play,
        Pause,
        Failure,
        Win
    }
}