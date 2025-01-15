using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public Animator animator;

    [SerializeField] private AudioSource shit;
    [SerializeField] private AudioSource huh;
    [SerializeField] private AudioSource hmm;
    [SerializeField] private AudioSource well;
    
    private void Start()
    {
        StartCoroutine(WaitForAnimation());
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void PreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    
    public void NextNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    public void MenuScene()
    {
        SceneManager.LoadScene("Titelscene");
    }

    public void BonusLevel()
    {
        SceneManager.LoadScene("LiamScene4");
    }
    
    private IEnumerator WaitForAnimation()
    {
        if (animator != null)
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - 2);
            NextScene();
        }
    }

    public void ShitSound()
    {
        shit.Play();
    }
    
    public void WellSound()
    {
        well.Play();
    }
    
    public void HuhSound()
    {
        huh.Play();
    }
    
    public void HmmSound()
    {
        hmm.Play();
    }
}
