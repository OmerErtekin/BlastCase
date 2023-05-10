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

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnGridCreated, FindConnectedGroups);
        EventManager.StartListening(EventKeys.OnFillColumnsCompleted, FindConnectedGroups);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnGridCreated, FindConnectedGroups);
        EventManager.StopListening(EventKeys.OnFillColumnsCompleted, FindConnectedGroups);
    }

    private void Awake()
    {
        gridController = GetComponent<GridController>();
    }

    private void FindConnectedGroups(object obj = null)
    {
        visited = new bool[RowCount, ColumnCount];
        connectedGroups = new();
        int totalGroupCount = 0;

        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                if (visited[i, j] || CurrentMatrix[i,j] == null)
                    continue;

                List<Block> group = new();
                //For each element we make a depth first search to find the connected groups
                //If is there any connection, we store it
                DepthFirstSearch(i, j, CurrentMatrix[i, j].GetColor, group);
                if (group.Count > 1)
                {
                    totalGroupCount++;
                    connectedGroups.Add(group);
                }
            }
        }
        if(totalGroupCount > 0)
        {
            SetConnectedGroups();
        }
        else
        {
            EventManager.TriggerEvent(EventKeys.OnShuffleRequested, new object[] { });
        }
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

    private void DepthFirstSearch(int row, int col, BlockColor color, List<Block> group)
    {
        if (row < 0 || col < 0 || row >= RowCount || col >= ColumnCount ||
            visited[row, col] || CurrentMatrix[row, col].GetColor != color)
        {
            return;
        }

        visited[row, col] = true;
        group.Add(CurrentMatrix[row, col]);

        DepthFirstSearch(row - 1, col, color, group);
        DepthFirstSearch(row + 1, col, color, group);
        DepthFirstSearch(row, col - 1, color, group);
        DepthFirstSearch(row, col + 1, color, group);
    }
}
