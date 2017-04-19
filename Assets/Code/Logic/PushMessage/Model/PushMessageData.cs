using UnityEngine;
using System.Collections;
using Common.Util;
using System.Collections.Generic;
using System;
namespace Logic.PushMessage.Model
{
    public class PushMessageData
    {
        private static List<PushMessageData> _pushMessageDatas;

        public static List<PushMessageData> GetPushMessageDatas()
        {
            if (_pushMessageDatas == null)
            {
                _pushMessageDatas = CSVUtil.Parse<PushMessageData>("config/csv/push_message");
            }
            return _pushMessageDatas;
        }

        [CSVElement("id")]
        public int id;

        public DateTime time;
        [CSVElement("time")]
        public string timeStr
        {
            set
            {
                time = TimeUtil.FormatTime(value);
            }
        }

        [CSVElement("title")]
        public string title;

        [CSVElement("messinfo")]
        public string messageInfo;

        [CSVElement("register")]
        public bool register;
    }
}
