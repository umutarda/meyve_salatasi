using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSequence : MonoBehaviour
{
    [SerializeField] private MenuAnimation logoAnim;
    [SerializeField] private MenuAnimation playButtonAnim;
    [SerializeField] private MenuAnimation playSingleButtonAnim;
    [SerializeField] private MenuAnimation htpButtonAnim;
    [SerializeField] private float startDelay;
    [SerializeField] private GameObject htp1;
    [SerializeField] private GameObject htp2;
    [SerializeField] private bool includeSingle;

    void Start()
    {
        StartCoroutine(Sequence());
    }

    public void LoadGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    public void ShowHTP1() => htp1.SetActive(true);
    public void ShowHTP2() {htp1.SetActive(false); htp2.SetActive(true);}
    public void CloseHTP2() => htp2.SetActive(false);

    IEnumerator Sequence() 
    {
        yield return new WaitForSecondsRealtime(startDelay);
        float animWait;
        logoAnim.gameObject.SetActive(true);
        logoAnim.DOAnimation(out animWait);
        yield return new WaitForSecondsRealtime(animWait);
        playButtonAnim.gameObject.SetActive(true);
        playButtonAnim.DOAnimation(out animWait);
        yield return new WaitForSecondsRealtime(animWait);

        if (includeSingle) 
        {
            playSingleButtonAnim.gameObject.SetActive(true);
            playSingleButtonAnim.DOAnimation(out animWait);
            yield return new WaitForSecondsRealtime(animWait);
        }
       
        
        htpButtonAnim.gameObject.SetActive(true);
        htpButtonAnim.DOAnimation(out animWait);
        yield return new WaitForSecondsRealtime(animWait);
    }
}
