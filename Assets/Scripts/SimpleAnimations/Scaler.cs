using DG.Tweening;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    #region Variables
    [SerializeField] private float bigScale = 1.25f, smallScale = 0.75f, duration = 1;
    [SerializeField] private Ease animationEase = Ease.Linear;
    #endregion

    private void Start()
    {
        Sequence animationSequence = DOTween.Sequence();
        animationSequence.Append(transform.DOScale(bigScale, duration / 2).From(smallScale).SetUpdate(true).SetTarget(this).SetEase(animationEase));
        animationSequence.Append(transform.DOScale(smallScale, duration / 2).From(bigScale).SetUpdate(true).SetTarget(this).SetEase(animationEase));
        animationSequence.SetTarget(this).SetUpdate(true).SetLoops(-1);
    }
}
