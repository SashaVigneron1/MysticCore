using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUIManager : MonoBehaviour
{
    [Header("Balances")]
    [SerializeField] TextMeshProUGUI mainBalance;
    [SerializeField] TextMeshProUGUI premiumBalance;

    [Header("Menus")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject shopMenu;

    [Header("Upgrades")]
    [SerializeField] Button damageButton;
    [SerializeField] TextMeshProUGUI currDamage;
    [SerializeField] TextMeshProUGUI damagePrice;

    [SerializeField] Button attackSpeedButton;
    [SerializeField] TextMeshProUGUI currAttackSpeed;
    [SerializeField] TextMeshProUGUI attackSpeedPrice;

    [SerializeField] Button healthButton;
    [SerializeField] TextMeshProUGUI currHealth;
    [SerializeField] TextMeshProUGUI healthPrice;

    [SerializeField] Button healthRegenButton;
    [SerializeField] TextMeshProUGUI currHealthRegen;
    [SerializeField] TextMeshProUGUI healthRegenPrice;

    [SerializeField] Button shieldButton;
    [SerializeField] TextMeshProUGUI currShield;
    [SerializeField] TextMeshProUGUI shieldPrice;

    [SerializeField] Button coinButton;
    [SerializeField] TextMeshProUGUI currCoin;
    [SerializeField] TextMeshProUGUI coinPrice;


    [SerializeField] Button lifestealButton;
    [SerializeField] GameObject lifestealIcon;
    [SerializeField] TextMeshProUGUI currLifesteal;
    [SerializeField] TextMeshProUGUI lifestealPrice;
    [SerializeField] Button projectilesButton;
    [SerializeField] GameObject projectilesIcon;
    [SerializeField] TextMeshProUGUI currProjectiles;
    [SerializeField] TextMeshProUGUI projectilesPrice;

    [Header("Premium Stuff")]
    [SerializeField] Button doubleCoinsButton;
    [SerializeField] GameObject doubleCoinsIcon;
    [SerializeField] TextMeshProUGUI doubleCoinsPrice;
    bool doubleCoins = false;

    [Header("Other")]
    [SerializeField] TextMeshProUGUI highScore;

    [Header("Coin Packs")]
    [SerializeField] Button coinsPack100;
    [SerializeField] Button coinsPack225;
    [SerializeField] Button coinsPack600;
    [SerializeField] Button coinsPack1300;
    [SerializeField] Button coinsPack2800;
    [SerializeField] Button coinsPack7500;

    [Header("Daily Ads")]
    [SerializeField] GameObject adsScreen;
    [SerializeField] Button buyNoAdsButton;
    [SerializeField] Button watchAdButton;
    [SerializeField] Button receiveReward;
    [SerializeField] Slider adsSlider;


    int maxLifestealUpgrades = 1;
    int lifestealupgrades = 0;
    int maxProjectilesUpgrades = 1;
    int projectilesUpgrades = 0;

    [SerializeField] GameManager gameManager;

    private void Update()
    {
        float balance = gameManager.GetMainBalance();

        float price = float.Parse(damagePrice.text);
        if (price <= balance) damageButton.interactable = true;
        else damageButton.interactable = false;

        price = float.Parse(attackSpeedPrice.text);
        if (price <= balance) attackSpeedButton.interactable = true;
        else attackSpeedButton.interactable = false;

        price = float.Parse(healthPrice.text);
        if (price <= balance) healthButton.interactable = true;
        else healthButton.interactable = false;

        price = float.Parse(healthRegenPrice.text);
        if (price <= balance) healthRegenButton.interactable = true;
        else healthRegenButton.interactable = false;

        price = float.Parse(shieldPrice.text);
        if (price <= balance) shieldButton.interactable = true;
        else shieldButton.interactable = false;

        price = float.Parse(coinPrice.text);
        if (price <= balance) coinButton.interactable = true;
        else coinButton.interactable = false;

        //Premium
        float premiumBalance = gameManager.GetPremiumBalance();

        if (lifestealupgrades < maxLifestealUpgrades)
        {
            price = float.Parse(lifestealPrice.text);
            if (price <= premiumBalance) lifestealButton.interactable = true;
            else lifestealButton.interactable = false;
        }
        else
        {
            lifestealButton.interactable = false;
        }

        if (projectilesUpgrades < maxProjectilesUpgrades)
        {
            price = float.Parse(projectilesPrice.text);
            if (price <= premiumBalance) projectilesButton.interactable = true;
            else projectilesButton.interactable = false;
        }
        else
        {
            projectilesButton.interactable = false;
        }

        if (!doubleCoins)
        {
            price = float.Parse(doubleCoinsPrice.text);
            if (price <= premiumBalance) doubleCoinsButton.interactable = true;
            else doubleCoinsButton.interactable = false;
        }
        else
        {
            doubleCoinsButton.interactable = false;
        }

        //NoAds
        price = 999;
        if (price <= premiumBalance) buyNoAdsButton.interactable = true;
        else buyNoAdsButton.interactable = false;

        //coinsPack100
        price = 50;
        if (price <= premiumBalance) coinsPack100.interactable = true;
        else coinsPack100.interactable = false;

        //coinsPack225
        price = 100;
        if (price <= premiumBalance) coinsPack225.interactable = true;
        else coinsPack225.interactable = false;

        //coinsPack600
        price = 250;
        if (price <= premiumBalance) coinsPack600.interactable = true;
        else coinsPack600.interactable = false;

        //coinsPack1300
        price = 500;
        if (price <= premiumBalance) coinsPack1300.interactable = true;
        else coinsPack1300.interactable = false;

        //coinsPack2800
        price = 1000;
        if (price <= premiumBalance) coinsPack2800.interactable = true;
        else coinsPack2800.interactable = false;

        //coinsPack7500
        price = 2500;
        if (price <= premiumBalance) coinsPack7500.interactable = true;
        else coinsPack7500.interactable = false;
    }

    public void UpdateAllUI(GameManager gameManager)
    {
        int rounded = (int)gameManager.GetMainBalance();
        mainBalance.text = rounded.ToString();

        rounded = (int)gameManager.GetPremiumBalance();
        premiumBalance.text = rounded.ToString();
    }

    public void SwitchMenu(int index)
    {
        switch (index)
        {
            case 1:
                playMenu.SetActive(true);
                upgradeMenu.SetActive(false);
                shopMenu.SetActive(false);

                break;
            case 2:
                playMenu.SetActive(false);
                upgradeMenu.SetActive(true);
                shopMenu.SetActive(false);

                break;
            case 3:
                playMenu.SetActive(false);
                upgradeMenu.SetActive(false);
                shopMenu.SetActive(true);

                break;
        }
    }

    public void HideUI()
    {
        mainMenu.SetActive(false);
    }
    public void ShowUI()
    {
        mainMenu.SetActive(true);
        SwitchMenu(1);
    }


    public void UpdateADUI(int minValue, int maxValue, int currValue, bool hasReceived)
    {
        adsSlider.minValue = minValue;
        adsSlider.maxValue = maxValue;
        adsSlider.value = currValue;

        if (currValue == maxValue && !hasReceived)
        {
            receiveReward.interactable = true;
            watchAdButton.interactable = false;
        }
        else if (!hasReceived)
        {
            receiveReward.interactable = false;
            watchAdButton.interactable = true;
        }
        else
        {
            receiveReward.interactable = false;
            watchAdButton.interactable = false;
        }
    }
    public void ShowAdScreen(bool value)
    {
        adsScreen.SetActive(value);
    }



    #region Upgrades
    public void UpdateDamageUI(float currentDamage, float price)
    {
        currDamage.text = ((int)currentDamage).ToString();
        damagePrice.text = ((int)price).ToString();
    }
    public void UpdateAttackSpeedUI(float currentAS, float price)
    {
        currAttackSpeed.text = ((int)currentAS).ToString() + "%";
        attackSpeedPrice.text = ((int)price).ToString();
    }
    public void UpdateHealthUI(float currentHealth, float price)
    {
        currHealth.text = ((int)currentHealth).ToString();
        healthPrice.text = ((int)price).ToString();
    }
    public void UpdateHealthRegenUI(float currentHealthRegen, float price)
    {
        currHealthRegen.text = string.Format("{0:F2}", currentHealthRegen);
        healthRegenPrice.text = ((int)price).ToString();
    }
    public void UpdateShieldUI(float currentShield, float price)
    {
        currShield.text = string.Format("{0:F2}", currentShield);
        shieldPrice.text = ((int)price).ToString();
    }
    public void UpdateCoinUI(float currentCoin, float price)
    {
        currCoin.text = string.Format("{0:F2}", currentCoin);
        coinPrice.text = ((int)price).ToString();
    }
    public void UpdateLifestealUI(float currentLifesteal, float price, int upgradeCount, int maxUpgradeCount)
    {
        string value = string.Format("{0:F2}", currentLifesteal);
        currLifesteal.text = value + "%";
        lifestealPrice.text = ((int)price).ToString();

        lifestealupgrades = upgradeCount;
        maxLifestealUpgrades = maxUpgradeCount;

        if (lifestealupgrades >= maxLifestealUpgrades)
        {
            lifestealPrice.gameObject.SetActive(false);
            lifestealIcon.SetActive(false);
        }
    }
    public void UpdateProjectilesUI(float currentProjectiles, float price, int upgradeCount, int maxUpgradeCount)
    {
        currProjectiles.text = ((int)currentProjectiles).ToString();
        projectilesPrice.text = ((int)price).ToString();

        projectilesUpgrades = upgradeCount;
        maxProjectilesUpgrades = maxUpgradeCount;

        if (projectilesUpgrades >= maxProjectilesUpgrades)
        {
            projectilesPrice.gameObject.SetActive(false);
            projectilesIcon.SetActive(false);
        }

    }
    public void UpdateDoubleCoinsUI(bool doubleCoinsIn)
    {
        doubleCoins = doubleCoinsIn;
        if (doubleCoins)
        {
            doubleCoinsPrice.text = "Sold Out";
            doubleCoinsIcon.gameObject.SetActive(false);
        }
    }

    public void UpdateHighscoreText(float value)
    {
        int minutes = (int)(value / 60.0f);
        int seconds = (int)(value % 60.0f);
        highScore.text = "Highscore: " + minutes.ToString() + "m" + seconds.ToString() + "s";
    }
    #endregion
}
