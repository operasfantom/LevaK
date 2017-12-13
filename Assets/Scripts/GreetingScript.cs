using UnityEngine;
using UnityEngine.UI;

public class GreetingScript : MonoBehaviour {

    public Sprite[] sprites;
    public GameObject welcomePanel;

    private void InitWelcomePanel()
    {
        return;//!!!
        Image I = gameObject.transform.GetComponent<Image>();
        System.Random rnd = new System.Random();
        int index = rnd.Next(0, sprites.Length);
        I.sprite = sprites[index];
    }

    public void PushNotification(string s)
    {
        InitWelcomePanel();

        welcomePanel.SetActive(true);
        GameObject Greeting = welcomePanel.transform.Find("Greeting").gameObject;
        Text T = Greeting.GetComponentInChildren<Text>();
        T.text = s;
    }

    public void HideNotification()
    {
        welcomePanel.SetActive(false);
    }
}
