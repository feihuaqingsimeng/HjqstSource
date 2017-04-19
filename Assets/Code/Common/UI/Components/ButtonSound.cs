using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Logic.Enums;
using Logic.Audio.Controller;

namespace Common.UI.Components
{
    public class ButtonSound : MonoBehaviour, IPointerClickHandler
    {

        public ButtonSoundType type;


        public static ButtonSound Get(GameObject go)
        {
            if (go == null)
                return null;
            ButtonSound sound = go.GetComponent<ButtonSound>();
            if (sound == null)
                sound = go.AddComponent<ButtonSound>();
            return sound;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (type)
            {
                case ButtonSoundType.NormalClick:
                    AudioController.instance.PlayAudio(AudioController.CLICK, false);
                    break;
                case ButtonSoundType.Select:
                    AudioController.instance.PlayAudio(AudioController.SELECT, false);
                    break;
                case ButtonSoundType.SkillClick:
                    AudioController.instance.PlayAudio(AudioController.SKILL_CLICK, false);
                    break;
                case ButtonSoundType.Toggle:
                    AudioController.instance.PlayAudio(AudioController.SELECT, false);
                    break;
                case ButtonSoundType.ChatSend:
                    AudioController.instance.PlayAudio(AudioController.CHAT_SEND, false);
                    break;

            }
        }
    }
}

