using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


using PlayFab;
using PlayFab.ClientModels;

public class GameManager : MonoBehaviour
{
    public float timePlayed;

    [SerializeField] GameObject loadingScreenPrefab;

    [Header("Balances")]
    [SerializeField] float inGameBalance;
    [SerializeField] public float mainBalance;
    [SerializeField] public float premiumBalance;
    [SerializeField] float minMainCoinsToReceivePer30Seconds;
    [SerializeField] float maxMainCoinsToReceivePer30Seconds;

    [Header("Tower")]
    [SerializeField] float secondsPerAttack;
    [SerializeField] float attackSpeed;
    [SerializeField] float damage;

    [SerializeField] float maxHealth;
    [SerializeField] float healthRegenPerSecond;
    [SerializeField] float defense;

    [SerializeField] float radius;

    [SerializeField] float coinMultiplier;

    //Premium
    [SerializeField] float lifesteal;
    [SerializeField] int projectiles;

    [Header("Enemies")]
    [SerializeField] float minMoneyPerEnemy;
    [SerializeField] float maxMoneyPerEnemy;

    bool skipAds = false;
    [Header("Other")]
    [SerializeField] AdsManager adsManager;
    [SerializeField] PlayFabSaveManager iosSaveManager;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] UIManager uiManager;
    [SerializeField] MenuUIManager menuUIManager;
    [SerializeField] GameObject gameScene;
    [SerializeField] MysticTower tower;

    public enum GameState
    {
        mainMenu,
        game,
        dead
    }
    GameState gameState = GameState.mainMenu;
    float timeAlive;
    float highScore;

    private void Awake()
    {
        if (FindObjectsOfType<GameManager>().Length > 1) Destroy(this.gameObject);
        else DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        menuUIManager.UpdateAllUI(this);
        UpdateUI();
    }

    private void Update()
    {
        timePlayed += Time.deltaTime;

        switch (gameState)
        {
            case GameState.mainMenu:

                

                break;
            case GameState.game:
                timeAlive += Time.deltaTime;
                uiManager.SetTimeAlive(timeAlive);
                break;
            case GameState.dead:
                //Time.timeScale = 0.0f;

                break;
        }
    }

    private void OnApplicationQuit()
    {

    }

    float currCoinsToReceive;
    public float GetCoinsToReceive()
    {
        currCoinsToReceive = (1 + (int)(timeAlive / 30.0f)) * UnityEngine.Random.Range(minMainCoinsToReceivePer30Seconds, maxMainCoinsToReceivePer30Seconds);
        if (doubleCoins) currCoinsToReceive *= 2.0f;
        if (hardMode) currCoinsToReceive *= 2.0f;
        return currCoinsToReceive;
    }

    public void SetStateToDead()
    {
        gameState = GameState.dead;
        enemyManager.StopGame(true);
        tower.Stop(true);
    }

    public void EndGame()
    {
        Instantiate(loadingScreenPrefab, new Vector3(0,0,0), Quaternion.identity);

        StartCoroutine(EndGameWithLoadingScreen());
    }

    IEnumerator EndGameWithLoadingScreen()
    {
        yield return new WaitForSeconds(1.0f);

        mainBalance += currCoinsToReceive;

        gameScene.SetActive(false);
        enemyManager.DeleteAllEnemies();
        gameState = GameState.mainMenu;

        if (timeAlive > highScore) { highScore = timeAlive; menuUIManager.UpdateHighscoreText(highScore); }

        timeAlive = 0.0f;
        menuUIManager.ShowUI();
        UpdateUI();
        menuUIManager.UpdateAllUI(this);
        Time.timeScale = 1.0f;
        ForceSave();

        SceneManager.LoadScene(0);
    }

    public void SetTowerVariables(MysticTower tower)
    {
        if (tower != null)
        {
            tower.SetSecondsPerAttack(secondsPerAttack);
            tower.SetAttackSpeed(attackSpeed);
            tower.SetDamage(damage);
            tower.SetMaxHealth(maxHealth);
            tower.SetHealthRegenPerSecond(healthRegenPerSecond);
            tower.SetDefense(defense);
            tower.SetRadius(radius);
            tower.SetCoinMultiplier(coinMultiplier);
            tower.SetProjectileCount(projectiles);
            tower.SetLifeSteal(lifesteal);
            tower.SetDoubleCoins(doubleCoins);
            tower.ResetDeath();

            tower.ShowEndscreen(false);
            tower.ResetPowerup();
            tower.StopAdRoutine();

            tower.ResetUpgrades();

            StartCoroutine(tower.WaitAndShowAdPopup());

            tower.UpdateUI();
        }
    }

    public float AddEnemyMoney()
    {
        float addedValue = UnityEngine.Random.Range(minMoneyPerEnemy, maxMoneyPerEnemy);
        addedValue *= tower.GetCoinMultiplier();
        if (hardMode) addedValue /= 2.0f;
        if (addedValue < 1.0f) addedValue = 1.0f;

        inGameBalance += addedValue;
        uiManager.SetIngameBalanceUI(inGameBalance);
        return addedValue;
    }

