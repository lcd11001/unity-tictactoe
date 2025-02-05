using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField]
    private Transform crossPrefab;
    [SerializeField]
    private Transform circlePrefab;
    [SerializeField]
    private Transform winnerPrefab;

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
        GameManager.Instance.OnGameWinner += OnGameWinner;
    }

    public override void OnDestroy()
    {
        GameManager.Instance.OnCellClicked -= OnCellClicked;
        GameManager.Instance.OnGameWinner -= OnGameWinner;
        base.OnDestroy();
    }

    private void OnCellClicked(object sender, OnCellClickedEventArgs e)
    {
        Debug.Log("Visual Clicked on cell " + e.Y + ", " + e.X);

        SpawnElementRpc(e.X, e.Y, e.Type);
    }

    private void OnGameWinner(object sender, OnGameWinnerArgs e)
    {
        Debug.Log("Visual Game winner is " + e.Winner);
        Debug.Log("Visual Winner start position is " + e.StartPosition.y + ":" + e.StartPosition.y);
        Debug.Log("Visual Winner end position is " + e.EndPosition.y + ":" + e.EndPosition.x);

        Vector2Int center = new Vector2Int((e.StartPosition.x + e.EndPosition.x) / 2, (e.StartPosition.y + e.EndPosition.y) / 2);
        float angle = Mathf.Atan2(e.EndPosition.y - e.StartPosition.y, e.EndPosition.x - e.StartPosition.x) * Mathf.Rad2Deg;
        SpawnWinnerLineRpc(center.x, center.y, angle);
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

    [Rpc(SendTo.Server)]
    private void SpawnWinnerLineRpc(int x, int y, float angle)
    {
        Debug.Log("Server Spawning winner line at " + y + ", " + x + " angle " + angle);
        // spawn an element through the network
        Transform obj = Instantiate(winnerPrefab, GetGridWorldPosition(x, y), Quaternion.Euler(0, 0, angle));
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
