using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnCellClickedEventArgs> OnCellClicked;
    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayerChanged;

    [SerializeField]
    private PlayerType localPlayerType = PlayerType.None;
    [SerializeField]
    private NetworkVariable<PlayerType> currentPlayerType = new NetworkVariable<PlayerType>(PlayerType.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

    private void OnCurrentPlayerTypeValueChanged(PlayerType previousValue, PlayerType newValue)
    {
        OnCurrentPlayerChanged?.Invoke(this, EventArgs.Empty);
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayerType()
    {
        return currentPlayerType.Value;
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnCellRpc(int x, int y, PlayerType type)
    {
        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Not connected to a server");
            return;
        }

        if (type != GetCurrentPlayerType())
        {
            Debug.Log("Your turn " + type + " does not match current player " + GetCurrentPlayerType());
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

        // register to the event for both server and client
        currentPlayerType.OnValueChanged += OnCurrentPlayerTypeValueChanged;
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("OnNetworkDespawn clientID: " + NetworkManager.Singleton.LocalClientId + " server: " + IsServer);
        currentPlayerType.OnValueChanged -= OnCurrentPlayerTypeValueChanged;
    }

    private void OnClientConnected(ulong id)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            SetFirstPlayerRpc();

            TriggerOnGameStartedRpc();
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
        switch (GetCurrentPlayerType())
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
        if (type != GetCurrentPlayerType())
        {
            currentPlayerType.Value = type;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }
}
