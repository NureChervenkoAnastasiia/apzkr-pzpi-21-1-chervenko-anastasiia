package com.example.tastifymobile.activities

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import com.example.tastifymobile.R
import com.example.tastifymobile.api.NetworkModule
import com.example.tastifymobile.api.TokenManager
import com.example.tastifymobile.models.LoginRequest
import com.example.tastifymobile.services.AuthService

class LoginActivity : AppCompatActivity() {

    private lateinit var loginEditText: EditText
    private lateinit var passwordEditText: EditText
    private lateinit var loginButton: Button
    private lateinit var toRegisterTextView: TextView

    private lateinit var authService: AuthService
    private lateinit var tokenManager: TokenManager

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        savedInstanceState?.let {
            restoreSavedInstanceState(it)
        }
    }

    override fun onStart() {
        super.onStart()
        initUI()
        initServices()

        loginButton.setOnClickListener {
            performLogin()
        }

        toRegisterTextView.setOnClickListener {
            navigateToRegister()
        }
    }

    override fun onSaveInstanceState(outState: Bundle) {
        super.onSaveInstanceState(outState)
        saveInstanceState(outState)

        val preferences = getPreferences(MODE_PRIVATE).edit()
        preferences.putString("login", loginEditText.text.toString())
        preferences.putString("password", passwordEditText.text.toString())
        preferences.apply()
    }

    override fun onRestoreInstanceState(outState: Bundle) {
        super.onRestoreInstanceState(outState)
        saveInstanceState(outState)

        loginEditText.setText(getPreferences(MODE_PRIVATE).getString("login", ""))
        passwordEditText.setText(getPreferences(MODE_PRIVATE).getString("password", ""))
    }

    private fun saveInstanceState(outState: Bundle) {
        outState.putString("login", loginEditText.text.toString())
        outState.putString("password", passwordEditText.text.toString())
    }

    private fun restoreSavedInstanceState(savedInstanceState: Bundle) {
        loginEditText.setText(savedInstanceState.getString("login"))
        passwordEditText.setText(savedInstanceState.getString("password"))
    }

    private fun initUI() {
        loginEditText = findViewById(R.id.loginEditText)
        passwordEditText = findViewById(R.id.passwordEditText)
        loginButton = findViewById(R.id.loginButton)
        toRegisterTextView = findViewById(R.id.to_register)
    }

    private fun initServices() {
        tokenManager = TokenManager(this)
        authService = AuthService(NetworkModule.provideApiService(this))
    }

    private fun performLogin() {
        val login = loginEditText.text.toString()
        val password = passwordEditText.text.toString()

        if (login.isEmpty() || password.isEmpty()) {
            Toast.makeText(this, getResources().getString(R.string.login_no_info), Toast.LENGTH_SHORT).show()
            return
        }

        val loginRequest = LoginRequest(login, password)
        authService.login(loginRequest,
            onSuccess = { token ->
                tokenManager.saveJwtToken(token)
                navigateToProfile()
            },
            onError = { error ->
                Log.e("LoginActivity", error)
                Toast.makeText(this, error, Toast.LENGTH_SHORT).show()
            }
        )
    }

    private fun navigateToProfile() {
        val intent = Intent(this, ProfileActivity::class.java)
        startActivity(intent)
        finish()
    }

    private fun navigateToRegister() {
        val intent = Intent(this, RegisterActivity::class.java)
        startActivity(intent)
    }
}
