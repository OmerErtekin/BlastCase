using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    #region Components
    [SerializeField] private SpriteRenderer blockSprite;
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
    public BlockColor GetColor => currentColor;
    #endregion

    public void InitializeBlock(Vector2Int matrixPos, BlockColor color, BlockLevel level)
    {
        matrixPosition = matrixPos;
        currentColor = color;
        blockSprite.sprite = config.spriteList[(int)currentColor].levelSprites[(int)level];
    }
    
    public void BlastTheBlock()
    {
        transform.DOKill();
        transform.DOScale(0, 0.25f).SetTarget(this).SetEase(Ease.InBack).OnComplete(()=> gameObject.SetActive(false));
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
        name = $"{matrixPosition}";
        if(fallRoutine != null)
        {
            StopCoroutine(fallRoutine);
        }
        fallRoutine = StartCoroutine(FallDown(realWorldPosition));
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

    private IEnumerator FallDown(Vector3 targetPosition)
    {
        while((transform.position - targetPosition).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 10 * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
    }
}
