using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScript : MonoBehaviour {

    // Use this for initialization

    public GameObject[] Cities;
    int num;
	void Awake () {
		num = PlayerPrefs.GetInt("NUM");
        PlayerPrefs.SetInt("NUM", num + 1);
        int last = (num - 1) / 2;
        for (int i = 0; i < Cities.Length; i++)
        {
            Cities[i].SetActive(i <= last);
        }
    }
	
    public void NextLevel()
    {
        SceneManager.LoadScene(num + 1);
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}
