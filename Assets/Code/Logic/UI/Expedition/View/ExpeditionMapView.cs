using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
using Logic.UI.Expedition.Model;
namespace Logic.UI.Expedition.View
{
    public class ExpeditionMapView : MonoBehaviour
    {
        public RectTransform behindRootTran;
        public RectTransform aheadRootTran;
        public Transform dungeonRootTran;
        public ExpeditionDungeonButton dungeonPrefab;

        private float _behindMinX;
        private float _behindMaxX;
        private float _aheadMinX;
        private float _aheadMaxX;

        ScrollRect _scrollRect;

        void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();

            if (_scrollRect != null)
            {
                _scrollRect.onValueChanged.AddListener(Scroll);
            }
            _behindMinX = -(behindRootTran.sizeDelta.x - 1136);
            _behindMaxX = 0;
            _aheadMinX = -(aheadRootTran.sizeDelta.x - 1136);
            _aheadMaxX = 0;
            behindRootTran.anchoredPosition = new Vector2(_behindMaxX, behindRootTran.anchoredPosition.y);
        }
        void Start()
        {
            //Refresh();
        }
        public void Refresh()
        {
            InitDungeon();
        }
        private void InitDungeon()
        {
            dungeonPrefab.gameObject.SetActive(true);
            TransformUtil.ClearChildren(dungeonRootTran, true);
            List<ExpeditionDungeonInfo> infoList = ExpeditionProxy.instance.GetCurrentDungeonInfoListByChapter();
            int count = infoList.Count;
            ExpeditionDungeonInfo info;
            ExpeditionDungeonButton dungeonBtn;
            Vector2 rootSize = _scrollRect.content.sizeDelta;
            Vector3 curPos = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                info = infoList[i];
                dungeonBtn = Instantiate<ExpeditionDungeonButton>(dungeonPrefab);
                dungeonBtn.gameObject.name = i.ToString();
                dungeonBtn.transform.SetParent(dungeonRootTran, false);
                dungeonBtn.transform.localPosition = info.data.position;
                dungeonBtn.SetDungeonInfo(info);
                if (info.data.id == ExpeditionProxy.instance.CurrentExpeditionDungeonId)
                {
                    curPos = info.data.position;
                    
                    dungeonBtn.CreatePlayer();
                    
                    MoveTo(dungeonBtn);
                }
            }
            dungeonPrefab.gameObject.SetActive(false);
        }
        private void MoveTo(ExpeditionDungeonButton dungeonButton)
        {

            if (dungeonButton != null)
            {
                Vector2 _chapterScrollViewportSize = new Vector2(960, 640);

                Vector2 pos = dungeonButton.GetComponent<RectTransform>().anchoredPosition;
                Vector2 targetPos = -(pos - _chapterScrollViewportSize * 0.5f);
                float actualX = Mathf.Clamp(targetPos.x, -(_scrollRect.content.rect.size.x - _chapterScrollViewportSize.x * 0.5f), 0);
                float actualY = Mathf.Clamp(targetPos.y, -(_scrollRect.content.rect.size.y - _chapterScrollViewportSize.y * 0.5f), 0);
                targetPos = new Vector2(actualX, actualY);
                _scrollRect.content.anchoredPosition = targetPos;
            }
        }

        private void Scroll(Vector2 vec)
        {
            float x = Mathf.Lerp(_behindMaxX, _behindMinX, vec.x);
            behindRootTran.anchoredPosition = new Vector2(x, behindRootTran.anchoredPosition.y);
        }

    }

}
