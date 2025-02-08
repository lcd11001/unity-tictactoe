using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayNetworkManagerSingleton : MonoBehaviour
{
    public static RelayNetworkManagerSingleton Instance { get; private set; }

    [SerializeField]
    private string joinCode;

    public string JoinCode => joinCode;

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

    private async Task StartService()
    {
        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync();
        //Always authenticate your users beforehand
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    /// <summary>
    /// Creates a relay server allocation and start a host
    /// </summary>
    /// <param name="maxConnections">The maximum amount of clients that can connect to the relay</param>
    /// <returns>The join code</returns>
    public async Task<string> StartHostWithRelay(int maxConnections = 5)
    {
        await StartService();

        // Request allocation and join code
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        // Configure transport
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
        // Start host
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    /// <summary>
    /// Join a Relay server based on the JoinCode received from the Host or Server
    /// </summary>
    /// <param name="joinCode">The join code generated on the host or server</param>
    /// <returns>True if the connection was successful</returns>
    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        await StartService();

        // Join allocation
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        // Configure transport
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "dtls"));
        // Start client
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }

    public async Task<string> CreateLobbyAndStartHost(int maxConnections = 5)
    {
        joinCode = await StartHostWithRelay(maxConnections);

        try
        {
            // Create lobby and add joinCode to lobby data
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            };
            // Generate a unique lobby name using UUID
            string lobbyName = $"TicTacToe_{Guid.NewGuid()}";
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxConnections, options);
            if (lobby != null)
            {
                return lobby.Id;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return null;
    }

    public async Task<bool> QuickJoinRelayViaLobby()
    {
        await StartService();

        QuickJoinLobbyOptions options = new QuickJoinLobbyOptions
        {
            Filter = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
            }
        };

        try
        {
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);

            if (lobby != null && lobby.Data.TryGetValue("joinCode", out DataObject dataObject))
            {
                joinCode = dataObject.Value;
                // Use joinCode to join Relay
                return await RelayNetworkManagerSingleton.Instance.StartClientWithRelay(joinCode);
            }
            else
            {
                Debug.LogError("Join code not found in lobby data.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        return false;
    }
}