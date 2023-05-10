using UnityEngine;
using System;

public class GridReader : MonoBehaviour
{
    [Header("To test Shuffle mechanic, change the name with ShuffleLevelData\nYou can change levels in Resources folder")]

    [SerializeField] private string fileName = "LevelData";

    public int[,] LoadLevel()
    {
        TextAsset file = Resources.Load<TextAsset>(fileName);
        string[] lines = file.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;

        int[,] result = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split(',');
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = int.Parse(values[j]);
            }
        }
        return result;
    }
}
