using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
namespace Logic.Position.Model
{
    public class PositionData
    {
        private static Dictionary<uint, PositionData> _postionDataDic;

        public static Dictionary<uint, PositionData> GetPostionsData()
        {
            if (_postionDataDic == null)
            {
                _postionDataDic = CSVUtil.Parse<uint, PositionData>("config/csv/positiondata", "positionId");
            }
            return _postionDataDic;
        }

        public static PositionData GetPostionDataById(uint id)
        {
            if (_postionDataDic == null)
                GetPostionsData();
            if (_postionDataDic.ContainsKey(id))
                return _postionDataDic[id];
            Debugger.LogError("can't find position id:" + id);
            return null;
        }

        [CSVElement("positionId")]
        public uint positionId;

        [CSVElement("position")]
        public string pos
        {
            set
            {
                position = value.ToVector3(CSVUtil.SYMBOL_COLON);
            }
        }

        public Vector3 position;

        [CSVElement("rowNum")]
        public float rowNum;

        [CSVElement("columnNum")]
        public float columnNum;

        public static float GetEnemyPositionLevels(uint selfId, uint targetId)
        {
            float result = GetPostionDataById(selfId).columnNum + GetPostionDataById(targetId).columnNum - 2;
            return Mathf.Abs(result) - 1;
        }

        public static Vector3 GetPos(uint positionId)
        {
            if (_postionDataDic == null)
                GetPostionsData();
            if (_postionDataDic.ContainsKey(positionId))
                return _postionDataDic[positionId].position;
            return Vector3.zero;
        }

        public static List<uint> GetRows(int rowNum, bool isPlayer)
        {
            List<uint> result = new List<uint>();
            if (_postionDataDic == null)
                GetPostionsData();
            List<PositionData> list = _postionDataDic.GetValues();
            for (int i = 0, count = list.Count; i < count; i++)
            {
                PositionData pd = list[i];
                if (isPlayer)
                {
                    if (pd.positionId > 100)
                        continue;
                    if ((int)pd.rowNum == rowNum)
                        result.Add(pd.positionId);
                }
                else
                {
                    if (pd.positionId < 100)
                        continue;
                    if ((int)pd.rowNum == rowNum)
                        result.Add(pd.positionId);
                }
            }
            return result;
        }

        public static List<uint> GetColumns(int columnNum, bool isPlayer)
        {
            List<uint> result = new List<uint>();
            if (_postionDataDic == null)
                GetPostionsData();
            List<PositionData> list = _postionDataDic.GetValues();
            for (int i = 0, count = list.Count; i < count; i++)
            {
                PositionData pd = list[i];
                if (isPlayer)
                {
                    if (pd.positionId > 100)
                        continue;
                    if ((int)pd.columnNum == columnNum)
                        result.Add(pd.positionId);
                }
                else
                {
                    if (pd.positionId < 100)
                        continue;
                    if ((int)pd.columnNum == columnNum)
                        result.Add(pd.positionId);
                }
            }
            return result;
        }

        public override string ToString()
        {
            string result = string.Format("Id:{0},postion:{1},rowNum:{2},columnNum:{3}", positionId, position.ToString(), rowNum, columnNum);
            return result;
        }
    }
}
