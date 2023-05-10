using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/ConfigSO", order = 1)]
public class GameConfig : ScriptableObject
{
    [Header("Block Settings")]
    [Space(5)]
    public List<SpriteListForColor> spriteList;
    public List<int> minCountsForLevels;

    [Header("Game Settings")]
    [Space(5)]
    [Header("The higher values, the easier gameplay")]
    public float bonusForEachAdjacent = 5f;
    public float bonusForConnecteds = 0.1f;

    [Space(5)]
    public GameObject blockPrefab;
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