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
    public class AnimationData
    {
        private static Dictionary<uint, AnimationData> _animationDataDic;

        public static Dictionary<uint, AnimationData> GetAnimationDatas()
        {
            if (_animationDataDic == null)
            {
                _animationDataDic = CSVUtil.Parse<uint, AnimationData>("config/csv/animation", "SkillId");
            }
            return _animationDataDic;
        }

        public static AnimationData GetAnimationDataById(uint id)
        {
            if (_animationDataDic == null)
                GetAnimationDatas();
            if (_animationDataDic.ContainsKey(id))
                return _animationDataDic[id];
            Debugger.LogWarning("can't find AnimationData id:" + id);
            return null;
        }

        #region field
        [CSVElement("SkillId")]
        public uint skillId;

        [CSVElement("AnimName")]
        public string animName;

        public AnimType animType;
        [CSVElement("AnimType")]
        public int animTypeInt
        {
            set
            {
                animType = (AnimType)value;
            }
        }

        [CSVElement("Offset")]
        public float offset;

        [CSVElement("MoveTime")]//移动时间
        public float moveTime;

        [CSVElement("HitTime")]//攻击时间
        public float hitTime;

        [CSVElement("BackTime")]//回撤时间
        public float backTime;

        [CSVElement("EndShowTime")]
        public float endShowTime;//

        [CSVElement("Length")]
        public float length;

        [CSVElement("Run_Speed")]
        public float runSpeed;

        [CSVElement("IsTurn")]
        public bool isRotate;
        #endregion

        [CSVElement("CloseUp")]
        public bool closeup;

        [CSVElement("CloseUpOverTime")]
        public float closeupOverTime;

        [CSVElement("BlackScreen")]
        public uint blackScreen;

        [CSVElement("FullScreen")]
        public uint fullScreen;

        [CSVElement("BGEffect")]
        public uint bgEffect;

#if UNITY_EDITOR
        public static bool SaveCSV()
        {
            string path = Application.dataPath + "/Res/Resources/config/csv/animation.csv";
            Debugger.Log(path);
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            #region 写出列名称
            sb.Append("SkillId,");
            sb.Append("AnimName,");
            sb.Append("AnimType,");
            sb.Append("Offset,");
            sb.Append("MoveTime,");
            sb.Append("HitTime,");
            sb.Append("BackTime,");
            sb.Append("EndShowTime,");
            sb.Append("Length,");
            sb.Append("Run_Speed,");
            sb.Append("IsTurn,");
            sb.Append("CloseUp,");
            sb.Append("CloseUpOverTime,");
            sb.Append("BlackScreen,");
            sb.Append("FullScreen,");
            sb.Append("BGEffect");
            #endregion
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            #region 写出各行数据
            foreach (var kvp in _animationDataDic)
            {
                sb.Append(kvp.Value.skillId.ToString() + ",");
                sb.Append(kvp.Value.animName + ",");
                sb.Append(((int)kvp.Value.animType).ToString() + ",");
                sb.Append(kvp.Value.offset.ToString() + ",");
                sb.Append(kvp.Value.moveTime.ToString() + ",");
                sb.Append(kvp.Value.hitTime.ToString() + ",");
                sb.Append(kvp.Value.backTime.ToString() + ",");
                sb.Append(kvp.Value.endShowTime.ToString() + ",");
                sb.Append(kvp.Value.length.ToString() + ",");
                sb.Append(kvp.Value.runSpeed + ",");
                sb.Append((kvp.Value.isRotate ? "1" : "0") + ",");
                sb.Append((kvp.Value.closeup ? "1" : "0") + ",");
                sb.Append(kvp.Value.closeupOverTime.ToString() + ",");
                sb.Append(kvp.Value.blackScreen.ToString() + ",");
                sb.Append(kvp.Value.fullScreen.ToString() + ",");
                sb.Append(kvp.Value.bgEffect.ToString());
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
            if (_animationDataDic != null)
            {
                _animationDataDic.Clear();
                _animationDataDic = null;
                GetAnimationDatas();
            }
        }
#endif

    }

}