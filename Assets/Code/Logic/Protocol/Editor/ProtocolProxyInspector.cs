using UnityEditor;

namespace Logic.Protocol.Editor
{
    [CustomEditor(typeof(Logic.Protocol.ProtocolProxy))]
    public class ProtocolProxyInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Logic.Protocol.ProtocolProxy protocolProxy = target as Logic.Protocol.ProtocolProxy;
            EditorGUILayout.LabelField("Connect:", protocolProxy.Connected.ToString());
            EditorGUILayout.LabelField("Host:", protocolProxy.EditorHost);
            EditorGUILayout.LabelField("Port:", protocolProxy.EditorPort.ToString());
        }
    }
}