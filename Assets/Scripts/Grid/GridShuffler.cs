using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridShuffler : MonoBehaviour
{
    #region Components
    private GridController gridController;
    #endregion

    #region Variables
    HashSet<Block> movedBlocks = new ();
    private int[,] intGrid;
    private Block[,] shuffledGrid;
    #endregion

    #region Properties
    private Block[,] CurrentMatrix => gridController.BlockMatrix;
    private Vector3[,] PositionMatrix => gridController.PositionMatrix;
    private int RowCount => CurrentMatrix.GetLength(0);
    private int ColumnCount => CurrentMatrix.GetLength(1);
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnShuffleRequested, ShuffleGrid);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnShuffleRequested, ShuffleGrid);
    }

    private void Awake()
    {
        gridController = GetComponent<GridController>();
    }

    private void ShuffleGrid(object[] obj = null)
    {
        ConvertGridToInt();
        if (HasValidMove())
        {
            EventManager.TriggerEvent(EventKeys.OnShuffleCompleted, new object[] { });
            return;
        }
        StartCoroutine(ShuffleGridUntilValid());
    }

    private IEnumerator ShuffleGridUntilValid()
    {
        while (!HasValidMove())
        {
            FisherYatesShuffle();
            yield return null;
        }
        UpgradeGridPositions();

        yield return new WaitForSeconds(0.5f);
        EventManager.TriggerEvent(EventKeys.OnShuffledGridReady, new object[] { shuffledGrid });
    }

    private void ConvertGridToInt()
    {
        intGrid = new int[RowCount, ColumnCount];
        for(int i = 0;i<RowCount;i++)
        {
            for(int j = 0;j<ColumnCount;j++)
            {
                intGrid[i,j] = (int)CurrentMatrix[i,j].GetColor;
            }
        }
    }

    private void UpgradeGridPositions()
    {
        shuffledGrid = new Block[RowCount, ColumnCount];
        movedBlocks.Clear();
        Block correctBlock;
        Vector2Int newPosition;
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                correctBlock = FindBlockByColor((BlockColor)intGrid[i, j]);
                shuffledGrid[i, j] = correctBlock;
                newPosition = new Vector2Int(i, j);
                correctBlock.MoveToNewPosition(newPosition, PositionMatrix[i, j]);
            }
        }
    }

    private Block FindBlockByColor(BlockColor color)
    {
        //Since we are storing moved blocks in hashset, Contains() is much more faster.
        foreach (Block block in CurrentMatrix)
        {
            if (block.GetColor == color && !movedBlocks.Contains(block))
            {
                movedBlocks.Add(block);
                return block;
            }
        }
        return null;
    }

    private bool HasValidMove()
    {
        //To get the value without having out of bounds
        Func<int, int, int> get = (x, y) => (x >= 0 && x < RowCount && y >= 0 && y < ColumnCount) ? intGrid[x, y] : -1;

        for (int x = 0; x < RowCount; x++)
        {
            for (int y = 0; y < ColumnCount; y++)
            {
                int value = intGrid[x, y];
                if (get(x, y + 1) == value)
                {
                    if (get(x, y - 1) == value || get(x, y + 2) == value)
                    {
                        return true;
                    }
                }

                if (get(x + 1, y) == value)
                {
                    if (get(x - 1, y) == value || get(x + 2, y) == value)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void FisherYatesShuffle()
    {
        List<int> elements = new();

        for (int i = 0; i < intGrid.GetLength(0); i++)
        {
            for (int j = 0; j < intGrid.GetLength(1); j++)
            {
                elements.Add(intGrid[i, j]);
            }
        }

        int n = elements.Count;
        int randomIndex;
        int value;
        while (n > 1)
        {
            n--;
            randomIndex = UnityEngine.Random.Range(0, n + 1);
            value = elements[randomIndex];
            elements[randomIndex] = elements[n];
            elements[n] = value;
        }

        int index = 0;
        for (int i = 0; i < intGrid.GetLength(0); i++)
        {
            for (int j = 0; j < intGrid.GetLength(1); j++)
            {
                intGrid[i, j] = elements[index++];
            }
        }

    }

}
