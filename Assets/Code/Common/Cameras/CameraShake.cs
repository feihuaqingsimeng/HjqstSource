using UnityEngine;
using System.Collections;
namespace Common.Cameras
{
    public class CameraShake : MonoBehaviour
    {
        public float shakeTime = 0.2f;//振动时长
        private float _shakeTime = 0f;
        public float fps = 40.0f;
        private float frameTime = 0.0f;
        public float shakeDelta = 1f;//振动频率
        public Camera cam;
        public float shakeAmplitude = 0.03f;//振幅
        private float _shakeAmplitude = 0f;
        public float shakeFactor = 0.005f;//衰减系数
        private float _shakeFactor = 0f;
        public float shakeAmplitudeY = 0.015f;//振幅
        private float _shakeAmplitudeY = 0f;
        public float shakeFactorY = 0.003f;//衰减系数
        private float _shakeFactorY = 0f;
        public bool shake = false;
        private Rect rect;

        private Vector3 _startShakeLocalPosition;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            cam = GetComponent<Camera>();
            _startShakeLocalPosition = cam.transform.localPosition;
            _shakeTime = shakeTime;
            _shakeFactor = shakeFactor;
            _shakeAmplitude = shakeAmplitude;
            _shakeFactorY = shakeFactorY;
            _shakeAmplitudeY = shakeAmplitudeY;
            frameTime = 1 / fps;
            rect = cam.rect;
            //Debugger.Log (rect);
        }

        void Update()
        {
            if (shake)
            {
                if (_shakeTime > 0)
                {
                    _shakeTime -= Time.unscaledDeltaTime;
                    if (_shakeTime <= 0)
                    {
                        cam.rect = rect;
                        shake = false;
                        _shakeTime = shakeTime;
                        _shakeFactor = shakeFactor;
                        _shakeAmplitude = shakeAmplitude;
                        _shakeFactorY = shakeFactorY;
                        _shakeAmplitudeY = shakeAmplitudeY;
                        cam.transform.localPosition = _startShakeLocalPosition;
                    }
                    else
                    {
                        frameTime += Time.unscaledDeltaTime;
                        if (frameTime > 1.0 / fps)
                        {
                            frameTime = 0;
                            //cam.rect = new Rect(shakeDelta * (-1.0f + 2.0f * Random.value), shakeDelta * (-1.0f + 2.0f * Random.value), 1.0f, 1.0f);
                            _shakeAmplitude *= -1;
                            _shakeFactor *= -1;
                            _shakeAmplitude -= _shakeFactor;
                            _shakeAmplitudeY *= -1;
                            _shakeFactorY *= -1;
                            _shakeAmplitudeY -= _shakeFactorY;
                            //                            cam.rect = new Rect(shakeDelta * _shakeAmplitude, rect.y, rect.width, rect.height);

                            cam.transform.localPosition = new Vector3(_startShakeLocalPosition.x + shakeDelta * _shakeAmplitude, _startShakeLocalPosition.y + shakeDelta * _shakeAmplitudeY, _startShakeLocalPosition.z);
                        }
                    }
                }
            }
        }

        public void Shake()
        {
            shake = true;
        }
    }
}