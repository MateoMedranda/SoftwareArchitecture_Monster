package com.marpa.practica.model

import com.marpa.practica.model.entities.Unit
import com.marpa.practica.model.entities.UnitCategory
import com.marpa.practica.model.network.SessionExpiredException
import com.marpa.practica.model.network.SoapClient
import com.marpa.practica.model.storage.SessionStore

class UnitConverterModel (private val session: SessionStore,private val soap: SoapClient = SoapClient()) {
    fun unitsOf(category: UnitCategory): List<Unit> = UnitCatalog.unitsOf(category)

    fun convert(category: UnitCategory, value: Double, fromIndex: Int, toIndex: Int): Double{
        val units = unitsOf(category)
        val from = units[fromIndex].soapUnit
        val to = units[toIndex].soapUnit
        val operacion = category.soapOperation
        val token = session.getToken()
        try{
            return soap.convert(operacion, value, from, to, session.getToken())
        }
        catch (e: SessionExpiredException){
            session.clear()
            throw e
        }
    }
}
