using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    [SerializeField] private float spacingBetweenGrids = 1f;
    private Block[,] blockMatrix;
    private Vector3[,] positionMatrix;
    private List<int> affectedColumns = new();
    private int rowCount, columnCount;
    private int[,] startMatrix =
    {
        {0, 0, 1, 1, 1, 3},
        {1, 2, 2, 2, 2, 3},
        {1, 0, 3, 0, 2, 3},
        {1, 5, 5, 2, 2, 0},
        {0, 0, 2, 0, 0, 1},
        {1, 1, 4, 4, 3, 1}
    };
    #endregion

    #region Components
    private GridSwiper swiper;
    private GridConnectionFinder connectionFinder;
    private GridObjectSpawner objectSpawner;
    private BlockPool blockPool;
    #endregion

    #region Properties
    public Block[,] BlockMatrix => blockMatrix;
    public Vector3[,] PositionMatrix => positionMatrix;
    #endregion

    private void Awake()
    {
        swiper = GetComponent<GridSwiper>();
        connectionFinder = GetComponent<GridConnectionFinder>();
        objectSpawner = GetComponent<GridObjectSpawner>();
        blockPool = GetComponent<BlockPool>();
    }

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        rowCount = startMatrix.GetLength(0);
        columnCount = startMatrix.GetLength(1);
        blockMatrix = new Block[rowCount, columnCount];
        positionMatrix = new Vector3[rowCount, columnCount];

        Vector3 startPoint = transform.position - new Vector3((columnCount - 1) * spacingBetweenGrids / 2, (rowCount - 1) * spacingBetweenGrids / 2, 0);
        Vector3 targetPosition;

        for (int i = rowCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < columnCount; j++)
            {
                targetPosition = startPoint + new Vector3(j * spacingBetweenGrids, (rowCount - 1 - i) * spacingBetweenGrids, 0);
                positionMatrix[i, j] = targetPosition;
                SpawnABlock(targetPosition, new Vector2Int(i, j));
            }
        }
        connectionFinder.FindConnectedGroups();
    }

    public void BlastAGroup(List<Block> groupToBlast)
    {
        affectedColumns.Clear();
        for (int i = 0; i < groupToBlast.Count; i++)
        {
            //We store affected columns data to have more efficent FillEmptyIndexes method
            //We will only check affected columns for empty positions
            if (!affectedColumns.Contains(groupToBlast[i].GetPosition.y))
            {
                affectedColumns.Add(groupToBlast[i].GetPosition.y);
            }
            var position = groupToBlast[i].GetPosition;
            BlockMatrix[position.x, position.y] = null;
            groupToBlast[i].BlastTheBlock();
        }
        swiper.SwipeColumnsDown(groupToBlast);
        FillEmptyIndexes();
        connectionFinder.FindConnectedGroups();
    }

    private void FillEmptyIndexes()
    {
        foreach(var column in affectedColumns)
        {
            for(int i = 0;i<rowCount;i++)
            {
                if (blockMatrix[i, column] != null) continue;

                var index = new Vector2Int(i, column);
                Vector3 targetPosition = positionMatrix[i, column];
                SpawnABlock(targetPosition + Vector3.up * 5, index).SwipeDown(index, targetPosition);
            }
        }
    }

    private Block SpawnABlock(Vector3 targetPosition,Vector2Int index)
    {
        Block blockScript = blockPool.GetBlock();
        blockScript.transform.SetPositionAndRotation(targetPosition, transform.rotation);
        blockScript.InitializeBlock(index, (BlockColor)startMatrix[index.x, index.y]);
        blockMatrix[index.x, index.y] = blockScript;
        return blockScript;
    }
}