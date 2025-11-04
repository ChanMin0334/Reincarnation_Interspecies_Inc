package com.Mickey_13;

import android.app.Activity;
import android.content.Intent;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

/**
 * UnityPlayerActivity 확장: Google Sign-In 결과 전달
 */
public class GoogleSignInActivity extends UnityPlayerActivity {
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (requestCode == GoogleSignInHelper.RC_SIGN_IN) {
            if (resultCode == Activity.RESULT_OK && data != null) {
                GoogleSignInHelper.HandleSignInResult(data);
            } else {
                UnityPlayer.UnitySendMessage(
                        "GoogleSignInManager",
                        "OnGoogleSignInFailed",
                        "Google sign-in cancelled"
                );
            }
        }
    }
}
