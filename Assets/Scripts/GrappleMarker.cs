using UnityEngine;

public class GrappleMarker : MonoBehaviour
{
    [Tooltip("This is the camera which will be used to cast the rope object (the main camera most likely)")]
    [SerializeField] private Camera cam;
    [SerializeField] private float radius;
    
    private Vector2 closestPoint;
    
    private string[] layers;

    private void Awake()
    {
        layers = new string[2];

        layers[0] = "Ground";
        layers[1] = "Enemy";
    }
    
    private void Update()
    {
        // Convert mouse position to world space
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        
        RaycastHit2D hit = Physics2D.CircleCast(new Vector2(mouseWorldPos.x, mouseWorldPos.y), radius, Vector2.zero, 0, LayerMask.GetMask(layers));

        if (hit.collider)
        {
            closestPoint = Physics2D.ClosestPoint(new Vector2(mouseWorldPos.x, mouseWorldPos.y), hit.collider);
                    
            transform.position = closestPoint;
        }
        else if (!hit.collider)
        {
            transform.position = mouseWorldPos;
        }
        
    }
}
