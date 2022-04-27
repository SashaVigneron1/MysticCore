using System;

using UnityEngine;
using UnityEngine.UI;


#if UNITY_ANDROID

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

#endif

#if UNITY_ANDROID

[Flags]
public enum PlayServiceError : byte
{
    None = 0,
    Timeout = 1,
    NotAuthenticated = 2,
    SaveGameNotEnabled = 4,
    CloudSaveNameNotSet = 8
}

public class PlayServices : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [Header("Saving")]
    [SerializeField] bool enableSaveGame;
    [SerializeField] string cloudSaveName = "MysticCore";
    [SerializeField] DataSource dataSource = DataSource.ReadNetworkOnly;
    [SerializeField] ConflictResolutionStrategy conflictStrategy = ConflictResolutionStrategy.UseLongestPlaytime;


    [Header("UI")]
    [SerializeField] GameObject loadingOverlay;
    [SerializeField] GameObject loadingPopup;


    void Start()
    {
        //Create Builder
        PlayGamesClientConfiguration.Builder builder = new PlayGamesClientConfiguration.Builder();
        if (enableSaveGame)
            builder.EnableSavedGames();

        PlayGamesPlatform.InitializeInstance(builder.Build());
        PlayGamesPlatform.DebugLogEnabled = false;
        PlayGamesPlatform.Activate();

        ConnectToServer();
    }

    public void ConnectToServer()
    {
        loadingPopup.SetActive(false);

        Social.localUser.Authenticate((bool success, string err) =>
        {
            if (success)
            {
                Debug.Log("Login success");
                loadingOverlay.SetActive(false);
                gameManager.OnLoginSucces();
            }
            else
            {
                Debug.LogError("Login failed");
                Debug.LogError("Error: " + err);
                loadingPopup.SetActive(true);
            }
        });
    }

    //Save Game
    public void OpenCloudSave(Action<SavedGameRequestStatus, ISavedGameMetadata> callback, Action<PlayServiceError> errorCallback = null)
    {
        PlayServiceError currError = PlayServiceError.None;
        if (!Social.localUser.authenticated)
            currError |= PlayServiceError.NotAuthenticated;
        if (PlayGamesClientConfiguration.DefaultConfiguration.EnableSavedGames)
            currError |= PlayServiceError.SaveGameNotEnabled;
        if (string.IsNullOrWhiteSpace(cloudSaveName))
            currError |= PlayServiceError.CloudSaveNameNotSet;

        if (currError != PlayServiceError.None)
            errorCallback?.Invoke(currError);

        var platform = (PlayGamesPlatform)Social.Active;
        platform.SavedGame.OpenWithAutomaticConflictResolution(cloudSaveName, dataSource, conflictStrategy, callback);
    }
}
#endif

