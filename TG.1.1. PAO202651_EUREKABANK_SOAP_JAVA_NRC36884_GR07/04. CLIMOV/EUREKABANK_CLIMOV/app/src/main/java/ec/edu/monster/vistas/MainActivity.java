package ec.edu.monster.vistas;

import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import ec.edu.monster.configuracion.ConfiguracionSoap;
import ec.edu.monster.controladores.BancaControlador;
import ec.edu.monster.databinding.ActivityMainBinding;
import ec.edu.monster.databinding.ItemAccountBinding;
import ec.edu.monster.modelos.Modelos.*;
import java.math.BigDecimal;
import java.util.*;
import java.util.concurrent.*;

/** Vista Android: presenta resultados; el controlador conoce el dominio y SOAP. */
public final class MainActivity extends AppCompatActivity {
 private ActivityMainBinding b; private final BancaControlador c=new BancaControlador(); private final ExecutorService pool=Executors.newSingleThreadExecutor();
 @Override public void onCreate(Bundle saved){super.onCreate(saved);b=ActivityMainBinding.inflate(getLayoutInflater());setContentView(b.getRoot());((View)b.operationButton.getParent()).setVisibility(View.GONE);restaurarServidor();cargarLogo();b.loginButton.setOnClickListener(v->login());b.serverConfigButton.setOnClickListener(v->editarServidor());b.refreshButton.setOnClickListener(v->cargarCuentas());b.transferButton.setOnClickListener(v->transferir());b.historyButton.setOnClickListener(v->historial());b.logoutButton.setOnClickListener(v->salir());}
 private void cargarLogo(){int id=getResources().getIdentifier("logo_eurekabank","drawable",getPackageName());if(id!=0){b.logo.setImageResource(id);b.logoFallback.setVisibility(View.GONE);}}
 private void restaurarServidor(){String guardada=getSharedPreferences("eurekabank",MODE_PRIVATE).getString("soap_url",null);if(guardada!=null)ConfiguracionSoap.establecerUrl(guardada);}
 private void editarServidor(){EditText input=new EditText(this);input.setSingleLine(true);input.setText(ConfiguracionSoap.urlBase());input.setSelectAllOnFocus(false);int margen=(int)(24*getResources().getDisplayMetrics().density);new AlertDialog.Builder(this).setTitle("Servidor SOAP").setMessage("Solo modifica esta dirección si cambió el servidor.").setView(input,margen,0,margen,0).setNegativeButton("Cancelar",null).setPositiveButton("Guardar",(d,w)->{try{String url=input.getText().toString().trim();ConfiguracionSoap.establecerUrl(url);getSharedPreferences("eurekabank",MODE_PRIVATE).edit().putString("soap_url",ConfiguracionSoap.urlBase()).apply();b.loginStatus.setText("Servidor actualizado.");}catch(IllegalArgumentException e){b.loginStatus.setText(e.getMessage());}}).show();}
 private void login(){b.loginStatus.setText("Conectando…");enSegundoPlano(()->{Sesion s=c.login(b.userInput.getText().toString().trim(),b.passwordInput.getText().toString());runOnUiThread(()->{b.loginScreen.setVisibility(View.GONE);b.dashboardScreen.setVisibility(View.VISIBLE);b.welcomeText.setText("Hola, "+s.nombre);cargarCuentas();});},e->b.loginStatus.setText(limpiar(e)));}
 private void cargarCuentas(){enSegundoPlano(()->{List<Cuenta> cs=c.cuentas();runOnUiThread(()->pintarCuentas(cs));},e->b.dashboardStatus.setText(limpiar(e)));}
 private void pintarCuentas(List<Cuenta> cs){b.accountsContainer.removeAllViews();for(Cuenta x:cs){ItemAccountBinding i=ItemAccountBinding.inflate(getLayoutInflater(),b.accountsContainer,false);i.accountState.setText("CUENTA "+x.estado);i.accountId.setText("•••• "+ultimos(x.id));i.accountBalance.setText(String.format(Locale.US,"$ %,.2f",x.saldo));b.accountsContainer.addView(i.getRoot());}}
 private void transferir(){try{String o=b.originInput.getText().toString().trim(),d=b.destinationInput.getText().toString().trim();BigDecimal m=new BigDecimal(b.amountInput.getText().toString().trim());enSegundoPlano(()->{c.transferir(o,d,m);runOnUiThread(()->{b.dashboardStatus.setText("Transferencia completada y auditada.");cargarCuentas();});},e->b.dashboardStatus.setText(limpiar(e)));}catch(Exception e){b.dashboardStatus.setText("Escribe un monto válido.");}}
 private void historial(){String id=b.historyAccountInput.getText().toString().trim();enSegundoPlano(()->{List<Movimiento> ms=c.movimientos(id);StringBuilder x=new StringBuilder();for(Movimiento m:ms)x.append(m.tipo).append("  ·  $ ").append(m.monto).append("\n").append(m.fecha).append("\n\n");runOnUiThread(()->b.historyText.setText(x.length()==0?"No existen movimientos.":x.toString()));},e->b.historyText.setText(limpiar(e)));}
 private void operacion(){try{String tipo=b.operationTypeInput.getText().toString().trim().toUpperCase(),cuenta=b.operationAccountInput.getText().toString().trim();BigDecimal monto=new BigDecimal(b.operationAmountInput.getText().toString().trim());enSegundoPlano(()->{c.movimiento(tipo,cuenta,monto);runOnUiThread(()->{b.dashboardStatus.setText("Operación registrada de forma auditable.");cargarCuentas();});},e->b.dashboardStatus.setText(limpiar(e)));}catch(Exception e){b.dashboardStatus.setText("Completa tipo, cuenta y monto válido.");}}
 private void reversa(){String id=b.reverseIdInput.getText().toString().trim();enSegundoPlano(()->{c.reversar(id);runOnUiThread(()->{b.dashboardStatus.setText("Reversa registrada correctamente.");cargarCuentas();});},e->b.dashboardStatus.setText(limpiar(e)));}
 private void salir(){c.salir();b.dashboardScreen.setVisibility(View.GONE);b.loginScreen.setVisibility(View.VISIBLE);b.passwordInput.setText("");b.loginStatus.setText("");}
 private void enSegundoPlano(Tarea t,Error e){pool.execute(()->{try{t.run();}catch(Exception x){runOnUiThread(()->e.run(x));}});}
 private String ultimos(String x){return x.length()>8?x.substring(x.length()-8):x;} private String limpiar(Exception e){return e.getMessage()==null?"No se pudo conectar con el servidor.":e.getMessage().replaceAll("(?s)<[^>]*>","").trim();}
 @FunctionalInterface interface Tarea{void run()throws Exception;} @FunctionalInterface interface Error{void run(Exception e);}
 @Override protected void onDestroy(){pool.shutdownNow();super.onDestroy();}
}
