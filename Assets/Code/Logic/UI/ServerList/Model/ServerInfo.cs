using UnityEngine;
using System.Collections;
using Logic.Enums;
using Common.Localization;

namespace Logic.UI.ServerList.Model
{
    public class ServerInfo
    {
        public int serverId;
        public ServerData serverData;
        public ServerState state;
        public string name;
        public string host;
        public int port;
        public bool isInnerNet;//内网测试
        public ServerInfo(int serverId)
        {
            this.serverId = serverId;
            this.serverData = ServerData.GetServerDataByID(serverId);
            if (serverData == null)
                Debugger.LogError("serverData is null, serverId:" + serverId);
            this.state = (ServerState)serverData.server_state;
            this.name = serverData.server_name;
            this.host = serverData.server_ip;
            this.port = serverData.server_port;
            this.isInnerNet = serverData.inner == 1;
        }
        public ServerInfo(ServerJsonData data)
        {
            this.serverId = data.id.ToInt32();
            this.host = data.ip;
			this.port = data.port.ToInt32();
            this.name = data.name;
			this.state = (ServerState)data.status.ToInt32();
            this.isInnerNet = false;
        }
        public string description
        {
            get
            {
                return string.Format("{0}{1}", Localization.Get(name), GetServerStateString());
            }
        }

        private string GetServerStateString()
        {
            switch (state)
            {
                case ServerState.Full:
                    return Localization.Get("ui.serverListView.fullServer");
                    break;
                case ServerState.New:
                    return Localization.Get("ui.serverListView.newServer");
                    break;
                case ServerState.Recommend:
                    return Localization.Get("ui.serverListView.recommend");
                    break;
                case ServerState.Normal:
                    return Localization.Get("ui.serverListView.normal");
                    break;
                case ServerState.Maintain:
                    return Localization.Get("ui.serverListView.maintain");
                    break;
            }
            return string.Empty;
        }
    }

    //解析服务器发的服务器列表用的
    public class ServerJsonData
    {
        public string id;
        public string ip;
        public string port;
        public string name;
        public string status;
    }
}

