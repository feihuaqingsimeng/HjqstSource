using System.Collections.Generic;
using Logic.Enums;
using Common.ResMgr;
using Common.Util;
using UnityEngine;
#if UNITY_EDITOR
using System.Text;
using System.IO;
#endif
namespace Logic.Effect.Model
{
    public class EffectData
    {
        private static Dictionary<uint, EffectData> _effectDataDic;

        public static Dictionary<uint, EffectData> GetEffectDatas()
        {
            if (_effectDataDic == null)
            {
                _effectDataDic = CSVUtil.Parse<uint, EffectData>("config/csv/effect", "EffectId");
            }
            return _effectDataDic;
        }

        public static EffectData GetEffectDataById(uint id)
        {
            if (_effectDataDic == null)
                GetEffectDatas();
            if (_effectDataDic.ContainsKey(id))
                return _effectDataDic[id];
            Debugger.LogWarning("can't find effect id:" + id);
            return null;
        }

#if UNITY_EDITOR
        public static bool ExsitGetEffectData(uint id)
        {
            if (_effectDataDic == null)
                GetEffectDatas();
            return _effectDataDic.ContainsKey(id);
        }
#endif

        #region field
        [CSVElement("EffectId")]
        public uint effectId;

        [CSVElement("EffectName")]
        public string effectName;

