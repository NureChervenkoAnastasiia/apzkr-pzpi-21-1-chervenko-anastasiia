package com.example.tastifymobile.services

import android.util.Log
import com.example.tastifymobile.api.ApiService
import com.example.tastifymobile.models.GuestRegistration
import com.example.tastifymobile.models.LoginRequest
import com.example.tastifymobile.models.LoginResponse
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class AuthService(private val apiService: ApiService) {

    fun login(loginRequest: LoginRequest, onSuccess: (String) -> Unit, onError: (String) -> Unit) {
        apiService.login(loginRequest).enqueue(object : Callback<LoginResponse> {
            override fun onResponse(call: Call<LoginResponse>, response: Response<LoginResponse>) {
                if (response.isSuccessful) {
                    val token = response.body()?.token
                    if (token != null) {
                        onSuccess(token)
                    } else {
                        onError("Failed to retrieve token")
                    }
                } else {
                    onError("Login failed: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                onError("Login error: ${t.message}")
            }
        })
    }

    fun register(guestRegistration: GuestRegistration, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.registerGuest(guestRegistration).enqueue(object : Callback<LoginResponse> {
            override fun onResponse(call: Call<LoginResponse>, response: Response<LoginResponse>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("Registration failed: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                onError("Registration error: ${t.message}")
            }
        })
    }
}
