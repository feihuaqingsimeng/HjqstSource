using Common.Localization;
using UnityEngine;
using  Common.Util;
using UnityEngine.UI;
namespace Logic.UI.FirstFightEnd.View
{
    public class FirstFightEndView : MonoBehaviour
    {
        [SerializeField]
        private Graphic[] _components;
        [SerializeField]
        private float duration=3f;

        [SerializeField]
        private float stayTime = 3f;

        public const string PREFAB_PATH = "ui/first_fight_end/first_fight_end_view";
        private static System.Action closeAction;
        public static void Open(System.Action autoCloseAction)
        {
            UI.UIMgr.instance.Open<FirstFightEndView>(PREFAB_PATH);
            closeAction = autoCloseAction;
        }

        private void Awake()
        {
            for (int i = 0, count = _components.Length; i < count; i++)
            {
                var com = _components[i];
                com.color = new Color(com.color.r, com.color.g, com.color.b, 0);
            }

            _components[1].GetComponent<UnityEngine.UI.Text>().text = Localization.Get("OP0");
        }
        private void OnEnable()
        {
            SetAlpha(0,0,1, duration, ()=>this.DelayAction(new WaitForSeconds(stayTime), () => enabled = false));
            //LeanTween.alpha(bg.gameObject, 1, 3);
            this.DelayAction(new WaitForSeconds(1), () =>  SetAlpha(1,0,1,2, ()=>this.DelayAction(new WaitForSeconds(stayTime), () => enabled = false)));
        }

        private void OnDisable()
        {
            SetAlpha(0,1, 0, 0.2f, /*() => Object.DestroyImmediate(gameObject)*/Close);
            SetAlpha(1,1, 0, 0.2f, /*() => Object.DestroyImmediate(gameObject)*/Close);
            //LeanTween.alpha(bg.gameObject, 0, 3);
            //this.DelayAction(new WaitForSeconds(3), () => Object.DestroyImmediate(gameObject));
            if (closeAction != null)
                closeAction();
            closeAction = null;
        }


        [ContextMenu("Close")]
        private void Close()
        {
            UI.UIMgr.instance.Close(PREFAB_PATH);
        }

        private void SetAlpha(int index,float from, float to, float duration, System.Action endAction)
        {
            Graphic com = _components[index];
            com.color = new Color(com.color.r, com.color.g, com.color.b, from);
            float tiemTemp = Time.time;
            this.DelayAction(new WaitForSeconds(duration), () =>
            {
                com.color = new Color(com.color.r, com.color.g, com.color.b, Mathf.Lerp(from, to, Mathf.Clamp01((Time.time - tiemTemp) / duration)));
            }, endAction);
        }
    }
}
