using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Player.Model;
using Logic.Role.Model;
using Logic.Hero.Model;
using Logic.Protocol.Model;
using Common.ResMgr;

namespace Logic.UI.Friend.Model
{
    public class FriendInfo
    {
        public int id;

        public int level;
        public string name;
        public int power;
        public long lastLoginTime;
        ///是否领取体力了 true:领  false:未领
        public bool isGetPveAction = true;
        ///是否赠送了  true:赠  false:未赠
        public bool isDonate = false;
        ///双向朋友
        public bool isBothAuth;
        public string headIcon;
        public List<RoleInfo> formationHeroList = new List<RoleInfo>();
		public int vip;
        private PlayerInfo _playerInfo;
        public FriendInfo()
        {
            //			_playerInfo = new PlayerInfo(0,201,0,0,0,"");
            //		
            //			RoleInfo info = new HeroInfo(0,5,1,5,3,20);
            //
            //			formationHeroList.Add(info);
        }
        public FriendInfo(FriendProtoData data)
        {
            Update(data);
        }
        public void Update(FriendProtoData data)
        {
            this.id = data.id;
            if (!string.IsNullOrEmpty(data.roleName))
                name = data.roleName;
            if (data.lv != -1)
                level = data.lv;
            if (data.playerNo != -1)
            {
                _playerInfo = new PlayerInfo(0, (uint)data.playerNo, (uint)data.hairCutId, (uint)data.hairColorId, (uint)data.faceId, data.skinId, "");
            }
            if (data.combat != -1)
                power = data.combat;
            isBothAuth = data.isBothAuth;
            //if(data.lastLoginTime != -1)
            lastLoginTime = data.lastLoginTime;
            //else
            //				lastLoginTime = Common.GameTime.Controller.TimeController.instance.ServerTimeTicksSecond*1000;
            isDonate = data.isPresent;

            isGetPveAction = !data.isPresented;
            //headicon
            headIcon = UIUtil.ParseHeadIcon(data.headNo);
			vip = data.vipLv;
        }
        public void UpdateTeam(TeamDetailProtoData team)
        {
            formationHeroList.Clear();
            if (team.player != null)
            {
                formationHeroList.Add(new PlayerInfo(team.player));
            }
            for (int i = 0, count = team.heros.Count; i < count; i++)
            {
                formationHeroList.Add(new HeroInfo(team.heros[i]));
            }
        }
    }
}

