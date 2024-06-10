package com.example.tastifymobile.services

import android.util.Log
import android.widget.Toast
import com.example.tastifymobile.api.ApiService
import com.example.tastifymobile.models.Booking
import com.example.tastifymobile.models.Table
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class BookingsService(val apiService: ApiService) {

    fun fetchBookings(guestId: String, onResult: (List<Booking>) -> Unit, onError: (String) -> Unit) {
        apiService.getAllBookingsByGuestId(guestId).enqueue(object : Callback<List<Booking>> {
            override fun onResponse(call: Call<List<Booking>>, response: Response<List<Booking>>) {
                if (response.isSuccessful) {
                    val bookings = response.body() ?: emptyList()
                    onResult(bookings)
                } else {
                    onError("Failed to load bookings: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<List<Booking>>, t: Throwable) {
                onError("Error fetching bookings: ${t.message}")
            }
        })
    }

    fun fetchTables(onTablesFetched: (List<Table>) -> Unit, onError: (String) -> Unit) {
        apiService.getTables().enqueue(object : Callback<List<Table>> {
            override fun onResponse(call: Call<List<Table>>, response: Response<List<Table>>) {
                if (response.isSuccessful) {
                    val tables = response.body() ?: emptyList()
                    onTablesFetched(tables)
                } else {
                    onError("Failed to load tables: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<List<Table>>, t: Throwable) {
                onError("Error fetching tables: ${t.message}")
            }
        })
    }

    fun createBooking(newBooking: Booking, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.createBooking(newBooking).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("Failed to create booking: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                onError("Error creating booking: ${t.message}")
            }
        })
    }

    fun deleteBooking(bookingId: String, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.deleteBooking(bookingId).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("Failed to delete booking: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                onError("Error deleting booking: ${t.message}")
            }
        })
    }

    fun updateBooking(bookingId: String, updatedBooking: Booking, onSuccess: () -> Unit, onError: (String) -> Unit) {
        apiService.updateBooking(bookingId, updatedBooking).enqueue(object : Callback<Void> {
            override fun onResponse(call: Call<Void>, response: Response<Void>) {
                if (response.isSuccessful) {
                    onSuccess()
                } else {
                    onError("Failed to update booking: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<Void>, t: Throwable) {
                onError("Error updating booking: ${t.message}")
            }
        })
    }
}
