using UnityEditor;

namespace Observers.Editor
{
    [CustomEditor(typeof(Observers.Facade))]
    public class FacadeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Observers.Facade facade = target as Observers.Facade;
            int i = 0;
            if (facade.EDITOR_ObserverMap != null)
            {
                foreach (var kv in facade.EDITOR_ObserverMap)
                {
                    EditorGUILayout.LabelField((++i).ToString(), string.Format("key:{0},valueNum:{1}", kv.Key, kv.Value.Count));
                }
            }
        }
         
        
    }
}