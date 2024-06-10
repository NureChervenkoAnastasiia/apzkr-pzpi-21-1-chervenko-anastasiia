package com.example.tastifymobile.models

import java.util.Date

data class Booking(
    val id: String? = null,
    val tableId: String? = null,
    val guestId: String? = null,
    val bookingDateTime: Date,
    val personsCount: Int,
    val comment: String? = null
)