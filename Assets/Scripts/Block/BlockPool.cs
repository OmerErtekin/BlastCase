using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform parentTransform;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private int initialPoolSize = 50;
    private Queue<Block> blockPool = new Queue<Block>();
    #endregion

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        Block blockScript;
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject instance = Instantiate(blockPrefab, parentTransform);
            instance.SetActive(false);
            blockScript = instance.GetComponent<Block>();
            blockScript.SetBlockPool(this);
            blockPool.Enqueue(blockScript);
        }
    }

    public Block GetBlock()
    {
        if (blockPool.Count > 0)
        {
            Block instance = blockPool.Dequeue();
            instance.gameObject.SetActive(true);
            return instance;
        }
        else
        {
            GameObject instance = Instantiate(blockPrefab, parentTransform);
            return instance.GetComponent<Block>();
        }
    }

    public void AddBlockToPool(Block block)
    {
        blockPool.Enqueue(block);
    }
}

