using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region Components
    [SerializeField] private SpriteRenderer blockSprite;
    #endregion

    #region Variables
    [SerializeField] private List<SpriteListForColor> spriteList = new();
    [SerializeField] private List<int> minCountForLevels = new();
    private Vector2Int matrixPosition;
    public BlockLevel currentLevel;
    private BlockColor currentColor;
    public List<Block> connectedGroup = new();
    #endregion

    #region Properties
    public Vector2Int GetPosition => matrixPosition;
    public BlockColor GetColor => currentColor;
    #endregion

    public void InitializeBlock(Vector2Int matrixPos, BlockColor color, BlockLevel level)
    {
        matrixPosition = matrixPos;
        name = $"{matrixPosition}";
        currentColor = color;
        blockSprite.sprite = spriteList[(int)currentColor].levelSprites[(int)level];
    }
    
    public void BlastTheBlock()
    {
        gameObject.SetActive(false);
    }

    public void SwipeDown(Vector2Int newPosition, Vector3 realWorldPosition)
    {
        matrixPosition = newPosition;
        name = $"{matrixPosition}";
        transform.position = realWorldPosition;
    }

    public void SetConnectedGroup(List<Block> group)
    {
        connectedGroup = group;
        DecideSprite();
    }

    public List<Block> GetConnectedGroup() => connectedGroup;

    private void DecideSprite()
    {
        if (connectedGroup == null)
        {
            currentLevel = BlockLevel.Default;
            blockSprite.sprite = spriteList[(int)currentColor].levelSprites[0];
            return;
        }

        for (int i = minCountForLevels.Count - 1; i >= 0; i--)
        {
            if (connectedGroup.Count >= minCountForLevels[i])
            {
                currentLevel = (BlockLevel)i;
                blockSprite.sprite = spriteList[(int)currentColor].levelSprites[i];
                return;
            }
        }
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
