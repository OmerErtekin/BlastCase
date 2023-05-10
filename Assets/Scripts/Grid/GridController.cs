using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    #region Variables
    [SerializeField] private float spacingBetweenGrids = 1f;
    private Block[,] blockMatrix;
    private Vector3[,] positionMatrix;
    private List<int> affectedColumns = new();
    private bool isReseting = false;
    private int rowCount, columnCount;
    private int[,] startMatrix;
    #endregion

    #region Components
    private SpawnColorDecider decider;
    private GridReader gridReader;
    private BlockPool blockPool;
    #endregion

    #region Properties
    public Block[,] BlockMatrix => blockMatrix;
    public Vector3[,] PositionMatrix => positionMatrix;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnGameStarted, CreateGrid);
        EventManager.StartListening(EventKeys.OnBlastRequested, BlastAGroup);
        EventManager.StartListening(EventKeys.OnSwipeDownCompleted, FillEmptyIndexes);
        EventManager.StartListening(EventKeys.OnShuffledGridReady, UpdateGridAfterShuffle);
        EventManager.StartListening(EventKeys.OnGridResetRequested, ResetGrid);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnGameStarted, CreateGrid);
        EventManager.StopListening(EventKeys.OnBlastRequested, BlastAGroup);
        EventManager.StopListening(EventKeys.OnSwipeDownCompleted, FillEmptyIndexes);
        EventManager.StopListening(EventKeys.OnShuffledGridReady, UpdateGridAfterShuffle);
        EventManager.StopListening(EventKeys.OnGridResetRequested, ResetGrid);
    }

    private void Awake()
    {
        decider = GetComponent<SpawnColorDecider>();
        blockPool = GetComponent<BlockPool>();
        gridReader = GetComponent<GridReader>();
        startMatrix = gridReader.LoadLevel();
    }

    private void CreateGrid(object[] obj = null)
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
                SpawnABlock((BlockColor)startMatrix[i,j],targetPosition,new Vector2Int(i,j));
            }
        }
        EventManager.TriggerEvent(EventKeys.OnGridCreated);
    }

    private void BlastAGroup(object[] obj = null)
    {
        var groupToBlast = (List<Block>)obj[0];
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
            groupToBlast[i].GetComponent<IBlastable>().Blast();
        }
        EventManager.TriggerEvent(EventKeys.OnBlastCompleted, new object[] { groupToBlast });
    }

    private void FillEmptyIndexes(object[] obj = null)
    {
        foreach(var column in affectedColumns)
        {
            for(int i = 0;i<rowCount;i++)
            {
                if (blockMatrix[i, column] != null) continue;

                var index = new Vector2Int(i, column);
                Vector3 targetPosition = positionMatrix[i, column];
                SpawnABlock(decider.GetColorToSpawn(index),targetPosition + Vector3.up * 5, index).SwipeDown(index, targetPosition);
            }
        }
        EventManager.TriggerEvent(EventKeys.OnFillColumnsCompleted);
    }

    private Block SpawnABlock(BlockColor color,Vector3 targetPosition,Vector2Int index)
    {
        Block blockScript = blockPool.GetBlock();
        blockScript.transform.SetPositionAndRotation(targetPosition, transform.rotation);
        blockScript.InitializeBlock(index,color);
        blockMatrix[index.x, index.y] = blockScript;
        return blockScript;
    }

    private void UpdateGridAfterShuffle(object[] obj = null)
    {
        Block[,] shuffledMatrix = (Block[,])obj[0];
        blockMatrix = shuffledMatrix;
        EventManager.TriggerEvent(EventKeys.OnShuffleCompleted);
    }

    private void ResetGrid(object[] obj = null)
    {
        if (isReseting) return;
        isReseting = true;
        StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                blockMatrix[i, j].Blast();
            }
        }
        yield return new WaitForSeconds(0.5f);
        CreateGrid();
        isReseting = false;
    }
}