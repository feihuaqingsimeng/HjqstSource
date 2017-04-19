using UnityEngine;
using System.Collections.Generic;
using Logic.Enums;
using Logic.Protocol.Model;
using LuaInterface;
using System.Collections;

namespace Logic.Formation.Model
{
    public class FormationProxy : SingletonMono<FormationProxy>
    {
        void Awake()
        {
            instance = this;
        }

        //阵型
        private Dictionary<int, FormationInfo> _formationInfoDictionary = new Dictionary<int, FormationInfo>();
        public Dictionary<int, FormationInfo> FormationInfoDictionary
        {
            get
            {
                if (_formationInfoDictionary.Count == 0)
                {
					_allFormationInfoList = null;
                    Dictionary<int, FormationData> formationDataDic = FormationData.FormationDataDictionary;
                    FormationInfo formationInfo = null;
                    foreach (var data in formationDataDic)
                    {
                        formationInfo = new FormationInfo(data.Value.id, 1, FormationState.Locked);
                        _formationInfoDictionary.Add(data.Value.id, formationInfo);
                    }
                }
                return _formationInfoDictionary;
            }
        }
		private LuaTable _formationModelLua;
		public LuaTable formationModelLua
		{
			get
			{
				if(_formationModelLua == null)
				{
					_formationModelLua = (LuaTable)LuaScriptMgr.Instance.CallLuaFunction("gamemanager.GetModel","formation_model")[0];
				}
				return _formationModelLua;
			}
		}
        //队伍
        private Dictionary<FormationTeamType, FormationTeamInfo> _formationTeamDictionary = new Dictionary<FormationTeamType, FormationTeamInfo>();

        public FormationTeamInfo GetFormationTeam(FormationTeamType type)
        {

            if (_formationTeamDictionary.ContainsKey(type))
                return _formationTeamDictionary[type];
            _formationTeamDictionary[type] = new FormationTeamInfo(type, 0, new SortedDictionary<FormationPosition, uint>());
            return _formationTeamDictionary[type];

        }

        public FormationInfo GetFormationInfo(int id)
        {
            if (FormationInfoDictionary.ContainsKey(id))
                return FormationInfoDictionary[id];
            return null;
        }
        private List<FormationInfo> _allFormationInfoList;
        public List<FormationInfo> GetAllFormationInfoList()
        {
            if (_allFormationInfoList == null)
            {
                _allFormationInfoList = FormationInfoDictionary.GetValues();
            }
            return _allFormationInfoList;
        }
        //阵型战力加成
        public float FormationPower(int formationId)
        {
            if (!FormationInfoDictionary.ContainsKey(formationId))
                return 0;
            FormationInfo info = FormationInfoDictionary[formationId];
            return info.Power;
        }
        ///解锁阵型
        public void AddAllUnlockFormation(List<LineupProtoData> datas)
        {
            _formationInfoDictionary.Clear();
            LineupProtoData data;
            for (int i = 0, count = datas.Count; i < count; i++)
            {
                data = datas[i];
                FormationInfo formationInfo = new FormationInfo(data.no, data.lv, FormationState.NotInUse);
                FormationInfoDictionary[formationInfo.id] = formationInfo;
            }
        }
        public void AddAllUnlockFormation(LuaTable datas)
        {
            _formationInfoDictionary.Clear();
            foreach (DictionaryEntry value in datas.ToDictTable())
            {
                LuaTable table = (LuaTable)value.Value;
				FormationInfo formationInfo = new FormationInfo(table["id"].ToString().ToInt32(), table["level"].ToString().ToInt32(), (FormationState)(table["formationState"].ToString().ToInt32()));
                formationInfo.isActiveAdditionAttr = table["isActiveAdditionAttr"].ToString().ToBoolean();
                FormationInfoDictionary[formationInfo.id] = formationInfo;
            }

        }

        //队伍集合
        public void AddAllTeamData(List<TeamProtoData> datas)
        {
            _formationTeamDictionary.Clear();
            //FormationTeamInfo info;
            for (int i = 0, count = datas.Count; i < count; i++)
            {
                AddTeamData(datas[i]);
            }
        }
        public void AddAllTeamData(LuaTable datas)
        {
            _formationTeamDictionary.Clear();
            foreach (DictionaryEntry value in datas.ToDictTable())
            {
                AddTeamData((FormationTeamType)value.Key.ToString().ToInt32(), (LuaTable)value.Value);
            }
        }
        public void AddTeamData(TeamProtoData data)
        {
            SortedDictionary<FormationPosition, uint> teamDic = new SortedDictionary<FormationPosition, uint>();
            for (int j = 0, count2 = data.posList.Count; j < count2; j++)
            {
                teamDic.Add((FormationPosition)data.posList[j].posIndex, (uint)data.posList[j].heroId);
            }
            FormationTeamInfo info = new FormationTeamInfo((FormationTeamType)data.teamNo, data.lineupNo, teamDic);
            _formationTeamDictionary[(FormationTeamType)data.teamNo] = info;
        }
        public void AddTeamData(FormationTeamType type, LuaTable data)
        {
            SortedDictionary<FormationPosition, uint> teamDic = new SortedDictionary<FormationPosition, uint>();
            LuaTable teamPosTable = (LuaTable)data["teamPosTable"];
            foreach (DictionaryEntry value in teamPosTable.ToDictTable())
            {
                teamDic.Add((FormationPosition)value.Key.ToString().ToInt32(), (uint)value.Value.ToString().ToInt32());
            }

            FormationTeamInfo info = new FormationTeamInfo(type, data["formationId"].ToString().ToInt32(), teamDic);
            _formationTeamDictionary[type] = info;
        }

        public void SetAdditionFormationAttrActive(int formationId, bool active)
        {
            FormationInfo formationInfo = GetFormationInfo(formationId);
            if (formationInfo != null)
                formationInfo.isActiveAdditionAttr = active;
        }
    }
}