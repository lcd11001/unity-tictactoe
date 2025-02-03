using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameVisualManager : MonoBehaviour
{
    [SerializeField]
    private Transform crossPrefab;
    [SerializeField]
    private Transform circlePrefab;

    [SerializeField]
    Transform parent;

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
        Transform obj = Instantiate(crossPrefab, GetGridWorldPosition(e.X, e.Y), Quaternion.identity);
        obj.SetParent(parent, false);
    }

    private Vector3 GetGridWorldPosition(int x, int y)
    {
        return new Vector3(startX + x * sizeX, startY + y * sizeY, 0);
    }
}
