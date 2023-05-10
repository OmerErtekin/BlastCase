using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/ConfigSO", order = 1)]
public class GameConfig : ScriptableObject
{
    [Header("Block Settings")]
    public List<SpriteListForColor> spriteList;
    [Header("Min counts for icons")]
    public List<int> minCountsForLevels;

    [Header("Game Settings")]
    public float bonusForEachAdjacent = 5f;
    public float bonusForConnecteds = 0.1f;
}


[System.Serializable]
public class SpriteListForColor
{
    public List<Sprite> levelSprites;
}

public enum BlockColor
{
    Blue,
    Green,
    Pink,
    Purple,
    Red,
    Yellow
}

public enum BlockLevel
{
    Default,
    A,
    B,
    C
}