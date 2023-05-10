using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Components
    [SerializeField] private CanvasGroup startGroup,ingameGroup;
    #endregion

    public void OnPlayButtonClicked()
    {
        EventManager.TriggerEvent(EventKeys.OnGameStarted);
        ShowIngameGroup();
        HideStartMenu();
    }

    public void OnResetButtonClicked()
    {
        EventManager.TriggerEvent(EventKeys.OnGridResetRequested);
    }

    private void HideStartMenu()
    {
        startGroup.DOFade(0, 0.5f).SetTarget(this).OnComplete(()=>
        {
            startGroup.gameObject.SetActive(false);
        });
    }

    private void ShowIngameGroup()
    {
        ingameGroup.gameObject.SetActive(true);
        ingameGroup.DOFade(1, 0.5f).SetTarget(this);
    }

}
