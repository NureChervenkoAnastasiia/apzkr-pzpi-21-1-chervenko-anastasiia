package com.example.tastifymobile.adapters

import android.app.AlertDialog
import android.app.DatePickerDialog
import android.app.TimePickerDialog
import android.content.Context
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.EditText
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.tastifymobile.R
import com.example.tastifymobile.api.ApiService
import com.example.tastifymobile.api.NetworkModule
import com.example.tastifymobile.models.Booking
import com.example.tastifymobile.models.Table
import com.example.tastifymobile.services.BookingsService
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.text.SimpleDateFormat
import java.util.*

class BookingsAdapter(
    private val context: Context,
    private val bookings: List<Booking>,
    private val bookingsService: BookingsService,
    private val onBookingDeleted: (Booking) -> Unit,
    private val onBookingUpdated: (Booking) -> Unit
) : RecyclerView.Adapter<BookingsAdapter.BookingViewHolder>() {

    private val apiService: ApiService = NetworkModule.provideApiService(context)

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): BookingViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.item_booking, parent, false)
        return BookingViewHolder(view)
    }

    override fun onBindViewHolder(holder: BookingViewHolder, position: Int) {
        val booking = bookings[position]

        booking.tableId?.let { fetchTableNumber(it, holder.tableNumberTextView) }

        holder.bookingDateTimeTextView.text = SimpleDateFormat("yyyy-MM-dd HH:mm", Locale.getDefault()).format(booking.bookingDateTime)
        holder.personsCountTextView.text = booking.personsCount.toString()
        holder.commentTextView.text = booking.comment

        holder.deleteButton.setOnClickListener {
            bookingsService.deleteBooking(booking.id!!, {
                onBookingDeleted(booking)
            }, { errorMessage ->
                Log.e("BookingsAdapter", errorMessage)
            })
        }

        holder.updateButton.setOnClickListener {
            showUpdateBookingDialog(holder, booking)
        }
    }

    override fun getItemCount(): Int {
        return bookings.size
    }

    private fun fetchTableNumber(tableId: String, tableNumberTextView: TextView) {
        apiService.getTableById(tableId).enqueue(object : Callback<Table> {
            override fun onResponse(call: Call<Table>, response: Response<Table>) {
                if (response.isSuccessful) {
                    val table = response.body()
                    if (table != null) {
                        tableNumberTextView.text = "Table Number: ${table.number}"
                    } else {
                        Log.e("BookingsAdapter", "Failed to retrieve table data")
                        tableNumberTextView.text = "Table ID: $tableId"
                    }
                } else {
                    Log.e("BookingsAdapter", "Failed to retrieve table data: ${response.message()}")
                    tableNumberTextView.text = "Table ID: $tableId"
                }
            }

            override fun onFailure(call: Call<Table>, t: Throwable) {
                Log.e("BookingsAdapter", "Error fetching table data: ${t.message}")
                tableNumberTextView.text = "Table ID: $tableId"
            }
        })
    }

    private fun showUpdateBookingDialog(holder: BookingViewHolder, booking: Booking) {
        val builder = AlertDialog.Builder(holder.itemView.context)
        val inflater = LayoutInflater.from(holder.itemView.context)
        val dialogView = inflater.inflate(R.layout.dialog_update_booking, null)
        builder.setView(dialogView)

        val dialog = builder.create()
        dialog.show()

        val editTextPersonsCount = dialogView.findViewById<EditText>(R.id.editTextPersonsCount)
        val editTextComment = dialogView.findViewById<EditText>(R.id.editTextComment)
        val editTextBookingDate = dialogView.findViewById<EditText>(R.id.editTextBookingDate)
        val editTextBookingTime = dialogView.findViewById<EditText>(R.id.editTextBookingTime)
        val btnUpdateBooking = dialogView.findViewById<Button>(R.id.btnUpdateBooking)

        editTextPersonsCount.setText(booking.personsCount.toString())
        editTextComment.setText(booking.comment)

        val dateFormat = SimpleDateFormat("yyyy-MM-dd", Locale.getDefault())
        val timeFormat = SimpleDateFormat("HH:mm", Locale.getDefault())

        // Set current booking date and time
        editTextBookingDate.setText(dateFormat.format(booking.bookingDateTime))
        editTextBookingTime.setText(timeFormat.format(booking.bookingDateTime))

        // Date picker logic
        editTextBookingDate.setOnClickListener {
            val calendar = Calendar.getInstance()
            val dateSetListener = DatePickerDialog.OnDateSetListener { _, year, month, dayOfMonth ->
                val selectedDate = Calendar.getInstance()
                selectedDate.set(year, month, dayOfMonth)
                editTextBookingDate.setText(dateFormat.format(selectedDate.time))
            }

            DatePickerDialog(
                holder.itemView.context,
                dateSetListener,
                calendar.get(Calendar.YEAR),
                calendar.get(Calendar.MONTH),
                calendar.get(Calendar.DAY_OF_MONTH)
            ).show()
        }

        // Time picker logic
        editTextBookingTime.setOnClickListener {
            val calendar = Calendar.getInstance()
            val timeSetListener = TimePickerDialog.OnTimeSetListener { _, hour, minute ->
                val selectedTime = Calendar.getInstance()
                selectedTime.set(Calendar.HOUR_OF_DAY, hour)
                selectedTime.set(Calendar.MINUTE, minute)
                editTextBookingTime.setText(timeFormat.format(selectedTime.time))
            }

            TimePickerDialog(
                holder.itemView.context,
                timeSetListener,
                calendar.get(Calendar.HOUR_OF_DAY),
                calendar.get(Calendar.MINUTE),
                true
            ).show()
        }

        btnUpdateBooking.setOnClickListener {
            val updatedPersonsCount = editTextPersonsCount.text.toString().toInt()
            val updatedComment = editTextComment.text.toString()
            val updatedBookingDate = dateFormat.parse(editTextBookingDate.text.toString())
            val updatedBookingTime = timeFormat.parse(editTextBookingTime.text.toString())

            if (updatedBookingDate != null && updatedBookingTime != null) {
                val calendar = Calendar.getInstance()
                calendar.time = updatedBookingDate
                val bookingCalendar = Calendar.getInstance()
                bookingCalendar.time = updatedBookingTime
                calendar.set(Calendar.HOUR_OF_DAY, bookingCalendar.get(Calendar.HOUR_OF_DAY))
                calendar.set(Calendar.MINUTE, bookingCalendar.get(Calendar.MINUTE))

                val updatedBooking = Booking(
                    id = booking.id,
                    tableId = booking.tableId,
                    bookingDateTime = calendar.time,
                    personsCount = updatedPersonsCount,
                    comment = updatedComment,
                    guestId = booking.guestId
                )

                bookingsService.updateBooking(updatedBooking.id!!, updatedBooking, {
                    onBookingUpdated(updatedBooking)
                    dialog.dismiss()
                }, { errorMessage ->
                    Log.e("BookingsAdapter", errorMessage)
                })
            }
        }
    }

    class BookingViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val tableNumberTextView: TextView = itemView.findViewById(R.id.tableNumberTextView)
        val bookingDateTimeTextView: TextView = itemView.findViewById(R.id.bookingDateTimeTextView)
        val personsCountTextView: TextView = itemView.findViewById(R.id.personsCountTextView)
        val commentTextView: TextView = itemView.findViewById(R.id.commentTextView)
        val deleteButton: Button = itemView.findViewById(R.id.deleteBookingButton)
        val updateButton: Button = itemView.findViewById(R.id.updateBookingButton)
    }


}
