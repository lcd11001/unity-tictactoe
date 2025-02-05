public class BoardGame
{
    private PlayerType[,] board;
    private int size;

    public BoardGame(int size)
    {
        this.size = size;
        board = new PlayerType[size, size];
    }

    public void Reset()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                board[i, j] = PlayerType.None;
            }
        }
    }

    public bool IsCellEmpty(int x, int y)
    {
        return board[x, y] == PlayerType.None;
    }

    public bool IsCellValid(int x, int y)
    {
        return x >= 0 && x < size && y >= 0 && y < size;
    }

    public bool IsBoardFull()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (board[i, j] == PlayerType.None)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsWinner(PlayerType playerType)
    {
        // check rows
        for (int i = 0; i < size; i++)
        {
            if (board[i, 0] == playerType && board[i, 1] == playerType && board[i, 2] == playerType)
            {
                return true;
            }
        }
        // check columns
        for (int i = 0; i < size; i++)
        {
            if (board[0, i] == playerType && board[1, i] == playerType && board[2, i] == playerType)
            {
                return true;
            }
        }
        // check diagonals
        if (board[0, 0] == playerType && board[1, 1] == playerType && board[2, 2] == playerType)
        {
            return true;
        }
        if (board[0, 2] == playerType && board[1, 1] == playerType && board[2, 0] == playerType)
        {
            return true;
        }
        return false;
    }

    public void SetCell(int x, int y, PlayerType playerType)
    {
        board[x, y] = playerType;
    }
}
