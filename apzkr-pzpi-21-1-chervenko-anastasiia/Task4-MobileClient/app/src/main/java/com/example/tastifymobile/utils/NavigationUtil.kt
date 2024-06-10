package com.example.tastifymobile.utils

import android.content.Context
import android.content.Intent
import com.example.tastifymobile.R
import com.example.tastifymobile.activities.BookingsActivity
import com.example.tastifymobile.activities.MenuActivity
import com.example.tastifymobile.activities.ProfileActivity
import com.google.android.material.appbar.MaterialToolbar
import com.google.android.material.bottomnavigation.BottomNavigationView

object NavigationUtil {

    fun setupBottomMenu(bottomNavMenu: BottomNavigationView, context: Context) {
        bottomNavMenu.setOnItemSelectedListener { item ->
            val activityClass = when (item.itemId) {
                R.id.profile -> ProfileActivity::class.java
                R.id.bookings -> BookingsActivity::class.java
                R.id.menu -> MenuActivity::class.java
                else -> null
            }

            if (activityClass != null && context::class.java != activityClass) {
                context.startActivity(Intent(context, activityClass))
                item.isChecked = true
                true
            } else {
                false
            }
        }
    }
}
