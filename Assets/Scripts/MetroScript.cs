using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MetroScript : MonoBehaviour {
    public GameObject Grid;
    public string[] lineNames;
    public bool[] isBoughtLine;
    public int[] lineCapacities;
    public Sprite tickSprite;

    public GameScript GameScript;
    public GreetingScript GreetingScript;

    public bool isOpenedMetro;
    public bool isAllLines;

    public GameObject multiplierPanel;

    private void Start()
    {
    	for (int i = 1; i < lineNames.Length; i++)
    	{
    	 	ReCalcItem(i);
    	}
    }

    private void OnEnable()
    {
        int sum = 0;
        for (int i = 1; i < lineNames.Length; i++)
        {
            lineCapacities[i] *= 18; lineCapacities[i] /= 10;
            GameObject L = Get(i);
            Button B = L.GetComponentInChildren<Button>();
            Text T = B.GetComponent<Text>();
            T.text = lineNames[i] + "\n" + GameScript.OutFloat(lineCapacities[i]) + " 000 чел.\n(" + GameScript.OutFloat(GetPrice(i)) + "₽, X" + GetMultiply(i).ToString("F4") + ")";
        }
        Debug.Log(sum);
    }

    private void LateUpdate()
    {
        if (!isOpenedMetro)
        {
            if (GameScript.capacity >= 300 * 1000 || AllItemsBought())
            {
                isOpenedMetro = true;
                GreetingScript.PushNotification("Этого транспорта явно недостаточно для такого большого города. Вы выступаете с предложением предоставить метрополитен на ночь, чтобы там ходили ваши оборудованные микроавтобусы в сцепке друг с другом\n" +
            "*Обратите внимание на кнопку метро в меню магазина и множитель денег на главном экране");
            }
        }
        GameScript.cars_to_roots.SetActive(GameScript.StoreIsOpen() && isOpenedMetro);
        multiplierPanel.SetActive(isOpenedMetro);
        multiplierPanel.GetComponent<Text>().text = "X" + GameScript.multiplier.ToString("F4");

        if (!isAllLines)
        {
            if (AllLinesBought())
            {
                StartCoroutine(FinishGame());
            }
        }
    }

    IEnumerator FinishGame()
    {
        isAllLines = true;
        GreetingScript.PushNotification("Ура! Вы захватили все города! Это стоило того, не правда ли? Подождите совсем чуть-чуть...");
        yield return StartCoroutine(GameScript.WaitForTouch());
        yield return new WaitForSecondsRealtime(5f);
        SceneManager.LoadScene("AboutScene");
    }

    private bool AllLinesBought()
    {
        foreach (bool ok in isBoughtLine)
        {
            if (!ok)
                return false;
        }
        return true;
    }
    private bool AllItemsBought()
    {
        foreach(Item I in GameScript.List_of_items)
        {
            if (I.quantity == 0)
                return false;
        }
        return true;
    }

    GameObject Get(int index)
    {
        string s = "MetroLine (" + index.ToString() + ")";
        return Grid.transform.Find(s).gameObject;
    }

    private void ReCalcItem(int index)
    {
        GameObject L = Get(index);

        if (isBoughtLine[index])
        {
            Transform T = L.transform.Find("Tick");
            Image I = T.GetComponent<Image>();
            I.sprite = tickSprite;
        }
    }

    public double GetMultiply(int index)
    {
        return 1.0 + lineCapacities[index] / 10000.0f * 2;
    }

    public double GetPrice(int index)
    {
        return lineCapacities[index] * 100000 * 5;
    }

    public void BuyLine(int index)
    {
        double price = GetPrice(index);
        if (GameScript.money < price)
        {
            Debug.Log("Not enough money");
            return;
        }    
        if (isBoughtLine[index])
        {
            Debug.Log("Already bought");
            return;
        }
        GameScript.money -= price;
        isBoughtLine[index] = true;
        GameScript.capacity += lineCapacities[index] * 1000;
        GameScript.multiplier *= GetMultiply(index);
        ReCalcItem(index);
    }
}