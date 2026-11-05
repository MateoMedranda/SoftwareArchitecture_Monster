import os
from docx import Document
from docx.shared import Pt, RGBColor
from docx.oxml.ns import qn
from docx.oxml import OxmlElement

def aplicar_fondo_celda(celda, color_hex):
    """Aplica un color de fondo a una celda de tabla usando XML de Word."""
    shading_elm = OxmlElement('w:shd')
    shading_elm.set(qn('w:val'), 'clear')
    shading_elm.set(qn('w:color'), 'auto')
    shading_elm.set(qn('w:fill'), color_hex)
    celda._tc.get_or_add_tcPr().append(shading_elm)

def exportar_java_a_docx(carpeta_origen, archivo_salida):
    doc = Document()
    contador_tabla = 1

    # Recorrer todos los directorios buscando archivos .java
    for root, dirs, files in os.walk(carpeta_origen):
        for file in files:
            if file.endswith('.cs'):
                filepath = os.path.join(root, file)
                nombre_clase = os.path.splitext(file)[0]
                
                # 1. Agregar el título (Ej: Tabla 13. Clase ControladorConversiones)
                p_titulo = doc.add_heading(level=5)
                run_titulo = p_titulo.add_run(f"Tabla {contador_tabla}. Clase {nombre_clase} - Cliente Móvil")
                run_titulo.italic = True
                run_titulo.font.color.rgb = RGBColor(31, 73, 125) # Azul oscuro similar a la imagen
                run_titulo.font.size = Pt(9)
                p_titulo.paragraph_format.space_after = Pt(6)
                
                # Leer el código del archivo
                try:
                    with open(filepath, 'r', encoding='utf-8') as f:
                        codigo = f.read()
                except Exception as e:
                    codigo = f"Error al leer el archivo: {e}"
                
                # 2. Crear una tabla 1x1 para lograr el borde exterior y el fondo unificado
                # Esto replica el recuadro negro que se ve en la captura
                table = doc.add_table(rows=1, cols=1)
                table.style = 'Table Grid'
                
                celda = table.cell(0, 0)
                
                # D9D9D9 es el equivalente hexadecimal de RGB(217, 217, 217)
                aplicar_fondo_celda(celda, 'D9D9D9') 
                
                # 3. Insertar y formatear el código
                p_codigo = celda.paragraphs[0]
                run_codigo = p_codigo.add_run(codigo)
                
                # Formato exacto solicitado
                run_codigo.font.name = 'Courier New'
                run_codigo.font.size = Pt(8)
                run_codigo.font.color.rgb = RGBColor(0, 0, 0)
                
                # Eliminar espaciado extra dentro del bloque de código
                p_codigo.paragraph_format.space_after = Pt(0)
                p_codigo.paragraph_format.line_spacing = 1.0
                
                # Añadir un salto de línea al final para separar de la siguiente tabla
                doc.add_paragraph()
                contador_tabla += 1

    doc.save(archivo_salida)
    print(f"✅ Proceso completado. Se ha generado el archivo: {archivo_salida}")

if __name__ == '__main__':
    # ---------------------------------------------------------
    # CONFIGURACIÓN
    # ---------------------------------------------------------
    # Cambia esta ruta por la carpeta donde están tus archivos (ej. proyecto SOAP o clientes)
    CARPETA_CODIGO = './' 
    ARCHIVO_DOCX = 'Documentacion_Archivos_CS.docx'
    
    # Verifica si el directorio existe antes de ejecutar
    if not os.path.exists(CARPETA_CODIGO):
        print(f"⚠️ La carpeta '{CARPETA_CODIGO}' no existe. Por favor, crea la carpeta o cambia la ruta en el script.")
    else:
        exportar_java_a_docx(CARPETA_CODIGO, ARCHIVO_DOCX)