        public Vector3 offset = Vector3.zero;
        [CSVElement("Offset")]
        public string offsetStr
        {
            set
            {
                offset = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public Vector3 endOffset = Vector3.zero;
        [CSVElement("EndOffset")]
        public string endOffsetStr
        {
            set
            {
                endOffset = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        [CSVElement("PartName")]
        public string partName;

        [CSVElement("MoveTime")]
        public float moveTime;

        [CSVElement("Delay")]
        public float delay;

        [CSVElement("Length")]
        public float length;

        public EffectType effectType;
        [CSVElement("EffectType")]
        public int effectTypeValue
        {
            set
            {
                effectType = (EffectType)value;
            }
        }

        [CSVElement("IgnoreTimeScale")]
        public bool ignoreTimeScale = false;

        public Color color;
        [CSVElement("Color")]
        public string colorStr
        {
            set
            {
                color = value.ToColor(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        [CSVElement("Random")]
        public string randomAnglesStr
        {
            set
            {
                randomAngles = new List<Vector3>();
                if (!string.IsNullOrEmpty(value))
                {
                    string[] result = value.ToArray(CSVUtil.SYMBOL_PIPE);
                    for (int i = 0, count = result.Length; i < count; i++)
                    {
                        randomAngles.Add(result[i].ToVector3(CSVUtil.SYMBOL_SEMICOLON));
                    }
                }
            }
        }

        public List<Vector3> randomAngles;

        public KeyValuePair<Vector3, float> curvePoint1;
        [CSVElement("CurvePoint1")]
        public string curvePoint1Str
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string[] result = value.ToArray(CSVUtil.SYMBOL_PIPE);
                    Vector3 vector3 = result[0].ToVector3(CSVUtil.SYMBOL_COLON);
                    float angle = 0f;
                    float.TryParse(result[1], out angle);
                    curvePoint1 = new KeyValuePair<Vector3, float>(vector3, angle);
                }
            }
        }

        public KeyValuePair<Vector3, float> curvePoint2;
        [CSVElement("CurvePoint2")]
        public string curvePoint2Str
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string[] result = value.ToArray(CSVUtil.SYMBOL_PIPE);
                    Vector3 vector3 = result[0].ToVector3(CSVUtil.SYMBOL_COLON);
                    float angle = 0f;
                    float.TryParse(result[1], out angle);
                    curvePoint2 = new KeyValuePair<Vector3, float>(vector3, angle);
                }
            }
        }

        public KeyValuePair<Vector3, float> curvePoint3;
        [CSVElement("CurvePoint3")]
        public string curvePoint3Str
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string[] result = value.ToArray(CSVUtil.SYMBOL_PIPE);
                    Vector3 vector3 = result[0].ToVector3(CSVUtil.SYMBOL_COLON);
                    float angle = 0f;
                    float.TryParse(result[1], out angle);
                    curvePoint3 = new KeyValuePair<Vector3, float>(vector3, angle);
                }
            }
        }

        [CSVElement("IsRotate")]
        public bool isRotate;

        public Vector3 scale;
        [CSVElement("Scale")]
        public string scaleStr
        {
            set
            {
                scale = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }

        public Vector3 rotate;
        [CSVElement("Rotate")]
        public string rotateStr
        {
            set
            {
                rotate = value.ToVector3(CSVUtil.SYMBOL_SEMICOLON);
            }
        }
        #endregion

#if UNITY_EDITOR
        public static bool SaveCSV()
        {
            string path = Application.dataPath + "/Res/Resources/config/csv/effect.csv";
            FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            #region 写出列名称
            sb.Append("EffectId,");
            sb.Append("EffectName,");
            sb.Append("Offset,");
            sb.Append("EndOffset,");
            sb.Append("PartName,");
            sb.Append("MoveTime,");
            sb.Append("Delay,");
            sb.Append("Length,");
            sb.Append("EffectType,");
            sb.Append("IgnoreTimeScale,");
            sb.Append("Color,");
            sb.Append("Random,");
            sb.Append("CurvePoint1,");
            sb.Append("CurvePoint2,");
            sb.Append("CurvePoint3,");
            sb.Append("IsRotate,");
            sb.Append("Scale,");
            sb.Append("Rotate");
            #endregion
            sw.WriteLine(sb.ToString());
            sb.Remove(0, sb.Length);
            #region 写出各行数据
            foreach (var kvp in _effectDataDic)
            {
                sb.Append(kvp.Value.effectId.ToString() + ",");
                sb.Append(kvp.Value.effectName.ToString() + ",");
                sb.Append(kvp.Value.offset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON) + ",");
                sb.Append(kvp.Value.endOffset.ToCustomString(CSVUtil.SYMBOL_SEMICOLON) + ",");
                sb.Append(kvp.Value.partName + ",");
                sb.Append(kvp.Value.moveTime.ToString() + ",");
                sb.Append(kvp.Value.delay.ToString() + ",");
                sb.Append(kvp.Value.length.ToString() + ",");
                sb.Append(((int)kvp.Value.effectType).ToString() + ",");
                sb.Append((kvp.Value.ignoreTimeScale ? 1 : 0).ToString() + ",");
                sb.Append(kvp.Value.color.ToCustomString(CSVUtil.SYMBOL_SEMICOLON) + ",");
                if (kvp.Value.randomAngles.Count > 0)
                {
                    for (int i = 0, length = kvp.Value.randomAngles.Count; i < length; i++)
                    {
                        sb.Append(kvp.Value.randomAngles[i].ToCustomString(CSVUtil.SYMBOL_SEMICOLON));
                        if (i != length - 1)
                            sb.Append(CSVUtil.SYMBOL_PIPE);
                    }
                }
                sb.Append(",");
                if (kvp.Value.curvePoint1.Key == Vector3.zero && kvp.Value.curvePoint1.Value == 0)
                    sb.Append(",");
                else
                    sb.Append(kvp.Value.curvePoint1.Key.ToCustomString(CSVUtil.SYMBOL_COLON) + CSVUtil.SYMBOL_PIPE[0] + kvp.Value.curvePoint1.Value.ToString() + ",");
                if (kvp.Value.curvePoint2.Key == Vector3.zero && kvp.Value.curvePoint2.Value == 0)
                    sb.Append(",");
                else
                    sb.Append(kvp.Value.curvePoint2.Key.ToCustomString(CSVUtil.SYMBOL_COLON) + CSVUtil.SYMBOL_PIPE[0] + kvp.Value.curvePoint2.Value.ToString() + ",");
                if (kvp.Value.curvePoint3.Key == Vector3.zero && kvp.Value.curvePoint3.Value == 0)
                    sb.Append(",");
                else
                    sb.Append(kvp.Value.curvePoint3.Key.ToCustomString(CSVUtil.SYMBOL_COLON) + CSVUtil.SYMBOL_PIPE[0] + kvp.Value.curvePoint3.Value.ToString() + ",");
                sb.Append((kvp.Value.isRotate ? "1" : "0") + ",");
				sb.Append(kvp.Value.scale.ToCustomString(CSVUtil.SYMBOL_SEMICOLON) + ",");
				sb.Append(kvp.Value.rotate.ToCustomString(CSVUtil.SYMBOL_SEMICOLON));
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
            if (_effectDataDic != null)
            {
                _effectDataDic.Clear();
                _effectDataDic = null;
                GetEffectDatas();
            }
        }
#endif
    }
}