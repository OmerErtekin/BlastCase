using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridSwiper : MonoBehaviour
{
    #region Components
    GridController gridController;
    #endregion

    #region Variables
    private Dictionary<int, List<Vector2Int>> columnGroups = new();
    #endregion

    #region Properties
    private Block[,] CurrentMatrix => gridController.BlockMatrix;
    private Vector3[,] PositionMatrix => gridController.PositionMatrix;
    private int RowCount => gridController.BlockMatrix.GetLength(0);
    #endregion

    private void Awake()
    {
        gridController = GetComponent<GridController>();
    }

    public void SwipeColumnsDown(List<Block> blocks)
    {
        GroupBlocksByColumn(blocks);
        foreach (var column in columnGroups)
        {
            SwipeTheColumnDown(column.Value);
        }
    }

    private void SwipeTheColumnDown(List<Vector2Int> destroyedPoses)
    {
        if (destroyedPoses[0].x == 0) return;
        //We can directly start from the deepest affected row since we know that the bottom rows are not affected and they will not swipe down
        for (int i = destroyedPoses[0].x; i >= 0; i--)
        {
            var currentBlock = CurrentMatrix[i, destroyedPoses[0].y];
            for (; ShouldSwipeDown(currentBlock);)
            {
                var pos = currentBlock.GetPosition;
                currentBlock.SwipeDown(new Vector2Int(pos.x + 1, pos.y), PositionMatrix[pos.x + 1, pos.y]);
                CurrentMatrix[pos.x, pos.y] = null;
                CurrentMatrix[pos.x + 1, pos.y] = currentBlock;
            }
        }
    }

    public void GroupBlocksByColumn(List<Block> blocks)
    {
        //We don't need to check every single column after a blast
        //I create dictionary for affected columns and the pieces that are destroyed in this column. And sort the group by blocks row index
        //After that we will use the first element of the group to check the deepest block
        //Then we will start swiping down from the block that one block above the destroyed block
        columnGroups.Clear();
        columnGroups = new Dictionary<int, List<Vector2Int>>();

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
    }

    private bool ShouldSwipeDown(Block current)
    {
        return current != null && current.GetPosition.x < RowCount - 1 && CurrentMatrix[current.GetPosition.x + 1, current.GetPosition.y] == null;
    }
}
