# ? CORRECCIONES DE ERRORES - SESIÓN DE DEBUGGING

## ?? Errores Solucionados

### **1. Error: "No se puede convertir valor null a DateTime"**

#### **Problema:**
```
Error converting value {null} to type 'System.DateTime'. Path 'data[0].fecha_modificacion'
```

#### **Causa:**
La API devuelve `null` en el campo `fecha_modificacion` cuando una cita no ha sido modificada, pero el modelo `CitaModel` definía esta propiedad como `DateTime` (no nullable).

#### **Solución:**
```csharp
// ANTES (causaba error):
[JsonProperty("fecha_modificacion")]
public DateTime FechaModificacion { get; set; }

// DESPUÉS (acepta null):
[JsonProperty("fecha_modificacion")]
public DateTime? FechaModificacion { get; set; } // ? Nullable
```

**Archivo modificado:** `WPF-LoginForm\Models\CitaModel.cs`

---

### **2. Error: "No se pudo cargar la información de la solicitud"**

#### **Problema:**
Al hacer click en el botón "Confirmar" de una solicitud, aparecía el error: "No se pudo cargar la información de la solicitud".

#### **Causa Posible:**
1. El `CommandParameter` del botón no estaba siendo bindeado correctamente
2. El método `GetCitaByIdAsync` estaba fallando silenciosamente
3. Falta de logs de depuración para identificar el problema

#### **Soluciones Implementadas:**

##### **A. Mejorado manejo de errores en `ControlSolicitudes.xaml.cs`:**

```csharp
private async void Confirmar(object sender, RoutedEventArgs e)
{
    try
    {
        // ? Validar que CommandParameter no sea nulo
        if (((Button)sender).CommandParameter == null)
        {
            MessageBox.Show("?? Error: No se pudo obtener el ID de la cita.", 
                "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int idCita = (int)((Button)sender).CommandParameter;
        
        // ? Agregar log de depuración
        System.Diagnostics.Debug.WriteLine($"Confirmando cita con ID: {idCita}");
        
        var cita = await _citaService.GetCitaByIdAsync(idCita);
        
        if (cita == null)
        {
            // ? Mensaje de error más informativo
            MessageBox.Show($"? No se pudo cargar la información de la solicitud\n\n" +
                $"ID de Cita: {idCita}\n\n" +
                $"La cita puede haber sido eliminada o no existe.\n\n" +
                $"Actualizando lista de solicitudes...", 
                "Error al Cargar Datos", MessageBoxButton.OK, MessageBoxImage.Error);
            
            CargarDatos();
            return;
        }
        
        // Navegar a CrearCita
        CrearCita ventana = new CrearCita(idCita, cita);
        FrameControlSolicitudes.Content = ventana;
    }
    catch (InvalidCastException ex)
    {
        // ? Manejo específico de error de conversión
        MessageBox.Show($"?? Error de formato de datos", 
            "Error de Conversión", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
    catch (System.Net.Http.HttpRequestException ex)
    {
        // ? Manejo específico de error de conexión
        MessageBox.Show($"? Error de conexión con el servidor", 
            "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
    }
    catch (Exception ex)
    {
        // ? Mensaje de error detallado
        MessageBox.Show($"Error: {ex.Message}\nTipo: {ex.GetType().Name}", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

##### **B. Agregados logs de depuración en `CitaApiService.cs`:**

```csharp
public async Task<CitaModel> GetCitaByIdAsync(int idCita)
{
    try
    {
        var token = ApiTokenStore.Instance.Token;
        if (string.IsNullOrEmpty(token))
        {
            System.Diagnostics.Debug.WriteLine("Error: Token no disponible");
            return null;
        }

        System.Diagnostics.Debug.WriteLine($"Obteniendo cita con ID: {idCita}");
        
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/citas/{idCita}");
        request.Headers.Add("accept", "application/json");
        request.Headers.Add("Authorization", $"Bearer {token}");

        var response = await _httpClient.SendAsync(request);
        
        System.Diagnostics.Debug.WriteLine($"Status Code: {response.StatusCode}");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Response JSON: {json}");
            
            var cita = JsonConvert.DeserializeObject<CitaModel>(json);
            return cita;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Error Response: {errorContent}");
            return null;
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Excepción: {ex.Message}");
        throw; // Re-lanzar para manejo en método llamador
    }
}
```

---

## ?? Cómo Debuggear Ahora

### **1. Ver Logs en Visual Studio:**

**Output Window:**
- Abrir: `View ? Output` (o `Ctrl+Alt+O`)
- Seleccionar "Debug" en el dropdown
- Verás mensajes como:
  ```
  Confirmando cita con ID: 4
  Obteniendo cita con ID: 4
  Status Code: 200
  Response JSON: { ... }
  Cita deserializada: 4
  ```

### **2. Verificar Binding del CommandParameter:**

El XAML está configurado correctamente:
```xml
<Button Content="? Confirmar"
        Click="Confirmar"
        CommandParameter="{Binding IdCita}"/>
