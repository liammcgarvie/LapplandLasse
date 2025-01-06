using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GrapplingHookRevamped : MonoBehaviour
{
    [Tooltip("This is the layer on which you can grapple onto")]
    [SerializeField] private LayerMask grappleLayer;
    [Tooltip("This is the layer on which you can pull things toward you")]
    [SerializeField] private LayerMask enemyPullLayer;
    [Tooltip("This is the ground layer")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("This is the player layer")]
    [SerializeField] private LayerMask playerLayer;
    [Tooltip("This is the rope object (the object needs a LineRenderer component)")]
    [SerializeField] private LineRenderer rope;
    [Tooltip("This is the camera which will be used to cast the rope object (the main camera most likely)")]
    [SerializeField] private Camera cam;
    [Tooltip("This is the hook object (can be a sprite for example)")]
    [SerializeField] private GameObject hook;
    [Tooltip("This is the marker object (can be a sprite for example)")]
    [SerializeField] private GameObject marker;
    [Tooltip("This is the rigidbody of the player")] 
    [SerializeField] private Rigidbody2D rb;
    [Tooltip("This is the speed of the grappling hook when you shoot it at a roof or a wall (the speed of the rope animation)")]
    [SerializeField] private float ropeSpeed = 0.1f;
    [Tooltip("This is the speed of the grappling hook when you shoot it at an enemy (the speed of the rope animation)")]
    [SerializeField] private float enemyRopeSpeed = 0.1f;
    [Tooltip("This is the max distance in which you can grapple")]
    [SerializeField] private float maxDistance = 100.0f;
    [Tooltip("This is the radius of how far away from the mouse you can detect a grapple")]
    [SerializeField] private float radius = 3.0f;
    [Tooltip("This is the collider on the hook")]
    [SerializeField] private BoxCollider2D hookCollider;
    [Tooltip("This is the distance joint 2D")]
    [SerializeField] private DistanceJoint2D joint;
    
    [SerializeField] private AudioSource grappleHitSound;
    [SerializeField] private AudioSource grappleShootSound;
    
    private RaycastHit2D markerHit;
    private Vector2 closestPoint;
    
    private Vector3 grapplePoint;
    private Vector3 startPoint;
    private Vector3 endPoint;

    private string[] grappleLayers;
    private float elapsedTime;
    private bool isPressing;
    private bool canPlayGrappleHitSound;
    private bool isGrappling;
    
    public UnityEvent OnGrapple;
    public UnityEvent OffGrapple;
    
    private void Awake()
    {
        grappleLayers = new string[2];
        grappleLayers[0] = "Enemy";
        grappleLayers[1] = "Ground";
        
        hook.SetActive(false);
        joint.enabled = false;
        rope.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPressing = true;
        }
        
        rope.SetPosition(1, transform.position); //Makes sure that the rope does not disconnect from the player
        
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));
        
        markerHit = Physics2D.CircleCast(new Vector2(mouseWorldPos.x, mouseWorldPos.y), radius, Vector2.zero, 0, LayerMask.GetMask(grappleLayers));

        if (markerHit.collider)
        {
            closestPoint = Physics2D.ClosestPoint(new Vector2(mouseWorldPos.x, mouseWorldPos.y), markerHit.collider);
                
            marker.transform.position = closestPoint;

            if (Input.GetMouseButtonDown(0))
            {
                isPressing = true;
                rope.enabled = true;
                hook.SetActive(true);
                
                rope.SetPosition(1, transform.position); // Players position
                rope.SetPosition(0, transform.position); // Anchors end point
                
                StartCoroutine(OnGrappleAnimation());
                
                OnGrapple.Invoke();
                grapplePoint = marker.transform.position;
                joint.connectedAnchor = grapplePoint;
                joint.distance = Vector2.Distance(transform.position, grapplePoint);
                joint.enabled = true;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                isPressing = false;
                
                CancelGrapple();
            }
        }
        else
        {
            marker.transform.position = mouseWorldPos;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPressing = false;
            
            CancelGrapple();
        }
    }
    
    private IEnumerator OnGrappleAnimation() // Makes the rope move towards the grapple point and not teleport to it
    {
        float distance = Vector3.Distance(transform.position, grapplePoint);

        if (distance > maxDistance)
        {
            StartCoroutine(OffGrappleAnimation());
            yield break;
        }
        
        elapsedTime = 0f;
        
        startPoint = transform.position;
        
        canPlayGrappleHitSound = true;
        
        while (isPressing)
        {
            elapsedTime += Time.deltaTime * ropeSpeed;
            
            endPoint = Vector3.Lerp(startPoint, grapplePoint, elapsedTime);
            rope.SetPosition(0, endPoint);
            hook.transform.position = endPoint;
            
            if (hook.transform.position == grapplePoint && canPlayGrappleHitSound)
            {
                grappleHitSound.Play();
                canPlayGrappleHitSound = false;
                yield break;
            }

            yield return null;
        }
    }
    
    private IEnumerator OffGrappleAnimation() // Makes the rope move back towards the player after releasing the grappling hook
    {
        elapsedTime = 0f;
        
        startPoint = endPoint;
        
        while (isPressing == false)
        {
            elapsedTime += Time.deltaTime * ropeSpeed;
            
            endPoint = Vector3.Lerp(startPoint, transform.position, elapsedTime);
            rope.SetPosition(1, transform.position);
            rope.SetPosition(0, endPoint);
            hook.transform.position = endPoint;
            
            if (hook.transform.position == transform.position)
            {
                hook.SetActive(false);
                rope.enabled = false;
                yield break;
            }

            yield return null;
        }
    }

    private void CancelGrapple()
    {
        OffGrapple.Invoke();
        joint.enabled = false;
        isGrappling = false;
        StartCoroutine(OffGrappleAnimation());
    }
}
