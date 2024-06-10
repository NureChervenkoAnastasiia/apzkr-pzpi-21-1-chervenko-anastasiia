package com.example.tastifymobile.api

import com.example.tastifymobile.models.Booking
import com.example.tastifymobile.models.Coupon
import com.example.tastifymobile.models.Guest
import com.example.tastifymobile.models.GuestRegistration
import com.example.tastifymobile.models.LoginRequest
import com.example.tastifymobile.models.LoginResponse
import com.example.tastifymobile.models.Menu
import com.example.tastifymobile.models.Table
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.DELETE
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path
import retrofit2.http.Query

interface ApiService {
    @POST("api/Guest/login")
    fun login(@Body loginRequest: LoginRequest): Call<LoginResponse>

    @GET("api/Guest/{guestId}")
    fun getGuestById(@Path("guestId") guestId: String): Call<Guest>

    @PUT("api/Guest/{guestId}")
    fun updateGuest(@Path("guestId") guestId: String, @Body updatedGuest: Guest): Call<Void>

    @POST("api/Guest/register")
    fun registerGuest(@Body guestRegistration: GuestRegistration): Call<LoginResponse>

    @GET("api/Booking/guest-bookings/{guestId}")
    fun getAllBookingsByGuestId(@Path("guestId") guestId: String): Call<List<Booking>>

    @POST("api/Booking/")
    fun createBooking(@Body booking: Booking): Call<ResponseBody>

    @PUT("api/Booking/{bookindId}")
    fun updateBooking(@Path ("bookindId") bookingId: String, @Body updatedBooking: Booking): Call<Void>

    @DELETE("api/Booking/{bookindId}")
    fun deleteBooking(@Path ("bookindId") bookingId: String): Call<Void>

    @GET("api/Table/")
    fun getTables(): Call<List<Table>>

    @GET("api/Menu")
    fun getAllDishes(): Call<List<Menu>>

    @POST("api/Guest/make-coupon")
    fun makeCoupon(@Query("bonus") bonus: Int): Call<Coupon>

    @GET("api/Table/{tableId}")
    fun getTableById(@Path ("tableId") tableId: String): Call<Table>
}
