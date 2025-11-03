# Sistema de Gestión de Citas - Documentación

## ?? Resumen de Implementación

Se ha implementado un sistema completo para gestionar citas psicológicas usando una API REST, reemplazando las conexiones SQL directas.

---

## ??? Arquitectura Implementada

### Modelos (Models/)
1. **CitaModel.cs** - Representa una cita con paciente, psicólogo y servicio
2. **ServicioModel.cs** - Servicios psicológicos disponibles
3. **PsicologoModel.cs** - Información de psicólogos
4. **PacienteModel.cs** - Información de pacientes
5. **UserModel.cs** - Usuarios del sistema
6. **LoginResponse.cs** - Respuesta de autenticación

### Servicios API (Services/)
1. **CitaApiService.cs** - CRUD completo de citas
2. **ServicioApiService.cs** - Obtención de servicios
3. **PsicologoApiService.cs** - Obtención de psicólogos
4. **PacienteApiService.cs** - Obtención de pacientes

### Repositorios (Repositories/)
1. **UserRepository.cs** - Gestión de usuarios
2. **ApiTokenStore.cs** - Almacenamiento del token JWT

---

## ?? Autenticación

### Login
```csharp
// En LoginViewModel.cs
var loginResponse = await userRepository.AuthenticateUserAsync(Email, plainPassword);
ApiTokenStore.Instance.Token = loginResponse.Token;
```

### Uso del Token
Todos los servicios API usan automáticamente el token almacenado:
```csharp
var token = ApiTokenStore.Instance.Token;
request.Headers.Add("Authorization", $"Bearer {token}");
```

---

## ?? Ejemplo de Uso: Crear una Cita

### 1. Cargar Datos en la Vista
```csharp
// En CrearCita.xaml.cs
private async void CargarDatosIniciales()
{
    // Cargar servicios
    var servicios = await _servicioService.GetServiciosActivosAsync();
    cbServicio.ItemsSource = servicios;
    cbServicio.DisplayMemberPath = "DisplayName";
    
    // Cargar psicólogos
    var psicologos = await _psicologoService.GetPsicologosActivosAsync();
    cbPsicologo.ItemsSource = psicologos;
    
    // Cargar pacientes
    var pacientes = await _pacienteService.GetPacientesActivosAsync();
    cbPaciente.ItemsSource = pacientes;
}
```

### 2. Crear una Cita
```csharp
private async void CrearCita()
{
    var nuevaCita = new CitaModel
    {
        FechaCita = dpFecha.SelectedDate.Value.ToString("yyyy-MM-dd"),
        HoraInicio = "09:00:00",
        HoraFin = "10:00:00",
        MotivoConsulta = txtMotivo.Text,
        IdPaciente = (int)cbPaciente.SelectedValue,
        IdPsicologo = (int)cbPsicologo.SelectedValue,
        IdServicio = (int)cbServicio.SelectedValue,
        IdEstadoCita = 1, // Pendiente
    };
    
    var citaCreada = await _citaService.CreateCitaAsync(nuevaCita);
    
    if (citaCreada != null)
    {
        MessageBox.Show("Cita creada exitosamente");
    }
}
```

### 3. Listar Citas
```csharp
private async void CargarCitas()
{
    var citas = await _citaService.GetAllCitasAsync();
    dgCitas.ItemsSource = citas;
}
```

### 4. Actualizar una Cita
```csharp
private async void ActualizarCita(int idCita)
{
    var cita = await _citaService.GetCitaByIdAsync(idCita);
    
    // Modificar campos
    cita.Observaciones = "Cita confirmada";
    cita.IdEstadoCita = 2; // Confirmada
    
    var citaActualizada = await _citaService.UpdateCitaAsync(idCita, cita);
}
```

### 5. Eliminar una Cita
```csharp
private async void EliminarCita(int idCita)
{
    var resultado = MessageBox.Show(
        "¿Está seguro de eliminar esta cita?", 
        "Confirmar", 
        MessageBoxButton.YesNo);
        
    if (resultado == MessageBoxResult.Yes)
    {
        bool eliminada = await _citaService.DeleteCitaAsync(idCita);
        if (eliminada)
        {
            MessageBox.Show("Cita eliminada exitosamente");
        }
    }
}
```

---

## ?? Endpoints de la API

### Base URL
```
http://147.182.240.177:8000/api/v1
```

