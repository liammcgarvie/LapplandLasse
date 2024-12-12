using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GrapplingHook : MonoBehaviour
{
    [Tooltip("This is the layer on which you can grapple onto")]
    [SerializeField] private LayerMask grappleLayer;
    [Tooltip("This is the layer on which you can pull things toward you")]
    [SerializeField] private LayerMask enemyPullLayer;
    [Tooltip("This is the rope object (the object needs a LineRenderer component)")]
    [SerializeField] private LineRenderer rope;
    [Tooltip("This is the camera which will be used to cast the rope object (the main camera most likely)")]
    [SerializeField] private Camera cam;
    [Tooltip("This is the hook object (can be a sprite for example)")]
    [SerializeField] private GameObject hook;
    [Tooltip("This is the speed of the grappling hook when you shoot it at a roof or a wall (the speed of the rope animation)")]
    [SerializeField] private float ropeSpeed = 0.1f;
    [Tooltip("This is the speed of the grappling hook when you shoot it at an enemy (the speed of the rope animation)")]
    [SerializeField] private float enemyRopeSpeed = 0.1f;
    [Tooltip("This is the extra length of the rope when grappling towards an enemy")]
    [SerializeField] private float enemyRopeLength = 10f;
    [Tooltip("This is the max distance in which you can grapple")]
    [SerializeField] private float maxDistance = 100.0f;
    [Tooltip("This is the amount of time that it takes for you to be able to grapple again after disengaging the grappling hook")]
    [SerializeField] private float grappleCooldownTime = 0.5f;
    
    [SerializeField] private AudioSource grappleSound;
    
    private Vector3 grapplePoint;
    private Vector3 endPoint;
    private Vector3 startPoint;
    private DistanceJoint2D joint;
    private Vector2 direction;
    private bool isGrappling;
    private bool canGrapple;
    private bool canPlayGrappleSound;
    private bool isPressing;
    private bool shooting;
    private float elapsedTime;
    private float grappleCooldownTimer;
    
    public UnityEvent OnGrapple;
    public UnityEvent OffGrapple;
    
    //TODO: Förbättra grappling hook cooldown
    //TODO: Fixa så att man inte behöver byta direction knapp när man snurrar runt saker
    void Start()
    {
        // Sets joint to the local DistanceJoint2D
        joint = gameObject.GetComponent<DistanceJoint2D>();
        joint.enabled = false; // Disables the grappling hook at the start
        rope.enabled = false;
        hook.SetActive(false); // Disables the hook object in the start
        canGrapple = true; // canGrapple is related to grappling hook cooldown
        grappleCooldownTimer = 0; // grappleCooldownTimer is related to grappling hook cooldown
    }
    
    void Update()
    {
        // Makes it so that you cant spam the grappling hook
        if (canGrapple == false)
        {
            grappleCooldownTimer += Time.deltaTime;
            
            if (grappleCooldownTimer >= grappleCooldownTime) // You can set your own cooldown time
            {
                canGrapple = true;
                grappleCooldownTimer = 0;
            }
        }
        
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
            isPressing = true;
            
            // Performs a raycast in the calculated direction for grapple layers
            RaycastHit2D grappleHit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, grappleLayer);
            
            // Performs a raycast in the calculated direction for enemies
            RaycastHit2D enemyHit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, enemyPullLayer);

            // Debugs the ray to visualize it in the Scene view
            Debug.DrawRay(transform.position, direction * 10, Color.red, 2f);

            // Starts grappling if the raycast hits an enemy
            if (enemyHit.collider && canGrapple)
            {
                hook.SetActive(true);

                if (enemyHit.point.x > transform.position.x)
                {
                    grapplePoint = enemyHit.point + new Vector2(enemyRopeLength, 0);
                }
                
                if (enemyHit.point.x < transform.position.x)
                {
                    grapplePoint = enemyHit.point - new Vector2(enemyRopeLength, 0);
                }
                
            
                rope.enabled = true;
                rope.SetPosition(1, transform.position); // Players position
                rope.SetPosition(0, transform.position); // Anchors end point
            
                StartCoroutine(EnemyGrappleAnimation());
            }
            
            // Starts grappling if the raycast hits a viable layer and if the direction is not downwards
            if (grappleHit.collider && enemyHit.collider == false && direction.y >= 0 && canGrapple)
            {
                OnGrapple.Invoke();
                grapplePoint = grappleHit.point;
                joint.connectedAnchor = grapplePoint;
                joint.distance = Vector2.Distance(transform.position, grapplePoint);
                joint.enabled = true;
                
                rope.enabled = true;
                rope.SetPosition(1, transform.position); // Players position
                rope.SetPosition(0, transform.position);  // Anchors end point
                
                isGrappling = true;
                StartCoroutine(OnGrappleAnimation());

                // Prevents you from grappling too far
                if (grappleHit.distance > maxDistance)
                {
                    joint.enabled = false;
                }
            }
            
            if (grappleHit.collider == false && enemyHit.collider == false && direction.y >= 0 && canGrapple)
            {
                hook.SetActive(true);
                
                grapplePoint = mouseWorldPos;
            
                rope.enabled = true;
                rope.SetPosition(1, transform.position); // Players position
                rope.SetPosition(0, transform.position); // Anchors end point
            
                StartCoroutine(MissedGrappleAnimation());
            }
        }

        // Ends the Grapple
        if (Input.GetMouseButtonUp(0))
        {
            isPressing = false;
            
            if (isGrappling)
            {
                canGrapple = false;
                OffGrapple.Invoke();
                joint.enabled = false;
                isGrappling = false;
            
                StartCoroutine(OffGrappleAnimation());
            }
        }

        // Updates the ropes player end position
        if (rope.enabled)
        {
            rope.SetPosition(1, transform.position);
        }

        //Borde inte funka som jag vill men det funkar
        if (isPressing == false)
        {
            hook.transform.position = transform.position;
        }
    }
    
    private IEnumerator OnGrappleAnimation() // Makes the rope move towards the grapple point and not teleport to it
    {
        float distance = Vector3.Distance(transform.position, grapplePoint);

        if (distance > maxDistance)
        {
            StartCoroutine(MissedGrappleAnimation());
            yield break;
        }
        
        hook.SetActive(true);
        
        elapsedTime = 0f;
        
        startPoint = transform.position;
        
        canPlayGrappleSound = true;
        
        while (isPressing)
        {
            elapsedTime += Time.deltaTime * ropeSpeed;
            
            endPoint = Vector3.Lerp(startPoint, grapplePoint, elapsedTime);
            rope.SetPosition(0, endPoint);
            hook.transform.position = endPoint;
            
            if (hook.transform.position == grapplePoint && canPlayGrappleSound)
            {
                grappleSound.Play();
                canPlayGrappleSound = false;
                yield break;
            }

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

    private IEnumerator MissedGrappleAnimation()
    {
        elapsedTime = 0;
        
        startPoint = transform.position;
        
        shooting = true;
        
        hook.SetActive(true);

        while (shooting)
        {
            elapsedTime += Time.deltaTime * ropeSpeed;
            
            endPoint = Vector3.Lerp(startPoint, grapplePoint, elapsedTime);
            rope.SetPosition(0, endPoint);
            hook.transform.position = endPoint;

            if (hook.transform.position == grapplePoint)
            {
                shooting = false;
                startPoint = endPoint;
                elapsedTime = 0;
            }
            
            yield return null;
        }

        while (shooting == false)
        {
            elapsedTime += Time.deltaTime * ropeSpeed;
            
            endPoint = Vector3.Lerp(startPoint, transform.position, elapsedTime);
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
    
    private IEnumerator EnemyGrappleAnimation()
    {
        elapsedTime = 0;
        
        startPoint = transform.position;
        
        canPlayGrappleSound = true;
        
        shooting = true;
        
        hook.SetActive(true);

        while (shooting)
        {
            elapsedTime += Time.deltaTime * enemyRopeSpeed;
            
            endPoint = Vector3.Lerp(startPoint, grapplePoint, elapsedTime);
            rope.SetPosition(0, endPoint);
            hook.transform.position = endPoint;
            
            if (hook.transform.position == grapplePoint)
            {
                shooting = false;
                startPoint = endPoint;
                elapsedTime = 0;
            }
            
            yield return null;
        }

        while (shooting == false)
        {
            elapsedTime += Time.deltaTime * enemyRopeSpeed;
            
            endPoint = Vector3.Lerp(startPoint, transform.position, elapsedTime);
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

    public void EnemyHit() // Can be used with events
    {
        shooting = false;
        startPoint = endPoint;
        elapsedTime = 0;
                
        grappleSound.Play();
        canPlayGrappleSound = false;
    }
}