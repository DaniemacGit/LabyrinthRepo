using UnityEngine;
using Firebase;
using Firebase.Auth;
using System;
using System.Threading.Tasks;


public class FirebaseInitializer : MonoBehaviour
{
    public static FirebaseInitializer Instance;

    public FirebaseAuth Auth;
    public FirebaseUser User;
    //  returns user ID if user is logged in
    public string Uid => User?.UserId;

    private void Awake()
    {
        // Singleton + DontDestroyOnLoad so Firebase persists across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

     // Initialize Firebase when the game starts
    private async void Start()
    {
        try
        {
            // Check and fix Firebase dependencies
            DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (status != DependencyStatus.Available)
            {
                Debug.LogError("Firebase dependency error: " + status);
                return;
            }

            // Set up Firebase Auth
            Auth = FirebaseAuth.DefaultInstance;

            // Sign in the user anonymously
            await SignInAnonymously();
        }
        catch (Exception e)
        {
            Debug.LogError("Firebase initialization error: " + e);
        }
    }

    // Handle anonymous sign-in using async and await
    private async Task SignInAnonymously()
    {
        try
        {
            // Sign in anonymously returns an AuthResult (UserCredential)
            var authResult = await Auth.SignInAnonymouslyAsync();

            // Get the FirebaseUser from the result
            User = authResult.User;

            if (User != null)
            {
                Debug.Log("Anonymous sign-in successful. UID = " + User.UserId);
            }
            else
            {
                Debug.LogWarning("Anonymous sign-in completed but User is null.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Anonymous sign-in failed: " + e);
        }
    }
}