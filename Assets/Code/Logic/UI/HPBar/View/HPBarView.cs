using UnityEngine;
using System.Collections;
using Logic.Enums;
using Logic.Cameras.Controller;
using Logic.Character;
using UnityEngine.UI;
using Common.ResMgr;
using System.Collections.Generic;
namespace Logic.UI.HPBar.View
{
    public class HPBarView : MonoBehaviour
    {
        #region static function
        public static HPBarView CreateHPBarView(CharacterEntity character)
        {
            GameObject prefab = ResMgr.instance.Load<GameObject>(PREFAB_PATH);
            GameObject go = Instantiate(prefab) as GameObject;
            HPBarView hpBarView = go.GetComponent<HPBarView>();
            hpBarView.character = character;
            return hpBarView;
        }
        #endregion

        public const string PREFAB_PATH = "ui/hpbar/hp_bar_view";
        public CharacterEntity character
        {
            private get { return _character; }
            set
            {
                _character = value;
                HPImage.fillAmount = 1f;
                if (value == null)
                {
                    _show = false;
                    Show(false);
                }
                else if (value is HeroEntity || value is PlayerEntity)
                {
                    core.transform.localScale = new Vector3(-1, 1, 1);
                    buffImg.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
        }
        private CharacterEntity _character;
        #region ui
        public GameObject core;
        public Image HPImage;
        public Image HPBGImage;
        public Image buffImg;
        public GameObject buffGo;
        public GameObject HPGo;
        #endregion
        private bool _show = false;
        private Color32 _color = new Color(255 / 255, 56 / 255, 56 / 255, 1);
        private float _colorEndTime;
        //private float _showEndTime;

        private float _lastTime = 0f;
        private const float INTERVAL = 1f;
        private int _currentIndex = 0;
        private float _buffProgress = 0f;
        private string _lastIconPath;


        void Awake()
        {
            Show(false);
            buffGo.gameObject.SetActive(false);
        }

        public void UpdateHPValue(bool isHit)
        {
            if (character && !character.isDead)
            {
                float currentHP = character.HP;
                float MaxHP = character.maxHP;

                if (!_show)
                {
                    _show = true;
                    Show(true);
                }
                //_showEndTime = Time.time + 5f;
                if (isHit)
                    Attack();
                float hpRate = currentHP / MaxHP;
                if (hpRate < 0.05f)
                    hpRate = 0.05f;
                HPImage.fillAmount = hpRate;
            }
            else
                HPImage.fillAmount = 0f;
        }

        public void Show(bool isShow)
        {
            if (!_show && isShow) return;
            HPGo.gameObject.SetActive(isShow);            
        }

        private void Attack()
        {
            HPImage.color = _color;
            _colorEndTime = Time.time + 0.3f;
        }

        void LateUpdate()
        {
            t_UpdatePosition();
            if (Time.time >= _colorEndTime) HPImage.color = Color.white;
            if (null != character)
            {
                if (!character.isDead && character.tickCD) UpdateBuffIcon();//版本屏蔽 
            }
            else buffGo.gameObject.SetActive(false);
        }

        private void UpdateBuffIcon()
        {
            List<string> buffIcons = character.GetBuffIcons();
            if (buffIcons.Count == 0)
            {
                buffGo.gameObject.SetActive(false);
                return;
            }
            if (Time.time - _lastTime >= INTERVAL)
            {               
                int count = buffIcons.Count;
                _currentIndex = Mathf.RoundToInt(_buffProgress * count);//取上
                if (_currentIndex >= count)
                    _currentIndex = 0;
                string path = buffIcons[_currentIndex];
                if (path == _lastIconPath)
                {
                    _currentIndex++;
                    if (_currentIndex >= count)
                        _currentIndex = 0;
                    path = buffIcons[_currentIndex];
                }
                buffGo.gameObject.SetActive(true);
				buffImg.SetSprite(ResMgr.instance.Load<Sprite>("sprite/" + path));
                _lastIconPath = path;
                _buffProgress = (float)_currentIndex / (float)count;
                _lastTime = Time.time;
            }
        }

        private void t_UpdatePosition()
        {
            if (character == null)
            {
                Show(false);
                return;
            }
            Vector3 screenPos = CameraController.instance.mainCamera.WorldToScreenPoint(character.pos + Vector3.up * (character.height + 0.3f));
            Vector3 uiPos = CameraController.instance.uiCamera.ScreenToWorldPoint(screenPos);
            uiPos = uiPos / UI.UIMgr.instance.basicCanvas.transform.localScale.x;
            this.transform.localPosition = (Vector3)(Vector2)uiPos;
        }
    }
}