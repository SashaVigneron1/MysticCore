using System;
using UnityEngine;


using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class IOSSaveState
{
    // Main
    public DateTime lastDay{ set; get; }
    public bool isEmpty { set; get; } = true;

    //// GameManager
    public float mainBalance { set; get; }
    public float premiumBalance { set; get; }
    // Tower
    public float secondsPerAttack { set; get; }
    public float attackSpeed { set; get; }
    public float damage { set; get; }
    public float maxHealth { set; get; }
    public float healthRegenPerSecond { set; get; }
    public float defense { set; get; }
    public float radius { set; get; }
    public float coinMultiplier { set; get; }
    public float lifesteal { set; get; }
    public int projectiles { set; get; }

    // Upgrades
    public int damageUpgrades { set; get; }
    public int attackSpeedUpgrades { set; get; }
    public int healthUpgrades { set; get; }
    public int healthRegenUpgrades { set; get; }
    public int shieldUpgrades { set; get; }
    public int coinUpgrades { set; get; }
    public int lifestealUpgrades { set; get; }
    public int projectilesUpgrades { set; get; }


    public float highScore { set; get; }

    public bool skipAds { set; get; }
    public bool doubleCoins { set; get; }

    public bool hasReceived { set; get; }
    public int currDailyAds { set; get; }
}


public class PlayFabSaveManager : MonoBehaviour
{

    [Header("UI")]
    [SerializeField] GameObject loadingOverlay;
    [SerializeField] GameObject loadingPopup;
    [SerializeField] GameManager gameManager;
    [SerializeField] bool onlyLoadIfEverythingIsPresent;
    IOSSaveState state = new IOSSaveState();
    bool loggingIn = false;
    bool isLoggedIn = false;

    private void Start()
    {
        Login();
    }

    public void Login()
    {
        loggingIn = true;
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSucces, OnError);
    }

    void OnSucces(LoginResult result)
    {
        Debug.Log("Successful login/account created!");
        loadingOverlay.SetActive(false);
        gameManager.OnLoginSucces();
        loggingIn = false;
        isLoggedIn = true;
    }
    void OnError(PlayFabError error)
    {
        if (loggingIn)
        {
            Debug.Log("Error while logging in/creating account!");
            loadingPopup.SetActive(true);
            loggingIn = false;
        }
        Debug.Log(error.GenerateErrorReport());
    }

    public void SaveData()
    {
        if (!isLoggedIn) return;

        state = gameManager.SetIOSSaveGame(state);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"mainBalance", state.mainBalance.ToString() },
                {"premiumBalance", state.premiumBalance.ToString() },

                {"secondsPerAttack", state.secondsPerAttack.ToString() },
                {"attackSpeed", state.attackSpeed.ToString() },
                {"damage", state.damage.ToString() },
                {"maxHealth", state.maxHealth.ToString() },
                {"healthRegenPerSecond", state.healthRegenPerSecond.ToString() },
                {"defense", state.defense.ToString() },
                {"radius", state.radius.ToString() },
                {"coinMultiplier", state.coinMultiplier.ToString() },
            }
        };

        
        
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
        StartCoroutine(WaitForRequest2());
    }

    IEnumerator WaitForRequest2()
    {
        yield return new WaitForSeconds(1.5f);
        var request2 = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"lifesteal", state.lifesteal.ToString() },
                {"projectiles", state.projectiles.ToString() },

                {"damageUpgrades", state.damageUpgrades.ToString() },
                {"attackSpeedUpgrades", state.attackSpeedUpgrades.ToString() },
                {"healthUpgrades", state.healthUpgrades.ToString() },
                {"healthRegenUpgrades", state.healthRegenUpgrades.ToString() },
                {"shieldUpgrades", state.shieldUpgrades.ToString() },
                {"coinUpgrades", state.coinUpgrades.ToString() },
                {"lifestealUpgrades", state.lifestealUpgrades.ToString() },
                {"projectilesUpgrades", state.projectilesUpgrades.ToString() },
            }
        };

        PlayFabClientAPI.UpdateUserData(request2, OnDataSend, OnError);

        StartCoroutine(WaitForRequest3());
    }
    IEnumerator WaitForRequest3()
    {
        yield return new WaitForSeconds(1.5f);
        var request3 = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"highScore", state.highScore.ToString() },

                {"skipAds", state.skipAds.ToString() },
                {"doubleCoins", state.doubleCoins.ToString() },

                {"hasReceived", state.hasReceived.ToString() },
                {"currDailyAds", state.currDailyAds.ToString() },
                {"lastDay", state.lastDay.ToString() },
            }
        };

        PlayFabClientAPI.UpdateUserData(request3, OnDataSend, OnError);
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Successful user data sent!");
    }


    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnError);
    }
    void OnDataReceived(GetUserDataResult result)
    {
        Debug.Log("Received user data!");

        if (onlyLoadIfEverythingIsPresent)
        {
            if (result.Data != null
                    && result.Data.ContainsKey("mainBalance")
                    && result.Data.ContainsKey("premiumBalance")

                    && result.Data.ContainsKey("secondsPerAttack")
                    && result.Data.ContainsKey("attackSpeed")
                    && result.Data.ContainsKey("damage")
                    && result.Data.ContainsKey("maxHealth")
                    && result.Data.ContainsKey("healthRegenPerSecond")
                    && result.Data.ContainsKey("defense")
                    && result.Data.ContainsKey("radius")
                    && result.Data.ContainsKey("coinMultiplier")
                    && result.Data.ContainsKey("lifesteal")
                    && result.Data.ContainsKey("projectiles")

                    && result.Data.ContainsKey("damageUpgrades")
                    && result.Data.ContainsKey("attackSpeedUpgrades")
                    && result.Data.ContainsKey("healthUpgrades")
                    && result.Data.ContainsKey("healthRegenUpgrades")
                    && result.Data.ContainsKey("shieldUpgrades")
                    && result.Data.ContainsKey("coinUpgrades")
                    && result.Data.ContainsKey("lifestealUpgrades")
                    && result.Data.ContainsKey("projectilesUpgrades")

                    && result.Data.ContainsKey("highScore")

                    && result.Data.ContainsKey("skipAds")
                    && result.Data.ContainsKey("doubleCoins")

                    && result.Data.ContainsKey("hasReceived")
                    && result.Data.ContainsKey("currDailyAds")
                    && result.Data.ContainsKey("lastDay")
                    )
            {
                gameManager.LoadUserData(result);
            }
            else
            {
                Debug.Log("Data not complete!");
            }
        }
        else if (result.Data != null)
        {
            gameManager.LoadUserData(result);
        }

    }
}
