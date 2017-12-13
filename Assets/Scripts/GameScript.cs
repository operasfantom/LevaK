using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameScript : MonoBehaviour {

    [Header("INITIALISATION")]
    public double money, capital, multiplier = 1.0;
    public int score;
    public int capacity;
    public double income;
    public Text moneyText;
    public Text scoreText;
    public GameObject clickButton;

    [Header("MY STORE")]
    public GameObject storePanel;
    public GameObject rootsPanel;
    public Button[] storeButtons;
    public Button[] rootsButtons;

    [Header("ROOT STORE")]
    public double[] licensePrices;
    public Item[] List_of_items;
    public bool[] isAvailable;

    [Header("Switcher")]
    public GameObject cars_to_roots;
    public Sprite picture1, picture2;


    private Save saveFile;
    private bool isWin = false;
    public GameObject ConfirmPanel;

    private string currentScene;

    public string[] firstGreeting;

    public MetroScript MetroScript;
    public GreetingScript GreetingScript;
    
    private void Awake()
    {
#if UNITY_EDITOR
        //PlayerPrefs.DeleteKey("SV");
#else
        Debug.Log("ANDROID");
#endif
        currentScene = SceneManager.GetActiveScene().name;
        if (PlayerPrefs.HasKey("SV"))
        {
            LoadGame();
        }
        else
        {
            SaveGame();
            switch (currentScene)
            {
                case "VladivostokGameScene":
                    ConfirmPanel.SetActive(true);
                    break;
                default:
                    GreetingScript.PushNotification(firstGreeting[0]);
                    break;
            }
        }
        StartCoroutine(Saver());
    }

    private void Start()
    {
        for (int i = 1; i < List_of_items.Length; i++) ReCalcItem(i);
        StartCoroutine(BonusPerSec());
        storePanel.SetActive(false);
        rootsPanel.SetActive(false);
        cars_to_roots.SetActive(false);
    }

    private void Update()
    {
        moneyText.text = OutFloat(money) + " ₽" + "\n(" + OutFloat(income * multiplier) + "₽/сек)";
        scoreText.text = capacity.ToString() + " из " + score.ToString() + " человек";

        if (capacity >= score && !isWin)
        {
            isWin = true;
            FinishLevel("ПОБЕДА!");
        }

        if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu))
        {
            if (StoreIsOpen())
            {
                SwitchStore();
            }
            else
            {
                BackToMenu();
            }
        }
    }

    public void FinishLevel(string message)
    {
        StopCoroutine(Saver());
        if (StoreIsOpen()) SwitchStore();
        //GreetingScript.welcomePanel.SetActive(message == "ПОБЕДА!");
        clickButton.GetComponentInChildren<Text>().text = message;
        NextLevel();
    }

    public void NextLevel()
    {
        StartCoroutine(NextLevelCoroutine());
    }

    IEnumerator NextLevelCoroutine()
    {
        Debug.Log("Next");
        yield return StartCoroutine(WaitForTouch());
        //yield return null;
        yield return new WaitForSeconds(3f);
        PlayerPrefs.DeleteKey("SV");
        SceneManager.LoadScene(1);
    }

#region
    IEnumerator Saver()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(5.0f);
            Debug.Log("I save!");
            SaveGame();
        }
    }

    private void LoadGame()
    {
        saveFile = JsonUtility.FromJson<Save>(PlayerPrefs.GetString("SV"));
        money = saveFile.money;
        score = saveFile.score;
        capacity = saveFile.capacity;
        List_of_items = saveFile.List_of_items;
        isAvailable = saveFile.isAvaliable;
        licensePrices = saveFile.licensePrices;
        income = saveFile.income;
        multiplier = saveFile.multiplier;
        if (currentScene == "MoscowGameScene")
        {
            MetroScript.isBoughtLine = saveFile.isBoughtLine;
        }
    }

    private void SaveGame()
    {
        int num = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("NUM", num);
        bool[] _isBoughtLine = ((currentScene == "MoscowGameScene" && PlayerPrefs.HasKey("SV")) ? MetroScript.isBoughtLine : new bool[12]);
        saveFile = new Save(money, score, capacity, income, multiplier, List_of_items, isAvailable, licensePrices, _isBoughtLine);
        PlayerPrefs.SetString("SV", JsonUtility.ToJson(saveFile));
    }
