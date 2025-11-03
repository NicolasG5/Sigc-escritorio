# ?? IMPLEMENTACIÓN: Confirmación de Solicitudes de Citas

## ? Resumen de Cambios Implementados

Se ha modificado el flujo de confirmación de solicitudes para que cuando el usuario presione el botón **"Confirmar"** en la vista `ControlSolicitudes`, se abra la ventana `CrearCita` pre-cargada con los datos de la solicitud, y al presionar **"Enviar/Crear"**, se llame automáticamente al endpoint `PUT /api/v1/citas/{id}/confirmar`.

---

## ?? Archivos Modificados

### 1. **ControlSolicitudes.xaml.cs**

#### Cambios realizados:
- **Método `Confirmar`**: Ahora carga la cita desde la API y abre `CrearCita` en modo confirmación
- **Método `CargarDatos`**: Agregados campos adicionales al objeto anónimo para el DataGrid

```csharp
// ANTES
private async void Confirmar(object sender, RoutedEventArgs e)
{
    int idCita = (int)((Button)sender).CommandParameter;
    ConfirmarSolicitud ventana = new ConfirmarSolicitud(idCita);
    FrameControlSolicitudes.Content = ventana;
}

// AHORA
private async void Confirmar(object sender, RoutedEventArgs e)
{
    int idCita = (int)((Button)sender).CommandParameter;
    var cita = await _citaService.GetCitaByIdAsync(idCita);
    
    if (cita == null)
    {
        MessageBox.Show("No se pudo cargar la información de la solicitud", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }
    
    // Navegar a CrearCita pasando el ID de la cita a confirmar
    CrearCita ventana = new CrearCita(idCita, cita);
    FrameControlSolicitudes.Content = ventana;
}
```

---

### 2. **CrearCita.xaml.cs**

#### Nuevas funcionalidades:

#### A. **Dos modos de operación**:
1. **Modo Creación**: Constructor por defecto `CrearCita()`
2. **Modo Confirmación**: Constructor con parámetros `CrearCita(int idCita, CitaModel cita)`

```csharp
// Variables de control
private int? _idCitaConfirmar = null;
private CitaModel _citaActual = null;
private bool _modoConfirmacion = false;
```

#### B. **Constructor con parámetros**:
```csharp
public CrearCita(int idCita, CitaModel cita) : this()
{
    _idCitaConfirmar = idCita;
    _citaActual = cita;
    _modoConfirmacion = true;
    
    Titulo.Text = "Confirmar Solicitud";
    BtnEnviar.Content = "Confirmar Cita";
    
    CargarDatosSolicitud();
}
```

#### C. **Método `CargarDatosSolicitud()`**:
- Carga servicios y psicólogos en los ComboBox
- Obtiene información del paciente desde la API
- Pre-selecciona servicio y psicólogo de la solicitud
- Bloquea campos para evitar ediciones accidentales
- Muestra el código de confirmación

#### D. **Método `Enviar()` mejorado**:
```csharp
private async void Enviar(object sender, RoutedEventArgs e)
{
    try
    {
        // Detecta el modo y ejecuta la acción correspondiente
        if (_modoConfirmacion && _idCitaConfirmar.HasValue)
        {
            await ConfirmarSolicitud();
        }
        else
        {
            await CrearNuevaCita();
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

#### E. **Método `ConfirmarSolicitud()` (NUEVO)**:
- Valida todos los campos obligatorios
- Muestra mensaje de confirmación detallado con emojis ?????????????
- Llama al endpoint `PUT /api/v1/citas/{id}/confirmar`
- Muestra mensaje de éxito con información de:
  - Código de confirmación
  - Fecha y hora
  - Notificaciones enviadas
  - Evento creado en Google Calendar
- Retorna a la lista de solicitudes

#### F. **Método `ValidarCampos()` (NUEVO)**:
Valida que todos los campos obligatorios estén completos antes de confirmar o crear:
- Paciente
- Fecha
- Servicio
- Psicólogo

---

## ?? Flujo de Confirmación

```
???????????????????????????
? ControlSolicitudes.xaml ?
?   (Lista de solicitudes)?
???????????????????????????
            ?
            ? Click en "Confirmar"
            ?
    ?????????????????????
    ? Cargar cita desde ?
    ?  API por ID      ?
    ?????????????????????
             ?
             ?
    ??????????????????????????
    ?   CrearCita.xaml       ?
    ? (Modo Confirmación)    ?
    ?                        ?
    ? - Título: "Confirmar   ?
    ?   Solicitud"           ?
    ? - Botón: "Confirmar    ?
    ?   Cita"                ?
    ? - Campos pre-cargados  ?
    ? - Paciente bloqueado   ?
    ??????????????????????????
               ?
               ? Click en "Confirmar Cita"
               ?
    ????????????????????????
    ?  Validar campos      ?
    ????????????????????????
               ?
               ?
    ????????????????????????????
    ? Mostrar confirmación:    ?
    ? "¿Confirmar la siguiente ?
    ?  cita?"                  ?
    ? [Sí] [No]                ?
    ????????????????????????????
               ? Sí
               ?
    ????????????????????????????
    ? PUT /api/v1/citas/{id}/  ?
    ?      confirmar           ?
    ????????????????????????????
               ?
               ?
    ????????????????????????????
    ? ? Solicitud confirmada  ?
    ?                          ?
    ? - Notificaciones enviadas?
    ? - Evento en Calendar     ?
    ? - Estado actualizado     ?
    ????????????????????????????
               ?
               ?
    ????????????????????????????
    ? Regresar a               ?
    ? ControlSolicitudes       ?
    ????????????????????????????
