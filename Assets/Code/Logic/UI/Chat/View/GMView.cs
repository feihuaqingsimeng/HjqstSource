using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Logic.UI.Chat.View
{
    public class GMView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/chat/gm_view";
        public static GMView Open()
        {
            GMView view = UIMgr.instance.Open<GMView>(PREFAB_PATH, EUISortingLayer.MainUI, UIOpenMode.Overlay);
            return view;
        }
        #region UI components
        public Text lastContentText;
        public InputField commandInputField;
        public Button logOutBtn;

        public List<string> commandTemplateStrings;
        public List<Button> commandTemplateButtons;
        #endregion UI components

        void Start()
        {
            int showLog = PlayerPrefs.GetInt("showLog", 0);
            Text txt = logOutBtn.GetComponentInChildren<Text>();
            if (showLog == 1)
                txt.text = "关闭显示Log";
            else
                txt.text = "打开显示Log";
        }

        #region UI event handlers
        public void ClickCloseHandler()
        {
            UIMgr.instance.Close(PREFAB_PATH);
        }

        public void ClickCommandTemplateButtonHandler(Button button)
        {
            int index = commandTemplateButtons.IndexOf(button);
            string commandTemplateString = commandTemplateStrings[index];
            commandInputField.text = commandTemplateString;
        }

        public void ClickSendHandler()
        {
            string content = commandInputField.text;
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            Logic.Protocol.Model.GMReq gm = new Logic.Protocol.Model.GMReq();
            gm.command = content;
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(gm);
            lastContentText.text = content;
        }

        public void ClickLogOutBtnHandler(Button button)
        {
            int showLog = PlayerPrefs.GetInt("showLog", 0);
            Text txt = logOutBtn.GetComponentInChildren<Text>();
            if (showLog == 1)
            {
                PlayerPrefs.SetInt("showLog", 0);
                txt.text = "打开显示Log";
            }
            else
            {
                PlayerPrefs.SetInt("showLog", 1);
                txt.text = "关闭显示Log";
            }
            Game.Controller.GameController.instance.InitOutLogger();
        }

        public void QuitApplicationBtnHandler()
        {
            Application.Quit();
        }

        public void ClickSpecifyTutorialChapterIDButton()
        {
            Debugger.Log(commandInputField.text);
            int nextChapterID = 0;
            int.TryParse(commandInputField.text, out nextChapterID);
            Logic.Tutorial.Model.TutorialProxy.instance.Init(nextChapterID);
            Logic.Protocol.Model.GuideReq guidReq = new Logic.Protocol.Model.GuideReq();
            guidReq.no = nextChapterID;
            Logic.Protocol.ProtocolProxy.instance.SendProtocol(guidReq);
        }
        #endregion UI event handlers
    }
}