using System.Collections;
using Common.Util;
using System.Collections.Generic;
using Logic.Enums;
#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System.Text;
#endif
namespace Logic.Skill.Model
{
    public class PassiveData
    {
        private static Dictionary<uint, PassiveData> _passiveDataDic;

        public static Dictionary<uint, PassiveData> GetPassiveDatas()
        {
            if (_passiveDataDic == null)
            {
                _passiveDataDic = CSVUtil.Parse<uint, PassiveData>("config/csv/passive", "SkillId");
            }
            return _passiveDataDic;
        }

        public static PassiveData GetPassiveDataById(uint id)
        {
            if (_passiveDataDic == null)
                GetPassiveDatas();
            if (_passiveDataDic.ContainsKey(id))
                return _passiveDataDic[id];
            Debugger.LogError("can't find PassiveData id:" + id);
            return null;
        }

        #region field
        [CSVElement("Id")]
        public uint passiveId;

        [CSVElement("Trigger")]
        public uint trigger;

        [CSVElement("SkillId")]
        public uint skillId;

        [CSVElement("MechanicId")]
        public uint mechanicId;
        #endregion

#if UNITY_EDITOR
        public static bool SaveCSV()
        {
            if (_passiveDataDic == null) return true;
            string path = Application.dataPath + "/Res/Resources/config/csv/passive.csv";
            Debugger.Log(path);
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            #region 写出列名称
            sb.Append("Id,");
            sb.Append("Trigger,");
            sb.Append("SkillId,");
            sb.Append("MechanicId");
            #endregion
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            #region 写出各行数据
            foreach (var kvp in _passiveDataDic)
            {
                sb.Append(kvp.Value.passiveId.ToString() + ",");
                sb.Append(kvp.Value.trigger.ToString() + ",");
                sb.Append(kvp.Value.skillId.ToString() + ",");
                sb.Append(kvp.Value.mechanicId.ToString());
                sb.Append(CSVUtil.SYMBOL_LINE[0]);
            }
            sw.WriteLine(sb.ToString());
            #endregion
            sw.Close();
            fs.Close();
            return true;
        }

        public static void ReImportCSV()
        {
            if (_passiveDataDic!=null)
            {
                _passiveDataDic.Clear();
                _passiveDataDic = null;
            GetPassiveDatas();
            }
        }
#endif
    }
}