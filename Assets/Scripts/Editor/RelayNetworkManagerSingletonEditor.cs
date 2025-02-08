using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RelayNetworkManagerSingleton))]
public class RelayNetworkManagerSingletonEditor : Editor
{
    public override async void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Add a space in the Inspector
        EditorGUILayout.Space();

        // Reference to the RelayNetworkManagerSingleton instance
        RelayNetworkManagerSingleton relayManager = (RelayNetworkManagerSingleton)target;

        // Add the "Start Relay Host" button
        if (GUILayout.Button("Start Relay Host"))
        {
            // Call the method to start the relay host
            string joinCode = await relayManager.StartHostWithRelay();
            Debug.Log($"Join code: {joinCode}");
        }

        // Add a space in the Inspector
        EditorGUILayout.Space();

        if (GUILayout.Button("Start Relay Client"))
        {
            // Call the method to start the relay client
            bool connectionResult = await relayManager.StartClientWithRelay(relayManager.JoinCode);
            Debug.Log($"Connection result: {connectionResult}");
        }
    }
}
