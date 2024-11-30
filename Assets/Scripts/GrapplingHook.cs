using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GrapplingHook : MonoBehaviour
{
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer rope;
    [SerializeField] private Camera cam;
    [SerializeField] private float ropeSpeed = 0.1f;
    [SerializeField] private float maxDistance = 100.0f;
    
    private Vector3 grapplePoint;
    private Vector3 endPoint;
    private Vector3 startPoint;
    private DistanceJoint2D joint;
    private Vector2 direction;
    private bool isGrappling;
    private float elapsedTime;
    
    public UnityEvent OnGrapple;
    public UnityEvent OffGrapple;
    
    //TODO: Grappling hook cooldown så att man inte kan spamma
    //TODO: Animera grappling hooken när den kommer tillbaka också
    //TODO: 
    void Start()
    {
        // Sets joint to the local DistanceJoint2D and disables the grappling hook at the start
        joint = gameObject.GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        rope.enabled = false;
    }
    
    void Update()
    {
        // Convert mouse position to world space
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            cam.nearClipPlane // Use the near clipping plane distance
        ));

        // Makes sure that you can move your mouse while grappling without affecting the direction of the grappling hook
        if (isGrappling)
        {
            // Calculates the direction from the player to the grapple point/connected anchor
            direction = (joint.connectedAnchor - new Vector2(transform.position.x , transform.position.y)).normalized;
        }
        else
        {
            // Calculates the direction from the player to the mouse
            direction = (mouseWorldPos - transform.position).normalized;
        }

        // Starts the Grapple
        if (Input.GetMouseButtonDown(0))
        {
            // Performs a raycast in the calculated direction
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, grappleLayer);

            // Debugs the ray to visualize it in the Scene view
            Debug.DrawRay(transform.position, direction * 10, Color.red, 2f);

            // Starts grappling if the raycast hits a viable layer and if the direction is not downwards
            if (hit.collider && direction.y >= 0)
            {
                OnGrapple.Invoke();
                grapplePoint = hit.point;
                joint.connectedAnchor = grapplePoint;
                joint.distance = Vector2.Distance(transform.position, grapplePoint);
                joint.enabled = true;
                
                rope.enabled = true;
                rope.SetPosition(1, transform.position); // Players position
                rope.SetPosition(0, transform.position);  // Anchors end point
                
                isGrappling = true;
                StartCoroutine(OnGrappleAnimation());

                // Prevents you from grappling too far
                if (hit.distance > maxDistance)
                {
                    joint.enabled = false;
                }
            }
        }

        // Ends the Grapple
        if (Input.GetMouseButtonUp(0))
        {
            OffGrapple.Invoke();
            joint.enabled = false;
            isGrappling = false;
            
            StartCoroutine(OffGrappleAnimation());
        }

        // Updates the ropes player end position
        if (rope.enabled)
        {
            rope.SetPosition(1, transform.position);
        }

        // Disables the rope if the joint breaks
        //if (joint.enabled == false)
        //{
        //    rope.enabled = false;
        //}

        // Disables the joint if the direction is downwards
        //if (direction.y < 0)
        //{
        //    joint.enabled = false;
        //}
    }
    
    private IEnumerator OnGrappleAnimation() // Makes the rope move towards the grapple point and not teleport to it
    {
        float distance = Vector3.Distance(transform.position, grapplePoint);

        if (distance > maxDistance)
        {
            rope.enabled = false;
            yield break;
        }
        
        elapsedTime = 0f;
        
        startPoint = transform.position;
        
        while (isGrappling)
        {
            elapsedTime += Time.deltaTime * ropeSpeed;
            
            endPoint = Vector3.Lerp(startPoint, grapplePoint, elapsedTime);
            rope.SetPosition(0, endPoint);

            yield return null;
        }
    }
    
    private IEnumerator OffGrappleAnimation() // Makes the rope move back towards the player after releasing the grappling hook
    {
        elapsedTime = 0f;
        
        startPoint = endPoint;
        
        while (isGrappling == false)
        {
            elapsedTime += Time.deltaTime * ropeSpeed;
            
            endPoint = Vector3.Lerp(startPoint, transform.position, elapsedTime);
            rope.SetPosition(0, endPoint);

            yield return null;
        }
        
        //rope.enabled = false;
    }
}