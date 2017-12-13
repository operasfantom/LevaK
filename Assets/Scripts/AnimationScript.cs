using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AnimationScript : MonoBehaviour {
    public AnimationClip clip;
    public GameObject dropPicture;
    public GameObject printText;
    private Animation dropAnimation;
    private Animation printAnimation;
    public string[] speech;

    void Start()
    {
        Debug.Log("START");
        int num = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("NUM", num);

        dropAnimation = dropPicture.GetComponent<Animation>();
        printAnimation = printText.GetComponent<Animation>();
        StartCoroutine(PlayAnimation());
    }

    public IEnumerator WaitForTouch()
    {
        while (!Input.GetMouseButton(0))
            yield return null;
    }

    IEnumerator PlayAnimation()
    {
        dropAnimation.Play("DropAnimation");
        yield return new WaitForSeconds(3);
        
        
        foreach(string s in speech)
        {
            printText.GetComponent<Text>().text = s;
            printAnimation.Play("PrintAnimation");
            yield return new WaitForSeconds(3);
        }
        

        yield return StartCoroutine(WaitForTouch());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