### Autenticación
- **POST** `/login/access-token` - Login y obtención de token

### Usuarios
- **GET** `/users/` - Listar usuarios
- **GET** `/users/me` - Usuario actual
- **GET** `/users/{id}` - Usuario por ID

### Citas
- **GET** `/citas/` - Listar todas las citas
- **POST** `/citas/` - Crear cita
- **GET** `/citas/{id}` - Obtener cita por ID
- **PUT** `/citas/{id}` - Actualizar cita
- **DELETE** `/citas/{id}` - Eliminar cita

### Servicios
- **GET** `/servicios/` - Listar servicios disponibles
- **GET** `/servicios/{id}` - Servicio por ID

### Psicólogos
- **GET** `/psicologos/` - Listar psicólogos
- **GET** `/psicologos/{id}` - Psicólogo por ID

### Pacientes
- **GET** `/pacientes/` - Listar pacientes
- **GET** `/pacientes/{id}` - Paciente por ID

---

## ?? Propiedades Útiles para UI

### ServicioModel
```csharp
servicio.DisplayName // "Consulta Individual Adultos - $45000.00 (50 min)"
servicio.NombreServicio
servicio.Precio
servicio.DuracionMinutos
```

### PsicologoModel
```csharp
psicologo.NombreCompleto // "Ana María Rodríguez Silva"
psicologo.DisplayName // "Ana María Rodríguez Silva - Psicóloga Clínica"
psicologo.TituloProfesional
psicologo.AniosExperiencia
```

### PacienteModel
```csharp
paciente.NombreCompleto // "Juan Pérez González"
paciente.DisplayName // "Juan Pérez González - 12345678-9"
paciente.Rut
paciente.Telefono
```

---

## ?? Tareas Pendientes (TODO)

### Prioridad Alta
1. ? Implementar modelos de Cita, Servicio, Psicólogo
2. ? Crear servicios API para cada endpoint
3. ? Implementar CrearCita con API
4. ? Crear selector de pacientes (ComboBox en lugar de TextBox)
5. ? Implementar selector de hora real
6. ? Agregar selector de sala

### Prioridad Media
7. ? Crear vista de listado de citas (DataGrid)
8. ? Implementar edición de citas
9. ? Agregar filtros de búsqueda
10. ? Implementar estados de cita (pendiente, confirmada, cancelada)

### Prioridad Baja
11. ? Agregar validación de horarios disponibles
12. ? Implementar notificaciones/recordatorios
13. ? Agregar reportes de citas
14. ? Implementar calendario visual

---

## ??? Seguridad

### Buenas Prácticas Implementadas
? Token JWT almacenado en memoria (no en código)
? HTTPS recomendado para producción
? Validación de campos en cliente
? Manejo de errores con try-catch

### Mejoras Recomendadas
- Implementar refresh token
- Agregar timeout de sesión
- Encriptar datos sensibles
- Validar permisos por rol

---

## ?? Cómo Ejecutar

1. **Iniciar sesión**
   - Usuario: admin@example.com
   - Contraseña: [tu contraseña]

2. **Crear una cita**
   - Ir a "Crear Cita"
   - Seleccionar servicio
   - Seleccionar psicólogo
   - Ingresar datos del paciente
   - Click en "Enviar"

3. **Ver citas**
   - Ir a "Control de Solicitudes"
   - Las citas se cargan automáticamente

---

## ?? Recursos

- **API Base**: http://147.182.240.177:8000/
- **Documentación API**: http://147.182.240.177:8000/docs
- **Framework**: WPF .NET Framework 4.7.2
- **Patrón**: MVVM
- **Librerías**: Newtonsoft.Json, FontAwesome.Sharp

---

## ?? Solución de Problemas

### Error: "Token no válido"
- Verificar que el token no haya expirado
- Hacer login nuevamente

### Error: "No se puede conectar a la API"
- Verificar que la URL base sea correcta
- Verificar conexión a internet
- Confirmar que el servidor API esté activo

### Error: "Campo Estado no se deserializa"
- Ya corregido: `Estado` es `string` en lugar de `bool`

---

## ?? Contacto y Soporte

Para dudas o problemas, revisar:
1. Logs de la aplicación
2. Respuestas de la API en el debugger
3. Documentación de la API en `/docs`

---

**Última actualización**: 2025-01-15
**Versión**: 1.0.0
