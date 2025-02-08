using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button buttonHost;

    [SerializeField]
    private Button buttonClient;

    [SerializeField]
    private GameObject loadingPannel;

    [SerializeField]
    private GameObject waitingPannel;

    [Space(20)]
    [SerializeField]
    private bool useRelay = false;

    [SerializeField]
    private string joinCode;

    [Space(20)]
    [SerializeField]
    private bool useLobby = false;
    [SerializeField]
    private string lobbyId;

    void Awake()
    {
        if (useLobby)
        {
            buttonHost.onClick.AddListener(OnLobbyHostClicked);
            buttonClient.onClick.AddListener(OnLobbyClientClicked);
        }
        else if (useRelay)
        {
            buttonHost.onClick.AddListener(OnRelayHostClicked);
            buttonClient.onClick.AddListener(OnRelayClientClicked);
        }
        else
        {
            buttonHost.onClick.AddListener(OnHostClicked);
            buttonClient.onClick.AddListener(OnClientClicked);
        }
        loadingPannel.SetActive(false);
        waitingPannel.SetActive(false);
    }

    private void ShowLoading()
    {
        loadingPannel.SetActive(true);
        waitingPannel.SetActive(false);
    }

    private void ShowWaiting()
    {
        loadingPannel.SetActive(false);
        waitingPannel.SetActive(true);
    }

    private void HideButtons()
    {
        buttonHost.gameObject.SetActive(false);
        buttonClient.gameObject.SetActive(false);
    }

    private void OnClientClicked()
    {
        HideButtons();
        ShowLoading();
        bool success = NetworkManager.Singleton.StartClient();
        if (success)
        {
            ShowWaiting();
        }
    }

    private void OnHostClicked()
    {
        HideButtons();
        ShowLoading();
        bool success = NetworkManager.Singleton.StartHost();
        if (success)
        {
            ShowWaiting();
        }
    }

    private async void OnRelayHostClicked()
    {
        HideButtons();
        ShowLoading();
        joinCode = await RelayNetworkManagerSingleton.Instance.StartHostWithRelay(1);
        if (!string.IsNullOrEmpty(joinCode))
        {
            Debug.Log($"Join code: {joinCode}");
            ShowWaiting();
        }
        else
        {
            Debug.LogError("Failed to start host");
        }
    }

    private async void OnRelayClientClicked()
    {
        HideButtons();
        ShowLoading();
        bool success = await RelayNetworkManagerSingleton.Instance.StartClientWithRelay(joinCode);
        if (success)
        {
            Debug.Log("Connected to host");
            ShowWaiting();
        }
        else
        {
            Debug.LogError("Failed to connect to host");
        }
    }

    private async void OnLobbyHostClicked()
    {
        HideButtons();
        ShowLoading();
        lobbyId = await RelayNetworkManagerSingleton.Instance.CreateLobbyAndStartHost(2);
        if (!string.IsNullOrEmpty(lobbyId))
        {
            Debug.Log($"Lobby created: {lobbyId}");
            ShowWaiting();
        }
        else
        {
            Debug.LogError("Failed to create lobby");
        }
    }

    private async void OnLobbyClientClicked()
    {
        HideButtons();
        ShowLoading();
        bool success = await RelayNetworkManagerSingleton.Instance.QuickJoinRelayViaLobby();
        if (success)
        {
            Debug.Log("Connected to host");
            ShowWaiting();
        }
        else
        {
            Debug.LogError("Failed to connect to host");
        }
    }
}
