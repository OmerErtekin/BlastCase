using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridObjectSpawner : MonoBehaviour
{
    #region Components
    private GridController gridController;
    #endregion

    #region Variables
    [SerializeField] private GameConfig gameConfig;
    private Dictionary<BlockColor, float> surroundingColors = new();

    Dictionary<BlockColor, float> baseProbabilities = new Dictionary<BlockColor, float>()
{
    {BlockColor.Blue, 1.66f},
    {BlockColor.Green, 1.66f},
    {BlockColor.Pink, 1.66f},
    {BlockColor.Purple, 1.66f},
    {BlockColor.Red, 1.66f},
    {BlockColor.Yellow, 1.66f}
};
    Dictionary<BlockColor, float> adjustedProbabiltys = new Dictionary<BlockColor, float>()
{
    {BlockColor.Blue, 1.66f},
    {BlockColor.Green, 1.66f},
    {BlockColor.Pink, 1.66f},
    {BlockColor.Purple, 1.66f},
    {BlockColor.Red, 1.66f},
    {BlockColor.Yellow, 1.66f}
};

    private float cumulativeProbabilty;

    #endregion

    #region Properties
    private Block[,] CurrentMatrix => gridController.BlockMatrix;
    private int RowCount => gridController.BlockMatrix.GetLength(0);
    private int ColumnCount => gridController.BlockMatrix.GetLength(1);
    #endregion
    private void Awake()
    {
        gridController = GetComponent<GridController>();
    }

    public BlockColor GetColorToSpawn(Vector2Int position)
    {
        CalculateProbabiltys(position);
        cumulativeProbabilty = adjustedProbabiltys.Values.Sum();

        float randomNumber = Random.value * cumulativeProbabilty;
        foreach (var kvp in adjustedProbabiltys)
        {
            if (randomNumber <= kvp.Value)
            {
                return kvp.Key;
            }
            randomNumber -= kvp.Value;
        }

        return BlockColor.Blue;
    }

    private void CalculateProbabiltys(Vector2Int position)
    {
        SetSurroundingObjects(position);
        SetAdjustedProbabilitys();
    }

    private void SetSurroundingObjects(Vector2Int position)
    {
        surroundingColors.Clear();
        cumulativeProbabilty = 0;

        if (position.x != 0 && CurrentMatrix[position.x - 1, position.y])
        {
            AddColorToDictionary(CurrentMatrix[position.x - 1, position.y]);
        }
        if (position.y != 0 && CurrentMatrix[position.x, position.y - 1])
        {
            AddColorToDictionary(CurrentMatrix[position.x, position.y - 1]);
        }
        if (position.x != RowCount - 1 && CurrentMatrix[position.x + 1, position.y])
        {
            AddColorToDictionary(CurrentMatrix[position.x + 1, position.y]);
        }
        if (position.y != ColumnCount - 1 && CurrentMatrix[position.x, position.y + 1])
        {
            AddColorToDictionary(CurrentMatrix[position.x, position.y + 1]);
        }
    }

    private void SetAdjustedProbabilitys()
    {
        ResetAdjustedProbabiltys();
        foreach (var kvp in surroundingColors)
        {
            if (adjustedProbabiltys.ContainsKey(kvp.Key))
            {
                adjustedProbabiltys[kvp.Key] += kvp.Value * gameConfig.bonusForEachAdjacent;
            }
        }
    }

    private void AddColorToDictionary(Block block)
    {
        if (surroundingColors.ContainsKey(block.GetColor))
        {
            surroundingColors[block.GetColor]++;
            if(block.GetConnectedGroup() != null)
            {
                surroundingColors[block.GetColor]+= block.GetConnectedGroup().Count * gameConfig.bonusForConnecteds;
            }
        }
        else
        {
            surroundingColors[block.GetColor] = 1;
            if (block.GetConnectedGroup() != null)
            {
                surroundingColors[block.GetColor] += block.GetConnectedGroup().Count * gameConfig.bonusForConnecteds;
            }
        }
    }

    private void ResetAdjustedProbabiltys()
    {
        foreach (var key in adjustedProbabiltys.Keys.ToList())
        {
            adjustedProbabiltys[key] = baseProbabilities[key];
        }
    }

    private void PrintProbabiltys()
    {
        var str = "";
        foreach (var pair in adjustedProbabiltys)
        {
            str += $" {pair.Key} : {pair.Value}";
        }
        Debug.Log(str);
    }
}
