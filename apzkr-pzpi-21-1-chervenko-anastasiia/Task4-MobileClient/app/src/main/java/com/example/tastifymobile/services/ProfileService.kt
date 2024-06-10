package com.example.tastifymobile.services

import android.util.Log
import com.example.tastifymobile.api.ApiService
import com.example.tastifymobile.models.Coupon
import com.example.tastifymobile.models.Guest
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class ProfileService(private val apiService: ApiService) {

    fun fetchGuestInfo(guestId: String, onSuccess: (Guest) -> Unit, onError: (String) -> Unit) {
        apiService.getGuestById(guestId).enqueue(object : Callback<Guest> {
            override fun onResponse(call: Call<Guest>, response: Response<Guest>) {
                if (response.isSuccessful) {
                    response.body()?.let(onSuccess) ?: onError("Failed to retrieve guest data")
                } else {
                    onError("Failed to retrieve guest data: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Guest>, t: Throwable) {
                onError("Error fetching guest data: ${t.message}")
            }
        })
    }

    fun makeCoupon(bonus: Int, onSuccess: (Coupon) -> Unit, onError: (String) -> Unit) {
        apiService.makeCoupon(bonus).enqueue(object : Callback<Coupon> {
            override fun onResponse(call: Call<Coupon>, response: Response<Coupon>) {
                if (response.isSuccessful) {
                    response.body()?.let(onSuccess) ?: onError("Failed to retrieve coupon")
                } else {
                    onError("Failed to retrieve coupon: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Coupon>, t: Throwable) {
                onError("Error fetching coupon: ${t.message}")
            }
        })
    }

    fun updateGuestInfo(guestId: String, guest: Guest, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.updateGuest(guestId, guest).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("Failed to update guest: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                onError("Error updating guest: ${t.message}")
            }
        })
    }
}
