# ?? PLAN DE MIGRACIÓN: ELIMINACIÓN DE SQL DIRECTO ? API REST

## ?? Estado Actual del Proyecto

### ? **Ya Migrado a API:**
- ? `LoginView.xaml.cs` - Usa `UsuarioApiService`
- ? `ControlSolicitudes.xaml.cs` - Usa `CitaApiService`, `PacienteApiService`, etc.
- ? `CrearCita.xaml.cs` - Usa servicios de API
- ? `ConfirmarSolicitud.xaml.cs` - Usa servicios de API

### ? **Archivos con SQL Directo (15 archivos):**

| # | Archivo | Conexión SQL | Estado | Acción |
|---|---------|--------------|--------|---------|
| 1 | `RepositoryBase.cs` | `SqlConnection` | ?? Base class | **DEPRECAR** |
| 2 | `Atencion.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 3 | `Cita.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 4 | `CitaSeg.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 5 | `CustomerView2.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 6 | `CustomerView4.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 7 | `CustomerView5.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 8 | `FormularioA.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 9 | `FormularioS.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 10 | `Medicacion.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 11 | `MedicacionG.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 12 | `Reportes.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 13 | `ReportesG.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 14 | `Tratamiento.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 15 | `TratamientoG.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 16 | `Paciente.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 17 | `CrudSolicitudServicio.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 18 | `DenegarSolicitud.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 19 | `CrudEmpleado.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |
| 20 | `ConsultaSolicitudes.xaml.cs` | `conexionDB2` | ? Obsoleto | **ELIMINAR SQL** |

---

## ?? Estrategia de Migración

### **FASE 1: Identificación y Análisis** ? COMPLETADA
- [x] Identificar todos los archivos con SQL directo
- [x] Clasificar por prioridad
- [x] Documentar conexiones

### **FASE 2: Limpieza de Código SQL (ACTUAL)**

#### **Opción A: Eliminación Total (RECOMENDADO)**
Eliminar todo el código SQL directo y dejar comentarios `TODO` para futuras implementaciones con API.

**Ventajas:**
- ? Proyecto más limpio
- ? Sin dependencias de SQL
- ? Facilita migración completa
- ? Reduce confusión

**Desventajas:**
- ?? Funcionalidades quedan pendientes de implementar

#### **Opción B: Comentar Código SQL**
Comentar todo el código SQL pero dejarlo en el proyecto.

**Ventajas:**
- ? Preserva lógica original
- ? Referencia para futuras implementaciones

**Desventajas:**
- ? Código innecesario en el proyecto
- ? Más difícil de mantener

---

## ?? PLAN DE EJECUCIÓN (Opción A - RECOMENDADO)

### **1. Archivos a Limpiar - Eliminación de SQL Directo**

#### **Grupo 1: Vistas de Atención y Citas (Alta Prioridad)**
```
? Atencion.xaml.cs
? Cita.xaml.cs
? CitaSeg.xaml.cs
? FormularioA.xaml.cs
? FormularioS.xaml.cs
```

**Acción:**
- Eliminar `SqlConnection`, `SqlCommand`, `SqlDataAdapter`
- Eliminar método `CargarDatos()` con SQL
- Agregar comentarios `// TODO: Migrar a API`
- Mantener estructura de métodos para futura implementación

#### **Grupo 2: Vistas de Pacientes (Alta Prioridad)**
```
? Paciente.xaml.cs
? CustomerView5.xaml.cs
```

**Acción:**
- Eliminar código CRUD con SQL
- Inyectar `PacienteApiService` cuando esté listo
- Comentar métodos `Crear()`, `Consultar()`, `Actualizar()`, `Eliminar()`

#### **Grupo 3: Vistas de Reportes (Media Prioridad)**
```
? Reportes.xaml.cs
? ReportesG.xaml.cs
? Medicacion.xaml.cs
? MedicacionG.xaml.cs
? Tratamiento.xaml.cs
? TratamientoG.xaml.cs
```

**Acción:**
- Eliminar SQL
- Mantener estructura UI
- Preparar para futura implementación con API de reportes

