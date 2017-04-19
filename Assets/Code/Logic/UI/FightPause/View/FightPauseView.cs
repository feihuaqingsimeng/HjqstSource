using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.GameTime.Controller;
using Common.ResMgr;
namespace Logic.UI.FightPause.View
{
    public class FightPauseView : MonoBehaviour
    {
        public const string PREFAB_PATH = "ui/fight_pause/fight_pause_view";
        #region ui
        public Image musicImg;
        public Image soundImg;
        public Text musicTxt;
        public Text soundTxt;
        public Text resumeTxt;
        public Text quitFightTxt;
        #endregion
        private bool _music;
        private bool _sound;
        // Use this for initialization
        void Start()
        {
            _music = Audio.Controller.AudioController.instance.isOpenAudioBg;
            _sound = Audio.Controller.AudioController.instance.isOpenAudio;
            UpdateSoundImage();
            UpdateMusicImage();
            musicTxt.text = Common.Localization.Localization.Get("ui.fight_pause.btn_music");
            soundTxt.text = Common.Localization.Localization.Get("ui.fight_pause.btn_sound");
            resumeTxt.text = Common.Localization.Localization.Get("ui.fight_pause.btn_resume");
            quitFightTxt.text = Common.Localization.Localization.Get("ui.fight_pause.btn_quit_fight");
        }

        private void UpdateMusicImage()
        {
            string path = string.Empty;
            if (_music)
                path = "sprite/main_ui/btn_option_volume_open";
            else
                path = "sprite/main_ui/btn_option_volume_close";
			musicImg.SetSprite( ResMgr.instance.Load<Sprite>(path));
        }

        private void UpdateSoundImage()
        {
            string path = string.Empty;
            if (_sound)
                path = "sprite/main_ui/btn_option_sound_open";
            else
                path = "sprite/main_ui/btn_option_sound_close";
			soundImg.SetSprite(ResMgr.instance.Load<Sprite>(path));
        }

        #region ui handler
        public void ResumeBtnHandler()
        {
            TimeController.instance.playerPause = false;
            UI.UIMgr.instance.Close(PREFAB_PATH);
        }

        public void QuitFightBtnHandler()
        {
            TimeController.instance.playerPause = false;
            Logic.Net.Controller.DataMessageHandler.DataMessage_ForceFightFinished(false, Logic.Enums.FightOverType.ForceOver);
            UI.UIMgr.instance.Close(PREFAB_PATH);
        }

        public void MusicSwitchBtnHandler()
        {
            _music = !_music;
            UpdateMusicImage();
            Audio.Controller.AudioController.instance.isOpenAudioBg = _music;
            Logic.Audio.Controller.AudioController.instance.SavePlayerPref();
        }

        public void SoundSwitchBtnHandler()
        {
            _sound = !_sound;
            UpdateSoundImage();
            Audio.Controller.AudioController.instance.isOpenAudio = _sound;
            Logic.Audio.Controller.AudioController.instance.SavePlayerPref();
        }
        #endregion
    }
}