#endregion

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void BackToMenu()
    {
        SaveGame();
        SceneManager.LoadScene(0);
    }

    private double f()
    {
        return (int)(Math.Pow(money + capital, 0.33) + income / 20);
    }

    public void OnClick()
    {
        System.Random rnd = new System.Random();
        double delta = Math.Ceiling(rnd.NextDouble() * f());
        money += delta;
        StartCoroutine(ShowTax(delta));
    }

    IEnumerator ShowTax(double x)
    {
        Text T = clickButton.GetComponentInChildren<Text>();
        T.text = OutFloat(x);
        yield return new WaitForSecondsRealtime(0.1f);
        T.text = "КЛИК!";
    }

    private void SwitchMod(GameObject G)
    {
        G.SetActive(!G.activeSelf);
    }

    public bool StoreIsOpen()
    {
        return storePanel.activeSelf || rootsPanel.activeSelf;
    }

    public void SwitchStore()//Open or Close store
    {
        if (StoreIsOpen())//store is open
        {
            storePanel.SetActive(false);
            rootsPanel.SetActive(false);
        }
        else
        {
            storePanel.SetActive(true);
            cars_to_roots.GetComponent<Image>().sprite = picture1;
        }
        SwitchMod(cars_to_roots);
    }

    public void ChangeModStore()//Cars<->Roots
    {
        Image t = cars_to_roots.GetComponent<Image>();
        t.sprite = (t.sprite == picture1 ? picture2 : picture1);
        SwitchMod(storePanel);
        SwitchMod(rootsPanel);
    }

    private void ReCalcItem(int index)
    {
        storeButtons[index].GetComponentInChildren<Text>().text = List_of_items[index].type + "(" + OutFloat(List_of_items[index].cost) + "₽, " + List_of_items[index].capacity + " чел.," + List_of_items[index].ticket.ToString() + "₽/чел." + "): " + List_of_items[index].quantity.ToString();
        storeButtons[index].GetComponentInChildren<Text>().enabled = isAvailable[index];

        if (currentScene != "MoscowGameScene")
        {
            if (rootsButtons[index] == null) return;
            rootsButtons[index].GetComponentInChildren<Text>().text = "Лицензия на " + List_of_items[index].type + " :\n" + OutFloat(licensePrices[index]) + " ₽";
            rootsButtons[index].GetComponentInChildren<Text>().enabled = !isAvailable[index];
        }
    }
    
    public void BuyItem(int index)
    {
        if (!isAvailable[index])
        {
            Debug.Log("Not avaliable");
            return;
        }
        if (money >= List_of_items[index].cost)
        {
            List_of_items[index].quantity++;
            capacity += List_of_items[index].capacity;
            money -= List_of_items[index].cost;
            income += List_of_items[index].capacity * List_of_items[index].ticket;
            capital += List_of_items[index].cost;

            ReCalcItem(index);

            if (List_of_items[index].quantity == 1)
            {
                GreetingScript.PushNotification(firstGreeting[index]);
            }
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    public void BuyLicense(int index)
    {
        if (isAvailable[index])
        {
            Debug.Log("Already purchased");
            return;
        }

        if (money >= licensePrices[index])
        {
            money -= licensePrices[index];
            isAvailable[index] = true;
            ReCalcItem(index);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    public void CheckTramLicense()
    {
        if (currentScene == "OmskGameScene")
        {
            if (isAvailable[2] && !isAvailable[3])
            {
                money += licensePrices[2];
            }
        }
    }

    IEnumerator BonusPerSec()
    {
        while (true)
        {
            money += income;
            yield return new WaitForSeconds(1);
        }
    }

    public void SkipTutorial()
    {
        ConfirmPanel.SetActive(false);
        GreetingScript.PushNotification(firstGreeting[0]);
    }

    public void StartTutorial()
    {
        StartCoroutine(Tutorial());
    }

    private string ReverseString(string s)
    {
        return new string(s.ToCharArray().Reverse().ToArray());
    }

    public string OutFloat(double x)
    {
        if (x < 1) return "0";
        string res = "";
        while (x >= 1)
        {
            x = Math.Floor(x);
            string tmp = (x % 1000).ToString("000");
            x /= 1000;
            tmp = ReverseString(tmp);
            res += tmp + " ";
        }
        int cur = res.Length - 1;
        while (cur > 0 && res[cur] == '0' || res[cur] == ' ') cur--;
        res = res.Remove(cur + 1);
        return ReverseString(res);
    }

#region
    [Header("TUTORIAL")]
    public GameObject[] HintsMain;
    public GameObject[] HintsStore;
    public GameObject[] HintsRoots;
    public GameObject[] HintsLuck;

    IEnumerator TutorialPart(GameObject[] a)
    {
        foreach(GameObject G in a){
            G.SetActive(true);
            yield return new WaitForSeconds(2);
            yield return StartCoroutine(WaitForTouch());
            Debug.Log("CLICK");
            G.SetActive(false);
        }
    }
    IEnumerator Tutorial()
    {
        Debug.Log("TUTORIAL");
        this.ConfirmPanel.SetActive(false);
        yield return StartCoroutine(TutorialPart(HintsMain));

        SwitchStore();

        yield return StartCoroutine(TutorialPart(HintsStore));

        ChangeModStore();

        yield return StartCoroutine(TutorialPart(HintsRoots));

        SwitchStore();

        yield return StartCoroutine(TutorialPart(HintsLuck));

        GreetingScript.PushNotification(firstGreeting[0]);
    }
    
    public IEnumerator WaitForTouch()
    {
        while (!Input.GetMouseButton(0))
            yield return null;
    }
#endregion
}

[Serializable]
public class Item
{
    public Item(string _type, int _cost, int _capacity, int _quantity, int _ticket)
    {
        type = _type; cost = _cost; capacity = _capacity; ticket = _ticket;
    }
    public string type;
    public int cost;
    public int capacity;
    public int quantity;
    public int ticket;
}