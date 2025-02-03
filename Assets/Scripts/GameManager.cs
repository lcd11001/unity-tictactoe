using System;
using UnityEngine;

public class OnCellClickedEventArgs : EventArgs
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public OnCellClickedEventArgs(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler<OnCellClickedEventArgs> OnCellClicked;

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
        OnCellClicked?.Invoke(this, new OnCellClickedEventArgs(x, y));
    }
}
