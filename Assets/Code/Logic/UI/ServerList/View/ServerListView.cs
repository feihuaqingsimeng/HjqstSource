using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Logic.UI.ServerList.Model;
using System.Collections.Generic;
using Logic.Enums;
using Common.Util;
using Logic.Game.Controller;


namespace Logic.UI.ServerList.View
{
    public class ServerListView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/server_list/server_list_view";
        public static ServerListView Open()
        {
            ServerListView view = UIMgr.instance.Open<ServerListView>(PREFAB_PATH, EUISortingLayer.Tips);
            return view;
        }

        public Transform newServerRoot;
        public Transform fullServerRoot;
        public Transform maintainServerRoot;
        public Transform lastLoginServerRoot;
        public ServerButton serverButtonPrefab;
        public GameObject fullRoot;
        public GameObject maintainRoot;
        public RectTransform scrollContent;

        void Start()
        {
            Refresh();
        }
        private void Refresh()
        {
            Dictionary<int, ServerInfo> serverListDic = ServerListProxy.instance.ServerListDictionary;
            Dictionary<int, ServerInfo> newRecommendDic = new Dictionary<int, ServerInfo>();
            Dictionary<int, ServerInfo> fullDic = new Dictionary<int, ServerInfo>();
            Dictionary<int, ServerInfo> maintainDic = new Dictionary<int, ServerInfo>();

            foreach (var server in serverListDic)
            {
                if (server.Value.state == ServerState.New)
                {
                    newRecommendDic.Add(server.Key, server.Value);
                }
                else if (server.Value.state == ServerState.Maintain)
                {
                    maintainDic.Add(server.Key, server.Value);
				}
				else
				{
					fullDic.Add(server.Key, server.Value);
				}
            }
            //新区
            serverButtonPrefab.gameObject.SetActive(true);
            TransformUtil.ClearChildren(newServerRoot, true);
            foreach (var server in newRecommendDic)
            {
                ServerButton serverBtn = Instantiate<ServerButton>(serverButtonPrefab);
                serverBtn.transform.SetParent(newServerRoot, false);
                serverBtn.SetServerInfo(server.Value);
            }
            //火爆
            TransformUtil.ClearChildren(fullServerRoot, true);
            foreach (var server in fullDic)
            {
                ServerButton serverBtn = Instantiate<ServerButton>(serverButtonPrefab);
                serverBtn.transform.SetParent(fullServerRoot, false);
                serverBtn.SetServerInfo(server.Value);
            }
            if (fullDic.Count == 0)
                fullRoot.SetActive(false);

            //维护
            TransformUtil.ClearChildren(maintainServerRoot, true);
            foreach (var server in maintainDic)
            {
                ServerButton serverBtn = Instantiate<ServerButton>(serverButtonPrefab);
                serverBtn.transform.SetParent(maintainServerRoot, false);
                serverBtn.SetServerInfo(server.Value);
            }
            if (maintainDic.Count == 0)
                maintainRoot.SetActive(false);

            //上次登入
            TransformUtil.ClearChildren(lastLoginServerRoot, true);
            ServerInfo info = serverListDic.GetValue(ServerListProxy.instance.lastServerId);
            if (info != null)
            {
                ServerButton serverBtn = Instantiate<ServerButton>(serverButtonPrefab);
                serverBtn.transform.SetParent(lastLoginServerRoot, false);
                serverBtn.transform.localPosition = Vector3.zero;
                serverBtn.SetServerInfo(info);
            }
            serverButtonPrefab.gameObject.SetActive(false);
            //scroll size
            float fullRootY = fullRoot.transform.localPosition.y;
            GridLayoutGroup group = fullServerRoot.GetComponent<GridLayoutGroup>();
            float y = (group.cellSize.y + group.spacing.y) * (fullDic.Count / group.constraintCount + 1) + Mathf.Abs(fullRootY) + 20;
            scrollContent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
        }

        public void ClickServerButtonHandler(ServerButton btn)
        {
            UIMgr.instance.Close(PREFAB_PATH);
            ServerListProxy.instance.ChangeServer(btn.serverInfo);
            Logic.Game.Controller.GameController.instance.SendExternalData(Logic.Enums.ExtraDataType.SelectServer);
        }
    }

}
