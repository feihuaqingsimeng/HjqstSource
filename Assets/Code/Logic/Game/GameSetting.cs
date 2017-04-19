using UnityEngine;
using System.Collections;
namespace Logic.Game
{
    public class GameSetting : SingletonMono<GameSetting>
    {
        #region static field
        public const string PUSH_MSG = "pushMsg";
        public const string EFFECT_PLAYABLE = "effectPlayable";
        #endregion
        public float deltaTimeFight { get { return _deltaTimeFight; } }
        private float _deltaTimeFight;
        public float deltaTimeUI { get { return _deltaTimeUI; } }
        private float _deltaTimeUI;

        public float slowSpeed = 0.05f;

        private bool _closeupCameraable = true;
        public bool closeupCameraable
        {
            get
            {
                return false;//屏蔽镜头
                return _closeupCameraable;
            }
        }

        private bool _effectable = true;
        public bool effectable
        {
            get { return _effectable; }
        }

        private bool _effectPlayable;
        public bool effectPlayable
        {
            get { return _effectPlayable; }
            set
            {
                Debugger.Log("effectable:{0}", value);
                _effectPlayable = value;
                _effectable = value;
                _closeupCameraable = value;
                PlayerPrefs.SetInt(EFFECT_PLAYABLE, _effectPlayable ? 1 : 0);
            }
        }

        private bool _pushMessage = true;
        public bool pushMessage
        {
            get { return _pushMessage; }
            set
            {
                _pushMessage = value;
                Game.Controller.GameController.instance.CleanLocalNotificator();
                if (value)
                    Game.Controller.GameController.instance.LocalNotificator();
                PlayerPrefs.SetInt(PUSH_MSG, _pushMessage ? 1 : 0);
            }
        }

        private GameFrameType _frameType;
        public GameFrameType frameType
        {
            set
            {
                if (value == GameFrameType.None || value == _frameType)
                    return;
                _frameType = value;
                if (_frameType == GameFrameType.Fight)
                    Application.targetFrameRate = 60;
                else
                    Application.targetFrameRate = 30;
            }
            get
            {
                return _frameType;
            }
        }

        private GameSpeedMode _lastFightSpeedMode = GameSpeedMode.Normal;
        public GameSpeedMode lastFightSpeedMode
        {
            get
            {
                return _lastFightSpeedMode;
            }
            set
            {
                _lastFightSpeedMode = value;
            }
        }

        private GameSpeedMode _speedMode;
        public GameSpeedMode speedMode
        {
            get
            {
                return _speedMode;
            }
            set
            {
                _speedMode = value;
                switch (value)
                {
                    case GameSpeedMode.Normal:
                        _speed = 1;
                        Time.timeScale = 1f;
                        PlayerPrefs.SetInt("speedMode", (int)GameSpeedMode.Normal);
                        break;
                    case GameSpeedMode.Double:
                        _speed = 1.5f;
                        Time.timeScale = 1.5f;
                        PlayerPrefs.SetInt("speedMode", (int)GameSpeedMode.Double);
                        break;
                    case GameSpeedMode.Triple:
                        _speed = 2f;
                        Time.timeScale = 2f;
                        PlayerPrefs.SetInt("speedMode", (int)GameSpeedMode.Triple);
                        break;
                    case GameSpeedMode.ComboWaiting:
                        _speed = slowSpeed;
                        Time.timeScale = slowSpeed;
                        break;
                }
            }
        }

        private float _speed;
        public float speed
        {
            get
            {
                return _speed;
            }
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            _deltaTimeFight = 1f / 60f;
            _deltaTimeUI = 1f / 30f;
            effectPlayable = PlayerPrefs.GetInt(EFFECT_PLAYABLE, 1) == 1;
            //_speedMode = (GameSpeedMode)PlayerPrefs.GetInt("speedMode", 1);
            //_lastFightSpeedMode = _speedMode;
            _speedMode = GameSpeedMode.Normal; 
            _speed = 1;
            Time.timeScale = 1f;
            _lastFightSpeedMode = _speedMode;
            Debugger.Log("FrameRate:" + Application.targetFrameRate + "   deltaTime60:" + _deltaTimeFight + "     deltaTime30:" + _deltaTimeUI);
        }

    }
    public enum GameFrameType
    {
        None = 0,
        UI = 1,
        Fight = 2
    }

    public enum GameSpeedMode
    {
        Normal = 1,
        Double = 2,
        Triple = 3,
        ComboWaiting = 4,
    }
}