using UnityEngine;
using System.Collections;

namespace Logic.Model.View
{
    public class ModelInputer : MonoBehaviour
    {
        public bool canRotate = true;
        public Camera targetCamera;

        ModelBehaviour model;

        void OnEnable()
        {
            EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;
            EasyTouch.On_TouchStart += EasyTouch_On_DragStart;
            EasyTouch.On_TouchDown += EasyTouch_On_Drag;
            EasyTouch.On_TouchUp += EasyTouch_On_DragEnd;
        }
        void OnDisable()
        {
            EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
            EasyTouch.On_TouchStart -= EasyTouch_On_DragStart;
            EasyTouch.On_TouchDown -= EasyTouch_On_Drag;
            EasyTouch.On_TouchUp -= EasyTouch_On_DragEnd;
        }
        void EasyTouch_On_DragEnd(Gesture gesture)
        {
            if (model != null)
            {
                model.canRotate = false;
                model = null;
            }
        }

        void EasyTouch_On_Drag(Gesture gesture)
        {
            if (canRotate)
            {
                if (model != null)
                {
                    model.Rotate(gesture.deltaPosition.x);
                }
            }
        }

        void EasyTouch_On_DragStart(Gesture gesture)
        {
            if (canRotate)
            {
                GameObject go = SelectModel(gesture.position);
                if (go == null)
                    return;
                model = go.GetComponent<ModelBehaviour>();
                model.canRotate = true;
            }
        }

        void EasyTouch_On_SimpleTap(Gesture gesture)
        {
            GameObject go = SelectModel(gesture.position);
            if (go == null)
                return;
            model = go.GetComponent<ModelBehaviour>();
            model.ClickBehavior();
        }
        
        private GameObject SelectModel(Vector2 pos)
        {
            Ray ray;
            int layer = 0;
            if (targetCamera)
            {
                ray = targetCamera.ScreenPointToRay(pos);
                layer = targetCamera.cullingMask;
            }
            else
            {
                ray = Logic.Cameras.Controller.CameraController.instance.uiCamera.ScreenPointToRay(pos);
                layer = Logic.Cameras.Controller.CameraController.instance.uiCamera.cullingMask;
            }
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
            {
                if (hit.collider.CompareTag("Character"))
                {
                    return hit.collider.gameObject;
                }
            }
            return null;
        }
    }
}
