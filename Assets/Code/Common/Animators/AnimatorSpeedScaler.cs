using UnityEngine;
using System.Collections;
namespace Common.Animators
{
    public class AnimatorSpeedScaler : MonoBehaviour
    {
        #region static function
        public static void ScaleSpeed(Animator anim, float delay, float speed)
        {
            if (!anim) return;
            GameObject go = new GameObject(anim.name + "_" + typeof(AnimatorSpeedScaler).Name);
            AnimatorSpeedScaler ass = go.AddComponent<AnimatorSpeedScaler>();
            ass.Scale(anim, delay, speed);
        }
        #endregion

        private float _delay;
        private float _time;
        private float _originalSpeed;
        private Animator _anim;
        private float _currentTime;

        void Update()
        {
            if (!_anim)
            {
                UnityEngine.Object.Destroy(gameObject);
                return;
            }
            if (Time.realtimeSinceStartup - _time >= _delay)
            {
                _anim.speed = _originalSpeed;
                _anim = null;
                UnityEngine.Object.Destroy(gameObject);
                return;
            }
            if (Common.GameTime.Controller.TimeController.instance.playerPause)
                _time += (Time.realtimeSinceStartup - _currentTime);
            _currentTime = Time.realtimeSinceStartup;
        }

        public void Scale(Animator anim, float delay, float speed)
        {
            _delay = delay;
            _anim = anim;
            _originalSpeed = anim.speed;
            anim.speed = speed;
            _time = Time.realtimeSinceStartup;
            _currentTime = _time;
        }
    }
}