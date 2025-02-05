using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/using-networkscenemanager/
public class NetworkSceneManager : NetworkBehaviour
{
    public static NetworkSceneManager Instance { get; private set; }

#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_SceneName = SceneAsset.name;
        }
    }
#endif

    [SerializeField]
    private string m_SceneName;
    private Scene m_LoadedScene;

    public bool SceneIsLoaded
    {
        get
        {
            if (m_LoadedScene.IsValid() && m_LoadedScene.isLoaded)
            {
                return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // Remove this line to prevent NullReferenceException:
        //NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer && !string.IsNullOrEmpty(m_SceneName))
        {
            NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
        }

        base.OnNetworkSpawn();
    }


    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
        }
        base.OnDestroy();
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");

        // Only host/server handles scene loading
        if (!IsServer) return;

        // Wait for both players before starting
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            LoadNetworkScene();
        }
    }

    public void LoadNetworkScene()
    {
        var status = NetworkManager.Singleton.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
        CheckStatus(status, true);
    }

    public void UnloadNetworkScene()
    {
        // Assure only the server calls this when the NetworkObject is spawned and the scene is loaded.
        if (!IsServer || !IsSpawned || !m_LoadedScene.IsValid() || !m_LoadedScene.isLoaded)
        {
            return;
        }

        var status = NetworkManager.Singleton.SceneManager.UnloadScene(m_LoadedScene);
        CheckStatus(status, false);
    }

    private void CheckStatus(SceneEventProgressStatus status, bool isLoading)
    {
        var sceneEventAction = isLoading ? "load" : "unload";
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogError($"Failed to {sceneEventAction} {m_SceneName} with a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        // https://docs-multiplayer.unity3d.com/netcode/current/basics/scenemanagement/scene-events/
        var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";

        Debug.Log($"SceneName: {sceneEvent.SceneName} EventType: {sceneEvent.SceneEventType} {clientOrServer}ID: {sceneEvent.ClientId}");

        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadComplete:
                {
                    if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                    {
                        m_LoadedScene = sceneEvent.Scene;
                    }
                    break;
                }
        }
    }
}
