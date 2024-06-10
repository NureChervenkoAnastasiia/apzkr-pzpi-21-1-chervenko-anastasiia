package com.example.tastifymobile.activities

import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.tastifymobile.R
import com.example.tastifymobile.adapters.MenuAdapter
import com.example.tastifymobile.api.ApiService
import com.example.tastifymobile.api.NetworkModule
import com.example.tastifymobile.models.Menu
import com.example.tastifymobile.utils.NavigationUtil
import com.google.android.material.bottomnavigation.BottomNavigationView
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class MenuActivity : AppCompatActivity() {

    private lateinit var apiService: ApiService
    private lateinit var menuRecyclerView: RecyclerView
    private lateinit var menuAdapter: MenuAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_menu)
    }

    override fun onStart() {
        super.onStart()
        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtil.setupBottomMenu(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.menu).isChecked = true

        menuRecyclerView = findViewById(R.id.menuRecyclerView)
        menuRecyclerView.layoutManager = LinearLayoutManager(this)

        apiService = NetworkModule.provideApiService(this)
        fetchAllDishes()
    }

    private fun fetchAllDishes() {
        apiService.getAllDishes().enqueue(object : Callback<List<Menu>> {
            override fun onResponse(call: Call<List<Menu>>, response: Response<List<Menu>>) {
                if (response.isSuccessful) {
                    val menuList = response.body()
                    if (menuList != null) {
                        menuAdapter = MenuAdapter(menuList)
                        menuRecyclerView.adapter = menuAdapter
                    }
                } else {
                    Log.e("MenuActivity", "Failed to retrieve menu items: ${response.message()}")
                }
            }

            override fun onFailure(call: Call<List<Menu>>, t: Throwable) {
                Log.e("MenuActivity", "Error fetching menu items: ${t.message}")
            }
        })
    }
}
