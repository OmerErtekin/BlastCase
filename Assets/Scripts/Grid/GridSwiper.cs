using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSwiper : MonoBehaviour
{
    #region Components
    GridController gridController;
    #endregion

    #region Variables
    private Block[,] CurrentMatrix => gridController.BlockMatrix;
    private Vector3[,] PositionMatrix => gridController.PositionMatrix;
    private int RowCount => gridController.BlockMatrix.GetLength(0);
    #endregion

    private void Awake()
    {
        gridController = GetComponent<GridController>();
    }

    public void SwipeTheColumnsDown(List<Vector2Int> destroyedPoses)
    {
        if (destroyedPoses[0].x == 0) return;

        int column = destroyedPoses[0].y;

        StartCoroutine(SwipeDownUntilNotNull(column, destroyedPoses[0].x));
    }

    public Dictionary<int, List<Vector2Int>> GroupBlocksByColumn(List<Block> blocks)
    {
        Dictionary<int, List<Vector2Int>> columnGroups = new Dictionary<int, List<Vector2Int>>();

        for (int i = 0; i < blocks.Count; i++)
        {
            int column = blocks[i].GetPosition.y;
            if (!columnGroups.ContainsKey(column))
            {
                columnGroups[column] = new List<Vector2Int>();
            }
            columnGroups[column].Add(blocks[i].GetPosition);
        }

        List<int> columnKeys = columnGroups.Keys.ToList();
        for (int i = 0; i < columnKeys.Count; i++)
        {
            int column = columnKeys[i];
            columnGroups[column] = columnGroups[column].OrderByDescending(block => block.x).ToList();
        }

        return columnGroups;
    }

    private IEnumerator SwipeDownUntilNotNull(int column, int deepestRow)
    {
        for (int i = deepestRow; i >= 0; i--)
        {
            var currentBlock = CurrentMatrix[i, column];
            while (ShouldSwipeDown(currentBlock))
            {
                var pos = currentBlock.GetPosition;
                currentBlock.SwipeDown(new Vector2Int(pos.x + 1, pos.y), PositionMatrix[pos.x + 1, pos.y]);
                CurrentMatrix[pos.x, pos.y] = null;
                CurrentMatrix[pos.x + 1, pos.y] = currentBlock;
                yield return null;
            }
        }
        yield return null;
    }

    private bool ShouldSwipeDown(Block current)
    {
        return current != null && current.GetPosition.x < RowCount - 1 && CurrentMatrix[current.GetPosition.x + 1, current.GetPosition.y] == null;
    }
}