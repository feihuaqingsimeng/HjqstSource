using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Common.Util;
using Logic.Enums;

namespace Logic.UI.ComboBar.View
{
    public class ComboBarView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/combobar/combo_bar_view";

        #region UI components
        public Text comboCountText;
        public GameObject coreGO;
        #endregion

        public System.Action<uint, uint> onComboOver;
        private uint currentCount = 0;
        public Vector3 pos
        {
            set
            {
                //_pos = value;
                this.transform.localPosition = value;
            }
        }
        //private Vector3 _pos;
        private float showTime = 2f;      //显示时间

        void Awake()
        {
            coreGO.SetActive(false);
        }
        
        private IEnumerator HideComboBarCoroutine(float time, uint id)
        {
            yield return new WaitForSeconds(time);
            coreGO.SetActive(false);
            if (onComboOver != null)
                onComboOver(id, 0);
        }

        public void SetComboCount(uint id, uint count)
        {
            if (currentCount >= count)
            {
                if (onComboOver != null)
                    onComboOver(id, count);
            }
            currentCount = count;
            coreGO.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HideComboBarCoroutine(showTime, id));

            comboCountText.text = count.ToString();
            LTDescr lTDescr = LeanTween.scale(comboCountText.gameObject, Vector3.one, 0.08f);
            lTDescr.setFrom(new Vector3(2f, 2f, 1));
            lTDescr.setIgnoreTimeScale(true);
        }
    }
}