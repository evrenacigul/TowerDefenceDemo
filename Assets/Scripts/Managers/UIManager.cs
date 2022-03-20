using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance { get; private set; }

        [SerializeField] TMP_Text _pointText;
        [SerializeField] TMP_Text _goldText;
        [SerializeField] TMP_Text _baseHpText;
        [SerializeField] Button _startButton;
        [SerializeField] TMP_Text _failureText;
        [SerializeField] TMP_Text _winnerText;
        [SerializeField] GameObject _wavePanel;
        [SerializeField] TMP_Text _waveText;

        void Awake() 
        {
            instance = this; 
        }

        void Start()
        {
            EventManager.instance.onUIUpdate += OnUIUpdate;
            EventManager.instance.onGameStateChange += OnGameStateChange;
            EventManager.instance.onWaveUIUpdate += OnWaveUIUpdate;
            _startButton.onClick.AddListener(StartGame);
        }

        void StartGame()
        {
            GameManager.instance.SetGameState(GameState.Play);
        }

        void OnGameStateChange(GameState state)
        {
            switch(state)
            {
                case GameState.WaitToStart:
                    _startButton.gameObject.SetActive(true);
                    _wavePanel.gameObject.SetActive(false);
                    break;
                case GameState.Play:
                    _startButton.gameObject.SetActive(false);
                    _wavePanel.gameObject.SetActive(true);
                    break;
                case GameState.Win:
                    _winnerText.gameObject.SetActive(true);
                    break;
                case GameState.Failure:
                    _failureText.gameObject.SetActive(true);
                    break;
            }
        }

        void OnUIUpdate()
        {
            _pointText.text = "Score: " + GameManager.instance.ScorePoints;
            _goldText.text = "Gold: " + GameManager.instance.Gold;
            _baseHpText.text = "BaseHP: " + GameManager.instance.BaseHealth;
        }

        void OnWaveUIUpdate(int nextWave, int secondsLeft)
        {
            _waveText.text = "Wave: " + nextWave + "<br>" + "TimeLeft: " + secondsLeft + " sec.";
        }
    }
}