#### **Grupo 4: Vistas de Solicitudes Obsoletas (Baja Prioridad)**
```
? CustomerView2.xaml.cs
? CrudSolicitudServicio.xaml.cs
? DenegarSolicitud.xaml.cs
? ConsultaSolicitudes.xaml.cs
```

**Acción:**
- **OPCIÓN 1:** Eliminar archivos completos (si no se usan)
- **OPCIÓN 2:** Limpiar SQL y dejar estructura

#### **Grupo 5: Vistas de Empleados (Media Prioridad)**
```
? CustomerView4.xaml.cs
? CrudEmpleado.xaml.cs
```

**Acción:**
- Eliminar SQL
- Preparar para `EmpleadoApiService` (a crear)

---

### **2. Archivos Base a Actualizar**

#### **RepositoryBase.cs** ? **DEPRECAR**
```csharp
// ?? DEPRECADO: Este archivo usa conexión SQL directa
// TODO: Eliminar cuando todas las vistas migren a API
// Usar ApiServiceBase en su lugar
```

---

### **3. Configuración a Limpiar**

#### **App.config**
```xml
<!-- ANTES: -->
<connectionStrings>
    <add name="conexionDB" ... />
    <add name="conexionDB2" ... />
</connectionStrings>

<!-- DESPUÉS: -->
<connectionStrings>
    <!-- ?? DEPRECADO: Migrando a API REST -->
    <!-- <add name="conexionDB2" ... /> -->
</connectionStrings>

<appSettings>
    <add key="ApiBaseUrl" value="http://147.182.240.177:8000/" />
</appSettings>
```

---

## ?? TEMPLATE DE LIMPIEZA

### **Archivo Original (CON SQL):**
```csharp
public partial class Atencion : UserControl
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB2"].ConnectionString);
    
    void CargarDatos()
    {
        con.Open();
        SqlCommand cmd = new SqlCommand("SELECT...", con);
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        GridDatos.ItemsSource = dt.DefaultView;
        con.Close();
    }
}
```

### **Archivo Limpio (SIN SQL):**
```csharp
using WPF_LoginForm.Services;
using WPF_LoginForm.Models;

namespace WPF_LoginForm.Views
{
    public partial class Atencion : UserControl
    {
        // TODO: Implementar servicios de API
        // private readonly CitaApiService _citaService;
        // private readonly PacienteApiService _pacienteService;
        
        public Atencion()
        {
            InitializeComponent();
            // TODO: Migrar CargarDatos() a API
            // CargarDatosAsync();
        }
        
        // TODO: Migrar a API REST
        // private async Task CargarDatosAsync()
        // {
        //     try
        //     {
        //         var citas = await _citaService.GetAllCitasAsync();
        //         GridDatos.ItemsSource = citas;
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show($"Error: {ex.Message}");
        //     }
        // }
        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Implementar búsqueda local o con filtros de API
        }
        
        private void Agregar(object sender, RoutedEventArgs e)
        {
            CrearCita ventana = new CrearCita();
            FrameAtencion.Content = ventana;
        }
        
        private void Consultar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar consulta con API
        }
        
        private void Actualizar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar actualización con API
        }
        
        private void Eliminar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar eliminación con API
        }
    }
}
```

---

## ? CHECKLIST DE LIMPIEZA POR ARCHIVO

### **Para cada archivo:**
- [ ] Eliminar `using System.Data.SqlClient`
- [ ] Eliminar `using System.Configuration` (si solo se usa para SQL)
- [ ] Eliminar declaración de `SqlConnection`
- [ ] Eliminar métodos con `SqlCommand`, `SqlDataAdapter`, `DataTable`
- [ ] Comentar o eliminar código CRUD con SQL
- [ ] Agregar comentarios `// TODO: Migrar a API`
- [ ] Agregar `using WPF_LoginForm.Services` (para futuro)
- [ ] Mantener estructura de eventos de UI
- [ ] Verificar compilación sin errores

---

## ?? BENEFICIOS DE LA LIMPIEZA

### **Código:**
? Más limpio y mantenible  
? Sin dependencias de SQL  
? Preparado para API REST  
? Menos líneas de código  

