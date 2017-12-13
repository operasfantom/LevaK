using UnityEngine;
using UnityEngine.SceneManagement;

public class AboutScript : MonoBehaviour {

    private void Update()
    {
        if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu))
        {
            SceneManager.LoadScene(0);
        }
    }
    
    public void Hyperlink(string site)
    {
        Application.OpenURL(site);
    }
}
