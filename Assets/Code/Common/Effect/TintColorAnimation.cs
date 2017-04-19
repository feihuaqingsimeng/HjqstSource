using UnityEngine;

namespace Common.Effect
{
    public class TintColorAnimation : MonoBehaviour
    {
        private Material _material;

        public float delay;
        public float time;
        public bool destroyOnComplete = false;
        public Color colorFrom;
        public Color colorTo;
        public int repeatTimes;

        void Awake()
        {
            _material = GetComponent<Renderer>().material;
        }

        void Start()
        {
            LTDescr ltDescr = LeanTween.value(gameObject, colorFrom, colorTo, time);
            ltDescr.setDelay(delay);
            ltDescr.loopType = LeanTweenType.pingPong;
            ltDescr.setOnUpdateColor(OnUpdateColor);
            ltDescr.setDestroyOnComplete(destroyOnComplete);
            ltDescr.setRepeat(repeatTimes);
        }

        private void OnUpdateColor(Color color)
        {
            if (_material != null)
            {
                _material.SetColor("_TintColor", color);
            }
        }
    }
}