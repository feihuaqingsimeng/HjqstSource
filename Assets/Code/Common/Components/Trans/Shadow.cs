//using UnityEngine;
//using System.Collections;
//namespace Common.Components.Trans
//{
//    public class Shadow : MonoBehaviour
//    {
//        public Transform trans;

//        // Update is called once per frame
//        void Update()
//        {
//            if (!trans)
//                UnityEngine.Object.Destroy(this);
//            if (trans.position != transform.position)
//                transform.position = trans.position + new Vector3(0f, -trans.position.y, 0f);
//        }
//    }
//}