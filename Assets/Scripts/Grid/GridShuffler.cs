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
    private HashSet<Block> movedBlocks = new ();
    private int[,] intGrid;
    private Block[,] shuffledGrid;
    private Coroutine shuffleRoutine;
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
        EventManager.StartListening(EventKeys.OnGridResetRequested, CancelShuffle);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnShuffleRequested, ShuffleGrid);
        EventManager.StopListening(EventKeys.OnGridResetRequested, CancelShuffle);
    }

    private void Awake()
    {
        gridController = GetComponent<GridController>();
    }

    private void CancelShuffle(object[] obj = null)
    {
        if(shuffleRoutine != null)
        {
            StopCoroutine(shuffleRoutine);
        }
    }

    private void ShuffleGrid(object[] obj = null)
    {
        ConvertGridToInt();
        if (HasValidMove())
        {
            EventManager.TriggerEvent(EventKeys.OnShuffleCompleted);
            return;
        }
        shuffleRoutine = StartCoroutine(ShuffleGridUntilValid());
    }

    private IEnumerator ShuffleGridUntilValid()
    {
        //To give some time to player for understanding there is no match
        yield return new WaitForSeconds(1f);
        while (!HasValidMove())
        {
            FisherYatesShuffle();
            yield return null;
        }
        UpgradeGridPositions();
        //To wait until doMove finish
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
                correctBlock.MoveToShuffledPosition(newPosition, PositionMatrix[i, j]);
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
        int value;
        for (int x = 0; x < RowCount; x++)
        {
            for (int y = 0; y < ColumnCount; y++)
            {
                value = intGrid[x, y];
                if (GetValueOfPosition(x, y + 1) == value)
                {
                    if (GetValueOfPosition(x, y - 1) == value || GetValueOfPosition(x, y + 2) == value)
                    {
                        return true;
                    }
                }

                if (GetValueOfPosition(x + 1, y) == value)
                {
                    if (GetValueOfPosition(x - 1, y) == value || GetValueOfPosition(x + 2, y) == value)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private int GetValueOfPosition(int rowIndex,int columnIndex)
    {
        if (rowIndex < 0 || rowIndex >= RowCount || columnIndex < 0 || columnIndex >= ColumnCount) return -1;

        return intGrid[rowIndex, columnIndex];
    }

    private void FisherYatesShuffle()
    {
        List<int> elements = new();

        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
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
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                intGrid[i, j] = elements[index++];
            }
        }

    }

}
