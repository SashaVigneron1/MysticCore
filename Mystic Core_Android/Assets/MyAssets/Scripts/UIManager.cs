using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Balances")]
    [SerializeField] TextMeshProUGUI inGameBalance;
    [SerializeField] TextMeshProUGUI mainBalance;
    [SerializeField] TextMeshProUGUI premiumBalance;

    [Header("Upgrades")]
    [SerializeField] Button damageButton;
    [SerializeField] TextMeshProUGUI currDamage;
    [SerializeField] TextMeshProUGUI currDamage2;
    [SerializeField] TextMeshProUGUI damagePrice;

    [SerializeField] Button attackSpeedButton;
    [SerializeField] TextMeshProUGUI currAttackSpeed;
    [SerializeField] TextMeshProUGUI currAttackSpeed2;
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

    [Header("Ads")]
    [SerializeField] GameObject adScreen;
    [SerializeField] GameObject adPopUp;
    [SerializeField] GameObject mainActivePower;
    [SerializeField] GameObject activePowerupAS;
    [SerializeField] GameObject activePowerupLifesteal;
    [SerializeField] GameObject activePowerupProjectiles;
    [SerializeField] GameObject activePowerupHealthRegen;
    [SerializeField] TextMeshProUGUI adPowerup;

    [Header("Other")]
    [SerializeField] TextMeshProUGUI timeAlive;
    [SerializeField] TextMeshProUGUI timeAliveEnd;
    [SerializeField] GameObject doubleCoinsEnd;
    [SerializeField] TextMeshProUGUI receiveCoinsEnd;
    [SerializeField] GameManager gameManager;

    public void SetIngameBalanceUI(float value)
    {
        int rounded = (int)value;
        inGameBalance.text = rounded.ToString();
    }

    public void SetTimeAlive(float value)
    {
        int rounded = (int)value;
        timeAlive.text = "Time Alive: " + rounded.ToString() + "s";
    }

    public void UpdateAllUI(GameManager gameManager)
    {
        int rounded = (int)gameManager.GetInGameBalance();
        inGameBalance.text = rounded.ToString();

        rounded = (int)gameManager.GetMainBalance();
        mainBalance.text = rounded.ToString();

        rounded = (int)gameManager.GetPremiumBalance();
        premiumBalance.text = rounded.ToString();

        SetTimeAlive(gameManager.GetTimeAlive());
    }

    private void Update()
    {
        float balance = gameManager.GetInGameBalance();

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
    }

    #region Upgrades
    public void UpdateDamageUI(float currentDamage, float price)
    {
        currDamage.text = ((int)currentDamage).ToString();
        currDamage2.text = ((int)currentDamage).ToString();
        damagePrice.text = ((int)price).ToString();
    }
    public void UpdateAttackSpeedUI(float currentAS, float price)
    {
        currAttackSpeed.text = ((int)currentAS).ToString() + "%";
        currAttackSpeed2.text = ((int)currentAS).ToString() + "%";
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

    public void UpdateEndScreen(float timeAlive, float coinsToReceive, bool doubleCoins)
    {
        int minutes = (int)(timeAlive / 60.0f);
        int seconds = (int)(timeAlive % 60.0f);
        timeAliveEnd.text = "You lasted: " + minutes.ToString() + "m" + seconds.ToString() + "s";
        receiveCoinsEnd.text = "And earned " + ((int)coinsToReceive).ToString();
        doubleCoinsEnd.SetActive(doubleCoins);
    }
    #endregion

    // Ads
    public void ShowAdPopUp(bool value)
    {
        adPopUp.SetActive(value);
    }
    public void ShowAdScreen(bool value)
    {
        adScreen.SetActive(value);
    }
    public void UpdateAdScreen(MysticTower.TypeOfPowerup powerUpType, float duration)
    {
        switch(powerUpType)
        {
            case MysticTower.TypeOfPowerup.lifesteal:
                adPowerup.text = "10% Lifesteal for " + ((int)duration).ToString() + "s";
                break;
            case MysticTower.TypeOfPowerup.attackSpeed:
                adPowerup.text = "50% AttackSpeed for " + ((int)duration).ToString() + "s";
                break;
            case MysticTower.TypeOfPowerup.projectile:
                adPowerup.text = "1 extra projectile for " + ((int)duration).ToString() + "s";
                break;
            case MysticTower.TypeOfPowerup.healthRegen:
                adPowerup.text = "10hp/s Regen for " + ((int)duration).ToString() + "s";
                break;
        }    
    }
    public void UpdateActiveApp(MysticTower.TypeOfPowerup powerUpType, bool active)
    {
        if (active)
        {
            mainActivePower.SetActive(true);
            activePowerupAS.SetActive(false);
            activePowerupHealthRegen.SetActive(false);
            activePowerupLifesteal.SetActive(false);
            activePowerupProjectiles.SetActive(false);

            switch (powerUpType)
            {
                case MysticTower.TypeOfPowerup.lifesteal:
                    activePowerupLifesteal.SetActive(true);
                    break;
                case MysticTower.TypeOfPowerup.attackSpeed:
                    activePowerupAS.SetActive(true);
                    break;
                case MysticTower.TypeOfPowerup.projectile:
                    activePowerupProjectiles.SetActive(true);
                    break;
                case MysticTower.TypeOfPowerup.healthRegen:
                    activePowerupHealthRegen.SetActive(true);
                    break;
            }
        }
        else
        {
            mainActivePower.SetActive(false);
            activePowerupAS.SetActive(false);
            activePowerupHealthRegen.SetActive(false);
            activePowerupLifesteal.SetActive(false);
            activePowerupProjectiles.SetActive(true);
        }
    }
}
