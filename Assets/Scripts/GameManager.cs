using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnCellClickedEventArgs> OnCellClicked;

    private PlayerType localPlayerType = PlayerType.None;

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

    public void ClickedOnCell(int x, int y)
    {
        Debug.Log("Clicked on cell " + y + ", " + x);
        OnCellClicked?.Invoke(this, new OnCellClickedEventArgs(x, y, localPlayerType));
    }

    override public void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn clientID: " + NetworkManager.Singleton.LocalClientId + " server: " + IsServer);
        if (IsServer)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }
    }
}
