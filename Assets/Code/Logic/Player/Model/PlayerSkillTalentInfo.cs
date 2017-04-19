using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Skill.Model;
using Logic.Game.Model;
using Logic.Enums;
using Common.ResMgr;

namespace Logic.Player.Model
{
	public class PlayerSkillTalentInfo
	{
		public int id;

		public PlayerSkillTalentData talentData;

		public int exp;
		public int level;

		private bool _isCarry;//多选一是否携带
		public bool IsCarry
		{
			get
			{
				return _isCarry && talentData.groupType != PlayerSkillTalentType.PassiveNormal;
			}
			set
			{
				_isCarry = value;
			}
		}
		public int maxLevel
		{
			get
			{
				return talentData.max_lv;
			}
		}
		public bool IsMaxLevel
		{
			get
			{
				return level == maxLevel;
			}
		}
		public bool isMaxExp
		{
			get
			{
				return exp >= talentData.exp_need;
			}
		}
		public Sprite skillIcon
		{
			get
			{
				return ResMgr.instance.Load<Sprite>(ResPath.GetSkillIconPath(talentData.icon));
			}
		}
        public string name 
        {
            get
            {
                return talentData.GetNameByLevel(level);
            }
        }
        public string des
        {
            get
            {
                return talentData.GetDesByLevel(level);
            }
        }
		public PlayerSkillTalentInfo(int talentid,int level,int exp)
		{
			Set(talentid,level,exp);
		}
		public void Set(int talentid,int level,int exp)
		{
			this.id = talentid;
			
			talentData = PlayerSkillTalentData.GetSkillTalentDataByID(talentid);
			this.level = level;
			this.exp = exp;
		}
		public void Set(int level ,int exp)
		{
			this.level = level;
			this.exp = exp;
		}

		public List<GameResData> UpgradeCost
		{
			get
			{
				return talentData.GetCostResData(level+1);
			}
		}
	}
}

