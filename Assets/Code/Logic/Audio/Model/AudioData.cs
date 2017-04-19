using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Util;
namespace Logic.Audio.Model
{
    public class AudioData
    {
        private static Dictionary<int, AudioData> _audioDataDic;

        public static Dictionary<int, AudioData> GetAudioDatas()
        {
            if (_audioDataDic == null)
            {
                _audioDataDic = CSVUtil.Parse<int, AudioData>("config/csv/audio", "id");
            }
            return _audioDataDic;
        }

        public static AudioData GetAudioDataById(int id)
        {
            if (_audioDataDic == null)
                GetAudioDatas();
            if (_audioDataDic.ContainsKey(id))
                return _audioDataDic[id];
            //Debugger.LogError("can't find audio id:" + id);
            return null;
        }

        public static List<AudioData> GetAuidoDatasByType(int type)
        {
            List<AudioData> result = new List<AudioData>();
            if (_audioDataDic == null)
                GetAudioDatas();
            foreach (var kvp in _audioDataDic)
            {
                if (kvp.Value.audioType == type)
                    result.Add(kvp.Value);
            }
            return result;
        }

        [CSVElement("id")]
        public int id;

        [CSVElement("type_id")]
        public int audioType;

        [CSVElement("name")]
        public string audioName;

        [CSVElement("accelerate")]
        public bool accelerate;
    }
}