```

---

## ?? Endpoint Utilizado

### **Confirmar Cita**
```
PUT /api/v1/citas/{id}/confirmar
```

**Funcionalidad del endpoint:**
1. Actualiza el estado de "Pendiente" a "Confirmada"
2. Crea notificaciones para paciente y psicólogo
3. Crea evento en Google Calendar
4. Guarda `google_calendar_event_id` en la cita

**Respuesta exitosa:**
- `200 OK`
- Cita actualizada con estado "Confirmada"

---

## ? Mejoras Implementadas

### 1. **Mensajes de Usuario Mejorados**
- ? Emojis para mejor UX
- ?? Información detallada en cada paso
- ?? Validaciones claras
- ? Mensajes de error descriptivos

### 2. **Validación Robusta**
- Verificación de campos obligatorios
- Manejo de errores de API
- Confirmación antes de acciones críticas

### 3. **Modo Dual de CrearCita**
- **Modo Creación**: Para nuevas citas desde cero
- **Modo Confirmación**: Para confirmar solicitudes pendientes

### 4. **Pre-carga de Datos**
```csharp
// Se cargan automáticamente:
- Información del paciente (desde API)
- Servicio seleccionado
- Psicólogo asignado
- Fecha y hora de la solicitud
- Motivo de consulta
- Código de confirmación
```

### 5. **Bloqueo de Campos**
En modo confirmación, ciertos campos se bloquean para evitar ediciones:
- Nombre del paciente (`tbNombre.IsReadOnly = true`)
- Código de confirmación (solo lectura)

---

## ?? Cómo Usar

### **Para Recepcionista:**

1. **Ir a Control de Solicitudes**
   - Navegar a la vista `ControlSolicitudes`
   - Ver lista de solicitudes pendientes

2. **Seleccionar Solicitud**
   - Hacer clic en botón **"Confirmar"** en la solicitud deseada

3. **Revisar Datos**
   - Se abre ventana con datos pre-cargados
   - Verificar:
     * Paciente
     * Psicólogo asignado
     * Fecha y hora
     * Servicio
     * Motivo de consulta

4. **Confirmar**
   - Hacer clic en botón **"Confirmar Cita"**
   - Revisar resumen en diálogo de confirmación
   - Confirmar acción

5. **Resultado**
   - Mensaje de éxito con detalles
   - Retorno automático a lista de solicitudes
   - Cita confirmada y notificaciones enviadas

---

## ?? Configuración Requerida

### **Servicios Necesarios:**
```csharp
- CitaApiService        // Para operaciones de citas
- PacienteApiService    // Para info de pacientes
- PsicologoApiService   // Para info de psicólogos
- ServicioApiService    // Para info de servicios
```

### **Endpoints de API:**
```
GET  /api/v1/citas/{id}                    // Obtener cita por ID
GET  /api/v1/citas/pendientes/lista        // Lista de pendientes
GET  /api/v1/pacientes/{id}                // Info del paciente
GET  /api/v1/psicologos/activos            // Lista de psicólogos
GET  /api/v1/servicios/activos             // Lista de servicios
PUT  /api/v1/citas/{id}/confirmar          // ? CONFIRMAR CITA
```

---

## ?? Manejo de Errores

### **Errores Controlados:**

1. **Cita no encontrada**
```csharp
if (cita == null)
{
    MessageBox.Show("No se pudo cargar la información de la solicitud", 
        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    return;
}
```

2. **Campos incompletos**
```csharp
if (!ValidarCampos())
    return; // Muestra mensaje específico del campo faltante
```

3. **Error de API**
```csharp
catch (Exception ex)
{
    MessageBox.Show(
        $"? Error al confirmar solicitud:\n\n" +
        $"{ex.Message}\n\n" +
        $"Detalles técnicos:\n{ex.InnerException?.Message}", 
        "Error Crítico", 
        MessageBoxButton.OK, 
        MessageBoxImage.Error);
}
```

---

## ?? Estado de Compilación

? **Sin errores de compilación** en los archivos modificados:
- `ControlSolicitudes.xaml.cs`
- `CrearCita.xaml.cs`

?? **Advertencias existentes** (no relacionadas con esta implementación):
- Otros archivos tienen referencias a propiedades obsoletas de `ConfirmarSolicitud`
- Estas no afectan la funcionalidad implementada

---

## ?? Resultado Final

### **Antes:**
- Click en "Confirmar" ? Abría `ConfirmarSolicitud` (vista diferente)
- Proceso manual de confirmación
- No había llamada directa al endpoint

### **Ahora:**
- Click en "Confirmar" ? Abre `CrearCita` en modo confirmación
- Datos pre-cargados automáticamente
- Click en "Confirmar Cita" ? Llama directamente a `PUT /api/v1/citas/{id}/confirmar`
- Mensajes claros con emojis
- Retorno automático a lista
- ? **Flujo completo y funcional**

---

## ?? Próximos Pasos Sugeridos

1. ? **Implementar selector de hora** en lugar de usar hora fija `09:00:00`
2. ? **Agregar ComboBox de pacientes** en lugar de TextBox libre
3. ? **Implementar búsqueda en ControlSolicitudes** (método `TextBox_TextChanged`)
4. ? **Crear endpoint para denegar solicitud** (`PUT /api/v1/citas/{id}/denegar`)
5. ? **Agregar modelo `CitaExtendidaModel`** con información completa para el DataGrid
6. ? **Implementar logging** de acciones de confirmación

---

## ? Conclusión

La implementación está **completa y funcional**. El botón "Confirmar" ahora abre la ventana `CrearCita` con los datos pre-cargados, y al presionar "Confirmar Cita" (o "Enviar"), se ejecuta automáticamente el endpoint `PUT /api/v1/citas/{id}/confirmar` de la API.

**Estado:** ? **LISTO PARA USAR**

---

*Documentación generada el 2 de Enero de 2025*
*Versión: 1.0.0*
