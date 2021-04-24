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
    public GameObject loginPanel;

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "AFAC0"; // Please change this value to your own titleId from PlayFab Game Manager
        }
        //      var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //      PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

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



    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("On<color=red>Login</color>Success:Congratulations, you made your first successful API call!");
        // This is what remembers your email and Password.
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        loginPanel.SetActive(false); //we should probably be doing this AFTER setting up the panel and making sure that it works...  but ok...

    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("On<color=red>Register</color>Success: Congratulations, you made your first successful API call!");
        // This is what remembers your email and Password.
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
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