#region Mutators
    public float GetInGameBalance()
    {
        return inGameBalance;
    }
    public float GetMainBalance()
    {
        return mainBalance;
    }
    public float GetPremiumBalance()
    {
        return premiumBalance;
    }
    public float GetTimeAlive()
    {
        return timeAlive;
    }
#endregion


    public void RemoveInGameBalance(float value)
    {
        inGameBalance -= value;
        uiManager.SetIngameBalanceUI(inGameBalance);
    }

#region Upgrades
    int damageUpgrades = 0;
    int attackSpeedUpgrades = 0;
    int healthUpgrades = 0;
    int healthRegenUpgrades = 0;
    int shieldUpgrades = 0;
    int coinUpgrades = 0;

    // premium
    int lifestealUpgrades = 0;
    int projectilesUpgrades = 0;
    int maxLifestealUpgrades = 5;
    int maxProjectileUpgrades = 4;

    [Header("Upgrades")]
    [SerializeField] float damageUpgradePrice;
    [SerializeField] float attackSpeedUpgradePrice;
    [SerializeField] float healthUpgradePrice;
    [SerializeField] float healthRegenUpgradePrice;
    [SerializeField] float shieldUpgradePrice;
    [SerializeField] float coinMultiplierUpgradePrice;
    //Premium
    [Header("Premium Upgrades")]
    [SerializeField] float lifestealUpgradePrice;
    [SerializeField] float projectilesUpgradePrice;
    [SerializeField] bool doubleCoins;

    public void UpgradeLifesteal()
    {
        // Add damage
        lifesteal += 0.02f;

        // Pay for it
        premiumBalance -= lifestealUpgradePrice + (lifestealUpgrades * lifestealUpgrades * 1.05f);
        lifestealUpgrades++;

        // Set new price
        float newPrice = 0.0f;
        switch (lifestealUpgrades)
        {
            case 1:
                newPrice = 400.0f;
                break;
            case 2:
                newPrice = 600.0f;
                break;
            case 3:
                newPrice = 800.0f;
                break;
            case 4:
                newPrice = 1000.0f;
                break;
            default:
                break;
        }


        menuUIManager.UpdateAllUI(this);
        menuUIManager.UpdateLifestealUI(lifesteal * 100, newPrice, lifestealUpgrades, maxLifestealUpgrades);
    }
    public void UpgradeProjectiles()
    {
        // Add damage
        projectiles += 1;

        // Pay for it
        premiumBalance -= projectilesUpgradePrice + (projectilesUpgrades * projectilesUpgrades * 1.05f);

        projectilesUpgrades++;

        // Set new price
        float newPrice = 0.0f;
        switch (projectilesUpgrades)
        {
            case 1:
                newPrice = 500.0f;
                break;
            case 2:
                newPrice = 700.0f;
                break;
            case 3:
                newPrice = 1000.0f;
                break;
            default:
                break;
        }

        menuUIManager.UpdateAllUI(this);
        menuUIManager.UpdateProjectilesUI(projectiles, newPrice, projectilesUpgrades, maxProjectileUpgrades);
    }
    public void UpgradeDamage()
    {
        // Add damage
        damage *= 1.2f;

        // Pay for it
        mainBalance -= damageUpgradePrice + (damageUpgrades * damageUpgrades * 1.05f);
        damageUpgrades++;

        // Set new price
        float newPrice = damageUpgradePrice + (damageUpgrades * damageUpgrades * 1.05f);

        menuUIManager.UpdateAllUI(this);
        menuUIManager.UpdateDamageUI(damage, newPrice);
    }
    public void UpgradeAttackSpeed()
    {
        // Add damage
        attackSpeed *= 1.01f;

        // Pay for it
        mainBalance -= attackSpeedUpgradePrice + (attackSpeedUpgrades * attackSpeedUpgrades * 1.10f);
        attackSpeedUpgrades++;

        // Set new price
        float newPrice = attackSpeedUpgradePrice + (attackSpeedUpgrades * attackSpeedUpgrades * 1.10f);

        menuUIManager.UpdateAllUI(this);
        menuUIManager.UpdateAttackSpeedUI(attackSpeed * 100, newPrice);
    }
    public void UpgradeHealth()
    {
        // Add damage
        float lastHealth = maxHealth;
        maxHealth *= 1.2f;
        float diffHealth = maxHealth - lastHealth;

        // Pay for it
        mainBalance -= healthUpgradePrice + (healthUpgrades * healthUpgrades * 1.05f);

        healthUpgrades++;

        // Set new price
        float newPrice = healthUpgradePrice + (healthUpgrades * healthUpgrades * 1.05f);

        menuUIManager.UpdateAllUI(this);
        menuUIManager.UpdateHealthUI(maxHealth, newPrice);
    }
    public void UpgradeHealthRegen()
    {
        // Add damage
        healthRegenPerSecond *= 1.2f;


        // Pay for it
        mainBalance -= healthRegenUpgradePrice + (healthRegenUpgrades * healthRegenUpgrades * 1.05f);

        healthRegenUpgrades++;

        // Set new price
        float newPrice = healthRegenUpgradePrice + (healthRegenUpgrades * healthRegenUpgrades * 1.05f);

        menuUIManager.UpdateAllUI(this);
        menuUIManager.UpdateHealthRegenUI(healthRegenPerSecond, newPrice);
    }
    public void UpgradeShield()
    {
        // Add damage
        defense *= 1.05f;


        // Pay for it
        mainBalance -= shieldUpgradePrice + (shieldUpgrades * shieldUpgrades * 1.05f);

        shieldUpgrades++;

        // Set new price
        float newPrice = shieldUpgradePrice + (shieldUpgrades * shieldUpgrades * 1.05f);

        menuUIManager.UpdateAllUI(this);
        menuUIManager.UpdateShieldUI(defense, newPrice);
    }
    public void UpgradeCoin()
    {
        // Add damage
        coinMultiplier *= 1.02f;


        // Pay for it
        mainBalance -= coinMultiplierUpgradePrice + (coinUpgrades * coinUpgrades * 1.2f);
        coinUpgrades++;

        // Set new price
        float newPrice = coinMultiplierUpgradePrice + (coinUpgrades * coinUpgrades * 1.2f);

        menuUIManager.UpdateAllUI(this);
        menuUIManager.UpdateCoinUI(coinMultiplier, newPrice);
    }

    [Header("Play Settings")]
    [SerializeField] TMP_Dropdown dropDown;
    public void SetHardMode()
    {
        if (dropDown.value == 0)
        {
            hardMode = false;
        }
        else
        {
            hardMode = true;
        }
    }

    bool hardMode = false;

    public void StartGame()
    {
        Instantiate(loadingScreenPrefab, new Vector3(0,0,0), Quaternion.identity);
        StartCoroutine(StartGameWithLoadingScreen());
       
    }
    IEnumerator StartGameWithLoadingScreen()
    {
        yield return new WaitForSeconds(1.0f);
        gameState = GameState.game;
        SetTowerVariables(tower);
        gameScene.SetActive(true);
        ForceSave();

        inGameBalance = 0.0f;
        uiManager.UpdateAllUI(this);
        menuUIManager.UpdateAllUI(this);
        menuUIManager.HideUI();

        enemyManager.ResetLevelTimer();

        tower.Stop(false);
        enemyManager.StopGame(false);

        if (hardMode)
        {
            enemyManager.SetHardMode(true);
        }
        else
        {
            enemyManager.SetHardMode(false);
        }

        SceneManager.LoadScene(1);
    }

    public void UpdateUI()
    {
        float damagePrice = damageUpgradePrice + (damageUpgrades * damageUpgrades * 1.05f);
        float attackSpeedPrice = attackSpeedUpgradePrice + (attackSpeedUpgrades * attackSpeedUpgrades * 1.05f);
        float healthPrice = healthUpgradePrice + (healthUpgrades * healthUpgrades * 1.05f);
        float healthRegenPrice = healthRegenUpgradePrice + (healthRegenUpgrades * healthRegenUpgrades * 1.05f);
        float shieldPrice = shieldUpgradePrice + (shieldUpgrades * shieldUpgrades * 1.05f);
        float coinPrice = coinMultiplierUpgradePrice + (coinUpgrades * coinUpgrades * 1.05f);

        menuUIManager.UpdateDamageUI(damage, damagePrice);
        menuUIManager.UpdateAttackSpeedUI(attackSpeed * 100, attackSpeedPrice);
        menuUIManager.UpdateHealthUI(maxHealth, healthPrice);
        menuUIManager.UpdateHealthRegenUI(healthRegenPerSecond, healthRegenPrice);
        menuUIManager.UpdateShieldUI(defense, shieldPrice);
        menuUIManager.UpdateCoinUI(coinMultiplier, coinPrice);

        float lifestealPrice = 0.0f;
        switch (lifestealUpgrades)
        {
            case 0:
                lifestealPrice = 200.0f;
                break;
            case 1:
                lifestealPrice = 400.0f;
                break;
            case 2:
                lifestealPrice = 600.0f;
                break;
            case 3:
                lifestealPrice = 800.0f;
                break;
            case 4:
                lifestealPrice = 1000.0f;
                break;
            default:
                break;
        }
        menuUIManager.UpdateLifestealUI(lifesteal * 100, lifestealPrice, lifestealUpgrades, maxLifestealUpgrades);
        float projectilePrice = 0.0f;
        switch (projectilesUpgrades)
        {
            case 0:
                projectilePrice = 300.0f;
                break;
            case 1:
                projectilePrice = 500.0f;
                break;
            case 2:
                projectilePrice = 700.0f;
                break;
            case 3:
                projectilePrice = 1000.0f;
                break;
            default:
                break;
        }
        menuUIManager.UpdateProjectilesUI(projectiles, projectilePrice, projectilesUpgrades, maxProjectileUpgrades);

        menuUIManager.UpdateDoubleCoinsUI(doubleCoins);
        menuUIManager.UpdateHighscoreText(highScore);
    }
