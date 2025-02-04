using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : NetworkBehaviour
{
    public static NetworkSceneManager Instance { get; private set; }

    [Header("Scene Names")]
    private const string MAIN_SCENE = "Main";
    private const string GAME_SCENE = "Game";

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
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
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
            LoadNetworkGameScene();
        }
    }

    private void LoadNetworkGameScene()
    {
        var status = NetworkManager.Singleton.SceneManager.LoadScene(GAME_SCENE, LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogError($"Failed to load {GAME_SCENE} scene: {status}");
        }
    }
}
