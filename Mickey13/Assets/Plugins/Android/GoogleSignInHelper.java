package com.Mickey_13;

import android.app.Activity;
import android.content.Intent;
import com.google.android.gms.auth.api.signin.GoogleSignIn;
import com.google.android.gms.auth.api.signin.GoogleSignInAccount;
import com.google.android.gms.auth.api.signin.GoogleSignInClient;
import com.google.android.gms.auth.api.signin.GoogleSignInOptions;
import com.google.android.gms.common.api.ApiException;
import com.google.android.gms.tasks.Task;
import com.unity3d.player.UnityPlayer;

public class GoogleSignInHelper {
    static final int RC_SIGN_IN = 9001;
    private static GoogleSignInClient mGoogleSignInClient;
    
    public static void SignIn(String webClientId) {
        Activity activity = UnityPlayer.currentActivity;
        
        GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
                .requestIdToken(webClientId)
                .requestEmail()
                .build();
        
        mGoogleSignInClient = GoogleSignIn.getClient(activity, gso);
        
        Intent signInIntent = mGoogleSignInClient.getSignInIntent();
        activity.startActivityForResult(signInIntent, RC_SIGN_IN);
    }
    
    public static void HandleSignInResult(Intent data) {
    Task<GoogleSignInAccount> task = GoogleSignIn.getSignedInAccountFromIntent(data);
        try {
            GoogleSignInAccount account = task.getResult(ApiException.class);
            String idToken = account.getIdToken();
            UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnGoogleSignInSuccess", idToken);
        } catch (ApiException e) {
            UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnGoogleSignInFailed", e.getMessage());
        }
    }
}