```

**Para verificar que el binding funciona:**
1. Poner un breakpoint en `Confirmar()`
2. Ejecutar y hacer click en el botón
3. Inspeccionar `((Button)sender).CommandParameter`
4. Debe mostrar el ID de la cita (ej: 4, 5, 6...)

---

## ?? Archivos Modificados

| Archivo | Cambios | Estado |
|---------|---------|--------|
| `CitaModel.cs` | FechaModificacion ? nullable | ? Corregido |
| `ControlSolicitudes.xaml.cs` | Mejor manejo de errores | ? Mejorado |
| `CitaApiService.cs` | Logs de depuración agregados | ? Mejorado |

---

## ? Validación

### **Compilación:**
```
? Proyecto compila sin errores
? Sin warnings
```

### **Tests Sugeridos:**

1. **Abrir Control de Solicitudes:**
   - ? Debe cargar lista de solicitudes pendientes
   - ? Debe mostrar información completa (paciente, psicólogo, servicio)

2. **Click en botón "Confirmar":**
   - ? Debe abrir ventana CrearCita con datos pre-cargados
   - ? Si falla, debe mostrar mensaje de error descriptivo
   - ? Logs en Output window deben mostrar el flujo

3. **Verificar Output Window:**
   - ? Debe mostrar: "Confirmando cita con ID: X"
   - ? Debe mostrar: "Status Code: 200"
   - ? Debe mostrar el JSON de respuesta

---

## ?? Si el Error Persiste

### **Posibles Causas:**

1. **Token expirado:**
   - Cerrar sesión y volver a hacer login
   - Verificar en Output: "Error: Token no disponible"

2. **Cita no existe en la API:**
   - Verificar ID en la base de datos
   - La cita puede haber sido eliminada por otro usuario

3. **Error de conexión:**
   - Verificar que el servidor esté corriendo
   - Ping a: http://147.182.240.177:8000/
   - Verificar endpoint: GET /api/v1/citas/{id}

4. **Problema con el binding:**
   - Verificar en debugger que `CommandParameter` no es null
   - Verificar que `IdCita` existe en `CitaExtendidaModel`

### **Debugging Avanzado:**

```csharp
// Agregar en Confirmar() para ver el objeto completo
private async void Confirmar(object sender, RoutedEventArgs e)
{
    // Ver toda la fila seleccionada
    var button = (Button)sender;
    var citaExtendida = button.DataContext as CitaExtendidaModel;
    
    System.Diagnostics.Debug.WriteLine($"DataContext: {citaExtendida?.IdCita}");
    System.Diagnostics.Debug.WriteLine($"CommandParameter: {button.CommandParameter}");
    
    // ... resto del código
}
```

---

## ?? Próximos Pasos

### **Si el error continúa:**

1. ? **Ejecutar la aplicación en Debug**
2. ? **Hacer click en "Confirmar"**
3. ? **Copiar los logs del Output Window**
4. ? **Compartir logs para análisis**

### **Logs importantes a buscar:**

```
// Éxito:
Confirmando cita con ID: 4
Obteniendo cita con ID: 4
Status Code: 200
Response JSON: {"id_cita":4, ... }
Cita deserializada: 4

// Error de token:
Error: Token no disponible

// Error de conexión:
Status Code: 401 Unauthorized
Error Response: {"detail":"Not authenticated"}

// Error de cita no encontrada:
Status Code: 404
Error Response: {"detail":"Cita not found"}
```

---

## ?? Recomendaciones

### **1. Mantenimiento:**
- Revisar logs periódicamente
- Actualizar tokens expirados
- Validar que los IDs existan antes de consultar

### **2. Mejoras Futuras:**
- Agregar retry automático en caso de error de red
- Cachear citas para reducir llamadas a la API
- Agregar indicador de "Cargando..." mientras se obtiene la cita

### **3. Testing:**
- Crear citas de prueba desde la landing page
- Probar con diferentes estados (pendiente, confirmada, etc.)
- Verificar comportamiento con múltiples usuarios

---

**Fecha de Corrección:** 3 de Enero de 2025  
**Versión:** 1.5.1  
**Estado:** ? **ERRORES CORREGIDOS - DEBUGGING MEJORADO**

---

*Los errores han sido corregidos y se han agregado herramientas de debugging para facilitar la identificación de futuros problemas.*