#endregion

    public void UnlockCoinMultiplier(float price)
    {
        doubleCoins = true;
        premiumBalance -= price;
        menuUIManager.UpdateAllUI(this);
        menuUIManager.UpdateDoubleCoinsUI(doubleCoins);
        ForceSave();

    }

    public void BuyCoinPack(int amount)
    {
        float cost = 0.0f;
        float reward = 0.0f;
        switch (amount)
        {
            case 100:
                cost = 50.0f;
                reward = 100.0f;
                break;
            case 225:
                cost = 100.0f;
                reward = 225.0f;
                break;
            case 600:
                cost = 250.0f;
                reward = 600.0f;
                break;
            case 1300:
                cost = 500.0f;
                reward = 1300.0f;
                break;
            case 2800:
                cost = 1000.0f;
                reward = 2800.0f;
                break;
            case 7500:
                cost = 2500.0f;
                reward = 7500.0f;
                break;
        }
        premiumBalance -= cost;
        mainBalance += reward;
        menuUIManager.UpdateAllUI(this);
        ForceSave();

    }
    public void BuyShardsPack(int amount)
    {
        premiumBalance += amount;
        menuUIManager.UpdateAllUI(this);
        ForceSave();
    }

    public void SetHighScore(float value)
    {
        highScore = value;
        menuUIManager.UpdateHighscoreText(highScore);
    }

    public void SetGameState(GameManager.GameState state)
    {
        gameState = state;
    }

    public void RemoveAdsPurchase(float price)
    {
        skipAds = true;
        premiumBalance -= price;
        adsManager.SetSkipAds(skipAds);
        menuUIManager.UpdateAllUI(this);
        ForceSave();
    }

    // Daily Ads
    int totalDailyAds = 3;
    int currDailyAds = 0;

    //ToDo: Do Daily Checks
    bool hasReceived = false;

    public void WatchDailyAd()
    {
#if UNITY_IOS
        adsManager.ShowRewardedAd("IOS_DailyAd");
#else
        adsManager.ShowRewardedAd("Android_DailyAd");
#endif
    }
    public void WatchedDailyAd()
    {
        currDailyAds++;

        menuUIManager.UpdateADUI(0, totalDailyAds, currDailyAds, hasReceived);
        menuUIManager.UpdateAllUI(this);
    }
    public void SetHasReceivedAdReward(bool value)
    {
        hasReceived = true;
    }

    public void AddAdBalance(float amount)
    {
        premiumBalance += amount;
        hasReceived = true;
        menuUIManager.UpdateADUI(0, totalDailyAds, currDailyAds, hasReceived);
        menuUIManager.UpdateAllUI(this);
        ForceSave();

    }

    public void ShowDailyAdsScreen(bool value)
    {
        menuUIManager.ShowAdScreen(value);
        menuUIManager.UpdateADUI(0, totalDailyAds, currDailyAds, hasReceived);
    }


    public void TryLoggingInAgain()
    {
        iosSaveManager.Login();
    }


    public IOSSaveState SetIOSSaveGame(IOSSaveState state)
    {
        DateTime currentDay = DateTime.Now;

        if ((currentDay - state.lastDay).TotalDays >= 1)
            state.lastDay = currentDay;

        // Balances
        state.mainBalance = mainBalance;
        state.premiumBalance = premiumBalance;

        // Tower
        state.secondsPerAttack = secondsPerAttack;
        state.attackSpeed = attackSpeed;
        state.damage = damage;
        state.maxHealth = maxHealth;
        state.healthRegenPerSecond = healthRegenPerSecond;
        state.defense = defense;
        state.radius = radius;
        state.coinMultiplier = coinMultiplier;
        state.lifesteal = lifesteal;
        state.projectiles = projectiles;

        state.damageUpgrades = damageUpgrades;
        state.attackSpeedUpgrades = attackSpeedUpgrades;
       state.healthRegenUpgrades = healthRegenUpgrades;
        state.shieldUpgrades = shieldUpgrades;
        state.coinUpgrades = coinUpgrades;
        state.lifestealUpgrades = lifestealUpgrades;
        state.projectilesUpgrades = projectilesUpgrades;

        state.highScore = highScore;

        state.skipAds = skipAds;
        state.doubleCoins = doubleCoins;

        state.hasReceived = hasReceived;
        state.currDailyAds = currDailyAds;

        state.isEmpty = false;

        return state;
    }

    public void LoadUserData(GetUserDataResult result)
    {
        // Balances
        mainBalance = float.Parse(result.Data["mainBalance"].Value);
        premiumBalance = float.Parse(result.Data["premiumBalance"].Value);

        // Tower
        secondsPerAttack = float.Parse(result.Data["secondsPerAttack"].Value);
        attackSpeed = float.Parse(result.Data["attackSpeed"].Value);
        damage = float.Parse(result.Data["damage"].Value);
        maxHealth = float.Parse(result.Data["maxHealth"].Value);
        healthRegenPerSecond = float.Parse(result.Data["healthRegenPerSecond"].Value);
        defense = float.Parse(result.Data["defense"].Value);
        radius = float.Parse(result.Data["radius"].Value);
        coinMultiplier = float.Parse(result.Data["coinMultiplier"].Value);
        lifesteal = float.Parse(result.Data["lifesteal"].Value);
        projectiles = int.Parse(result.Data["projectiles"].Value);

        // Upgrades
        damageUpgrades = int.Parse(result.Data["damageUpgrades"].Value);
        attackSpeedUpgrades = int.Parse(result.Data["attackSpeedUpgrades"].Value);
        healthUpgrades = int.Parse(result.Data["healthUpgrades"].Value);
        healthRegenUpgrades = int.Parse(result.Data["healthRegenUpgrades"].Value);
        shieldUpgrades = int.Parse(result.Data["shieldUpgrades"].Value);
        coinUpgrades = int.Parse(result.Data["coinUpgrades"].Value);
        lifestealUpgrades = int.Parse(result.Data["lifestealUpgrades"].Value);
        projectilesUpgrades = int.Parse(result.Data["projectilesUpgrades"].Value);

        highScore = float.Parse(result.Data["highScore"].Value);

        skipAds = bool.Parse(result.Data["skipAds"].Value);
        doubleCoins = bool.Parse(result.Data["doubleCoins"].Value);

        hasReceived = bool.Parse(result.Data["hasReceived"].Value);
        currDailyAds = int.Parse(result.Data["currDailyAds"].Value);


        DateTime currentDay = DateTime.Now;
        DateTime lastDay = DateTime.Parse(result.Data["lastDay"].Value);

        if ((currentDay - lastDay).TotalDays >= 1)
        {
            currDailyAds = 0;
            hasReceived = false;
        }

        menuUIManager.UpdateAllUI(this);
        UpdateUI();
        menuUIManager.UpdateADUI(0, totalDailyAds, currDailyAds, hasReceived);
    }

    public void OnLoginSucces()
    {
        Load();
        StartCoroutine(WaitForSave());
    }
    IEnumerator WaitForSave()
    {
        yield return new WaitForSeconds(300.0f);

        iosSaveManager.SaveData();
        StartCoroutine(WaitForSave());
    }
    public void Load()
    {
        iosSaveManager.GetUserData();
        Debug.Log("Loading from cloud...");
    }


    void ForceSave()
    {
        iosSaveManager.SaveData();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            ForceSave();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            ForceSave();
    }
}
