using UnityEngine;

public class FOVController : MonoBehaviour
{
    #region Components
    private Camera mainCamera;
    #endregion

    private void OnEnable()
    {
        EventManager.StartListening(EventKeys.OnGridCreated, UpdateCameraFOV);   
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventKeys.OnGridCreated, UpdateCameraFOV);
    }

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void UpdateCameraFOV(object[] obj = null)
    {
        int rowCount = (int)obj[0];
        mainCamera.fieldOfView = 30 + 5 * rowCount;
    }
}
