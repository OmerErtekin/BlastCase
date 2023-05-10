using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour, IBlastable
{
    #region Components
    [SerializeField] private SpriteRenderer blockSprite;
    private BlockPool blockPool;
    #endregion

    #region Variables
    [SerializeField] private GameConfig config;
    private Vector2Int matrixPosition;
    private BlockColor currentColor;
    private List<Block> connectedGroup = new();
    private Coroutine fallRoutine;
    #endregion

    #region Properties
    public Vector2Int GetPosition => matrixPosition;
    public List<Block> GetConnectedGroup => connectedGroup;
    public BlockColor GetColor => currentColor;
    #endregion

    public void InitializeBlock(Vector2Int matrixPos, BlockColor color)
    {
        matrixPosition = matrixPos;
        currentColor = color;
        blockSprite.sprite = config.spriteList[(int)currentColor].levelSprites[0];
    }

    public void Blast()
    {
        if (fallRoutine != null)
        {
            StopCoroutine(fallRoutine);
        }
        transform.DOKill();
        blockPool.AddBlockToPool(this);
        gameObject.SetActive(false);
    }

    public void ShakeTheBlock()
    {
        transform.DOKill();
        transform.DOShakeRotation(0.5f, new Vector3(0, 0, 30)).SetTarget(this).OnComplete(() =>
        {
            transform.DORotate(Vector3.zero, 0.1f).SetTarget(this);
        });
    }

    public void SwipeDown(Vector2Int newPosition, Vector3 realWorldPosition)
    {
        matrixPosition = newPosition;
        if (fallRoutine != null)
        {
            StopCoroutine(fallRoutine);
        }
        fallRoutine = StartCoroutine(FallDown(realWorldPosition));
    }

    public void MoveToNewPosition(Vector2Int newPosition, Vector3 realWorldPosition)
    {
        matrixPosition = newPosition;
        transform.DOMove(realWorldPosition, 0.5f).SetTarget(this);
    }

    private IEnumerator FallDown(Vector3 targetPosition)
    {
        while ((transform.position - targetPosition).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 15 * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }

    public void SetConnectedGroup(List<Block> group)
    {
        connectedGroup = group;
        DecideSprite();
    }



    public void SetBlockPool(BlockPool pool) => blockPool = pool;

    private void DecideSprite()
    {
        if (connectedGroup == null)
        {
            blockSprite.sprite = config.spriteList[(int)currentColor].levelSprites[0];
            return;
        }

        for (int i = config.minCountsForLevels.Count - 1; i >= 0; i--)
        {
            if (connectedGroup.Count >= config.minCountsForLevels[i])
            {
                blockSprite.sprite = config.spriteList[(int)currentColor].levelSprites[i];
                return;
            }
        }
    }
}
