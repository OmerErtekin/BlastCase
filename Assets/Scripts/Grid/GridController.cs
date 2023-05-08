using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class GridController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject blockPrefab, backgroundPrefab;
    [SerializeField] private Transform backgroudParent, gridObjectsParent;
    [SerializeField] private float spacingBetweenGrids = 1f;
    private Block[,] blockMatrix;
    private Vector3[,] positionMatrix;
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
    #endregion

    #region Properties
    public Block[,] BlockMatrix => blockMatrix;
    public Vector3[,] PositionMatrix => positionMatrix;
    #endregion

    void Start()
    {
        swiper = GetComponent<GridSwiper>();
        connectionFinder = GetComponent<GridConnectionFinder>();
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
                blockScript = Instantiate(blockPrefab, targetPosition, transform.rotation, gridObjectsParent).GetComponent<Block>();
                positionMatrix[i, j] = targetPosition;
                blockMatrix[i, j] = blockScript;
                blockScript.InitializeBlock(new Vector2Int(i, j), (BlockColor)testMatrix[i, j], BlockLevel.Default);
                blockScript.gameObject.name = $"{i} {j}";
            }
        }
    }

    public void BlastAGroup(List<Block> groupToBlast)
    {
        for (int i = 0; i < groupToBlast.Count; i++)
        {
            var position = groupToBlast[i].GetPosition;
            BlockMatrix[position.x, position.y] = null;
            groupToBlast[i].BlastTheBlock();
        }
        swiper.SwipeColumnsDown(groupToBlast);
        PrintTheMatrix();
    }

    private void PrintTheMatrix()
    {
        for(int i = 0;i<rowCount;i++)
        {
            var str = "";
            for(int j = 0;j <columnCount;j++)
            {
                str += blockMatrix[i, j];
            }
            Debug.Log(str);
        }
    }
}