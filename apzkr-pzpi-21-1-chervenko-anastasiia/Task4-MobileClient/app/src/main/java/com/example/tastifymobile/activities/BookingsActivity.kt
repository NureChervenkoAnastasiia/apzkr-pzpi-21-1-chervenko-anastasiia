package com.example.tastifymobile.activities

import android.app.DatePickerDialog
import android.app.TimePickerDialog
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.widget.ArrayAdapter
import android.widget.Button
import android.widget.EditText
import android.widget.Spinner
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.tastifymobile.R
import com.example.tastifymobile.adapters.BookingsAdapter
import com.example.tastifymobile.api.NetworkModule
import com.example.tastifymobile.api.TokenManager
import com.example.tastifymobile.models.Booking
import com.example.tastifymobile.models.Table
import com.example.tastifymobile.services.BookingsService
import com.example.tastifymobile.utils.NavigationUtil
import com.google.android.material.bottomnavigation.BottomNavigationView
import java.text.SimpleDateFormat
import java.util.*

class BookingsActivity : AppCompatActivity() {

    private lateinit var bookingsService: BookingsService
    private lateinit var bookingsAdapter: BookingsAdapter
    private lateinit var recyclerView: RecyclerView
    private lateinit var tokenManager: TokenManager
    private var guestId: String? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_bookings)

        val apiService = NetworkModule.provideApiService(this)
        bookingsService = BookingsService(apiService)

        val bottomNavMenu = findViewById<BottomNavigationView>(R.id.bottom_navigation)
        NavigationUtil.setupBottomMenu(bottomNavMenu, this)
        bottomNavMenu.menu.findItem(R.id.profile).isChecked = true

        recyclerView = findViewById(R.id.recyclerViewBookings)
        recyclerView.layoutManager = LinearLayoutManager(this)

        tokenManager = TokenManager(this)
        guestId = tokenManager.getGuestIdFromToken()

        fetchBookings()

        val addBookingButton: Button = findViewById(R.id.addBookingButton)
        addBookingButton.setOnClickListener {
            showAddBookingDialog()
        }
    }

    private fun fetchBookings() {
        bookingsService.fetchBookings(guestId!!, { bookings ->
            bookingsAdapter = BookingsAdapter(this, bookings, bookingsService, { deletedBooking ->
                Toast.makeText(this, getResources().getString(R.string.booking_deleted), Toast.LENGTH_SHORT).show()
                fetchBookings() // Refresh bookings after deletion
            }, { updatedBooking ->
                Toast.makeText(this, getResources().getString(R.string.booking_updated), Toast.LENGTH_SHORT).show()
                fetchBookings() // Refresh bookings after update
            })
            recyclerView.adapter = bookingsAdapter
        }, { errorMessage ->
            Log.e("BookingsActivity", errorMessage)
            Toast.makeText(this, errorMessage, Toast.LENGTH_SHORT).show()
        })
    }

    private fun showAddBookingDialog() {
        bookingsService.fetchTables({ tables ->
            val builder = AlertDialog.Builder(this)
            val inflater = LayoutInflater.from(this)
            val dialogView = inflater.inflate(R.layout.dialog_add_booking, null)
            builder.setView(dialogView)

            val dialog = builder.create()
            dialog.show()

            val spinnerTableId = dialogView.findViewById<Spinner>(R.id.spinnerTableId)
            val editTextPersonsCount = dialogView.findViewById<EditText>(R.id.editTextPersonsCount)
            val editTextComment = dialogView.findViewById<EditText>(R.id.editTextComment)
            val btnAddBooking = dialogView.findViewById<Button>(R.id.btnAddBooking)
            val editTextBookingDate = dialogView.findViewById<EditText>(R.id.editTextBookingDate)
            val editTextBookingTime = dialogView.findViewById<EditText>(R.id.editTextBookingTime)

            val dateFormat = SimpleDateFormat("yyyy-MM-dd", Locale.getDefault())
            val timeFormat = SimpleDateFormat("HH:mm", Locale.getDefault())

            // Set up the spinner with table numbers
            val tableNumbers = tables.map { it.number.toString() }
            val adapter = ArrayAdapter(this, android.R.layout.simple_spinner_item, tableNumbers)
            adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
            spinnerTableId.adapter = adapter

            editTextBookingDate.setOnClickListener {
                val calendar = Calendar.getInstance()
                val dateSetListener = DatePickerDialog.OnDateSetListener { _, year, month, dayOfMonth ->
                    val selectedDate = Calendar.getInstance()
                    selectedDate.set(year, month, dayOfMonth)
                    editTextBookingDate.setText(dateFormat.format(selectedDate.time))
                }

                DatePickerDialog(
                    this,
                    dateSetListener,
                    calendar.get(Calendar.YEAR),
                    calendar.get(Calendar.MONTH),
                    calendar.get(Calendar.DAY_OF_MONTH)
                ).show()
            }

            editTextBookingTime.setOnClickListener {
                val calendar = Calendar.getInstance()
                val timeSetListener = TimePickerDialog.OnTimeSetListener { _, hourOfDay, minute ->
                    val selectedTime = Calendar.getInstance()
                    selectedTime.set(Calendar.HOUR_OF_DAY, hourOfDay)
                    selectedTime.set(Calendar.MINUTE, minute)
                    editTextBookingTime.setText(timeFormat.format(selectedTime.time))
                }

                TimePickerDialog(
                    this,
                    timeSetListener,
                    calendar.get(Calendar.HOUR_OF_DAY),
                    calendar.get(Calendar.MINUTE),
                    true
                ).show()
            }

            btnAddBooking.setOnClickListener {
                val selectedTableNumber = spinnerTableId.selectedItem.toString().toIntOrNull()
                val personsCount = editTextPersonsCount.text.toString().toIntOrNull()
                val comment = editTextComment.text.toString()
                val bookingDateStr = editTextBookingDate.text.toString()
                val bookingTimeStr = editTextBookingTime.text.toString()

                if (selectedTableNumber != null && personsCount != null && personsCount > 0 && guestId != null) {
                    val selectedTable = tables.find { it.number == selectedTableNumber }

                    val bookingDate = dateFormat.parse(bookingDateStr)
                    val bookingTime = timeFormat.parse(bookingTimeStr)
                    val calendar = Calendar.getInstance()
                    calendar.time = bookingDate
                    calendar.set(Calendar.HOUR_OF_DAY, bookingTime.hours)
                    calendar.set(Calendar.MINUTE, bookingTime.minutes)

                    val newBooking = Booking(
                        id = null,
                        tableId = selectedTable?.id,
                        guestId = guestId,
                        bookingDateTime = calendar.time,
                        personsCount = personsCount,
                        comment = comment
                    )

                    bookingsService.createBooking(newBooking, {
                        dialog.dismiss()
                        fetchBookings()
                    }, { errorMessage ->
                        Toast.makeText(this, "Failed to create booking: $errorMessage", Toast.LENGTH_SHORT).show()
                    })
                } else {
                    Toast.makeText(this, "Invalid input for creating booking", Toast.LENGTH_SHORT).show()
                }
            }
        }, { errorMessage ->
            Toast.makeText(this, "Failed to load tables: $errorMessage", Toast.LENGTH_SHORT).show()
        })
    }
}
