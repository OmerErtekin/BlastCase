using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region Components
    [SerializeField] private SpriteRenderer blockSprite;
    #endregion

    #region Variables
    [SerializeField] private List<SpriteListForColor> spriteList = new();
    private Vector2Int matrixPosition;
    private BlockColor currentColor;
    private BlockLevel currentLevel;
    #endregion

    #region Properties
    public Vector2Int GetPosition => matrixPosition;
    public BlockColor GetColor => currentColor;
    #endregion

    public void SetBlock(Vector2Int position,BlockColor color,BlockLevel level)
    {
        matrixPosition = position;
        currentColor = color;
        currentLevel = level;
        blockSprite.sprite = spriteList[(int)color].levelSprites[(int)level];
    }
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