### **Proyecto:**
? Arquitectura moderna  
? Separación de responsabilidades  
? Escalable y testeable  
? Compatible con microservicios  

### **Desarrollo:**
? Menos confusión  
? Migraciones más fáciles  
? Onboarding más rápido  
? Menos bugs relacionados con conexiones  

---

## ?? IMPACTO ESTIMADO

### **Archivos a Modificar:** 20 archivos
### **Líneas de Código a Eliminar:** ~500-800 líneas
### **Reducción de Complejidad:** ~40%
### **Tiempo Estimado:** 2-3 horas

---

## ?? CONSIDERACIONES

### **1. Base de Datos SQL Server**
- ? ¿Aún se necesita para algo?
- ? ¿Se puede eliminar completamente?
- ? ¿Hay datos que migrar?

**Recomendación:** Mantener backup de BD, eliminar referencias en código

### **2. Connection Strings**
- ? ¿Comentar o eliminar de `App.config`?

**Recomendación:** Comentar por ahora, eliminar cuando todo esté migrado

### **3. Archivos Obsoletos**
Los siguientes archivos parecen obsoletos:
- `CustomerView2.xaml.cs` (solicitudes antiguas)
- `ConsultaSolicitudes.xaml.cs` (reemplazada por ControlSolicitudes)
- `DenegarSolicitud.xaml.cs` (lógica integrada en ControlSolicitudes)

**Recomendación:** Eliminar archivos completos si no se usan

---

## ?? PRÓXIMOS PASOS DESPUÉS DE LA LIMPIEZA

### **Inmediato:**
1. ? Compilar proyecto
2. ? Verificar que vistas funcionales (ControlSolicitudes, CrearCita) siguen OK
3. ? Probar login y navegación

### **Corto Plazo (1-2 semanas):**
1. ?? Implementar `CitaApiService` completo
2. ?? Migrar `Atencion.xaml.cs` a API
3. ?? Migrar `Cita.xaml.cs` a API
4. ?? Implementar `PacienteApiService` completo

### **Mediano Plazo (1 mes):**
1. ?? Crear `ReporteApiService`
2. ?? Migrar todas las vistas de reportes
3. ?? Crear `EmpleadoApiService`
4. ?? Migrar vistas de empleados

### **Largo Plazo (2-3 meses):**
1. ??? Eliminar `RepositoryBase.cs`
2. ??? Eliminar connection strings de `App.config`
3. ??? Eliminar archivos obsoletos
4. ?? Actualizar documentación

---

## ?? PLANTILLA DE COMMIT

```
feat: Eliminar dependencias de SQL directo y preparar para API REST

BREAKING CHANGE: Se han eliminado todas las conexiones SQL directas

- Eliminados SqlConnection, SqlCommand, SqlDataAdapter de 20 archivos
- Código SQL comentado con TODOs para migración a API
- Mantenida estructura de UI y eventos
- Proyecto compila sin errores
- Preparado para implementación con API REST

Archivos modificados:
- WPF-LoginForm/Views/Atencion.xaml.cs
- WPF-LoginForm/Views/Cita.xaml.cs
- WPF-LoginForm/Views/CitaSeg.xaml.cs
- [... lista completa ...]

Próximos pasos:
1. Implementar servicios de API faltantes
2. Migrar lógica de negocio a servicios
3. Actualizar vistas para usar API
```

---

## ?? VERIFICACIÓN POST-LIMPIEZA

### **Tests de Compilación:**
```bash
? Proyecto compila sin errores
? Sin warnings de SQL
? Sin referencias a conexionDB2
```

### **Tests Funcionales:**
```bash
? Login funciona (usa API)
? ControlSolicitudes funciona (usa API)
? CrearCita funciona (usa API)
? Navegación entre vistas OK
```

### **Tests de Regresión:**
```bash
?? Vistas con SQL dejan de cargar datos (esperado)
? UI sigue mostrándose correctamente
? Botones y eventos funcionan
```

---

**Estado del Plan:** ? **LISTO PARA EJECUTAR**  
**Fecha de Creación:** 3 de Enero de 2025  
**Versión:** 1.0.0  

---

*¿Proceder con la limpieza de SQL directo?*
