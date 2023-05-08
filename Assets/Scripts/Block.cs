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
    private BlockColor currentColor;
    private BlockLevel currentLevel;
    public BlockColor testColor;
    public BlockLevel testLevel;
    #endregion
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            SetBlock(testColor, testLevel);
        }
    }

    public void SetBlock(BlockColor color,BlockLevel level)
    {
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
