using UnityEngine;

public class GrappleMarker : MonoBehaviour
{
    [Tooltip("This is the camera which will be used to cast the rope object (the main camera most likely)")]
    [SerializeField] private Camera cam;

    private void Update()
    {
        // Convert mouse position to world space
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
    }
}
