using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.ResMgr;
using Logic.Game;
using Logic.Enums;
using Logic.UI;
using Common.Util;


namespace Logic.Audio.Controller
{
    public class AudioController : SingletonMono<AudioController>
    {
        #region static field
        public const string SELECTDUNGEON = "SelectLevel";
        public const string MAINSCENE = "MainScene";
        public const string LOGIN = "Login";
        public const string WIN = "win";
        public const string FAIL = "fail";
        public const string CLICK = "click";
        public const string CHAT_SEND = "chat_send";
        public const string SKILL_CLICK = "skill_click";
        public const string SELECT = "select";
        public const int ROLE_VECTORY_AUDIO_ID = 30067;
        public const string OPEN_AUDIO_BG = "openAudioBg";
        public const string OPEN_AUDIO = "openAudio";
        public const string BATTLE_FAIL_AUDIO = "battle_fail";
        public const string BATTLE_START_AUDIO = "battle_start";
        public const string BATTLE_VICTORY_AUDIO = "battle_victory";
		//界面音效
		public const string open_box_audio = "openBox";
		public const string fireworks_audio = "fireworks";
		public const string account_level_up = "accountLevelUp";
		public const string starEvaluate_audio = "starEvaluate";
		public const string addExp_audio = "addExp";
#endregion

        private bool _isOpenAudioBg;
        public bool isOpenAudioBg
        {
            set
            {
                SetBGMusicState(value);
                _isOpenAudioBg = value;
            }
            get
            {
                return _isOpenAudioBg;
            }
        }
        private bool _isOpenAudio;
        public bool isOpenAudio
        {
            set
            {
                _isOpenAudio = value;
            }
            get
            {
                return _isOpenAudio;
            }
        }

        private bool _pushMsg;
        private bool _effectPlayable;

        AudioSource audioSourceBG;
		private List<AudioSource> audioList = new List<AudioSource>();
        void Awake()
        {
            instance = this;
        }
        public void SavePlayerPref()
        {
            PlayerPrefs.SetInt(OPEN_AUDIO_BG, isOpenAudioBg ? 1 : 0);
            PlayerPrefs.SetInt(OPEN_AUDIO, isOpenAudio ? 1 : 0);
        }

        // Use this for initialization
        void Start()
        {
            audioSourceBG = gameObject.AddComponent<AudioSource>();
            audioSourceBG.volume = 0.5f;
            audioSourceBG.loop = true;
            audioSourceBG.playOnAwake = false;
            audioSourceBG.Pause();

            _isOpenAudioBg = PlayerPrefs.GetInt(OPEN_AUDIO_BG, 1) == 1;
            _isOpenAudio = PlayerPrefs.GetInt(OPEN_AUDIO, 1) == 1;
        }

        public void SetBGMusicState(bool state)
        {
            if (audioSourceBG == null)
                return;
            if (state)
                audioSourceBG.Play();
            else
                audioSourceBG.Pause();
        }

        public void PlayBGMusic(Logic.Enums.FightType type)
        {
            string bgName = "";

            switch (type)
            {
                case Logic.Enums.FightType.PVE:
                case Logic.Enums.FightType.MineFight:
                    {
                        int random = Random.Range(0, 2);
                        if (random == 0)
                            bgName = "bgm_09_pve_2";
                        else
                            bgName = "bgm_08_pve_1";
                    }
                    break;
                case Logic.Enums.FightType.Arena:
                case Logic.Enums.FightType.DailyPVE:
                case Logic.Enums.FightType.Expedition:	 //远征
                    bgName = "bgm_10_pvp_1";
                    break;
                case Logic.Enums.FightType.WorldTree:   //世界树
                    bgName = "bgm_11_worldtree";
                    break;
                case Logic.Enums.FightType.FirstFight:
                case Logic.Enums.FightType.WorldBoss:   //世界Boss
                    bgName = "bgm_07_boss_1";
                    break;
            }
            audioSourceBG.clip = ResMgr.instance.Load<AudioClip>("audio/" + bgName);
            if (isOpenAudioBg)
                audioSourceBG.Play();
        }

