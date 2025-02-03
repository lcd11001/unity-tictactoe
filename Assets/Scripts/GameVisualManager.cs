using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
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

    void Start()
    {
        GameManager.Instance.OnCellClicked += OnCellClicked;
    }

    private void OnCellClicked(object sender, OnCellClickedEventArgs e)
    {
        Debug.Log("Visual Clicked on cell " + e.Y + ", " + e.X);

        SpawnElementRpc(e.X, e.Y);
    }

    [Rpc(SendTo.Server)]
    private void SpawnElementRpc(int x, int y)
    {
        Debug.Log("Server Spawning element at " + x + ", " + y);

        // spawn an element through the network
        Transform obj = Instantiate(crossPrefab);
        NetworkObject networkObject = obj.GetComponent<NetworkObject>();
        networkObject.Spawn(true);

        // set the position of the object
        obj.position = GetGridWorldPosition(x, y);

        // set the parent to the network object
        // and keep the local position
        networkObject.transform.SetParent(parent.transform, false);
    }

    private Vector3 GetGridWorldPosition(int x, int y)
    {
        return new Vector3(startX + x * sizeX, startY + y * sizeY, 0);
    }
}
