using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{
    bool isDebugging = false;
    bool isAdReady = false;

    bool skipAds = false;

    public void SetSkipAds(bool value)
    {
        skipAds = value;
    }

#if UNITY_IOS
    string gameId = "4484714";
#else
    string gameId = "4484715";
#endif

    GameManager gameManager;
    [SerializeField] MysticTower tower;

    private void Start()
    {
        Advertisement.Initialize(gameId);
        Advertisement.AddListener(this);

        gameManager = GetComponent<GameManager>();
    }

    public void ShowInterstitialAd()
    {
        if (Advertisement.IsReady("Interstitial_Android"))
        {
            Advertisement.Show("Interstitial_Android");
        }
        else
        {
            if (isDebugging) Debug.LogError("Interstitial ad is not ready!");
        }
    }

    public void ShowRewardedAd(string placementName)
    {
        if (skipAds)
        {
            OnUnityAdsDidFinish(placementName, ShowResult.Finished);
        }
        else
        {
            if (Advertisement.IsReady(placementName))
            {
                Advertisement.Show(placementName);
            }
            else
            {
                if (isDebugging) Debug.LogError("Rewarded ad is not ready!");
            }
        }
        
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (isDebugging) Debug.Log("Ads are ready!");
        isAdReady = true;
    }

    public void OnUnityAdsDidError(string message)
    {
        if (isDebugging) Debug.LogError("Error: " + message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        if (isDebugging) Debug.Log("Video started!");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
#if UNITY_IOS
        if (placementId == "IOS_DailyAd" && showResult == ShowResult.Finished)
        {
            gameManager.WatchedDailyAd();
        }
        else if (placementId == "IOS_InGameAd" && showResult == ShowResult.Finished)
        {
            tower.WatchedAd();
        }
#else
        if (placementId == "Android_DailyAd" && showResult == ShowResult.Finished)
        {
            gameManager.WatchedDailyAd();
        }
        else if (placementId == "Android_InGameAd" && showResult == ShowResult.Finished)
        {
            tower.WatchedAd();
        }
#endif

    }

    public bool IsAdReady()
    {
        return isAdReady;
    }

}