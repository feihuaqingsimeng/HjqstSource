using UnityEngine;
namespace Common.Tools
{
    [ExecuteInEditMode]
    public class MultiScreen : MonoBehaviour
    {
        public const float TARGET_WIDTH = 960f;
        public const float TARGET_HEIGHT = 640f;


        // Use this for initialization
        void Start()
        {
            Cal();
        }

        void Cal()
        {
            int width = Screen.width;
            int height = Screen.height;

            float aspect = width * 1.0f / height;
            if (aspect < TARGET_WIDTH / TARGET_HEIGHT)
            {
                //Adjust based on width
                transform.localScale = Vector3.one * aspect / TARGET_WIDTH * TARGET_HEIGHT;
            }
            else
            {
                /* Adjust based on height
                 * NGUI already do it
                 * So we do nothing here!
                 */
            }
        }

#if UNITY_EDITOR
        void Update()
        {
            Cal();
        }
#endif
    }
}