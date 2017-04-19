using UnityEngine;
using System.Collections;
using Logic.UI.LoadGame.View;
using System.Collections.Generic;
using LuaInterface;
namespace Logic.UI.LoadGame.Controller
{
    public class LoadGameController : SingletonMono<LoadGameController>
    {
        private List<string> _tipsStrings = new List<string>()
        {
            "配置更新，请务必坚持到底！"
        };
        private const string _decompressionTips = "正在解压配置文件，不消耗流量，请耐心等待！";
        private const string _checkConfigTips = "配置更新，请务必坚持到底！";
        private const string _updateConfigTips = "初始配置中，请骑士们耐心等待（建议在wifi环境下进行游戏）";
        private const string _preloadResTips = "请耐心等待，资源加载中，不消耗流量！";
        private const string _loadResTips = "资源更新中，请耐心等待！";
        private const string TIPS_PREFIX = "提示：{0}";

        private int _index = 0;
        private int currentIndex
        {
            get
            {
                _index++;
                if (_index >= _tipsStrings.Count)
                    _index = 0;
                return _index;
            }
        }

        void Awake()
        {
            instance = this;
        }

        public void AddLoadTips()
        {
            LuaTable luaTable = LuaScriptMgr.Instance.GetLuaTable("gamedataTable.loadTips");
            object[] rs = luaTable.ToArray();
            _tipsStrings.Clear();
            for (int i = 0, length = rs.Length; i < length; i++)
            {
                Debugger.Log(rs[i].ToString());
                _tipsStrings.Add(rs[i].ToString());
            }
        }

        public string GetTipsString()
        {
            return string.Format(TIPS_PREFIX, _tipsStrings[currentIndex]);
        }

        public LoadGameView OpenLoadGameView()
        {
            Logic.UI.LoadGame.View.LoadGameView loadGameView = UI.UIMgr.instance.Get<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            if (loadGameView == null)
                loadGameView = UI.UIMgr.instance.Open<Logic.UI.LoadGame.View.LoadGameView>(Logic.UI.LoadGame.View.LoadGameView.PREFAB_PATH);
            return loadGameView;
        }

        public void SetDelayTime(float delay, System.Action callback)
        {
            LoadGameView loadGameView = OpenLoadGameView();
            if (loadGameView)
                loadGameView.SetDelayTime(delay, callback);
        }

        public LoadGameView Switch2DecompressionTips()
        {
            LoadGameView loadGameView = OpenLoadGameView();
            if (loadGameView)
                loadGameView.tips = string.Format(TIPS_PREFIX, _decompressionTips);
            return loadGameView;
        }

        public LoadGameView Switch2CheckConfigTips()
        {
            LoadGameView loadGameView = OpenLoadGameView();
            if (loadGameView)
                loadGameView.tips = string.Format(TIPS_PREFIX, _checkConfigTips);
            return loadGameView;
        }

        public LoadGameView Switch2UpdateConfigTips()
        {
            LoadGameView loadGameView = OpenLoadGameView();
            if (loadGameView)
                loadGameView.tips = string.Format(TIPS_PREFIX, _updateConfigTips);
            return loadGameView;
        }

        public LoadGameView Switch2PreloadResTips()
        {
            LoadGameView loadGameView = OpenLoadGameView();
            if (loadGameView)
                loadGameView.tips = string.Format(TIPS_PREFIX, _preloadResTips);
            return loadGameView;
        }

        public LoadGameView Switch2LoadResTips()
        {
            LoadGameView loadGameView = OpenLoadGameView();
            if (loadGameView)
                loadGameView.tips = string.Format(TIPS_PREFIX, _loadResTips);
            return loadGameView;
        }
    }
}
