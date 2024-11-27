using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    [SerializeField] private float z;
    
    void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, z);
    }
}
