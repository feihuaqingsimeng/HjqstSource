using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Role.Model;
using Logic.Hero.Model;
using Common.Localization;
using Logic.Protocol.Model;
using Logic.Game.Model;
using Logic.Player.Model;
using Logic.Enums;
using Logic.Equipment.Model;
using Logic.Hero;
using Logic.Role;
using LuaInterface;

namespace Logic.UI.IllustratedHandbook.Model
{
    public class IllustratedHandbookProxy : SingletonMono<IllustratedHandbookProxy>
    {

        public System.Action UpdateIllustrationDelegate;
        public System.Action InitIllustrationDelegate;

        public int selectToggleId = 0;
        public float scrollPercent = 1;
        void Awake()
        {
            instance = this;
        }
        ///已获得的图鉴英雄 string:("modelid,star")   int: 1
        private Dictionary<string, bool> _illustrationDictionary = new Dictionary<string, bool>();
        public Dictionary<string, bool> IllustrationDictionary
        {
            get
            {
                if (_illustrationDictionary.Count == 0)
                {
                    LuaTable luaModelTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "illustration_model")[0];
                    LuaTable illusDicTable = (LuaTable)luaModelTable.GetLuaFunction("GetIllustrationDictionary").Call(null)[0];
                    foreach (DictionaryEntry kvp in illusDicTable.ToDictTable())
                    {
                        _illustrationDictionary.Add(kvp.Key.ToString(), kvp.Value.ToString().ToBoolean());
                    }
                }
                return _illustrationDictionary;

            }
        }


        ///表格数据
        private Dictionary<int, Dictionary<int, List<IllustrationInfo>>> _illustrationDataDictionary = new Dictionary<int, Dictionary<int, List<IllustrationInfo>>>();
        ///表格数据
        public Dictionary<int, Dictionary<int, List<IllustrationInfo>>> IllustrationDataDictionary
        {
            get
            {
                InitDictionary();
                return _illustrationDataDictionary;
            }
        }
        public Dictionary<int, string> bigTitleStringDictionary = new Dictionary<int, string>();
        public Dictionary<int, string> smallTitleStringDictionary = new Dictionary<int, string>();

        public List<List<IllustrationInfo>> currentSelectRoleList = new List<List<IllustrationInfo>>();
        public List<int> currentSelectTitleList = new List<int>();

