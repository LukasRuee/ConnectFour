using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public int columns { get; private set; } = 7;
    [SerializeField] public int rows { get; private set; } = 6;

    [SerializeField] public Color playerColor1 { get; private set; } = Color.red;
    [SerializeField] public Color playerColor2 { get; private set; } = Color.yellow;
    [SerializeField] private Color playerBackgroundColor1 = Color.red;
    [SerializeField] private Color playerBackgroundColor2 = Color.yellow;

    [HideInInspector] public int[,] Board { get; private set; }
    [HideInInspector] public int CurrentPlayer { get; private set; } = 1;
    [HideInInspector] public List<Vector2Int> LastWinCells { get; private set; } = new List<Vector2Int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        Board = new int[columns, rows];
    }

    public bool IsColumnFull(int col) => Board[col, rows - 1] != 0;

    public int DropPiece(int col)
    {
        if (col < 0 || col >= columns || IsColumnFull(col)) return -1;

        for (int r = 0; r < rows; r++)
        {
            if (Board[col, r] == 0)
            {
                Board[col, r] = CurrentPlayer;
                return r;
            }
        }
        return -1;
    }

    public void SwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;

        Color targetBg = CurrentPlayer == 1 ? playerBackgroundColor1 : playerBackgroundColor2;
        StartCoroutine(CameraManager.Instance.LerpBackground(targetBg, 0.3f));
    }

    public bool CheckWin(int col, int row)
    {
        LastWinCells.Clear();
        int p = Board[col, row];
        if (p == 0) return false;

        Vector2Int[] dirs = {
            new Vector2Int(1,0), new Vector2Int(0,1),
            new Vector2Int(1,1), new Vector2Int(1,-1)
        };

        foreach (var d in dirs)
        {
            int count = 1 + CountDirection(col, row, d.x, d.y, p) + CountDirection(col, row, -d.x, -d.y, p);
            if (count >= 4)
            {
                CollectWinCells(col, row, d, p);
                return true;
            }
        }
        return false;
    }

    private int CountDirection(int col, int row, int dx, int dy, int p)
    {
        int count = 0;
        int x = col + dx, y = row + dy;
        while (x >= 0 && x < columns && y >= 0 && y < rows && Board[x, y] == p)
        {
            count++; x += dx; y += dy;
        }
        return count;
    }

    private void CollectWinCells(int col, int row, Vector2Int dir, int p)
    {
        LastWinCells.Clear();

        int x = col, y = row;
        while (x >= 0 && x < columns && y >= 0 && y < rows && Board[x, y] == p)
        {
            LastWinCells.Add(new Vector2Int(x, y));
            x -= dir.x; y -= dir.y;
        }

        LastWinCells.Reverse();

        x = col + dir.x; y = row + dir.y;
        while (x >= 0 && x < columns && y >= 0 && y < rows && Board[x, y] == p)
        {
            LastWinCells.Add(new Vector2Int(x, y));
            x += dir.x; y += dir.y;
        }
    }

    public bool IsBoardFull()
    {
        for (int c = 0; c < columns; c++)
            if (!IsColumnFull(c)) return false;
        return true;
    }

    public bool ThreatDetected(int col, int row)
    {
        int p = Board[col, row];
        Vector2Int[] dirs = {
            new Vector2Int(1,0), new Vector2Int(0,1),
            new Vector2Int(1,1), new Vector2Int(1,-1)
        };
        foreach (var d in dirs)
        {
            if (CountDirection(col, row, d.x, d.y, p) + CountDirection(col, row, -d.x, -d.y, p) >= 2)
                return true;
        }
        return false;
    }
}