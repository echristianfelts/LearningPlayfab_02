using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using PlayFab.Json;
using UnityEngine.UI;

public class PlayFabController : MonoBehaviour
{
    public static PlayFabController PFC;

    private string userEmail;
    private string userPassword;
    private string username;
    public GameObject loginPanel;
    public GameObject addLoginPanel;
    public GameObject recoverButton;

    private void OnEnable()
    {
        if (PlayFabController.PFC == null)
        {
            PlayFabController.PFC = this;
        }
        else
        {
            if (PlayFabController.PFC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "AFAC0"; // Please change this value to your own titleId from PlayFab Game Manager
        }
        //      var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //      PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);


        //      PlayerPrefs.DeleteAll();


        if (PlayerPrefs.HasKey("EMAIL"))
        {

            // "If our EMAIL key exists, then most likely, our PASSWORD key will exist as well 
            //  because we are setting those player preferences at the same time...  So if the EMAIL exists, 
            //  then we want to take the EMAIL and the PASSWORD and put them int thier variables..."
            //          Not sure I follow.  I will follow up.
            //
            //Upon Review...  This should check for the PW also. 

            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else
        {
#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(),CreateAccount=true };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif

#if UNITY_IOS
            var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = ReturnMobileID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif


        }

    }

    #region Login

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("On<color=red>Login</color>Success:Congratulations, you made your first successful API call!");
        // This is what remembers your email and Password.
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        loginPanel.SetActive(false); //we should probably be doing this AFTER setting up the panel and making sure that it works...  but ok...
        recoverButton.SetActive(false);
        GetStats();         //Gets the stats from the server..?  I don't know.  I am starting to get lost.
    }

    private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("On<color=red>Login</color>Success:Congratulations, you made your first successful API call!");
        GetStats();         //Gets the stats from the server..?  I don't know.  I am starting to get lost.
        loginPanel.SetActive(false);

    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("On<color=red>Register</color>Success: Congratulations, you made your first successful API call!");
        // This is what remembers your email and Password.
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        GetStats();         //Gets the stats from the server..?  The player should not have any values at this point, though...
        loginPanel.SetActive(false); //we should probably be doing this AFTER setting up the panel and making sure that it works...  but ok...

    }


    private void OnLoginFailure(PlayFabError error)
    {
        //      Debug.LogWarning("Something went wrong with your first API call.  :(");
        //      Debug.LogError("Here's some debug information:");
        //      Debug.LogError(error.GenerateErrorReport());

        // So right now this says that if the initial log in fails, then go right to setting up an account.  
        // For the final version there will have to be a gap between those two events.
        // An option notification. A press this then it happens kind of thing...
        // This should go to a screen that does this.  
        // There should be a homescreen "register" button that goes to the same place.
        // A "Login Falure/First Time Registration" Screen.


        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = username };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnLoginMobileFailure(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void GetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }

    public void GetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    public void GetUsername(string usernameIn)
    {
        username = usernameIn;
    }

    public void OnClickLogin()
    {
        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);

    }

    public static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    public void OpenAddLogin()
    {
        addLoginPanel.SetActive(true);
    }

    public void OnClickAddLogin()
    {
        var addLoginRequest = new AddUsernamePasswordRequest { Email = userEmail, Password = userPassword, Username = username };
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, OnAddLoginSuccess, OnRegisterFailure);
    }

    private void OnAddLoginSuccess(AddUsernamePasswordResult result)
    {
        Debug.Log("On<color=red>Register</color>Success: Congratulations, you made your first successful API call!");
        // This is what remembers your email and Password.
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        GetStats();         //Gets the stats from the server..?  Sure.  Why not?  The tut was not clear as to the gain.  If it messes stuff up, this is the first thing to get commented out.
        addLoginPanel.SetActive(false);

    }

    #endregion

    public int playerLevel;
    public int gameLevel;

    public int playerHealth;
    public int playerDamage;

    public int playerHighScore;

    public int numberofTimesPlayed;
    public int numberofWins;

    #region PlayerStats

    public void SetStats() // Basic test here copy pasted from https://docs.microsoft.com/en-us/gaming/playfab/features/data/playerdata/using-player-statistics
                           // This function gets called any time we want to push new information to the cloud.
                           // to call use PlayFabController.PFC.SetStats();
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
            Statistics = new List<StatisticUpdate> {
            //  new StatisticUpdate { StatisticName = "strength", Value = 18 },     //The value and the change.  These are the original values.
            new StatisticUpdate { StatisticName = "STATPlayerLevel", Value = playerLevel },         //Setting the name of the statistic and assigning it a local variable.
            new StatisticUpdate { StatisticName = "STATGameLevel", Value = gameLevel },
            new StatisticUpdate { StatisticName = "STATPlayerHealth", Value = playerHealth },
            new StatisticUpdate { StatisticName = "STATPlayerDamage", Value = playerDamage },
            new StatisticUpdate { StatisticName = "STATPlayerHighScore", Value = playerHighScore },
            new StatisticUpdate { StatisticName = "STATPlayerTotalPlays", Value = numberofTimesPlayed },
            new StatisticUpdate { StatisticName = "STATPlayerWins", Value = numberofWins },
            }
        },                                                              // The above is setting the Request and the values that we want to change.    
        result => { Debug.Log("User statistics updated"); },            // Callback for succsessful post
        error => { Debug.LogError(error.GenerateErrorReport()); });     // Callback for failed post
    }

    void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName)
            {
                case "STATPlayerLevel":
                    playerLevel = eachStat.Value;
                    break;

                case "STATGameLevel":
                    playerLevel = eachStat.Value;
                    break;

                case "STATPlayerHealth":
                    playerLevel = eachStat.Value;
                    break;

                case "STATPlayerDamage":
                    playerLevel = eachStat.Value;
                    break;

                case "STATPlayerHighScore":
                    playerLevel = eachStat.Value;
                    break;

                case "STATPlayerTotalPlays":
                    playerLevel = eachStat.Value;
                    break;

                case "STATPlayerWins":
                    playerLevel = eachStat.Value;
                    break;

            }
        }
    }


    // Build the request object and access the API
    public void StartCloudUpdatePlayerStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new {
                CLDPlayerLevel = playerLevel,
                CLDGameLevel = gameLevel,
                CLDPlayerHealth = playerHealth,
                CLDPlayerDamage = playerDamage,
                CLDPlayerHighScore = playerHighScore,
                CLDNumberofTimesPlayed = numberofTimesPlayed,
                CLDNumberofWins = numberofWins
            }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudUpdateStats, OnErrorShared);
    }
    // OnCloudHelloWorld defined in the next code block

    private static void OnCloudUpdateStats(ExecuteCloudScriptResult result)
    {
        // CloudScript returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        Debug.Log(result.FunctionResult.ToString() as string);  // original line follows :  Debug.Log(JsonWrapper.SerializeObject(result.FunctionResult));
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in CloudScript
        Debug.Log((string)messageValue);
    }

    private static void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }


    #endregion PlayerStats

}
