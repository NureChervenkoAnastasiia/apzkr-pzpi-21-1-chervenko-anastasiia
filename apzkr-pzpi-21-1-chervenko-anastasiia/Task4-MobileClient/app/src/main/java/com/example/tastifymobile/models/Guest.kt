package com.example.tastifymobile.models

import android.os.Parcelable

data class Guest(
    val id: String? = null,
    val name: String? = null,
    val phone: String? = null,
    val bonus: Int = 0,
    val email: String? = null,
    val password: String? = null,
    val login: String? = null
)
