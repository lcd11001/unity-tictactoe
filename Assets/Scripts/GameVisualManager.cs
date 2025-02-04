using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    public static GameVisualManager Instance { get; private set; }

    [SerializeField]
    private Transform crossPrefab;
    [SerializeField]
    private Transform circlePrefab;

    [SerializeField]
    NetworkObject parent;

    [SerializeField]
    private float startX = 0f;
    [SerializeField]
    private float startY = 0f;

    [SerializeField]
    private float sizeX = 0f;
    [SerializeField]
    private float sizeY = 0f;

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

    void Start()
    {
        GameManager.Instance.OnCellClicked += OnCellClicked;
    }

    private void OnCellClicked(object sender, OnCellClickedEventArgs e)
    {
        Debug.Log("Visual Clicked on cell " + e.Y + ", " + e.X);

        SpawnElementRpc(e.X, e.Y, e.Type);
    }

    [Rpc(SendTo.Server)]
    private void SpawnElementRpc(int x, int y, PlayerType type)
    {
        Debug.Log("Server Spawning element at " + y + ", " + x);

        // spawn an element through the network
        Transform obj = Instantiate(type == PlayerType.Cross ? crossPrefab : circlePrefab, GetGridWorldPosition(x, y), Quaternion.identity);
        NetworkObject networkObject = obj.GetComponent<NetworkObject>();
        networkObject.Spawn(true);

        // set the parent to the network object
        // and keep the local position
        networkObject.transform.SetParent(parent.transform, false);
    }

    private Vector3 GetGridWorldPosition(int x, int y)
    {
        return new Vector3(startX + x * sizeX, startY + y * sizeY, 0);
    }
}
