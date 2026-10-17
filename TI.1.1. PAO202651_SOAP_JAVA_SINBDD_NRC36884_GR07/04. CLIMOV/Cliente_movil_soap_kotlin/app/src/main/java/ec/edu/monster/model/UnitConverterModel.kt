package ec.edu.monster.model

import ec.edu.monster.model.entities.Unit
import ec.edu.monster.model.entities.UnitCategory
import ec.edu.monster.model.network.SessionExpiredException
import ec.edu.monster.model.network.SoapClient
import ec.edu.monster.model.storage.SessionStore

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
