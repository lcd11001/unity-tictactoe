public class BoardGame
{
    private PlayerType[,] board;
    private int size;

    public BoardGame(int size)
    {
        this.size = size;
        board = new PlayerType[size, size];
        this.Reset();
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
        if (CheckRows(playerType))
        {
            return true;
        }
        // check columns
        if (CheckColumns(playerType))
        {
            return true;
        }
        // check diagonals
        if (CheckDiagonal1(playerType))
        {
            return true;
        }
        if (CheckDiagonal2(playerType))
        {
            return true;
        }
        return false;
    }

    public void SetCell(int x, int y, PlayerType playerType)
    {
        board[x, y] = playerType;
    }

    private bool CheckRows(PlayerType playerType)
    {
        for (int i = 0; i < size; i++)
        {
            bool rowWin = true;

            for (int j = 0; j < size; j++)
            {
                if (board[i, j] != playerType)
                {
                    rowWin = false;
                    break;
                }
            }

            if (rowWin)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckColumns(PlayerType playerType)
    {
        for (int i = 0; i < size; i++)
        {
            bool columnWin = true;
            for (int j = 0; j < size; j++)
            {
                if (board[j, i] != playerType)
                {
                    columnWin = false;
                    break;
                }
            }
            if (columnWin)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckDiagonal1(PlayerType playerType)
    {
        for (int i = 0; i < size; i++)
        {
            if (board[i, i] != playerType)
            {
                return false;
            }
        }
        return true;
    }

    private bool CheckDiagonal2(PlayerType playerType)
    {
        for (int i = 0; i < size; i++)
        {
            if (board[i, size - i - 1] != playerType)
            {
                return false;
            }
        }
        return true;
    }
}
