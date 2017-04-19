using Logic.Enums;
using UnityEngine;
using System.Collections.Generic;
using Logic.Game.Model;
using Logic.Protocol.Model;
using LuaInterface;

namespace Logic.Formation.Model
{
    public class FormationInfo
    {
        public int id;
        public FormationData formationData;
        public int level;
		public FormationState formationState;
        public bool isActiveAdditionAttr;

        public FormationInfo(LuaTable table)
        {
            Update(table);
        }

        public FormationInfo(int formationDataID, int level, FormationState formationState = FormationState.Locked)
        {
            id = formationDataID;
            formationData = FormationData.GetFormationData(formationDataID);
            this.level = level;
            this.formationState = formationState;
        }
        public void Update(LuaTable table)
        {
            id = table["id"].ToString().ToInt32();
            formationData = FormationData.GetFormationData(id);
            level = table["level"].ToString().ToInt32();
            formationState = (FormationState)table["formationState"].ToString().ToInt32();
        }
        public void Update(LineupProtoData data)
        {
            this.level = data.lv;
            this.formationState = FormationState.NotInUse;
        }
        public bool IsMax
        {
            get
            {
                return level == MaxLevel;
            }
        }
        public bool isFollowAccountLevel
        {
            get
            {
                return formationData.maxLevel == -1;
            }
        }
        public int MaxLevel
        {
            get
            {
                if (formationData.maxLevel == -1)
                {
                    return GameProxy.instance.AccountLevel;
                }
                else
                {
                    return formationData.maxLevel;
                }
            }
        }
        //升级消耗1(下一级)
        public GameResData UpgradeResCost
        {
            get
            {
                float a = formationData.upgradeA * Mathf.Pow(level + 1, 2);
                float b = formationData.upgradeB * (level + 1);
                int count = (int)Mathf.Round(formationData.upgradeCostGameResData.count * (a + b + formationData.upgradeC));
                return new GameResData(formationData.upgradeCostGameResData.type, formationData.upgradeCostGameResData.id, count, formationData.upgradeCostGameResData.star);
            }
        }
        //升级消耗2(下一级)
        public int UpgradeTrainPointCost
        {
            get
            {
                return (int)(formationData.upgrade_base_a * (level + 1) + formationData.upgrade_base_b);
            }
        }
        //属性加成，类型、值
        public Dictionary<FormationEffectType, float> formationAttrDic
        {
            get
            {
                Dictionary<FormationEffectType, float> attrDic = new Dictionary<FormationEffectType, float>();

                List<FormationAttrData> attrDataList = FormationAttrData.GetFormationDatas(formationData.id, level);
                FormationAttrData data;
                for (int i = 0, count = attrDataList.Count; i < count; i++)
                {
                    data = attrDataList[i];
                    if (data.formationAttrType == FormationAttrType.Base)
					{
                    	attrDic.Add(data.effectType, data.effect_base + (level - 1) * data.effect_upgrade);
                    }
                }
                return attrDic;
            }
        }

		public Dictionary<FormationEffectType, float> formationAdditionAttrDic
		{
			get
			{
				Dictionary<FormationEffectType, float> additionAttrDic = new Dictionary<FormationEffectType, float>();
				
				List<FormationAttrData> additionAttrDataList = FormationAttrData.GetFormationDatas(formationData.id, FormationAttrType.Addition);
				FormationAttrData data;
				for (int i = 0, count = additionAttrDataList.Count; i < count; i++)
				{
					data = additionAttrDataList[i];
					additionAttrDic.Add(data.effectType, data.effect_base);
				}
				return additionAttrDic;
			}
		}

        public Dictionary<FormationEffectType, float> fightFormationAttrDic
        {
            get
            {
                Dictionary<FormationEffectType, float> attrDic = formationAttrDic;
                if (isActiveAdditionAttr)
                {
                    List<FormationAttrData> attrDataList = FormationAttrData.GetFormationDatas(formationData.id, FormationAttrType.Addition);
                    FormationAttrData data;
                    for (int i = 0, count = attrDataList.Count; i < count; i++)
                    {
                        data = attrDataList[i];
                        if (attrDic.ContainsKey(data.effectType))
                            attrDic[data.effectType] += data.effect_base;
                        else
                            attrDic.Add(data.effectType, data.effect_base);
                    }
                }
                return attrDic;
            }
        }

        public float GetFightFormationAttrValue(FormationEffectType formationEffectType)
        {
            return fightFormationAttrDic[formationEffectType];
        }

        public float GetFormationAttrValue(FormationEffectType formationEffectType)
        {
            return formationAttrDic[formationEffectType];
        }

		public float GetFormationAdditionAttrValue (FormationEffectType formationEffectType)
		{
			return formationAdditionAttrDic[formationEffectType];
		}

        public List<FormationAttrData> GetFormationDatas()
        {
            List<FormationAttrData> result = FormationAttrData.GetFormationDatas(id, level);
            if (isActiveAdditionAttr)
            {
                List<FormationAttrData> additionFormationAttrDatas = FormationAttrData.GetFormationDatas(id, FormationAttrType.Addition);
                for (int i = 0, count = result.Count; i < count; i++)
                {
                    for (int j = 0, jCount = additionFormationAttrDatas.Count; j < jCount; j++)
                    {
                        FormationAttrData formationAttrData = additionFormationAttrDatas[j];
                        if (formationAttrData.effectType != result[i].effectType)
                            result.Add(formationAttrData);
                    }
                }
            }
            return result;
        }

        public float Power
        {
            get
            {
                float power = 0;
                List<FormationAttrData> attrDataList = FormationAttrData.GetFormationDatas(formationData.id);
                FormationAttrData data;
                for (int i = 0, count = attrDataList.Count; i < count; i++)
                {
                    data = attrDataList[i];
                    if (level >= data.unlock_lv)
                    {
                        power += data.comat_a + (level - 1) * data.comat_b;
                    }
                }
                return power;
            }
        }
    }
}
