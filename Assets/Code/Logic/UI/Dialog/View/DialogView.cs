using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.Localization;
using Common.Util;
using Common.ResMgr;
using Logic.Dialog.Model;

namespace Logic.UI.Dialog.View
{
    public class DialogView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/dialog/dialog_view";

        public delegate void DialogShowEndDelegate();
        public DialogShowEndDelegate dialogShowEndDelegate;

        private DialogData _firstDialogData;
        private DialogData _lastDialogData;

        private DialogData _currentDialogData = null;
        private List<int> _realCharIndexList;
        private int _currentRealCharIndexIndex = 0;
        private bool isPrinting = false;

        #region UI components
		public GameObject leftNPCRoot;
		public GameObject rightNPCRoot;
        public RawImage leftNPCRawImage;
        public RawImage rightNPCRawImage;
        public GameObject leftNPCNameFrameGameObject;
        public GameObject rightNPCNameFrameGameObject;
        public Text leftNPCNameText;
        public Text rightNPCNameText;
        public Text dialogText;
        public Image enterImage;

		public Button skipButton;
        #endregion UI components

        void Start()
        {
            LTDescr ltDescr = LeanTween.moveLocalY(enterImage.gameObject, enterImage.transform.localPosition.y - 20f, 0.5f);
            ltDescr.setIgnoreTimeScale(true);
            ltDescr.setLoopPingPong();
            ltDescr.setRepeat(-1);
        }

        public void SetInitDialogData(DialogData initDialogData, DialogShowEndDelegate dialogShowEndDelegate)
        {
            _firstDialogData = initDialogData;
            _lastDialogData = GetLastDialogData();
            _currentDialogData = initDialogData;

			skipButton.gameObject.SetActive(_currentDialogData.canSkip);

            this.dialogShowEndDelegate = dialogShowEndDelegate;
            ShowCurrentDialog();
        }

        public static void Open(int initDialogID, DialogShowEndDelegate dialogShowEndDelegate)
        {
            DialogData initDialogData = DialogData.GetDialogDataByID(initDialogID);
            if (initDialogData != null)
            {
                DialogView dialogView = UIMgr.instance.Open<DialogView>(PREFAB_PATH);
                dialogView.SetInitDialogData(initDialogData, dialogShowEndDelegate);
            }
            else
            {
                if (dialogShowEndDelegate != null)
                {
                    dialogShowEndDelegate();
                }
            }
        }

        public void ShowCurrentDialog()
        {
            LeanTween.cancel(gameObject);
            if (_currentDialogData == null)
            {
                FinishDialog();
                return;
            }

            if (_currentDialogData.dialogNPCSide == Logic.Enums.DialogNPCSide.Left)
            {
                leftNPCRawImage.texture = ResMgr.instance.Load<Texture>(ResPath.GetNPCTexturePath(_currentDialogData.headIcon));
                //				leftNPCRawImage.SetNativeSize();
				string leftNPCName = string.Empty;
				if (_currentDialogData.heroName == "hero_name_my")
					leftNPCName = Logic.Game.Model.GameProxy.instance.AccountName;
				else
					leftNPCName = Localization.Get(_currentDialogData.heroName);
                leftNPCNameText.text = leftNPCName;

//                leftNPCRawImage.gameObject.SetActive(true);
//                rightNPCRawImage.gameObject.SetActive(false);

				leftNPCRoot.SetActive(true);
				rightNPCRoot.SetActive(false);

                leftNPCNameFrameGameObject.gameObject.SetActive(true);
                rightNPCNameFrameGameObject.gameObject.SetActive(false);

				enterImage.rectTransform.localScale = new Vector3(1, 1, 1);
				enterImage.rectTransform.anchoredPosition = new Vector2(442, 72);
            }
            else
            {
                rightNPCRawImage.texture = ResMgr.instance.Load<Texture>(ResPath.GetNPCTexturePath(_currentDialogData.headIcon));
                //				rightNPCRawImage.SetNativeSize();
				string rightNPCName = string.Empty;
				if (_currentDialogData.heroName == "hero_name_my")
					rightNPCName = Logic.Game.Model.GameProxy.instance.AccountName;
				else
					rightNPCName = Localization.Get(_currentDialogData.heroName);
                rightNPCNameText.text = rightNPCName;

//                leftNPCRawImage.gameObject.SetActive(false);
//                rightNPCRawImage.gameObject.SetActive(true);

				leftNPCRoot.SetActive(false);
				rightNPCRoot.SetActive(true);

                leftNPCNameFrameGameObject.gameObject.SetActive(false);
                rightNPCNameFrameGameObject.gameObject.SetActive(true);

				enterImage.rectTransform.localScale = new Vector3(-1, 1, 1);
				enterImage.rectTransform.anchoredPosition = new Vector2(-442, 72);
            }
            _realCharIndexList = RichTextFormatUtil.GetRTFStringRealCharIndexList(Localization.Get(_currentDialogData.message));
            isPrinting = true;
            dialogText.text = string.Empty;
            enterImage.gameObject.SetActive(false);
            StartCoroutine("WaitAndPrint");
        }

        public void ShowCurrentDialogInstantly()
        {
            dialogText.text = Localization.Get(_currentDialogData.message);
            StopCoroutine("WaitAndPrint");
            isPrinting = false;
            _currentRealCharIndexIndex = 0;
            _currentDialogData = DialogData.GetNextDialogData(_currentDialogData);
            enterImage.gameObject.SetActive(true);

            AutoDelayShowNextDialog();
        }

        IEnumerator WaitAndPrint()
        {
            while (_currentRealCharIndexIndex < _realCharIndexList.Count)
            {
                yield return new WaitForSeconds(0.08f);
                dialogText.text = RichTextFormatUtil.GetRTFSubString(Localization.Get(_currentDialogData.message), _realCharIndexList[_currentRealCharIndexIndex]);
                _currentRealCharIndexIndex++;

                if (_currentRealCharIndexIndex >= _realCharIndexList.Count)
                {
                    StopCoroutine("WaitAndPrint");
                    isPrinting = false;
                    _currentRealCharIndexIndex = 0;
                    _currentDialogData = DialogData.GetNextDialogData(_currentDialogData);
                    enterImage.gameObject.SetActive(true);

                    AutoDelayShowNextDialog();
                }
            }
        }

        private DialogData GetLastDialogData()
        {
            DialogData dialogData = _firstDialogData;
            if (dialogData != null)
            {
                while (DialogData.GetNextDialogData(dialogData) != null)
                {
                    dialogData = DialogData.GetNextDialogData(dialogData);
                }
            }
            return dialogData;
        }

        private void FinishDialog()
        {
            if (dialogShowEndDelegate != null)
            {
                dialogShowEndDelegate();
            }
            Observers.Facade.Instance.SendNotification(string.Format("OnDialogFinish::{0}", _lastDialogData.id));
            Debugger.Log("====================");
            Debugger.Log(string.Format("OnDialogFinish::{0}", _lastDialogData.id));
            Debugger.Log("====================");
            UIMgr.instance.Close(PREFAB_PATH);
        }

        private void AutoDelayShowNextDialog()
        {
            LeanTween.delayedCall(gameObject, 3f, ShowCurrentDialog);
        }

        #region UI events
        public void ClickSkipButtonHandler()
        {
            FinishDialog();
        }

        public void ClickHandler()
        {
            if (isPrinting)
            {
                ShowCurrentDialogInstantly();
            }
            else
            {
                ShowCurrentDialog();
            }
        }
        #endregion UI events
    }
}