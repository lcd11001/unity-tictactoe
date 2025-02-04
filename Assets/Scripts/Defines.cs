using System;

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