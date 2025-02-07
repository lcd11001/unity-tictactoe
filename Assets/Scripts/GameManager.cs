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
    public event EventHandler<OnGameWinnerArgs> OnGameWinner;
    public event EventHandler OnGameRematch;
    public event EventHandler OnGameDraw;
    public event EventHandler OnPlayerScoreChanged;
    public event EventHandler<OnGameSoundArgs> OnGameSound;

    [SerializeField]
    private PlayerType localPlayerType = PlayerType.None;
    [SerializeField]
    private NetworkVariable<PlayerType> currentPlayerType = new NetworkVariable<PlayerType>(PlayerType.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField]
    private NetworkVariable<int> playerCrossScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField]
    private NetworkVariable<int> playerCircleScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private BoardGame boardGame;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Random.InitState(DateTime.Now.Millisecond);

            boardGame = new BoardGame(3);
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

    private void OnPlayerScoreValueChanged(int previousValue, int newValue)
    {
        Debug.Log("Player Cross: " + playerCrossScore.Value + " Player Circle: " + playerCircleScore.Value);
        OnPlayerScoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }

    public PlayerType GetCurrentPlayerType()
    {
        return currentPlayerType.Value;
    }

    public int GetPlayerScore(PlayerType type)
    {
        switch (type)
        {
            case PlayerType.Cross:
                return playerCrossScore.Value;
            case PlayerType.Circle:
                return playerCircleScore.Value;
            default:
                return 0;
        }
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnCellRpc(int x, int y, PlayerType type, RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log($"process click message from client id {clientId}");

        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Not connected to a server");
            return;
        }

        if (type != GetCurrentPlayerType())
        {
            Debug.Log("Your turn " + type + " does not match current player " + GetCurrentPlayerType());
            TriggerOnSoundToClient(SoundType.Error, clientId);
            return;
        }

        if (!boardGame.IsCellValid(x, y) || !boardGame.IsCellEmpty(x, y))
        {
            Debug.Log("Invalid cell " + y + ", " + x);
            TriggerOnSoundToClient(SoundType.Error, clientId);
            return;
        }

        Debug.Log("Clicked on cell " + y + ", " + x);
        boardGame.SetCell(x, y, type);
        OnCellClicked?.Invoke(this, new OnCellClickedEventArgs(x, y, type));
        TriggerOnSoundToEveryone(SoundType.Place);


        if (boardGame.IsBoardFull())
        {
            SetCurrentPlayerTypeRpc(PlayerType.None);
            Debug.Log("Draw");
            TriggerOnGameDrawRpc();
        }
        else if (boardGame.IsWinner(type))
        {
            SetCurrentPlayerTypeRpc(PlayerType.None);
            SetScore(type);
            Debug.Log("Winner " + type);
            //OnGameWinner?.Invoke(this, new OnGameWinnerArgs(type, boardGame.WinnerStart, boardGame.WinnerEnd));
            TriggerOnGameWinnerRpc(type, boardGame.WinnerStart, boardGame.WinnerEnd, boardGame.WinnerAngle);

            TriggerOnSoundToClient(SoundType.Win, clientId);
            TriggerOnSoundExceptClient(SoundType.Lose, clientId);
        }
        else
        {
            ChangePlayerRpc();
        }
    }

    private void SetScore(PlayerType type)
    {
        switch (type)
        {
            case PlayerType.Cross:
                playerCrossScore.Value++;
                break;
            case PlayerType.Circle:
                playerCircleScore.Value++;
                break;
        }
    }

    override public void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn clientID: " + NetworkManager.Singleton.LocalClientId + " server: " + IsServer);
        if (IsServer)
        {
            localPlayerType = PlayerType.Cross;
            playerCrossScore.Value = 0;
            playerCircleScore.Value = 0;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }


        // register to the event for both server and client
        currentPlayerType.OnValueChanged += OnCurrentPlayerTypeValueChanged;
        playerCircleScore.OnValueChanged += OnPlayerScoreValueChanged;
        playerCrossScore.OnValueChanged += OnPlayerScoreValueChanged;
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("OnNetworkDespawn clientID: " + NetworkManager.Singleton.LocalClientId + " server: " + IsServer);
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
        currentPlayerType.OnValueChanged -= OnCurrentPlayerTypeValueChanged;
        playerCircleScore.OnValueChanged -= OnPlayerScoreValueChanged;
        playerCrossScore.OnValueChanged -= OnPlayerScoreValueChanged;

        base.OnNetworkDespawn();
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
    public void RematchRpc()
    {
        Debug.Log("Rematch");
        boardGame.Reset();
        SetFirstPlayerRpc();

        TriggerOnGameRematchRpc();
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

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinnerRpc(PlayerType type, Vector2Int start, Vector2Int end, float angle)
    {
        OnGameWinner?.Invoke(this, new OnGameWinnerArgs(type, start, end, angle));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameDrawRpc()
    {
        OnGameDraw?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameRematchRpc()
    {
        OnGameRematch?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void TriggerOnSoundRpc(SoundType type, RpcParams rpcParams = default)
    {
        OnGameSound?.Invoke(this, new OnGameSoundArgs(type));
    }

    private void TriggerOnSoundToClient(SoundType type, ulong clientId)
    {
        TriggerOnSoundRpc(type, RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }

    private void TriggerOnSoundToEveryone(SoundType type)
    {
        TriggerOnSoundRpc(type, RpcTarget.ClientsAndHost);
    }

    private void TriggerOnSoundExceptClient(SoundType type, ulong clientId)
    {
        TriggerOnSoundRpc(type, RpcTarget.Not(clientId, RpcTargetUse.Temp));
    }
}
