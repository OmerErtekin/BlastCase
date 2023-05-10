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
    private GridController controller;
    #endregion

    #region Variables
    private bool canSelect = true;
    #endregion
    private void Start()
    {
        connectionFinder = GetComponent<GridConnectionFinder>();
        controller = GetComponent<GridController>();
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
        if (clickedBlock.GetConnectedGroup() != null)
        {
            controller.BlastAGroup(clickedBlock.GetConnectedGroup());
        }
        else
        {
            clickedBlock.ShakeTheBlock();
        }
    }
}
