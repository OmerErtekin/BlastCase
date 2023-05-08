using System.Collections.Generic;
using UnityEngine;

public class GridConnectionFinder : MonoBehaviour
{
    #region Components
    private GridController gridController;
    #endregion

    #region Variables
    private bool[,] visited;
    private List<List<Block>> connectedGroups;
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

    public void FindConnectedGroups()
    {
        visited = new bool[RowCount, ColumnCount];
        connectedGroups = new();

        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                if (visited[i, j])
                    continue;

                List<Block> group = new();
                DFS(i, j, CurrentMatrix[i, j].GetColor, group);
                if (group.Count > 1)
                {
                    connectedGroups.Add(group);
                }
            }
        }

        SetConnectedGroups();
    }

    private void SetConnectedGroups()
    {
        for (int i = RowCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                SetGroupForABlock(CurrentMatrix[i, j]);
            }
        }
    }

    private void SetGroupForABlock(Block blockToControl)
    {
        List<Block> selectedGroup = null;
        for(int i = 0; i < connectedGroups.Count;i++)
        {
            if (connectedGroups[i].Contains(blockToControl))
            {
                selectedGroup = connectedGroups[i];
                break;
            }
        }
        blockToControl.SetConnectedGroup(selectedGroup);
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

    private void DFS(int row, int col, BlockColor color, List<Block> group)
    {
        if (row < 0 || col < 0 || row >= RowCount || col >= ColumnCount || visited[row, col] || CurrentMatrix[row, col].GetColor != color)
        {
            return;
        }

        visited[row, col] = true;
        group.Add(CurrentMatrix[row, col]);

        DFS(row - 1, col, color, group);
        DFS(row + 1, col, color, group);
        DFS(row, col - 1, color, group);
        DFS(row, col + 1, color, group);
    }
}