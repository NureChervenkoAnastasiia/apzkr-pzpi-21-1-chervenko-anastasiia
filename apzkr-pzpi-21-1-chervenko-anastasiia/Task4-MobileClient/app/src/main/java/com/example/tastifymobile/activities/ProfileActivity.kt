package com.example.tastifymobile.activities

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.ImageButton
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.tastifymobile.R
import com.example.tastifymobile.api.ApiService
import com.example.tastifymobile.api.NetworkModule
import com.example.tastifymobile.api.TokenManager
import com.example.tastifymobile.models.Coupon
import com.example.tastifymobile.models.Guest
import com.example.tastifymobile.services.ProfileService
import com.example.tastifymobile.utils.NavigationUtil
import com.google.android.material.bottomnavigation.BottomNavigationView

class ProfileActivity : AppCompatActivity() {

    private lateinit var tokenManager: TokenManager
    private lateinit var profileService: ProfileService
    private lateinit var nameEditText: EditText
    private lateinit var emailEditText: EditText
    private lateinit var phoneEditText: EditText
    private lateinit var loginEditText: EditText
    private lateinit var bonusTextView: TextView
    private lateinit var getCouponButton: Button
    private lateinit var saveButton: Button
    private var currentBonus: Int = 0
    private var guestId: String? = null
    private var currentGuest: Guest? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_profile)
    }

    override fun onStart() {
        super.onStart()
        setupUI()
        setupBottomNavigation()
        initializeServices()

        guestId = tokenManager.getGuestIdFromToken()
        guestId?.let { fetchGuestInfo(it) } ?: Log.e("ProfileActivity", "No guest ID found in token")
    }

    override fun onSaveInstanceState(outState: Bundle) {
        super.onSaveInstanceState(outState)
        outState.putString("name", nameEditText.text.toString())
        outState.putString("email", emailEditText.text.toString())
        outState.putString("phone", phoneEditText.text.toString())
        outState.putString("login", loginEditText.text.toString())
        outState.putInt("currentBonus", currentBonus)
    }

    override fun onRestoreInstanceState(savedInstanceState: Bundle) {
        super.onRestoreInstanceState(savedInstanceState)
        nameEditText.setText(savedInstanceState.getString("name"))
        emailEditText.setText(savedInstanceState.getString("email"))
        phoneEditText.setText(savedInstanceState.getString("phone"))
        loginEditText.setText(savedInstanceState.getString("login"))
        currentBonus = savedInstanceState.getInt("currentBonus")
        bonusTextView.text = currentBonus.toString()
    }

    private fun setupUI() {
        nameEditText = findViewById(R.id.nameEditText)
        emailEditText = findViewById(R.id.emailEditText)
        phoneEditText = findViewById(R.id.phoneEditText)
        loginEditText = findViewById(R.id.loginEditText)
        bonusTextView = findViewById(R.id.bonusTextView)
        getCouponButton = findViewById(R.id.getCouponButton)
        saveButton = findViewById(R.id.saveButton)

        getCouponButton.setOnClickListener { handleGetCouponButtonClick() }
        saveButton.setOnClickListener { handleSaveButtonClick() }
        findViewById<ImageButton>(R.id.exit_btn).setOnClickListener { handleExitButtonClick() }
    }

    private fun setupBottomNavigation() {
        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtil.setupBottomMenu(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.profile).isChecked = true
    }

    private fun initializeServices() {
        tokenManager = TokenManager(this)
        profileService = ProfileService(NetworkModule.provideApiService(this))
    }

    private fun fetchGuestInfo(guestId: String) {
        profileService.fetchGuestInfo(guestId,
            onSuccess = { guest ->
                currentGuest = guest
                nameEditText.setText(guest.name)
                emailEditText.setText(guest.email)
                phoneEditText.setText(guest.phone)
                loginEditText.setText(guest.login)
                currentBonus = guest.bonus ?: 0
                bonusTextView.text = currentBonus.toString()
            },
            onError = { error -> Log.e("ProfileActivity", error) }
        )
    }

    private fun handleGetCouponButtonClick() {
        if (currentBonus > 0) {
            makeCoupon(currentBonus)
        } else {
            Toast.makeText(this, getResources().getString(R.string.no_bonus), Toast.LENGTH_SHORT).show()
            Log.e("ProfileActivity", "No bonus available")
        }
    }

    private fun handleSaveButtonClick() {
        currentGuest?.let {
            val updatedGuest = it.copy(
                name = nameEditText.text.toString(),
                email = emailEditText.text.toString(),
                phone = phoneEditText.text.toString(),
                bonus = bonusTextView.text.toString().toInt(),
                login = loginEditText.text.toString()
            )
            updateGuestInfo(updatedGuest)
        }
    }

    private fun handleExitButtonClick() {
        startActivity(Intent(this, LoginActivity::class.java))
        finish()
    }

    private fun makeCoupon(bonus: Int) {
        profileService.makeCoupon(bonus,
            onSuccess = { coupon ->
                showCouponDialog(coupon)
                updateBonus(coupon.bonus)
            },
            onError = { error -> Log.e("ProfileActivity", error) }
        )
    }

    private fun showCouponDialog(coupon: Coupon) {
        AlertDialog.Builder(this)
            .setTitle(getResources().getString(R.string.received_coupon))
            .setMessage(getResources().getString(R.string.discount) + " ${coupon.discount ?: 0.0}" + getResources().getString(R.string.grn))
            .setPositiveButton("OK") { dialog, _ -> dialog.dismiss() }
            .show()
    }

    private fun updateBonus(newBonus: Int) {
        currentBonus = newBonus
        bonusTextView.setText(currentBonus.toString())
        handleSaveButtonClick()
    }

    private fun updateGuestInfo(guest: Guest) {
        profileService.updateGuestInfo(guestId!!, guest,
            onSuccess = { Toast.makeText(this, "Guest updated successfully!", Toast.LENGTH_SHORT).show() },
            onError = { error -> Log.e("ProfileActivity", error) }
        )
    }
}
