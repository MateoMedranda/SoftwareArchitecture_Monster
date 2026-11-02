package ec.edu.monster.controladores;

import ec.edu.monster.modelos.*;
import ec.edu.monster.servicios.ClienteSoapEureka;
import jakarta.enterprise.context.RequestScoped;
import jakarta.inject.Inject;
import jakarta.inject.Named;
import java.math.BigDecimal;
import java.util.*;

/** Casos de uso del panel; valida la entrada antes de enviar SOAP. */
@Named
@RequestScoped
public class BancaBean {
    @Inject private SesionBean sesion;
    private final ClienteSoapEureka soap = new ClienteSoapEureka();
    private List<CuentaVista> cuentas = List.of();
    private List<MovimientoVista> movimientos = List.of();
    private List<ClienteCuentaVista> clientes = List.of();
    private String cuentaId, origen, destino, tipo, transaccionId, clienteSeleccionado, mensaje;
    private BigDecimal monto;
    private CuentaVista cuentaCreada;
    private PerfilVista perfil;

    public void cargarCuentas() { try { cuentas = soap.cuentas(sesion.getToken()); actualizarDisponibilidades(); } catch (Exception e) { error(e); } }
    public void refrescarDisponibilidades() { try { if (cuentas.isEmpty()) cuentas = soap.cuentas(sesion.getToken()); actualizarDisponibilidades(); } catch (Exception ignored) { } }
    private void actualizarDisponibilidades() throws Exception { Map<String,String> estados = soap.disponibilidades(sesion.getToken()); for (CuentaVista cuenta : cuentas) cuenta.setDisponibilidad(estados.get(cuenta.getId())); }
    public void cargarPerfil() { try { perfil = soap.perfil(sesion.getToken(), sesion.getUsuarioId()); } catch (Exception e) { error(e); } }

    public void cargarClientes() { try { clientes = soap.clientesParaCuenta(sesion.getToken()); mensaje = null; } catch (Exception e) { error(e); } }
    public void crearCuenta() { try { if (clienteSeleccionado == null || clienteSeleccionado.isBlank()) { mensaje = "Selecciona un cliente antes de crear la cuenta."; return; } cuentaCreada = soap.crearCuentaParaUsuario(sesion.getToken(), clienteSeleccionado); mensaje = "Cuenta " + cuentaCreada.getNumero() + " creada y asignada correctamente."; clienteSeleccionado = null; cargarCuentas(); } catch (Exception e) { error(e); } }

    public void consultarMovimientos() { if (!validarCuenta(cuentaId, "Número de cuenta")) return; try { movimientos = soap.movimientos(sesion.getToken(), cuentaId.trim()); } catch (Exception e) { error(e); } }
    public void transferir() { if (!validarCuenta(origen, "Cuenta origen") || !validarCuenta(destino, "Cuenta destino") || !validarMonto()) return; if (origen.trim().equals(destino.trim())) { mensaje = "Las cuentas de origen y destino deben ser diferentes."; return; } try { soap.transferir(sesion.getToken(), origen.trim(), destino.trim(), monto); mensaje = "Transferencia completada y registrada en auditoría."; origen = destino = null; monto = null; cargarCuentas(); } catch (Exception e) { error(e); } }
    public void registrar() { if (tipo == null || !(tipo.equals("INGRESO") || tipo.equals("RETIRO"))) { mensaje = "Selecciona Depósito o Retiro."; return; } if (!validarCuenta(cuentaId, "Número de cuenta") || !validarMonto()) return; try { soap.movimiento(sesion.getToken(), tipo, cuentaId.trim(), monto); mensaje = "Operación registrada correctamente."; cuentaId = null; monto = null; tipo = null; cargarCuentas(); } catch (Exception e) { error(e); } }
    public void reversar() { if (transaccionId == null || !transaccionId.trim().matches("[0-9a-fA-F-]{36}")) { mensaje = "Ingresa un ID de transacción válido."; return; } try { soap.reversar(sesion.getToken(), transaccionId.trim()); mensaje = "Reversa registrada correctamente."; transaccionId = null; cargarCuentas(); } catch (Exception e) { error(e); } }

    private boolean validarCuenta(String valor, String campo) { if (valor == null || !valor.trim().matches("\\d{10}")) { mensaje = campo + " debe tener exactamente 10 dígitos."; return false; } return true; }
    private boolean validarMonto() { if (monto == null || monto.compareTo(BigDecimal.ZERO) <= 0) { mensaje = "El monto debe ser mayor a cero."; return false; } return true; }
    private void error(Exception e) { mensaje = e.getMessage() == null ? "No se pudo completar la operación." : e.getMessage().replaceAll("(?s)<[^>]*>", "").trim(); }

    public List<CuentaVista> getCuentas() { if (cuentas.isEmpty() && sesion.getActiva()) cargarCuentas(); return cuentas; }
    public List<ClienteCuentaVista> getClientes() { return clientes; }
    public List<MovimientoVista> getMovimientos() { return movimientos; }
    public PerfilVista getPerfil() { return perfil; }
    public String getClienteSeleccionado() { return clienteSeleccionado; } public void setClienteSeleccionado(String v) { clienteSeleccionado = v; }
    public CuentaVista getCuentaCreada() { return cuentaCreada; }
    public String getCuentaId() { return cuentaId; } public void setCuentaId(String v) { cuentaId = v; }
    public String getOrigen() { return origen; } public void setOrigen(String v) { origen = v; }
    public String getDestino() { return destino; } public void setDestino(String v) { destino = v; }
    public String getTipo() { return tipo; } public void setTipo(String v) { tipo = v; }
    public String getTransaccionId() { return transaccionId; } public void setTransaccionId(String v) { transaccionId = v; }
    public BigDecimal getMonto() { return monto; } public void setMonto(BigDecimal v) { monto = v; }
    public String getMensaje() { return mensaje; }
}
