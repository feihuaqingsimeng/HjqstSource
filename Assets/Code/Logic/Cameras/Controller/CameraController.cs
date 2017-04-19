using UnityEngine;
using System.Collections;
using Common.Cameras;
using System;
using Common.Components.Trans;
using System.Collections.Generic;
using Common.GameTime.Controller;
using Logic.Character;
using Logic.Position.Model;
using Logic.Enums;
namespace Logic.Cameras.Controller
{
    public class CameraController : SingletonMono<CameraController>
    {
        public Camera mainCamera;
        public Camera closeupCamera;
        public Camera uiCamera;
        public Camera sceneCamera;
        public Camera sceneFrontCamera;
        public CameraEvent mainCameraEvent;
        private Character.CharacterEntity _closeupCameraCharacter;
        public float moveTime = 0.1f;
        public float nearTime = 0.1f;
        public bool shake;
        public const string CAMERA_NODE = "Effect_camera_pre";
        private Vector3 _originalMainCameraPos;
        private int _cullMask;
        private float _offset = 2.5f;
        public float lerp = 0.2f;
        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //mainCamera = Camera.main;
            _originalMainCameraPos = mainCamera.transform.position;
            _cullMask = mainCamera.cullingMask;
            //ShowCloseupCamera(false, false, null);
            ResetCloseupCamera(null);
            fighting = false;
        }

#if UNITY_EDITOR
        //void Update()
        //{
        //    if (shake)
        //    {
        //        shake = false;
        //        Shake();
        //    }
        //}

        //Vector3 _target;
        //Vector3 _lerpTarget;

        //void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawSphere(_target, 2f);
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawLine(_target, _originalPos);
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawSphere(_lerpTarget, 2f);
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawLine(_target, _lerpTarget);
        //}
#endif

        public void Reset()
        {
            mainCamera.transform.position = _originalMainCameraPos;
            mainCamera.cullingMask = _cullMask;
        }

        public void Shake()
        {
            shake = false;
            mainCamera.GetComponent<CameraShake>().Shake();
            sceneCamera.GetComponent<CameraShake>().Shake();
            //uiCamera.GetComponent<CameraShake>().Shake();
        }

        public void MainCameraComboShowTime(Vector3 target, Vector3 offset)
        {
            Vector3 target1 = new Vector3(target.x, _originalMainCameraPos.y, _originalMainCameraPos.z) + offset;
            Vector3 end = Vector3.Lerp(target1, target, lerp);
#if UNITY_EDITOR
            //_target = target;
            //_lerpTarget = end;
#endif
            LTDescr ltDescr = LeanTween.move(mainCamera.gameObject, end, moveTime);
            ltDescr.tweenType = LeanTweenType.easeInOutSine;
            ltDescr.setIgnoreTimeScale(true);
        }

        private IEnumerator ResetMainCameraPosCoroutine(Vector3 target, float costTime)
        {
            LTDescr ltDescr = LeanTween.move(mainCamera.gameObject, target, costTime);
            ltDescr.tweenType = LeanTweenType.easeInOutSine;
            ltDescr.setIgnoreTimeScale(true);
            float time = Time.realtimeSinceStartup;
            float currentTime = time;
            while (Time.realtimeSinceStartup - time < moveTime)
            {
                yield return null;
                if (TimeController.instance.playerPause)
                    time += (Time.realtimeSinceStartup - currentTime);
                currentTime = Time.realtimeSinceStartup;
            }
            mainCamera.transform.position = _originalMainCameraPos;
        }

        private bool _fighting = false;
        public bool fighting
        {
            set
            {
                _fighting = value;
                mainCamera.gameObject.SetActive(value);
                sceneCamera.gameObject.SetActive(value);
                sceneFrontCamera.gameObject.SetActive(value);
            }
            get
            {
                return _fighting;
            }
        }

        public void SetMainCameraCullMask(int mask)
        {
            mainCamera.cullingMask = mask;
        }

        public void ResetMainCameraPos()
        {
            StartCoroutine(ResetMainCameraPosCoroutine(_originalMainCameraPos, moveTime));
        }

        public void ResetMainCameraMask()
        {
            mainCamera.cullingMask = _cullMask;
        }

        public void ShowMainCamera(bool show)
        {
            if (show)
                ResetMainCameraMask();
            else
                mainCamera.cullingMask = 0;
        }

        #region
        public void CloseTarget(CharacterEntity target, float delay)
        {
            StartCoroutine(CloseTargetCoroutine(target, delay));
        }

        private IEnumerator CloseTargetCoroutine(CharacterEntity target, float delay)
        {
            if (delay > 0)
            {
                delay /= Game.GameSetting.instance.speed;
                float time = Time.realtimeSinceStartup;
                float currentTime = time;
                while (Time.realtimeSinceStartup - time < delay)
                {
                    yield return null;
                    if (TimeController.instance.playerPause)
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
            }
            Vector3 offset = Vector3.zero;
            Vector3 centerPos = Vector3.zero;
            if (target.isPlayer)
                centerPos = PositionData.GetPos((uint)FormationPosition.Player_Position_2);
            else
                centerPos = PositionData.GetPos((uint)FormationPosition.Enemy_Position_2);
            offset.z = target.pos.z - centerPos.z;
            offset.z += _offset;
            Vector3 target1 = new Vector3(target.pos.x, _originalMainCameraPos.y, _originalMainCameraPos.z) + offset;
            Vector3 end = Vector3.Lerp(target1, target.pos, lerp);
            LTDescr ltDescr = LeanTween.move(mainCamera.gameObject, end, 0.3f);
            ltDescr.tweenType = LeanTweenType.easeInOutSine;
            ltDescr.setIgnoreTimeScale(true);
        }

        public void ResetMainCamera(float delay)
        {
            StartCoroutine(ResetMainCameraCoroutine(delay));
        }

        private IEnumerator ResetMainCameraCoroutine(float delay)
        {
            if (delay > 0)
            {
                delay /= Game.GameSetting.instance.speed;
                float time = Time.realtimeSinceStartup;
                float currentTime = time;
                while (Time.realtimeSinceStartup - time < delay)
                {
                    yield return null;
                    if (TimeController.instance.playerPause)
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
            }
            StartCoroutine(ResetMainCameraPosCoroutine(_originalMainCameraPos, moveTime));
        }

        #endregion

        #region closeup camera
        public void ShowCloseupCamera(Character.CharacterEntity character, Transform parent)
        {
            _closeupCameraCharacter = character;
            closeupCamera.gameObject.SetActive(true);
            if (parent)
                closeupCamera.transform.SetParent(parent, false);
            closeupCamera.transform.localScale = Vector3.one;
            closeupCamera.transform.localPosition = Vector3.zero;
            Quaternion qua = Quaternion.Euler(Vector3.up * 180);
            if (!character.isPlayer)//先注掉
            {
                qua = Quaternion.Euler(Vector3.forward * 180);
            }
            closeupCamera.transform.localRotation = qua;
        }

        public bool ResetCloseupCamera(Character.CharacterEntity character)
        {
            if (character && _closeupCameraCharacter && character != _closeupCameraCharacter) return false;
            _closeupCameraCharacter = null;
            closeupCamera.gameObject.SetActive(false);
            closeupCamera.transform.SetParent(transform, false);
            return true;
        }
        #endregion
    }
}