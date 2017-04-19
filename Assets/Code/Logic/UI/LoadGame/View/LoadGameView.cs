using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common.Localization;
using Logic.Game.Model;
using Common.ResMgr;
using System.Collections;
using Logic.UI.LoadGame.Controller;

namespace Logic.UI.LoadGame.View
{
    public class LoadGameView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/load_game/load_game_view";


		private string _loadingBGTextureName;
        public bool showRandomTips = true;
        public delegate void OnLoadCompleteDelegate();
        private System.Action _onLoadCompleteDelegate;

        #region UI components
        public RawImage bgRawImage;
        public Text tipsText;
        public Text progressText;
        public Slider loadProgressSlider;
        #endregion UI components

        public List<string> loadingBGImageNames;
        private int _index;

        public string tips
        {
            set
            {
				tipsText.gameObject.SetActive(true);
                tipsText.text = value;
            }
        }

        public static LoadGameView Open(System.Action onLoadCompleteDelegate, string loadingBGTextureName = null)
        {
            LoadGameView loadGameView = UIMgr.instance.Open<LoadGameView>(PREFAB_PATH);
            loadGameView._onLoadCompleteDelegate = onLoadCompleteDelegate;
			loadGameView._loadingBGTextureName = loadingBGTextureName;
            return loadGameView;
        }

        public void SetCompleteHandler(System.Action onLoadCompleteDelegate)
        {
            _onLoadCompleteDelegate = onLoadCompleteDelegate;
        }

		public void SetDefaultBG ()
		{
			_loadingBGTextureName = "loading_07";
			bgRawImage.texture = ResMgr.instance.Load<Texture>(string.Format("ui_textures/loading_textures/{0}", _loadingBGTextureName));
		}

        void Awake()
        {
            loadProgressSlider.value = 0;
        }

        void Start()
        {
            //            List<string> loadingTipsIDs = GlobalData.GetGlobalData().loadingTipsIDs;
            //            string tipsFormatString = Localization.Get("ui.load_game_view.tips");
            //			string tipsString = string.Empty;
            //			if (loadingTipsIDs == null || loadingTipsIDs.Count == 0)
            //			{
            //				tipsString = Localization.Get("tips1");
            //			}
            //			else
            //			{
            //            	tipsString = Localization.Get(loadingTipsIDs[Random.Range(0, loadingTipsIDs.Count)]);
            //			}

			if (_loadingBGTextureName == null || _loadingBGTextureName == string.Empty)
			{
            	int loadingBGNameIndex = Random.Range(0, loadingBGImageNames.Count);
				_loadingBGTextureName = loadingBGImageNames[loadingBGNameIndex];
			}
            bgRawImage.texture = ResMgr.instance.Load<Texture>(string.Format("ui_textures/loading_textures/{0}", _loadingBGTextureName));
            //string tipsFormatString = "提示:{0}";
            //string tipsString = tipsStrings[Random.Range(0, tipsStrings.Length)];
            //tipsText.text = string.Format(tipsFormatString, tipsString);
            if (showRandomTips)
                StartCoroutine("ShowTipsCoroutine");
            progressText.gameObject.SetActive(false);
        }

        private IEnumerator ShowTipsCoroutine()
        {
            while (true)
            {
                if (!showRandomTips)
                    yield break;
				tipsText.gameObject.SetActive(false);
                //tipsText.text = LoadGameController.instance.GetTipsString();
                yield return new WaitForSeconds(6f);
            }
        }

        public void SetDelayTime(float delay, System.Action callback)
        {
            StartCoroutine(SetDelayTimeCoroutine(delay, callback));
        }

        private IEnumerator SetDelayTimeCoroutine(float delay, System.Action callback)
        {
            float time = Time.time;
            while (Time.time <= time + delay)
            {
                yield return null;
                float progress = (Time.time - time) / delay;
                if (progress > 1)
                    progress = 1;
                OnProgressUpdate(progress);
            }
            yield return null;
            UI.UIMgr.instance.Close(PREFAB_PATH);
            if (callback != null)
                callback();
        }

        private void OnProgressUpdate(float progress)
        {
            if (!progressText.gameObject.activeInHierarchy)
                progressText.gameObject.SetActive(true);
            progressText.text = string.Format("{0}%", (int)(progress * 100));
            loadProgressSlider.value = progress;
        }

        private void OnUpdateLoadProgressComplete()
        {
            if (loadProgressSlider.value >= loadProgressSlider.maxValue)
                StartCoroutine("OnLoadComplete");
        }

        private IEnumerator OnLoadComplete()
        {
            yield return new WaitForSeconds(0.2f);
            if (_onLoadCompleteDelegate != null)
                _onLoadCompleteDelegate();
            _onLoadCompleteDelegate = null;
        }

        public void UpdateLoadProgress(float progress)
        {
            LeanTween.cancel(gameObject);
            float time = Mathf.Max(progress - loadProgressSlider.value, 0.2f);
            LTDescr ltDescr = LeanTween.value(gameObject, loadProgressSlider.value, progress, time);
            ltDescr.setOnUpdate(OnProgressUpdate);
            ltDescr.setOnComplete(OnUpdateLoadProgressComplete);
        }

        public void UpdateLoadProgress(float progress, int currentNum, int totalNum)
        {
            if (!progressText.gameObject.activeInHierarchy)
                progressText.gameObject.SetActive(true);
            //progressText.text = string.Format("{0}/{1}     {2}%", currentNum, totalNum, (int)(progress * 100));
            progressText.text = string.Format("{0}%", (int)(progress * 100));
            loadProgressSlider.value = progress;
            OnUpdateLoadProgressComplete();
        }

        public void UpdateLoadProgress(ResPreload target, float progress, int currentNum, int totalNum)
        {
            if (!progressText.gameObject.activeInHierarchy)
                progressText.gameObject.SetActive(true);
            //progressText.text = string.Format("{0}/{1}     {2}%", currentNum, totalNum, (int)(progress * 100));
            progressText.text = string.Format("{0}%", (int)(progress * 100));
            loadProgressSlider.value = progress;
            OnUpdateLoadProgressComplete();
        }
    }
}