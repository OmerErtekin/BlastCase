using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject blockPrefab, backgroundPrefab;
    [SerializeField] private Transform backgroudParent, gridObjectsParent;
    [SerializeField] private float spacingBetweenGrids = 1f;
    private Block[,] blockMatrix;
    private Vector3[,] positionMatrix;
    private List<int> affectedColumns = new();
    private int rowCount, columnCount;
    private int[,] testMatrix =
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
        connectionFinder.FindConnectedGroups();
    }

    private void CreateGrid()
    {
        rowCount = testMatrix.GetLength(0);
        columnCount = testMatrix.GetLength(1);
        blockMatrix = new Block[rowCount, columnCount];
        positionMatrix = new Vector3[rowCount, columnCount];

        Vector3 startPoint = transform.position - new Vector3((columnCount - 1) * spacingBetweenGrids / 2, (rowCount - 1) * spacingBetweenGrids / 2, 0);
        Vector3 targetPosition;
        Block blockScript;

        for (int i = rowCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < columnCount; j++)
            {
                targetPosition = startPoint + new Vector3(j * spacingBetweenGrids, (rowCount - 1 - i) * spacingBetweenGrids, 0);
                blockScript = blockPool.GetBlock();
                blockScript.transform.SetPositionAndRotation(targetPosition,Quaternion.Euler(0,0,0));

                positionMatrix[i, j] = targetPosition;
                blockMatrix[i, j] = blockScript;
                blockScript.InitializeBlock(new Vector2Int(i, j), (BlockColor)testMatrix[i, j], BlockLevel.Default);
            }
        }
    }

    private void CreateNewBlockForPosition(Vector2Int index)
    {
        Vector3 targetPosition = positionMatrix[index.x, index.y];
        Block blockScript = blockPool.GetBlock();
        blockScript.transform.SetPositionAndRotation(targetPosition + Vector3.up * 5, Quaternion.Euler(0, 0, 0));
        blockScript.InitializeBlock(index,objectSpawner.GetColorToSpawn(index),BlockLevel.Default);
        blockScript.SwipeDown(index, targetPosition);
        blockMatrix[index.x, index.y] = blockScript;    
    }

    public void BlastAGroup(List<Block> groupToBlast)
    {
        affectedColumns.Clear();
        for (int i = 0; i < groupToBlast.Count; i++)
        {
            if (!affectedColumns.Contains(groupToBlast[i].GetPosition.y))
            {
                affectedColumns.Add(groupToBlast[i].GetPosition.y);
            }
            var position = groupToBlast[i].GetPosition;
            BlockMatrix[position.x, position.y] = null;
            groupToBlast[i].BlastTheBlock();
        }
        swiper.SwipeColumnsDown(groupToBlast);
        FillEmptyColumns();
        connectionFinder.FindConnectedGroups();
    }

    private void FillEmptyColumns()
    {
        foreach(var column in affectedColumns)
        {
            for(int i = 0;i<rowCount;i++)
            {
                if (blockMatrix[i,column] == null)
                {
                    CreateNewBlockForPosition(new Vector2Int(i, column));
                }
            }
        }
    }
}