using UnityEngine;

namespace Common.Effect
{
    public class LookAtCamera : MonoBehaviour
    {
        void Update()
        {
            if (Camera.main)
                transform.LookAt(Camera.main.transform);
        }
    }
}