        public int filterIndex;//过滤类型RoleType
        public RoleInfo CheckedDetailRoleInfo;//预览的英雄
        public void Clear()
        {
            _illustrationDictionary.Clear();
        }
        public void InitDictionary()
        {

            if (_illustrationDataDictionary.Count != 0)
                return;
            Dictionary<int, IllustratedData> datas = IllustratedData.IllustratedDataDictionary;
            IllustratedData data;
            Dictionary<int, List<IllustrationInfo>> secondDic;
            List<IllustrationInfo> roleList;
            RoleInfo info;
            foreach (var value in datas)
            {
                data = value.Value;
                if (!_illustrationDataDictionary.ContainsKey(data.type))
                {
                    _illustrationDataDictionary.Add(data.type, new Dictionary<int, List<IllustrationInfo>>());
                    bigTitleStringDictionary.Add(data.type, Localization.Get(data.type_name));
                }
                secondDic = _illustrationDataDictionary[data.type];
                if (!secondDic.ContainsKey(data.sheet))
                {
                    secondDic.Add(data.sheet, new List<IllustrationInfo>());
                    smallTitleStringDictionary.Add(data.sheet, Localization.Get(data.sheet_name));
                }
                roleList = secondDic[data.sheet];
                if (data.resData.type == BaseResType.Hero)
                {
                    HeroData heroData = HeroData.GetHeroDataByID(data.resData.id);
                    if (heroData.hero_type == 2)
                    {
                        PlayerInfo player = new PlayerInfo((uint)0, (uint)data.resData.id, (uint)0, (uint)0, (uint)0, 0, "");
                        player.advanceLevel = data.resData.star;
                        player.level = GlobalData.GetGlobalData().playerLevelMax;
                        roleList.Add(new IllustrationInfo(player));
                    }
                    else
                    {
                        HeroInfo hero = new HeroInfo((uint)data.resData.id, data.resData.id, 1, 0, data.resData.star, GlobalData.GetGlobalData().playerLevelMax);
                        roleList.Add(new IllustrationInfo(hero));
                    }
                }
                else if (data.resData.type == BaseResType.Equipment)
                {

                }

            }
        }
        public void UpdateIllustrationList(List<IllustrationProto> protos, bool clear)
        {
            if (clear)
                IllustrationDictionary.Clear();
            for (int i = 0, count = protos.Count; i < count; i++)
            {
                UpdateIllustration(protos[i]);
            }
        }
        public void UpdateIllustration(IllustrationProto proto)
        {
            for (int i = 0, count = proto.stars.Count; i < count; i++)
            {
                string key = string.Format("{0},{1}", proto.heroNo, proto.stars[i]);
                if (!IllustrationDictionary.ContainsKey(key))
                    IllustrationDictionary.Add(key, true);
            }

        }
        public List<RoleInfo> GetIllustrationRoleList()
        {
            List<RoleInfo> roleList = new List<RoleInfo>();

            foreach (var data in IllustrationDictionary)
            {
                int[] info = data.Key.ToArray<int>(',');
                if (info.Length == 2)
                {
                    HeroData heroData = HeroData.GetHeroDataByID(info[1]);

                    if (heroData == null)
                        continue;
                    RoleInfo roleInfo = null;
                    if (heroData.hero_type == 2)//主角
                    {
                        roleInfo = new PlayerInfo((uint)0, (uint)heroData.id, (uint)0, (uint)0, (uint)0, 0, "");
                    }
                    else
                    {
                        roleInfo = new HeroInfo(0, heroData.id, 1, 0, 1, 1);
                    }
                    roleList.Add(roleInfo);
                }
            }
            return roleList;
        }
        public bool isHeroGotInIllustration(int heroModelId, int star)
        {
            string key = string.Format("{0},{1}", heroModelId, star);
            return IllustrationDictionary.ContainsKey(key);
        }

        public bool IsHeroCheck(int heroModelId)
        {
            LuaTable luaModelTable = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel", "illustration_model")[0];
            return luaModelTable.GetLuaFunction("IsHeroCheck").Call(heroModelId)[0].ToString().ToBoolean();
        }

        //切换toggle
        public void UpdateSelectToggleRoleDic(int id)
        {
            if (!IllustrationDataDictionary.ContainsKey(id))
                return;
            Dictionary<int, List<IllustrationInfo>> dic = IllustrationDataDictionary[id];
            currentSelectRoleList.Clear();
            currentSelectTitleList.Clear();
            //currentSelectRoleList = dic.GetValues();
            //currentSelectTitleList = dic.GetKeys();
            int useCount = 0;
            foreach (var value in dic)
            {
                currentSelectRoleList.Add(new List<IllustrationInfo>());
                currentSelectTitleList.Add(value.Key);
                useCount = 0;

                int count = value.Value.Count;
                List<IllustrationInfo> innerSubList;
                while (true)
                {
                    innerSubList = new List<IllustrationInfo>();
                    for (int i = useCount; i < useCount + 8 && i < count; i++)
                    {
                        innerSubList.Add(value.Value[i]);
                    }
                    useCount += 8;
                    if (innerSubList.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        currentSelectRoleList.Add(innerSubList);
                        currentSelectTitleList.Add(value.Key);
                    }
                }
            }


        }
        private static int CompareIllustrationInfo(IllustrationInfo a, IllustrationInfo b)
        {
            return RoleUtil.CompareRoleByQualityAsc(a.roleInfo, b.roleInfo);
        }
        #region from server update
        public void UpdateIllustrationByProtocol()
        {
            if (UpdateIllustrationDelegate != null)
                UpdateIllustrationDelegate();
        }
        public void InitIllustrationByProtocol()
        {
            if (InitIllustrationDelegate != null)
                InitIllustrationDelegate();
        }
        #endregion
    }
}

