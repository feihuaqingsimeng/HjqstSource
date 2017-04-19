using UnityEngine;
using System.Collections;
using Logic.Game.Controller;
using Logic.Game;
using Logic.Character;
using Common.GameTime.Controller;
namespace Common.TimeScale
{
    public class TimeScaler : SingletonMonoNewGO<TimeScaler>
    {
        private float _time;
        private float _delay;
        private bool _isNormalWork = false;
        private bool _isAnimatorWork = false;
        private bool _pause = false;
        private float _speed;
        private Animator _anim;
        private GameSpeedMode _speedMode;
        private float _currentTime;
        void Awake()
        {
            _instance = this;
        }

        void Update()
        {
            if (_isNormalWork && Time.realtimeSinceStartup - _time >= _delay)
            {
                if (_pause)
                    //GameController.instance.pause = false;
                    Common.GameTime.Controller.TimeController.instance.CancelPause();
                else
                    GameSetting.instance.speedMode = _speedMode;
                _isNormalWork = false;
                _pause = false;
                gameObject.SetActive(false);
            }
            if (_isAnimatorWork && Time.realtimeSinceStartup - _time >= _delay)
            {
                if (_pause)
                    //GameController.instance.pause = false;
                    Common.GameTime.Controller.TimeController.instance.CancelPause();
                else
                    GameSetting.instance.speedMode = _speedMode;
                _anim.updateMode = AnimatorUpdateMode.Normal;
                _anim.speed = _speed;
                _anim = null;
                _isAnimatorWork = false;
                _pause = false;
                gameObject.SetActive(false);
            }
            if (TimeController.instance.playerPause)
                _time += (Time.realtimeSinceStartup - _currentTime);
            _currentTime = Time.realtimeSinceStartup;
        }

        public void TimeScaleTrigger(float delay, GameSpeedMode speedMode, Animator anim = null)
        {
            gameObject.SetActive(true);
            if (_isNormalWork) return;
            _isNormalWork = true;
            this._delay = delay;
            _speedMode = GameSetting.instance.speedMode;
            _time = Time.realtimeSinceStartup;
            GameSetting.instance.speedMode = speedMode;
        }

        public void PauseTrigger(CharacterEntity character, float delay, GameScaleMode scaleMode, float speed, Animator anim = null)
        {
            gameObject.SetActive(true);
            if (_pause) return;
            _pause = true;
            if (scaleMode == GameScaleMode.Normal)
                _isNormalWork = true;
            else if (scaleMode == GameScaleMode.Animator)
            {
                _isAnimatorWork = true;
                _anim = anim;
                _speed = anim.speed;
                anim.speed = speed;
                _anim.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            this._delay = delay;
            _time = Time.realtimeSinceStartup;
            _currentTime = _time;
            //GameController.instance.pause = true;
            Common.GameTime.Controller.TimeController.instance.Pause(character);
        }
    }

    public enum GameScaleMode
    {
        None = 0,
        Normal = 1,
        Animator = 2,
    }
}
