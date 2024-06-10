package com.example.tastifymobile.models

data class Menu(
    val id: String,
    val restaurantId: String,
    val name: String,
    val size: Int,
    val price: Int,
    val info: String?,
    val type: String
)
