using UnityEngine;
using System.Collections;
using Logic.Character;
using Logic.Game.Controller;
using System.Text;
using Common.Util;
using LuaInterface;


namespace Common.GameTime.Controller
{
    public class TimeController : SingletonMono<TimeController>
    {
        void Awake()
        {
            instance = this;
        }

        private float _systemTimeMark = float.NaN;
        private long _systemTimeTicks;
        private StringBuilder _sb = new StringBuilder();
        [NoToLua]
        public bool login { get; set; }

        //服务器时间戳，单位为100纳秒
        private long ServerTimeTicks100NS
        {
            get
            {
                return _systemTimeTicks + (long)((Time.realtimeSinceStartup - _systemTimeMark) * 10000);
            }
        }

        //服务器时间戳，单位为秒
        public long ServerTimeTicksSecond
        {
            get
            {
                return ServerTimeTicks100NS / 10000;
            }
        }

        public long ServerTimeTicksMillisecond
        {
            set
            {
                _systemTimeMark = Time.realtimeSinceStartup;
                _systemTimeTicks = value * 10;
            }
            get
            {
                return ServerTimeTicks100NS / 10;
            }
        }

        public int ServerTimeTicksMillisecondAfter9
        {
            set
            {
                if (!login) return;
                if (value == 0) return;
                string localTime = ServerTimeTicksMillisecond.ToString();
                if (_sb.Length > 0)
                    _sb.Remove(0, _sb.Length);
                //Debugger.Log(localTime);
                _sb.Append(StringUtil.GetTheFirst(localTime, localTime.Length - 9));
                //Debugger.Log(_sb.ToString());
                string valueStr = value.ToString();
                // 不足9位前面补0
                if (valueStr.Length < 9)
                {
                    int dValue = 9 - valueStr.Length;
                    for (int i = 0; i < dValue; i++)
                    {
                        _sb.Append("0");
                    }
                }
                _sb.Append(valueStr);
                long currentTime = 0;
                long.TryParse(_sb.ToString(), out currentTime);
                ServerTimeTicksMillisecond = currentTime;
            }
            get
            {
                if (!login) return 0;
                return StringUtil.GetAfter(ServerTimeTicksMillisecond.ToString(), 9).ToInt32();
            }
        }

        public System.DateTime ServerTime
        {
            get
            {
                return Common.Util.TimeUtil.FormatTime((int)ServerTimeTicksSecond);
            }
        }

        public long GetDiffTimeWithServerTimeInSecond(long milliSecond)
        {
            return (milliSecond / 1000) - ServerTimeTicksSecond;
        }

        private float _fightSkillTime;
        public float fightSkillTime
        {
            get
            {
                return _fightSkillTime;
            }
            set
            {
                _fightSkillTime = value;
            }
        }

        #region pause system
        private CharacterEntity _character;
        private bool _playerPause;//玩家暂停

        [NoToLua]
        public bool playerPause
        {
            get
            {
                return _playerPause;
            }
            set
            {
                _playerPause = value;
                GameController.instance.pause = value;
                if (value)
                    Logic.Fight.Controller.FightController.instance.Pause();
                else
                    Logic.Fight.Controller.FightController.instance.CancelPause();
            }
        }

        [NoToLua]
        public void Pause(CharacterEntity character)
        {
            _character = character;
            GameController.instance.pause = true;
        }

        [NoToLua]
        public void Pause()
        {
            _character = null;
            GameController.instance.pause = true;
        }

        [NoToLua]
        public void CancelPause()
        {
            _character = null;
            GameController.instance.pause = false;
        }

        [NoToLua]
        public bool IgnorePause(CharacterEntity character)
        {
            if (_playerPause) return false;
            if (Logic.Fight.Controller.FightController.instance.isComboing) return true;
            if (!GameController.instance.pause) return true;
            //if (!_character) return false;
            return character == _character;
        }
        #endregion
    }
}