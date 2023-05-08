using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class GridBlockSelector : MonoBehaviour
{
    #region Components
    private Camera mainCamera;
    private GridConnectionFinder connectionFinder;
    #endregion

    #region Variables
    private bool canSelect = true;
    #endregion
    private void Start()
    {
        connectionFinder = GetComponent<GridConnectionFinder>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!canSelect) return;

        if(Input.GetMouseButtonDown(0))
            HandleBlockSelection();
    }

    private void HandleBlockSelection()
    {
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject.TryGetComponent(out Block clickedBlock))
                OnBlockClicked(clickedBlock);
        }
    }

    private void OnBlockClicked(Block clickedBlock)
    {
        var group = connectionFinder.FindGroupForABlock(clickedBlock);
        if (group != null)
        {
            for (int i = 0; i < group.Count; i++)
            {
                Destroy(group[i].gameObject);
            }
        }
        else
        {
            Debug.Log("no connection", clickedBlock.gameObject);
        }
    }
}
