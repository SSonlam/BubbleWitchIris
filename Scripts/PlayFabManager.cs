using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager thePlayFabManager = null;

    private string playerLeaderboardName;
    string myGUID = System.Guid.NewGuid().ToString();
    private static Random random = new Random();

    public GetLeaderboardResult publicLeaderboard;

    void Awake () 
    {
        DontDestroyOnLoad (transform.gameObject);
    }
    void Start()
    {
        if (!thePlayFabManager)
        {
            thePlayFabManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
        PlayFabSettings.TitleId = "82156";
        PlayFabSettings.DeveloperSecretKey = "ASFSQ4MXISF6AWPIOM3PAFCTT1OP588WOFQOYP3WMPAYE3EJ53";

        TempLogin();
    }
    public void Login(string loginName)
    {
        playerLeaderboardName = loginName + "/" + Random.Range(1000,9999);
        var request = new LoginWithCustomIDRequest 
        {
            CustomId = myGUID,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID( request, OnSuccess, OnError);
    }

    public void TempLogin()
    {
        //playerLeaderboardName = loginName;
        var request = new LoginWithCustomIDRequest 
        {
            CustomId = myGUID,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID( request, OnSuccess, OnError);
    }
    
    void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account create!");
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {DisplayName = playerLeaderboardName}, OnDisplayName, OnSendError);
    }

    void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(result.DisplayName + "This is your new display name");
    }

    void OnSendError(PlayFabError error)
    {
        Debug.Log("Error Sending Data");
        Debug.Log(error.GenerateErrorReport());
    }
    
    void OnError(PlayFabError error)
    {
        Debug.Log("Error while logging in/creating account");
        Debug.Log(error.GenerateErrorReport());
    }
    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "GameScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful Leaderboard sent");
    }
    public void GetLeaderboard(){
        var request = new GetLeaderboardRequest{
            StatisticName = "GameScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result){

        publicLeaderboard = result;

    }
    public GetLeaderboardResult returnLeaderboard()
    {
        return publicLeaderboard;
    }
}
