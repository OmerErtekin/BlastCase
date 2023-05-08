using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionFinder : MonoBehaviour
{
    private int[,] matrix = {
        {1, 0, 3, 2, 2, 3},
        {1, 1, 1, 2, 3, 0},
        {0, 0, 2, 2, 0, 1},
        {1, 1, 2, 2, 3, 1}
    };

    private bool[,] visited;
    private int rowCount;
    private int colCount;
    private List<List<Vector2Int>> connectedGroups;

    void Start()
    {
        FindConnectedGroups();
    }

    private List<List<Vector2Int>> FindConnectedGroups()
    {
        rowCount = matrix.GetLength(0);
        colCount = matrix.GetLength(1);
        visited = new bool[rowCount, colCount];
        connectedGroups = new List<List<Vector2Int>>();

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                if (!visited[i, j])
                {
                    List<Vector2Int> group = new List<Vector2Int>();
                    DFS(i, j, matrix[i, j], group);
                    connectedGroups.Add(group);
                }
            }
        }

        return connectedGroups;
    }

    private void PrintConnectedGroups()
    {
        foreach (var group in connectedGroups)
        {
            if (group.Count > 0)
            {
                Debug.Log("Connected Group:");
                var str = "";
                foreach (var cell in group)
                {
                    str += " " + cell;
                }

                Debug.Log(str);
            }
        }
    }

    private void DFS(int row, int col, int value, List<Vector2Int> group)
    {
        if (row < 0 || col < 0 || row >= rowCount || col >= colCount || visited[row, col] || matrix[row, col] != value)
        {
            return;
        }

        visited[row, col] = true;
        group.Add(new Vector2Int(row, col));

        DFS(row - 1, col, value, group);
        DFS(row + 1, col, value, group);
        DFS(row, col - 1, value, group);
        DFS(row, col + 1, value, group);
    }
}
