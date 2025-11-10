# Instrucciones para Depurar el DisplayName del Usuario

## Cambios Implementados

Se han agregado mensajes de depuración detallados en los siguientes archivos:

### 1. **LoginViewModel.cs**
- Muestra el email con el que se intenta iniciar sesión
- Confirma si el login fue exitoso y muestra el token
- Muestra el username que se establece en el Thread.CurrentPrincipal
- Registra el valor de Thread.CurrentPrincipal.Identity.Name después de establecerlo

### 2. **MainViewModel.cs**
- Muestra el username obtenido del Thread.CurrentPrincipal
- Registra si el username está vacío o null
- Muestra los datos del usuario obtenidos de la API
- Registra el DisplayName final que se establece
- Captura y muestra cualquier excepción que ocurra

### 3. **UserRepository.cs**
- Muestra el username que se pasa al método GetByUsernameAsync
- Verifica y muestra el estado del token
- Muestra la URL de la petición a la API
- Registra el código de estado HTTP de la respuesta
- Muestra el JSON completo recibido de la API
- Registra los datos del usuario deserializados
- Captura y muestra excepciones

## Cómo Depurar

### Paso 1: Abrir la Ventana de Salida (Output)
1. En Visual Studio, ve a **Ver** ? **Salida** (o presiona `Ctrl+Alt+O`)
2. En el menú desplegable "Mostrar salida desde:", selecciona **Depurar**

### Paso 2: Ejecutar la Aplicación en Modo Debug
1. Presiona **F5** o haz clic en el botón **Iniciar Depuración**
2. Inicia sesión con tus credenciales

### Paso 3: Revisar los Mensajes de Depuración

Busca los siguientes mensajes en la ventana de salida:

#### Durante el Login:
```
[ExecuteLoginCommand] Attempting login for: usuario@email.com
[ExecuteLoginCommand] Login successful. Token: eyJhbGciOiJIUzI1NiIs...
[ExecuteLoginCommand] Username from response: usuario@email.com
[ExecuteLoginCommand] Thread.CurrentPrincipal.Identity.Name: usuario@email.com
[ExecuteLoginCommand] Setting IsViewVisible to false
```

#### Al Cargar MainView:
```
[LoadCurrentUserData] Username from Principal: 'usuario@email.com'
[LoadCurrentUserData] Calling GetByUsernameAsync for: usuario@email.com
```

#### En el UserRepository:
```
[GetByUsernameAsync] Called with username: usuario@email.com
[GetByUsernameAsync] Token: eyJhbGciOiJIUzI1NiIs...
[GetByUsernameAsync] Sending request to: http://147.182.240.177:8000//api/v1/users/me
[GetByUsernameAsync] Response status: OK
[GetByUsernameAsync] Response JSON: {"id_usuario":"...","nombre_usuario":"...","nombres":"Juan","apellidos":"Pérez",...}
[GetByUsernameAsync] User deserialized: Juan Pérez (Username: usuario@email.com)
```

#### De vuelta en MainViewModel:
```
[LoadCurrentUserData] User found: Juan Pérez
[LoadCurrentUserData] DisplayName set to: Juan Pérez
```

## Posibles Problemas y Soluciones

### Problema 1: "Username from Principal" está vacío
**Síntoma:** `[LoadCurrentUserData] Username is null or empty`

**Causa:** El Thread.CurrentPrincipal no se estableció correctamente durante el login.

**Solución:** 
- Verifica que el login sea exitoso
- Verifica que loginResponse.Username tenga un valor
- Verifica que Thread.CurrentPrincipal se esté estableciendo antes de que IsViewVisible = false

### Problema 2: Token es NULL/EMPTY
**Síntoma:** `[GetByUsernameAsync] Token: NULL/EMPTY`

**Causa:** El token no se guardó en ApiTokenStore o se perdió.

**Solución:**
- Verifica que `ApiTokenStore.Instance.Token = loginResponse.Token;` se ejecute
- Verifica que el token no sea null en el loginResponse
- Verifica que ApiTokenStore sea un singleton

### Problema 3: Error 401 Unauthorized
**Síntoma:** `[GetByUsernameAsync] Response status: Unauthorized`

**Causa:** El token es inválido o expiró.

**Solución:**
- Verifica que el token sea válido
- Verifica que el header de Authorization se esté enviando correctamente
- Intenta hacer login nuevamente

### Problema 4: User es null después de deserializar
**Síntoma:** El JSON se recibe pero user es null

**Causa:** Los nombres de las propiedades en UserModel no coinciden con el JSON.

**Solución:**
- Verifica que los atributos [JsonProperty] en UserModel coincidan con el JSON recibido
- Revisa el JSON en los mensajes de depuración

### Problema 5: DisplayName no se actualiza en la UI
**Síntoma:** Los logs muestran que DisplayName se estableció correctamente pero no aparece en la ventana

**Causa:** INotifyPropertyChanged no está funcionando correctamente.

**Solución:**
- Verifica que UserAccountModel implemente INotifyPropertyChanged
- Verifica que la propiedad DisplayName llame a OnPropertyChanged
- Verifica que el binding en el XAML sea correcto: `{Binding CurrentUserAccount.DisplayName}`

## Prueba Adicional

Si después de revisar los logs todo parece correcto pero el nombre no aparece, agrega este código temporalmente al constructor de MainViewModel:

```csharp
public MainViewModel()
{
    userRepository = new UserRepository();
    CurrentUserAccount = new UserAccountModel();
    
    // PRUEBA TEMPORAL - Establecer un valor por defecto
    CurrentUserAccount.DisplayName = "PRUEBA - Usuario de Prueba";
    
    // ... resto del código ...
}
```

Si "PRUEBA - Usuario de Prueba" aparece en la UI, significa que el problema está en la carga asíncrona de datos, no en el binding.

## Información Adicional

- Los mensajes de depuración solo aparecen cuando ejecutas la aplicación en modo Debug (F5)
- Los mensajes aparecen en tiempo real en la ventana de Salida
- Si no ves ningún mensaje, verifica que el filtro de la ventana de Salida esté en "Depurar"
- Puedes usar Ctrl+F en la ventana de Salida para buscar "[LoadCurrentUserData]" o "[GetByUsernameAsync]"

## Siguiente Paso

Después de ejecutar la aplicación y revisar los logs, comparte los mensajes que veas en la ventana de Salida para que podamos diagnosticar exactamente dónde está el problema.
