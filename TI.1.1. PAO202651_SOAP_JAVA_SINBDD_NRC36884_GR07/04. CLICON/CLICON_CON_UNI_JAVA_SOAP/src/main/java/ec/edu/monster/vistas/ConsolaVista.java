package ec.edu.monster.vistas;

import ec.edu.monster.modelos.ResultadoConversion;
import ec.edu.monster.modelos.TipoConversion;
import java.io.InputStream;
import java.io.PrintStream;
import java.text.DecimalFormat;
import java.util.List;
import java.util.Scanner;

/** Vista MVC: concentra toda la entrada y salida de la consola. */
public class ConsolaVista {
    private final Scanner entrada;
    private final PrintStream salida;
    private final DecimalFormat formato = new DecimalFormat("0.##########");

    public ConsolaVista(InputStream entrada, PrintStream salida) {
        this.entrada = new Scanner(entrada);
        this.salida = salida;
    }

    public void mostrarBienvenida() {
        salida.println();
        salida.println("============================================================");
        salida.println("M     M   OOOOO   NN    N   SSSSS  TTTTTTT  EEEEEEE  RRRRRR");
        salida.println("MM   MM  OO   OO  NNN   N  SS          T    EE       RR   RR");
        salida.println("M M M M  OO   OO  NN N  N   SSSSS      T    EEEEE    RRRRRR");
        salida.println("M  M  M  OO   OO  NN  NNN       SS     T    EE       RR  RR");
        salida.println("M     M   OOOOO   NN    N  SSSSS       T    EEEEEEE  RR   RRR");
        salida.println("============================================================");
        salida.println("========================== MONSTER =========================");
        salida.println("============================================================");
        salida.println("=== SISTEMA DE CONVERSION DE UNIDADES - CLIENTE SOAP ===");
        salida.println("============================================================");
    }

    public String solicitarTexto(String etiqueta) {
        salida.print(etiqueta);
        return entrada.hasNextLine() ? entrada.nextLine().trim() : "";
    }

    public String solicitarContrasenia(String etiqueta) {
        return solicitarTexto(etiqueta);
    }

    public int mostrarMenuPrincipal() {
        salida.println("\n1. Convertir temperatura");
        salida.println("2. Convertir masa");
        salida.println("3. Convertir longitud");
        salida.println("4. Cambiar contraseña");
        salida.println("0. Salir");
        return solicitarEntero("Seleccione una opción: ");
    }

    public double solicitarValor() {
        while (true) {
            String texto = solicitarTexto("Valor a convertir: ").replace(',', '.');
            try {
                double valor = Double.parseDouble(texto);
                if (Double.isFinite(valor)) return valor;
            } catch (NumberFormatException ignored) { }
            mostrarError("Ingrese un número finito válido.");
        }
    }

    public String seleccionarUnidad(String etiqueta, List<String> unidades) {
        salida.println(etiqueta);
        for (int i = 0; i < unidades.size(); i++) {
            salida.printf("%d. %s%n", i + 1, unidades.get(i));
        }
        while (true) {
            int opcion = solicitarEntero("Seleccione una unidad: ");
            if (opcion >= 1 && opcion <= unidades.size()) return unidades.get(opcion - 1);
            mostrarError("Opción de unidad inválida.");
        }
    }

    public void mostrarConversion(TipoConversion tipo, ResultadoConversion resultado) {
        salida.printf("%n%s: %s %s = %s %s%n", tipo.titulo(),
                formato.format(resultado.valor()), resultado.unidadOrigen(),
                formato.format(resultado.resultado()), resultado.unidadDestino());
    }

    public void mostrarMensaje(String mensaje) { salida.println(mensaje); }
    public void mostrarError(String mensaje) { salida.println("Error: " + mensaje); }

    private int solicitarEntero(String etiqueta) {
        while (true) {
            String texto = solicitarTexto(etiqueta);
            try { return Integer.parseInt(texto); }
            catch (NumberFormatException ignored) { mostrarError("Ingrese una opción numérica."); }
        }
    }
}
