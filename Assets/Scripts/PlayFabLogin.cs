using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    private string userEmail;
    private string userPassword;
    private string username;

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "AFAC0"; // Please change this value to your own titleId from PlayFab Game Manager
        }
        //      var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //      PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

        var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);



    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("On<color=red>Login</color>Success:Congratulations, you made your first successful API call!");
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("On<color=red>Register</color>Success: Congratulations, you made your first successful API call!");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        //      Debug.LogWarning("Something went wrong with your first API call.  :(");
        //      Debug.LogError("Here's some debug information:");
        //      Debug.LogError(error.GenerateErrorReport());

        // So right now this says that if the initial log in fails, then go right to setting up an account.  
        // For the final version there will have to be a gap between those two events.
        // An option notification. A press this then it happens kind of thing...

        var registerRequest = new RegisterPlayFabUserRequest { Email = userEmail, Password = userPassword, Username = username};
        PlayFabClientAPI.RegisterPlayFabUser (registerRequest, OnRegisterSuccess, OnRegisterFailure);
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
}
