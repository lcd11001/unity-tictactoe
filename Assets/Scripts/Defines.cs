using System;
using UnityEngine;

public class OnCellClickedEventArgs : EventArgs
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public PlayerType Type { get; private set; }

    public OnCellClickedEventArgs(int x, int y, PlayerType type)
    {
        X = x;
        Y = y;
        Type = type;
    }
}

public enum PlayerType
{
    None,
    Cross,
    Circle
}

public class OnGameWinnerArgs : EventArgs
{
    public PlayerType Winner { get; private set; }
    public Vector2Int StartPosition { get; private set; }
    public Vector2Int EndPosition { get; private set; }
    public float Angle { get; private set; }
    public OnGameWinnerArgs(PlayerType winner, Vector2Int startPost, Vector2Int endPosition, float angle)
    {
        Winner = winner;
        StartPosition = startPost;
        EndPosition = endPosition;
        Angle = angle;
    }
}