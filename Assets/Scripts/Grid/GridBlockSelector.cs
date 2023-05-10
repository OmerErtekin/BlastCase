using UnityEngine;

public class GridBlockSelector : MonoBehaviour
{
    #region Components
    private Camera mainCamera;
    #endregion

    #region Variables
    private bool canSelectBlock = true;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnGridCreated, EnableSelection);
        EventManager.StartListening(EventKeys.OnShuffleRequested, DisableSelection);
        EventManager.StartListening(EventKeys.OnShuffleCompleted, EnableSelection);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnGridCreated, EnableSelection);
        EventManager.StopListening(EventKeys.OnShuffleRequested, DisableSelection);
        EventManager.StopListening(EventKeys.OnShuffleCompleted, EnableSelection);
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!canSelectBlock) return;

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
        if (clickedBlock.GetConnectedGroup != null)
        {
            EventManager.TriggerEvent(EventKeys.OnBlastRequested, new object[] { clickedBlock.GetConnectedGroup });
        }
        else
        {
            clickedBlock.ShakeTheBlock();
        }
    }

    private void EnableSelection(object[] obj = null) => canSelectBlock = true;

    private void DisableSelection(object[] obj = null) => canSelectBlock = false; 
}
