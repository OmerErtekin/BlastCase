using System.Collections.Generic;
using UnityEngine;

public class ConnectionFinder : MonoBehaviour
{
    #region Components
    private GridController gridController;
    #endregion

    #region Variables
    private bool[,] visited;
    private List<List<Vector2Int>> connectedGroups;
    #endregion

    #region Properties
    private Block[,] CurrentMatrix => gridController.BlockMatrix;
    private int RowCount => gridController.BlockMatrix.GetLength(0);
    private int ColumnCount => gridController.BlockMatrix.GetLength(1);
    #endregion

    private void Awake()
    {
        gridController = GetComponent<GridController>();
    }

    public List<List<Vector2Int>> FindConnectedGroups()
    {
        visited = new bool[RowCount, ColumnCount];
        connectedGroups = new List<List<Vector2Int>>();

        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                if (visited[i, j])
                    continue;

                List<Vector2Int> group = new List<Vector2Int>();
                DFS(i, j, CurrentMatrix[i, j].GetColor, group);
                if (group.Count > 1)
                {
                    connectedGroups.Add(group);
                }
            }
        }

        PrintConnectedGroups();
        return connectedGroups;
    }

    public bool IsThereAConnection(Block blockToControl)
    {
        return true;
    }

    private void PrintConnectedGroups()
    {
        foreach (var group in connectedGroups)
        {
            var str = "";
            foreach (var cell in group)
            {
                str += " " + cell;
            }

            Debug.Log(str);
        }
    }

    private void DFS(int row, int col, BlockColor color, List<Vector2Int> group)
    {
        if (row < 0 || col < 0 || row >= RowCount || col >= ColumnCount || visited[row, col] || CurrentMatrix[row, col].GetColor != color)
        {
            return;
        }

        visited[row, col] = true;
        group.Add(new Vector2Int(row, col));

        DFS(row - 1, col, color, group);
        DFS(row + 1, col, color, group);
        DFS(row, col - 1, color, group);
        DFS(row, col + 1, color, group);
    }
}
