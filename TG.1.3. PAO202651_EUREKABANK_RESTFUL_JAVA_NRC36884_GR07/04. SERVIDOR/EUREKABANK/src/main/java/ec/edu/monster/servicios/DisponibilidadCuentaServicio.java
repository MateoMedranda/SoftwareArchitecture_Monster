package ec.edu.monster.servicios;

import ec.edu.monster.modelos.DTO.EstadoCuentaDTO;
import ec.edu.monster.modelos.Respuestas.TransaccionExcepcion;
import jakarta.ejb.ConcurrencyManagement;
import jakarta.ejb.ConcurrencyManagementType;
import jakarta.ejb.LocalBean;
import jakarta.ejb.Singleton;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Comparator;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentMap;
import java.util.concurrent.atomic.AtomicLong;
import java.util.concurrent.locks.ReentrantLock;

/**
 * Coordina la disponibilidad temporal de cuentas en una instancia del servidor.
 *
 * El estado no se persiste: representa exclusivamente una operación que se está
 * ejecutando. La consistencia entre procesos se garantiza además con
 * {@code SELECT ... FOR UPDATE} en la transacción JDBC. Los clientes REST pueden
 * consultar el estado o hacer long polling mediante la versión del estado.
 */
@Singleton
@LocalBean
@ConcurrencyManagement(ConcurrencyManagementType.BEAN)
public class DisponibilidadCuentaServicio {

    private final ConcurrentMap<String, ReentrantLock> bloqueos = new ConcurrentHashMap<>();
    private final ConcurrentMap<String, AtomicLong> versiones = new ConcurrentHashMap<>();
    private final Object monitorCambios = new Object();

    @FunctionalInterface
    public interface Operacion<T> {
        T ejecutar() throws TransaccionExcepcion;
    }

    public <T> T ejecutarExclusivo(Collection<String> cuentaIds, Operacion<T> operacion)
            throws TransaccionExcepcion {
        List<String> ids = normalizarYOrdenar(cuentaIds);
        List<ReentrantLock> adquiridos = new ArrayList<>();
        try {
            for (String cuentaId : ids) {
                ReentrantLock bloqueo = bloqueos.computeIfAbsent(cuentaId, id -> new ReentrantLock(true));
                bloqueo.lock();
                adquiridos.add(bloqueo);
                publicarCambio(cuentaId);
            }
            return operacion.ejecutar();
        } finally {
            for (int i = adquiridos.size() - 1; i >= 0; i--) {
                String cuentaId = ids.get(i);
                adquiridos.get(i).unlock();
                publicarCambio(cuentaId);
            }
        }
    }

    public EstadoCuentaDTO obtenerEstado(String cuentaId) {
        ReentrantLock bloqueo = bloqueos.computeIfAbsent(cuentaId, id -> new ReentrantLock(true));
        long version = versiones.computeIfAbsent(cuentaId, id -> new AtomicLong(0)).get();
        return crearEstado(cuentaId, bloqueo.isLocked(), version);
    }

    /**
     * Espera, como máximo, treinta segundos a que cambie la versión de una
     * cuenta. Si no hay cambio, devuelve el estado actual para que el cliente
     * REST pueda volver a consultar sin perder conectividad.
     */
    public EstadoCuentaDTO esperarCambio(String cuentaId, long versionConocida, long esperaMaximaMs)
            throws InterruptedException {
        long espera = Math.max(0L, Math.min(esperaMaximaMs, 30_000L));
        long limite = System.currentTimeMillis() + espera;
        synchronized (monitorCambios) {
            while (versionActual(cuentaId) <= versionConocida) {
                long restante = limite - System.currentTimeMillis();
                if (restante <= 0) break;
                monitorCambios.wait(restante);
            }
        }
        return obtenerEstado(cuentaId);
    }

    private List<String> normalizarYOrdenar(Collection<String> cuentaIds) throws TransaccionExcepcion {
        if (cuentaIds == null || cuentaIds.isEmpty()) {
            throw new TransaccionExcepcion("Cuenta requerida", null);
        }
        Set<String> unicos = new HashSet<>();
        for (String cuentaId : cuentaIds) {
            if (cuentaId == null || cuentaId.trim().isEmpty()) {
                throw new TransaccionExcepcion("Cuenta requerida", null);
            }
            unicos.add(cuentaId.trim());
        }
        List<String> ids = new ArrayList<>(unicos);
        ids.sort(Comparator.naturalOrder());
        return ids;
    }

    private void publicarCambio(String cuentaId) {
        versiones.computeIfAbsent(cuentaId, id -> new AtomicLong(0)).incrementAndGet();
        synchronized (monitorCambios) {
            monitorCambios.notifyAll();
        }
    }

    private long versionActual(String cuentaId) {
        return versiones.computeIfAbsent(cuentaId, id -> new AtomicLong(0)).get();
    }

    private EstadoCuentaDTO crearEstado(String cuentaId, boolean bloqueada, long version) {
        EstadoCuentaDTO estado = new EstadoCuentaDTO();
        estado.setCuentaId(cuentaId);
        estado.setBloqueada(bloqueada);
        estado.setEstado(bloqueada ? "BLOQUEADA" : "DISPONIBLE");
        estado.setVersion(version);
        return estado;
    }
}
