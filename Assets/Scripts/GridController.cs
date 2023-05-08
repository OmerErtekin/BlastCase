using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject blockPrefab, backgroundPrefab;
    [SerializeField] private Transform backgroudParent, gridObjectsParent;
    [SerializeField] private float spacingBetweenGrids = 1f;
    private Block[,] blockMatrix;
    private int rowCount, columnCount;

    private int[,] testMatrix = 
    {
        {0, 0, 1, 2, 2, 3},
        {1, 2, 2, 0, 0, 3},
        {1, 0, 3, 2, 2, 3},
        {1, 5, 5, 2, 3, 0},
        {0, 0, 2, 2, 0, 1},
        {1, 1, 4, 4, 3, 1}
    };
    #endregion

    #region Components
    private ConnectionFinder connectionFinder;
    #endregion

    #region Properties
    public Block[,] BlockMatrix => blockMatrix;
    #endregion

    void Start()
    {
        connectionFinder = GetComponent<ConnectionFinder>();
        CreateGrid();
        connectionFinder.FindConnectedGroups();
    }

    void Update()
    {

    }

    private void CreateGrid()
    {
        rowCount = testMatrix.GetLength(0);
        columnCount = testMatrix.GetLength(1);

        blockMatrix = new Block[rowCount, columnCount];

        Vector3 startPoint = transform.position - new Vector3((columnCount - 1) * spacingBetweenGrids / 2, (rowCount - 1) * spacingBetweenGrids / 2, 0);
        Vector3 targetPosition;
        Block blockScript;

        for (int i = rowCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < columnCount; j++)
            {
                targetPosition = startPoint + new Vector3(j * spacingBetweenGrids, (rowCount - 1 - i) * spacingBetweenGrids, 0);
                blockScript = Instantiate(blockPrefab, targetPosition, transform.rotation, gridObjectsParent).GetComponent<Block>();
                blockMatrix[i, j] = blockScript;
                blockScript.SetBlock(new Vector2Int(i,j),(BlockColor)testMatrix[i, j], BlockLevel.Default);
                blockScript.gameObject.name = $"{i} {j}";
            }
        }
    }
}
