using UnityEngine;
using System.Collections;
namespace Common.Components.Effect
{
    public class Ghost : MonoBehaviour
    {

        public Mesh m;
        public MeshRenderer mr;
        public float time;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(FadeAndDestoryCoroutine(m, mr, time));
        }

        private IEnumerator FadeAndDestoryCoroutine(Mesh m, MeshRenderer mr, float time)
        {
            yield return null;
            float t = 0;
            Color c1 = Color.black;
            Color c2 = new Color(0.3f, 0.3f, 0.3f);
            bool fadein = false;
            while (true)
            {
                yield return new WaitForEndOfFrame();


                if (!mr)
                    break;
                for (int i = 0; i < mr.materials.Length; i++)
                {
                    mr.materials[i].SetColor("_TintColor", Color.Lerp(c1, c2, t / time * 2));
                }
                t += Time.deltaTime;
                if (t > time / 2 && !fadein)
                {
                    t = 0;
                    Color temp = c1;
                    c1 = c2;
                    c2 = temp;
                    fadein = true;
                }
                else if (t > time / 2 && fadein)
                    break;
            }
            if (mr)
            {
                mr.enabled = false;

                Destroy(m);
                for (int i = 0; i < mr.materials.Length; i++)
                {
                    Destroy(mr.materials[i]);
                }
                Destroy(mr.gameObject);
            }
        }
    }

}