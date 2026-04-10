using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }
    public int startingMoney = 6;
    public TMP_Text moneyText;

    int currentMoney;


    void Awake()
    {
        // ensure only one instance of TowerBuilder exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // when loading different scene, persists this instance
        // for now, disabled to help with reseting game logic
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentMoney = startingMoney;
        UpdateMoneyText();
    }

    public bool BuyTower(int cost)
    {
        if (currentMoney >= cost)
        {
            // reduce currentMoney and clamp prevent neg
            currentMoney = Mathf.Max(0, currentMoney - cost);
            UpdateMoneyText();
            return true;
        }
        return false;
    }

    public void GetMoney(int reward)
    {
        currentMoney += reward;
        UpdateMoneyText();
    }

    public void UpdateMoneyText()
    {
        if (moneyText)
        {
            moneyText.text = currentMoney.ToString();
        }
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public void ResetMoney()
    {
        currentMoney = startingMoney;
        UpdateMoneyText();
    }
}
