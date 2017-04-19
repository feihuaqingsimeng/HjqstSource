using UnityEngine;
using System.Collections;
namespace Logic.Game
{
    public class GameDataCenter : SingletonMonoNewGO<GameDataCenter>
    {
        public int loginNum = 0;
        public bool isTipLoginNotice = true;
        public bool isRegisterLocalNotificator = false;

        [HideInInspector]
        public bool isSdkLogin = false;
        public string sdkId;
        public string u8id;
        public string token;
        public int platformId;

        void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ReLoadMainScene()
        {
            isSdkLogin = false;
            StartCoroutine(LoadMainScene());
        }

        private System.Collections.IEnumerator LoadMainScene()
        {
            UI.UIMgr.instance.Close(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            string sceneName = "main";
            //if (Application.loadedLevelName == "main")
            //    sceneName = "main_1";
            Debugger.Log("load scene:" + sceneName);
            Logic.UI.LoadGame.View.LoadGameView loadGameView = UI.UIMgr.instance.Open<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            loadGameView.UpdateLoadProgress(1, 0, 1);
            yield return null;
            Application.LoadLevel(sceneName);

            //AsyncOperation ao = Application.LoadLevelAsync(sceneName);
            //Logic.UI.LoadGame.View.LoadGameView loadGameView = UI.UIMgr.instance.Open<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            //while (!ao.isDone)
            //{
            //    if (!loadGameView)
            //        yield break;
            //    loadGameView.UpdateLoadProgress(ao.progress, 0, 1);
            //    yield return null;
            //}
        }
    }
}