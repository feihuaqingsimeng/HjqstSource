using System.Collections.Generic;
using Logic.Enums;
using Common.Util;
#if UNITY_EDITOR
using System.Text;
using UnityEngine;
using System.IO;
using LuaInterface;
#endif
namespace Logic.Skill.Model
{
    public class SkillData
    {
        private static Dictionary<uint, SkillData> _skillDataDic;

        public static Dictionary<uint, SkillData> GetSkillDatas()
        {
            if (_skillDataDic == null)
            {
                _skillDataDic = CSVUtil.Parse<uint, SkillData>("config/csv/skill", "SkillId");
            }
            return _skillDataDic;
        }

        public static SkillData GetSkillDataById(uint id)
        {
            if (_skillDataDic == null)
                GetSkillDatas();
            if (_skillDataDic.ContainsKey(id))
#if UNITY_EDITOR
            {
                _skillDataDic[id].editorCD = _skillDataDic[id].CD;
#endif
                return _skillDataDic[id];
#if UNITY_EDITOR
            }
#endif
            //Debugger.LogError("can't find skill id:" + id);
            return null;
        }

        #region field
        [CSVElement("SkillId")]
        public uint skillId;

        [CSVElement("SkillName")]
        public string skillName;

        [CSVElement("SkillDesc")]
        public string skillDesc;

        [CSVElement("SkillIcon")]
        public string skillIcon;

        public SkillType skillType;
        [CSVElement("SkillType")]
        public int skillTypeInt
        {
            set
            {
                skillType = (SkillType)value;
            }
        }

        [CSVElement("DesTypeIcon")]
        public string desTypeIconName;

        [CSVElement("DesTypeIcon2")]
        public string desTypeIconName2;

        [CSVElement("ShowTime")]
        public bool showTime;

        [CSVElement("SkillCd")]
        public float CD;

#if UNITY_EDITOR
        private float editorCD = 0f;
        private List<List<Triple<float, float, float>>> editorMechanicsValues;
#endif

        public uint[] effectIds;

