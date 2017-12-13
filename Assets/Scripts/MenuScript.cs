using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MenuScript : MonoBehaviour {
    
    public void Play()
    {
        PlayerPrefs.DeleteKey("SV");
        PlayerPrefs.SetInt("NUM", 1);
        SceneManager.LoadScene(1);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("NUM"))
        {
            int num = PlayerPrefs.GetInt("NUM");
            SceneManager.LoadScene(num);
        }
        else
        {
            Debug.Log("No save file!");
        }
    }

    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

[Serializable]
public class Save
{
    public Save(double _money, int _score, int _capacity, double _income, double _multiplier, Item[] _List_of_items, bool[] _isAvaliable, double[] _licensePrices, bool[] _isBoughtLine)
    {
        money = _money; score = _score; capacity = _capacity; income = _income; multiplier = _multiplier;
        List_of_items = _List_of_items; isAvaliable = _isAvaliable; licensePrices = _licensePrices;
        isBoughtLine = _isBoughtLine;
    }
    public double money;
    public int score;
    public int capacity;
    public double income;
    public double multiplier;
    public Item[] List_of_items;
    public bool[] isAvaliable;
    public double[] licensePrices;
    public bool[] isBoughtLine;
}

