package com.example.tastifymobile.activities

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.tastifymobile.R
import com.example.tastifymobile.api.NetworkModule
import com.example.tastifymobile.models.GuestRegistration
import com.example.tastifymobile.services.AuthService

class RegisterActivity : AppCompatActivity() {

    private lateinit var nameEditText: EditText
    private lateinit var emailEditText: EditText
    private lateinit var phoneEditText: EditText
    private lateinit var loginEditText: EditText
    private lateinit var passwordEditText: EditText
    private lateinit var registerButton: Button

    private lateinit var authService: AuthService

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_register)
    }

    override fun onStart() {
        super.onStart()
        initUI()
        initServices()

        registerButton.setOnClickListener {
            performRegistration()
        }
    }

    override fun onSaveInstanceState(outState: Bundle) {
        super.onSaveInstanceState(outState)
        outState.putString("name", nameEditText.text.toString())
        outState.putString("email", emailEditText.text.toString())
        outState.putString("phone", phoneEditText.text.toString())
        outState.putString("login", loginEditText.text.toString())
        outState.putString("password", passwordEditText.text.toString())
    }

    override fun onRestoreInstanceState(savedInstanceState: Bundle) {
        super.onRestoreInstanceState(savedInstanceState)
        restoreState(savedInstanceState)
    }

    private fun restoreState(savedInstanceState: Bundle) {
        nameEditText.setText(savedInstanceState.getString("name"))
        emailEditText.setText(savedInstanceState.getString("email"))
        phoneEditText.setText(savedInstanceState.getString("phone"))
        loginEditText.setText(savedInstanceState.getString("login"))
        passwordEditText.setText(savedInstanceState.getString("password"))
    }

    private fun initUI() {
        nameEditText = findViewById(R.id.nameEditText)
        emailEditText = findViewById(R.id.emailEditText)
        phoneEditText = findViewById(R.id.phoneEditText)
        loginEditText = findViewById(R.id.loginEditText)
        passwordEditText = findViewById(R.id.passwordEditText)
        registerButton = findViewById(R.id.registerButton)
    }

    private fun initServices() {
        authService = AuthService(NetworkModule.provideApiService(this))
    }

    private fun performRegistration() {
        val name = nameEditText.text.toString()
        val email = emailEditText.text.toString()
        val phone = phoneEditText.text.toString()
        val login = loginEditText.text.toString()
        val password = passwordEditText.text.toString()

        if (name.isEmpty() || email.isEmpty() || phone.isEmpty() || login.isEmpty() || password.isEmpty()) {
            Toast.makeText(this, getResources().getString(R.string.registration_no_info), Toast.LENGTH_SHORT).show()
            return
        }

        val guestRegistration = GuestRegistration(
            name = name,
            email = email,
            phone = phone,
            login = login,
            password = password
        )

        authService.register(guestRegistration,
            onSuccess = {
                Toast.makeText(this@RegisterActivity, getResources().getString(R.string.registration_success), Toast.LENGTH_SHORT).show()
                navigateToLogin()
            },
            onError = { error ->
                Log.e("RegisterActivity", error)
                Toast.makeText(this@RegisterActivity, error, Toast.LENGTH_SHORT).show()
            }
        )
    }

    private fun navigateToLogin() {
        val intent = Intent(this, LoginActivity::class.java)
        startActivity(intent)
        finish()
    }
}
