// MenuAdapter.kt
package com.example.tastifymobile.adapters

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.tastifymobile.R
import com.example.tastifymobile.models.Menu

class MenuAdapter(private val menuList: List<Menu>) : RecyclerView.Adapter<MenuAdapter.MenuViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): MenuViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.item_menu, parent, false)
        return MenuViewHolder(itemView)
    }

    override fun onBindViewHolder(holder: MenuViewHolder, position: Int) {
        val menuItem = menuList[position]
        holder.nameTextView.text = "Name: ${menuItem.name}"
        holder.sizeTextView.text = "Size: ${menuItem.size}"
        holder.priceTextView.text = "Price: ${menuItem.price}"
        holder.infoTextView.text = "Info: ${menuItem.info}"
        holder.typeTextView.text = "Type: ${menuItem.type}"
    }

    override fun getItemCount() = menuList.size

    class MenuViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val nameTextView: TextView = itemView.findViewById(R.id.nameTextView)
        val sizeTextView: TextView = itemView.findViewById(R.id.sizeTextView)
        val priceTextView: TextView = itemView.findViewById(R.id.priceTextView)
        val infoTextView: TextView = itemView.findViewById(R.id.infoTextView)
        val typeTextView: TextView = itemView.findViewById(R.id.typeTextView)
    }
}
