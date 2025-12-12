using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    void Start()
    {
        // On Android, we need to make sure we have permission to read the ID
        // (Unity usually handles this, but good to be aware)
        Login();
    }

    public void Login()
    {
        Debug.Log("Attempting Android Silent Login...");

#if UNITY_ANDROID && !UNITY_EDITOR
        // This is the specific call for Android devices
        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true, // Creates a guest account if none exists
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };

        PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnLoginSuccess, OnLoginError);
#else
        // Fallback for testing in Unity Editor (since you don't have an Android ID on PC)
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
#endif
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Success!");

        // SECURITY CHECK:
        // Check if this account is linked to Google or is still just a "Guest"
        bool isGuest = true;
        
        if (result.InfoResultPayload != null && 
            result.InfoResultPayload.PlayerProfile != null && 
            result.InfoResultPayload.PlayerProfile.LinkedAccounts != null)
        {
            foreach (var account in result.InfoResultPayload.PlayerProfile.LinkedAccounts)
            {
                if (account.Platform == LoginIdentityProvider.GooglePlay)
                {
                    isGuest = false; // They are secure!
                }
            }
        }

        if (isGuest)
        {
            Debug.LogWarning("User is on an insecure Guest account. We should ask them to link Google soon.");
            // TODO: Show a "Link Account" button in your Settings menu
        }
    }

    private void OnLoginError(PlayFabError error)
    {
        Debug.LogError("Login Failed: " + error.GenerateErrorReport());
    }
}