        [CSVElement("EffectIds")]
        public string effectsIdsStr
        {
            set
            {
                effectIds = value.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public uint[] aoeFlyEffects;
        [CSVElement("AoeFlyEffect")]
        public string AoeFlyEffect
        {
            set
            {
                aoeFlyEffects = value.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }


        public uint[] aoeEffects;
        [CSVElement("AoeEffect")]
        public string AoeEffectStr
        {
            set
            {
                aoeEffects = value.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public List<uint> flyEffectIds;
        [CSVElement("FlyEffectIds")]
        public string flyEffectIdsStr
        {
            set
            {
                flyEffectIds = value.ToList<uint>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public Dictionary<float, List<uint>> timeline;

        [CSVElement("Timeline")]
        public string timelineStr
        {
            set
            {
                timeline = new Dictionary<float, List<uint>>();
                string[] timelines = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);
                for (int i = 0, count = timelines.Length; i < count; i++)
                {
                    string[] values = timelines[i].ToArray(CSVUtil.SYMBOL_COLON);
                    float time = 0;
                    float.TryParse(values[0], out time);
                    List<uint> list = values[1].ToList<uint>(CSVUtil.SYMBOL_PIPE);
                    timeline.Add(time, list);
                }
            }
        }

        public List<List<Triple<float, float, float>>> mechanicsValues;

        [CSVElement("MechanicsValue")]
        public string mechanicsValuesStr
        {
            set
            {
                mechanicsValues = new List<List<Triple<float, float, float>>>();
                string[] mechanicsValueArray = value.ToArray(CSVUtil.SYMBOL_SEMICOLON);
                for (int i = 0, count = mechanicsValueArray.Length; i < count; i++)
                {
                    string m = mechanicsValueArray[i];
                    string[] valueArray = m.ToArray(CSVUtil.SYMBOL_PIPE);
                    List<Triple<float, float, float>> list = new List<Triple<float, float, float>>();
                    for (int j = 0, jCount = valueArray.Length; j < jCount; j++)
                    {
                        float[] kvpValue = valueArray[j].ToArray<float>(CSVUtil.SYMBOL_AND);
                        Triple<float, float, float> triple = default(Triple<float, float, float>);
                        if (kvpValue.Length == 3)
                            triple = new Triple<float, float, float>(kvpValue[0], kvpValue[1], kvpValue[2]);
                        else if (kvpValue.Length == 2)
                            triple = new Triple<float, float, float>(kvpValue[0], kvpValue[1], 0);
                        list.Add(triple);
                    }
                    mechanicsValues.Add(list);
                }
#if UNITY_EDITOR
                editorMechanicsValues = mechanicsValues;
#endif
            }
        }

        [CSVElement("BootTime")]
        public float bootTime;

        public AttackableType attackableType;

        [CSVElement("Attackable")]
        public int AttackableInt
        {
            set
            {
                attackableType = (AttackableType)value;
            }
        }

        [CSVElement("AudioId")]
        public string audioIdStr
        {
            set
            {
                audioIds = value.ToList<int>(CSVUtil.SYMBOL_SEMICOLON);

            }
        }
        public List<int> audioIds;

        [CSVElement("AudioDelay")]
        public float audioDelay = 0f;

        //[CSVElement("PauseTime")]  cancel fight pause
        public float pauseTime = 0f;

        [CSVElement("PauseTime")] //for save pause time value
        public float pause;

        [CSVElement("FullScreenTime")]
        public float fullScreenTime;

        [CSVElement("FullScreenLength")]
        public float fullScreenLength;

        [CSVElement("BlackTime")]
        public float blackTime;

        [CSVElement("BlackLength")]
        public float blackLength;

        [CSVElement("CameraTime")]
        public float cameraTime;

        [CSVElement("CameraLength")]
        public float cameraLength;

        [CSVElement("Floatable")]
        public bool floatable;

        [CSVElement("Tumbleable")]
        public bool tumbleable;
        #endregion

#if UNITY_EDITOR
        [NoToLua]
        public static bool SaveCSV()
        {
            string path = Application.dataPath + "/Res/Resources/config/csv/skill.csv";
            Debugger.Log(path);
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            #region 写出列名称
            sb.Append("SkillId,");
            sb.Append("SkillName,");
            sb.Append("SkillDesc,");
            sb.Append("SkillIcon,");
            sb.Append("SkillType,");
            sb.Append("DesTypeIcon,");
            sb.Append("DesTypeIcon2,");
            sb.Append("ShowTime,");
            sb.Append("SkillCd,");
            sb.Append("EffectIds,");
            sb.Append("AoeFlyEffect,");
            sb.Append("AoeEffect,");
            sb.Append("FlyEffectIds,");
            sb.Append("Timeline,");
            sb.Append("MechanicsValue,");
            sb.Append("BootTime,");
            sb.Append("Attackable,");
            sb.Append("AudioId,");
            sb.Append("AudioDelay,");
            sb.Append("PauseTime,");
            sb.Append("FullScreenTime,");
            sb.Append("FullScreenLength,");
            sb.Append("BlackTime,");
            sb.Append("BlackLength,");
            sb.Append("CameraTime,");
            sb.Append("CameraLength,");
            sb.Append("Floatable,");
            sb.Append("Tumbleable");
            #endregion
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            #region 写出各行数据
            foreach (var kvp in _skillDataDic)
            {
                sb.Append(kvp.Value.skillId.ToString() + ",");
                sb.Append(kvp.Value.skillName + ",");
                sb.Append(kvp.Value.skillDesc + ",");
                sb.Append(kvp.Value.skillIcon + ",");
                sb.Append(((int)kvp.Value.skillType).ToString() + ",");
                sb.Append(kvp.Value.desTypeIconName + ",");
                sb.Append(kvp.Value.desTypeIconName2 + ",");
                sb.Append((kvp.Value.showTime ? "1" : "0") + ",");
                if (kvp.Value.editorCD == 0)
                    sb.Append(kvp.Value.CD.ToString() + ",");
                else
                    sb.Append(kvp.Value.editorCD.ToString() + ",");
                sb.Append(kvp.Value.effectIds.ToCustomString(CSVUtil.SYMBOL_SEMICOLON) + ",");
                sb.Append(kvp.Value.aoeFlyEffects.ToCustomString(CSVUtil.SYMBOL_SEMICOLON) + ",");
                sb.Append(kvp.Value.aoeEffects.ToCustomString(CSVUtil.SYMBOL_SEMICOLON) + ",");
                sb.Append(kvp.Value.flyEffectIds.ToCustomString(CSVUtil.SYMBOL_SEMICOLON) + ",");
                StringBuilder sbTimeline = new StringBuilder();
                int i = 0;
                foreach (var t in kvp.Value.timeline)
                {
                    i++;
                    sbTimeline.Append(t.Key);
                    sbTimeline.Append(CSVUtil.SYMBOL_COLON);
                    int j = 0;
                    foreach (var m in t.Value)
                    {
                        j++;
                        sbTimeline.Append(m.ToString());
                        if (j < t.Value.Count)
                            sbTimeline.Append(CSVUtil.SYMBOL_PIPE);
                    }
                    if (i < kvp.Value.timeline.Count)
                        sbTimeline.Append(CSVUtil.SYMBOL_SEMICOLON);
                }
                sb.Append(sbTimeline.ToString() + ",");
                StringBuilder sbMechanicsValues = new StringBuilder();
                int k = 0;
                foreach (var m in kvp.Value.editorMechanicsValues)
                {
                    k++;
                    int x = 0;
                    foreach (var vp in m)
                    {
                        x++;
                        if (vp.c > 0)
                        {
                            sbMechanicsValues.Append(vp.a.ToString());
                            sbMechanicsValues.Append(CSVUtil.SYMBOL_AND);
                            sbMechanicsValues.Append(vp.b.ToString());
                            sbMechanicsValues.Append(CSVUtil.SYMBOL_AND);
                            sbMechanicsValues.Append(vp.c.ToString());
                        }
                        else
                        {
                            sbMechanicsValues.Append(vp.a.ToString());
                            sbMechanicsValues.Append(CSVUtil.SYMBOL_AND);
                            sbMechanicsValues.Append(vp.b.ToString());
                        }
                        if (x < m.Count)
                            sbMechanicsValues.Append(CSVUtil.SYMBOL_PIPE);
                    }
                    if (k < kvp.Value.mechanicsValues.Count)
                        sbMechanicsValues.Append(CSVUtil.SYMBOL_SEMICOLON);
                }
                sb.Append(sbMechanicsValues.ToString() + ",");
                sb.Append(kvp.Value.bootTime.ToString() + ",");
                sb.Append(((int)kvp.Value.attackableType).ToString() + ",");
                sb.Append(kvp.Value.audioIds.ToCustomString(CSVUtil.SYMBOL_SEMICOLON) + ",");
                sb.Append(kvp.Value.audioDelay.ToString() + ",");
                sb.Append(kvp.Value.pause.ToString() + ",");
                sb.Append(kvp.Value.fullScreenTime.ToString() + ",");
                sb.Append(kvp.Value.fullScreenLength.ToString() + ",");
                sb.Append(kvp.Value.blackTime.ToString() + ",");
                sb.Append(kvp.Value.blackLength.ToString() + ",");
                sb.Append(kvp.Value.cameraTime.ToString() + ",");
                sb.Append(kvp.Value.cameraLength.ToString() + ",");
                sb.Append((kvp.Value.floatable ? "1" : "0") + ",");
                sb.Append((kvp.Value.tumbleable ? "1" : "0"));
                sb.Append(CSVUtil.SYMBOL_LINE[0]);
            }
            sw.WriteLine(sb.ToString());
            #endregion
            sw.Close();
            fs.Close();
            return true;
        }

        [NoToLua]
        public static void ReImportCSV()
        {
            if (_skillDataDic != null)
            {
                _skillDataDic.Clear();
                _skillDataDic = null;
                GetSkillDatas();
            }
        }
#endif
    }
}
