using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnCellClickedEventArgs> OnCellClicked;
    public event EventHandler OnGameStarted;
    public event EventHandler<PlayerType> OnCurrentPlayerChanged;

    [SerializeField]
    private PlayerType localPlayerType = PlayerType.None;
    [SerializeField]
    private NetworkVariable<PlayerType> currentPlayerType = new NetworkVariable<PlayerType>(PlayerType.None);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Random.InitState(DateTime.Now.Millisecond);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnCellRpc(int x, int y, PlayerType type)
    {
        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Not connected to a server");
            return;
        }

        if (type != currentPlayerType.Value)
        {
            Debug.Log("Your turn " + type + " does not match current player " + currentPlayerType.Value);
            return;
        }

        Debug.Log("Clicked on cell " + y + ", " + x);
        OnCellClicked?.Invoke(this, new OnCellClickedEventArgs(x, y, type));

        ChangePlayerRpc();
    }

    override public void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn clientID: " + NetworkManager.Singleton.LocalClientId + " server: " + IsServer);
        if (IsServer)
        {
            localPlayerType = PlayerType.Cross;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }
    }

    private void OnClientConnected(ulong id)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            SetFirstPlayerRpc();

            TriggerOnGameStartedRpc();
            TriggerOnCurrentPlayerChangedRpc(currentPlayerType.Value);
        }
    }

    [Rpc(SendTo.Server)]
    private void RematchRpc()
    {
        Debug.Log("Rematch");

        SetFirstPlayerRpc();
    }

    [Rpc(SendTo.Server)]
    private void SetFirstPlayerRpc()
    {
        if (Random.Range(0, 2) == 0)
        {
            SetCurrentPlayerTypeRpc(PlayerType.Circle);
        }
        else
        {
            SetCurrentPlayerTypeRpc(PlayerType.Cross);
        }
    }

    [Rpc(SendTo.Server)]
    private void ChangePlayerRpc()
    {
        switch (currentPlayerType.Value)
        {
            case PlayerType.Cross:
                SetCurrentPlayerTypeRpc(PlayerType.Circle);
                break;
            case PlayerType.Circle:
                SetCurrentPlayerTypeRpc(PlayerType.Cross);
                break;
            default:
                Debug.LogError("Invalid player type");
                break;
        }
    }

    [Rpc(SendTo.Server)]
    private void SetCurrentPlayerTypeRpc(PlayerType type)
    {
        if (type != currentPlayerType.Value)
        {
            currentPlayerType.Value = type;
            TriggerOnCurrentPlayerChangedRpc(type);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnCurrentPlayerChangedRpc(PlayerType type)
    {
        OnCurrentPlayerChanged?.Invoke(this, type);
    }
}
