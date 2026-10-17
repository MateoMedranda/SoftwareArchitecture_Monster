package com.marpa.practica.view.fragments

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.inputmethod.EditorInfo
import android.widget.ArrayAdapter
import android.widget.Filter
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.core.os.bundleOf
import androidx.core.widget.doAfterTextChanged
import androidx.fragment.app.Fragment
import androidx.lifecycle.lifecycleScope
import com.google.android.material.dialog.MaterialAlertDialogBuilder
import com.marpa.practica.controller.ConverterController
import com.marpa.practica.databinding.FragmentConverterBinding
import com.marpa.practica.model.UnitConverterModel
import com.marpa.practica.model.entities.UnitCategory
import com.marpa.practica.model.storage.SessionStore
import com.marpa.practica.view.ConverterView
import com.marpa.practica.view.LoginActivity
import com.marpa.practica.R
import com.marpa.practica.model.entities.Unit

/** Vista: una pestaña del conversor. Se reutiliza para todas las categorías. */
class ConverterFragment : Fragment(), ConverterView {

    private var _binding: FragmentConverterBinding? = null
    private val binding get() = _binding!!
    private lateinit var controller: ConverterController

    private var sessionDialog: AlertDialog? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentConverterBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        val category = UnitCategory.valueOf(requireArguments().getString(ARG_CATEGORY)!!)
        controller = ConverterController(
            view = this,
            model = UnitConverterModel(SessionStore(requireContext().applicationContext)),
            category = category,
            scope = viewLifecycleOwner.lifecycleScope   // se cancela solo al destruir la vista
        )

        binding.actFrom.setOnItemClickListener { _, _, position, _ -> controller.onFromSelected(position) }
        binding.actTo.setOnItemClickListener { _, _, position, _ -> controller.onToSelected(position) }
        binding.etValue.doAfterTextChanged { controller.onInputChanged() }
        binding.etValue.setOnEditorActionListener { _, actionId, _ ->
            if (actionId == EditorInfo.IME_ACTION_DONE) { convert(); true } else false
        }
        binding.btnConvert.setOnClickListener { convert() }

        controller.onViewCreated()
    }

    private fun convert() {
        controller.onConvertClicked(binding.etValue.text?.toString().orEmpty())
    }

    // --- ConverterView ---

    override fun showTitle(title: String) {
        binding.tvTitle.text = title
    }

    override fun showUnits(units: List<Unit>, fromIndex: Int, toIndex: Int) {
        val fullNames = resources.getBoolean(R.bool.show_full_unit_names)
        val labels : List<String> = units.map {
            if (fullNames)
            {
                it.name
            } else {
                it.symbol
            }
        }
        binding.actFrom.setAdapter(NoFilterAdapter(requireContext(), labels))
        binding.actTo.setAdapter(NoFilterAdapter(requireContext(), labels))
        // filter = false: evita que el adaptador filtre la lista con el texto actual.
        binding.actFrom.setText(labels[fromIndex], false)
        binding.actTo.setText(labels[toIndex], false)
    }

    override fun showSymbols(from: String, to: String) {
        binding.tilValue.suffixText = from
        binding.tilResult.suffixText = to
    }

    override fun showResult(text: String) {
        binding.etResult.setText(text)
    }

    override fun clearResult() {
        binding.etResult.setText("")
    }

    override fun showInputError(message: String?) {
        if(message == null) clearError()
        binding.tilValue.error = message
    }

    private fun clearError()
    {
        binding.tilValue.error = null
    }

    override fun showError(message: String) {
        context?.let { Toast.makeText(it, message, Toast.LENGTH_LONG).show() }
    }

    override fun showLoading(show: Boolean) {
        // Puede llegar después de onDestroyView (la corrutina se cancela): no tocar un binding nulo.
        _binding?.btnConvert?.isEnabled = !show
    }

    override fun showSessionExpiredError() {
        val ctx = context ?: return
        if (sessionDialog?.isShowing == true) return
        sessionDialog = MaterialAlertDialogBuilder(ctx)
            .setTitle(R.string.session_expired_title)
            .setMessage(R.string.session_expired_message)
            .setCancelable(false)   // solo se sale con uno de los dos botones
            .setPositiveButton(R.string.action_login_again) { _, _ -> goToLogin() }
            .setNegativeButton(R.string.action_exit) { _, _ -> activity?.finishAffinity() }
            .show()
    }

    private fun goToLogin() {
        // CLEAR_TASK vacía el historial: el botón "atrás" no regresa al conversor.
        startActivity(
            Intent(requireContext(), LoginActivity::class.java)
                .addFlags(Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK)
        )
    }

    override fun onDestroyView() {
        sessionDialog?.dismiss()
        sessionDialog = null
        super.onDestroyView()
        _binding = null
    }

    companion object {
        private const val ARG_CATEGORY = "category"

        fun newInstance(category: UnitCategory) = ConverterFragment().apply {
            arguments = bundleOf(ARG_CATEGORY to category.name)
        }
    }
}

/** Adaptador para dropdowns de selección fija: siempre muestra todas las opciones. */
private class NoFilterAdapter(context: Context, items: List<String>) :
    ArrayAdapter<String>(context, android.R.layout.simple_dropdown_item_1line, items) {

    private val all = items.toList()

    override fun getFilter() = object : Filter() {
        override fun performFiltering(constraint: CharSequence?) =
            FilterResults().apply { values = all; count = all.size }

        override fun publishResults(constraint: CharSequence?, results: FilterResults?) {
            notifyDataSetChanged()
        }
    }
}