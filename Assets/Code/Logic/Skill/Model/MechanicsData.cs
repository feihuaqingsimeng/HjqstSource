using System.Collections;
using Common.Util;
using System.Collections.Generic;
using Logic.Enums;
#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System.Text;
using LuaInterface;
#endif
namespace Logic.Skill.Model
{
    public class MechanicsData
    {
        private static Dictionary<uint, MechanicsData> _mechanicsDataDic;

        public static Dictionary<uint, MechanicsData> GetMechanicsDatas()
        {
            if (_mechanicsDataDic == null)
            {
                _mechanicsDataDic = CSVUtil.Parse<uint, MechanicsData>("config/csv/mechanics", "Id");
            }
            return _mechanicsDataDic;
        }

        public static MechanicsData GetMechanicsDataById(uint id)
        {
            if (_mechanicsDataDic == null)
                GetMechanicsDatas();
            if (_mechanicsDataDic.ContainsKey(id))
                return _mechanicsDataDic[id];
            Debugger.LogWarning("can't find MechanicsData id:" + id);
            return null;
        }

        public static bool ExistMechanicsId(uint id)
        {
            if (_mechanicsDataDic == null)
                GetMechanicsDatas();
            return _mechanicsDataDic.ContainsKey(id);
        }

        #region field
        [CSVElement("Id")]
        public uint mechanicsId;

        public TargetType targetType;
        [CSVElement("TargetType")]
        public int targetInt
        {
            set
            {
                targetType = (TargetType)value;
            }
        }

        public RangeType rangeType;
        [CSVElement("RangeType")]
        public int rangeTypeInt
        {
            set
            {
                rangeType = (RangeType)value;
            }
        }

        [CSVElement("NeedTarget")]
        public bool needTarget;

        public MechanicsType mechanicsType;//技能效果类型
        [CSVElement("MechanicsType")]
        public int mechanicsTypeInt
        {
            set
            {
                mechanicsType = (MechanicsType)value;
            }
        }

        public uint[] effectIds;
        [CSVElement("EffectIds")]
        public string effectIdsStr
        {
            set
            {
                effectIds = value.ToArray<uint>(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        [CSVElement("AudioType")]
        public int audioType;

        [CSVElement("AudioDelay")]
        public float audioDelay = 0f;

        [CSVElement("MaxLayer")]
        public uint maxLayer;
        #endregion

#if UNITY_EDITOR
        [NoToLua]
        public static bool SaveCSV()
        {
            string path = Application.dataPath + "/Res/Resources/config/csv/mechanics.csv";
            Debugger.Log(path);
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            #region 写出列名称
            sb.Append("Id,");
            sb.Append("TargetType,");
            sb.Append("RangeType,");
            sb.Append("NeedTarget,");
            sb.Append("MechanicsType,");
            sb.Append("EffectIds,");
            sb.Append("AudioType,");
            sb.Append("AudioDelay,");
            sb.Append("MaxLayer");
            #endregion
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            #region 写出各行数据
            if (GetMechanicsDatas() != null)
                foreach (var kvp in _mechanicsDataDic)
                {
                    sb.Append(kvp.Value.mechanicsId.ToString() + ",");
                    sb.Append(((int)kvp.Value.targetType).ToString() + ",");
                    sb.Append(((int)kvp.Value.rangeType).ToString() + ",");
                    sb.Append((kvp.Value.needTarget ? 1 : 0).ToString() + ",");
                    sb.Append(((int)kvp.Value.mechanicsType).ToString() + ",");
                    sb.Append(kvp.Value.effectIds.ToCustomString(CSVUtil.SYMBOL_SEMICOLON).ToString() + ",");
                    sb.Append(kvp.Value.audioType + ",");
                    sb.Append(kvp.Value.audioDelay.ToString() + ",");
                    sb.Append(kvp.Value.maxLayer.ToString());
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
            if (_mechanicsDataDic != null)
            {
                _mechanicsDataDic.Clear();
                _mechanicsDataDic = null;
                GetMechanicsDatas();
            }
        }
#endif
    }
}