        public void PlayBGMusic(string name)
        {
            if (!isOpenAudioBg) return;
            AudioClip clip = ResMgr.instance.Load<AudioClip>("audio/" + name);
            if (clip != audioSourceBG.clip)
            {
                audioSourceBG.clip = clip;
                audioSourceBG.Play();
            }
        }

		public void PlayAudio(string name, bool accelerate, float delay = 0f)
        {
            if (!isOpenAudio) return;
            if (!string.IsNullOrEmpty(name))
				StartCoroutine(PlayAudioCoroutine(name, delay, accelerate));
        }
		public void PlayAudioRepeat(string name,float delay = 0)
		{
			if (!isOpenAudio) return;
			if (!string.IsNullOrEmpty(name))
				StartCoroutine(PlayAudioCoroutine(name, delay, false,true));
		}
		private IEnumerator PlayAudioCoroutine(string name, float delay, bool accelerate,bool isRepeat = false)
        {
            if (delay > 0)
            {
                float time = Time.realtimeSinceStartup;
                if (accelerate)
                    delay /= GameSetting.instance.speed;
                float currentTime = time;
                while (Time.realtimeSinceStartup - time < delay)
                {
                    yield return null;
                    if (Common.GameTime.Controller.TimeController.instance.playerPause)
                        time += (Time.realtimeSinceStartup - currentTime);
                    currentTime = Time.realtimeSinceStartup;
                }
            }
            AudioClip clip = ResMgr.instance.Load<AudioClip>("audio/" + name);
            if (clip)
            {
				GameObject go = null;
				bool isCreate = true;

				if(isCreate)
				{
					go = new GameObject();
					go.name = name;
					AudioSource audioSource = go.AddComponent<AudioSource>();
					go.transform.SetParent(transform, false);
					audioSource.clip = clip;
					audioSource.rolloffMode = AudioRolloffMode.Linear;
					if (accelerate)
					{
						audioSource.pitch = Game.GameSetting.instance.speed;
						audioSource.volume = 0.7f;
					}
					else
					{
						audioSource.pitch = 1f;
						audioSource.volume = 0.9f;
					}
					audioSource.Play();
					audioList.Add(audioSource);
					float time = Time.realtimeSinceStartup;
					float length = clip.length;
					if (accelerate)
						length /= GameSetting.instance.speed;
					audioSource.loop = isRepeat;
					if (!isRepeat)
					{
						while (Time.realtimeSinceStartup - time < length)
							yield return null;
						if (audioSource)
							StopAudio(audioSource);
					}

				}
                
                
                
            }
        }

        public void StopAudio(string name)
        {
            AudioSource audioSource;
			for(int i = 0,count = audioList.Count;i<count;i++)
			{
				audioSource = audioList[i];
				if(audioSource.name.Equals(name))
				{
					UnityEngine.Object.Destroy(audioSource);
					UnityEngine.Object.Destroy(audioSource.gameObject);
					audioList.Remove(audioSource);
					break;
				}
			}
        }
		public void StopAudio(AudioSource audio)
		{
			AudioSource audioSource;
			for(int i = 0,count = audioList.Count;i<count;i++)
			{
				audioSource = audioList[i];
				if(audioSource == audio)
				{
					UnityEngine.Object.Destroy(audioSource);
					UnityEngine.Object.Destroy(audioSource.gameObject);
                    audioList.Remove(audioSource);
                    break;
                }
            }
        }
        
        public void StopAll()
        {
			List<AudioSource> audioSourceList = new List<AudioSource>();
			for(int i = 0,count = audioList.Count;i<count;i++)
			{
				audioSourceList.Add(audioList[i]);
            }
            for (int i = 0, count = audioSourceList.Count; i < count; i++)
            {
                if (audioSourceList[i])
                    UnityEngine.Object.Destroy(audioSourceList[i].gameObject);
            }
			audioSourceList.Clear();
            Clear();
        }

        public void Clear()
        {
			audioList.Clear();
        }

        void OnDestroy()
        {
            Clear();
        }
    }
}