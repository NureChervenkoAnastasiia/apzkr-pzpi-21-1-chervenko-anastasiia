package com.example.tastifymobile.api

import android.content.Context
import com.example.greenguardmobile.util.DateTypeUtil
import okhttp3.OkHttpClient
import okhttp3.Request
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import com.google.gson.GsonBuilder
import java.util.Date


object NetworkModule {

    fun provideApiService(context: Context): ApiService {
        val tokenManager = TokenManager(context)

        val client = OkHttpClient.Builder().addInterceptor { chain ->
            val request: Request = chain.request().newBuilder()
                .addHeader("Authorization", "Bearer ${tokenManager.getJwtToken()}")
                .build()
            chain.proceed(request)
        }.build()

        val gson = GsonBuilder()
            .registerTypeAdapter(Date::class.java, DateTypeUtil())
            .setLenient()
            .create()

        val retrofit = Retrofit.Builder()
            .baseUrl("http://10.0.2.2:5170/")
            .client(client)
            .addConverterFactory(GsonConverterFactory.create(gson))
            .build()

        return retrofit.create(ApiService::class.java)
    }
}
