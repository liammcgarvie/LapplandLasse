using UnityEngine;

public class UICounter : MonoBehaviour
{
    [Tooltip("The Text component where the counter value should be shown")]
    [SerializeField] private TMPro.TMP_Text counterText;

    private int counter = 0;
    
    public void AddToCounter(int add)
    {
        counter += add;
        counterText.text = counter.ToString();
    }
    
    public void SetCounter(int set)
    {
        counter = set;
        counterText.text = counter.ToString();
    }
}
