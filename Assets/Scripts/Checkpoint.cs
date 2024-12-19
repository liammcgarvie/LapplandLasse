using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    public void Trigger()
    {
        animator.SetBool("isLit", true);
    }
}
