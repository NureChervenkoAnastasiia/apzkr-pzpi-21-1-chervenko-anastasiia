// TokenManager.kt
package com.example.tastifymobile.api

import android.content.Context
import android.util.Log
import com.auth0.android.jwt.JWT

class TokenManager(private val context: Context) {

    fun saveJwtToken(token: String) {
        val sharedPreferences = context.getSharedPreferences("prefs", Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()
        editor.putString("jwt_token", token)
        editor.apply()
    }

    fun getJwtToken(): String? {
        val sharedPreferences = context.getSharedPreferences("prefs", Context.MODE_PRIVATE)
        return sharedPreferences.getString("jwt_token", null)
    }

    fun getGuestIdFromToken(): String? {
        val token = getJwtToken()
        Log.d("TokenManager", "Token: $token")
        if (token == null) {
            Log.d("TokenManager", "Token not found")
            return null
        }
        val jwt = JWT(token)
        val claim = jwt.getClaim("nameid")
        Log.d("TokenManager", "Claim: ${claim.asString()}")
        return claim.asString()
    }
}
