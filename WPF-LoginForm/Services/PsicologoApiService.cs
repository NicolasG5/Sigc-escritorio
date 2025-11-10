using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.Services
{
    public class PsicologoApiService
    {
        private readonly HttpClient _httpClient;

        public PsicologoApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        /// <summary>
        /// Modelo para deserializar la respuesta paginada de la API
        /// </summary>
        private class EmpleadosResponse
        {
            [JsonProperty("data")]
            public List<PsicologoModel> Data { get; set; }

            [JsonProperty("count")]
            public int Count { get; set; }
        }

        /// <summary>
        /// Modelo para crear empleado (POST)
        /// </summary>
        public class CreateEmpleadoModel
        {
            [JsonProperty("rut")]
            public string Rut { get; set; }

            [JsonProperty("nombres")]
            public string Nombres { get; set; }

            [JsonProperty("apellido_paterno")]
            public string ApellidoPaterno { get; set; }

            [JsonProperty("apellido_materno")]
            public string ApellidoMaterno { get; set; }

            [JsonProperty("fecha_nacimiento")]
            public string FechaNacimiento { get; set; }

            [JsonProperty("telefono")]
            public string Telefono { get; set; }

            [JsonProperty("email_personal")]
            public string EmailPersonal { get; set; }

            [JsonProperty("direccion")]
            public string Direccion { get; set; }

            [JsonProperty("registro_profesional")]
            public string RegistroProfesional { get; set; }

            [JsonProperty("titulo_profesional")]
            public string TituloProfesional { get; set; }

            [JsonProperty("universidad")]
            public string Universidad { get; set; }

            [JsonProperty("anios_experiencia")]
            public int AniosExperiencia { get; set; }

            [JsonProperty("foto_perfil")]
            public string FotoPerfil { get; set; }

            [JsonProperty("id_rol")]
            public int IdRol { get; set; }

            [JsonProperty("estado")]
            public string Estado { get; set; }

            [JsonProperty("id_usuario")]
            public int IdUsuario { get; set; }
        }

        /// <summary>
        /// Obtiene todos los psicólogos (empleados)
        /// GET /api/v1/empleados/
        /// </summary>
        public async Task<IEnumerable<PsicologoModel>> GetAllPsicologosAsync()
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[PsicologoApiService] Token no disponible");
                    return new List<PsicologoModel>();
                }

                var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/empleados/?skip=0&limit=1000");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] GET {_httpClient.BaseAddress}api/v1/empleados/");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response JSON (primeros 300 chars): {json.Substring(0, Math.Min(300, json.Length))}...");
                    
                    // Intentar deserializar como respuesta con data/count
                    EmpleadosResponse empleadosResponse = null;
                    List<PsicologoModel> psicologos = null;
                    
                    try
                    {
                        // Intento 1: Formato con data/count
                        empleadosResponse = JsonConvert.DeserializeObject<EmpleadosResponse>(json);
                        if (empleadosResponse?.Data != null)
                        {
                            psicologos = empleadosResponse.Data;
                            System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Deserializado formato data/count: {psicologos.Count} empleados");
                        }
                    }
                    catch
                    {
                        // Intento 2: Lista directa
                        try
                        {
                            psicologos = JsonConvert.DeserializeObject<List<PsicologoModel>>(json);
                            System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Deserializado formato lista directa: {psicologos?.Count ?? 0} empleados");
                        }
                        catch (Exception ex2)
                        {
                            System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Error al deserializar lista: {ex2.Message}");
                        }
                    }
                    
                    // Sincronizar IDs
                    if (psicologos != null && psicologos.Count > 0)
                    {
                        foreach (var psicologo in psicologos)
                        {
                            psicologo.IdPsicologo = psicologo.IdEmpleado;
                            System.Diagnostics.Debug.WriteLine($"  ? {psicologo.NombreCompleto} (ID: {psicologo.IdEmpleado}, Rol: {psicologo.RolEmpleado})");
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Total empleados cargados: {psicologos.Count}");
                        return psicologos;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[PsicologoApiService] No se obtuvieron empleados");
                        return new List<PsicologoModel>();
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Error HTTP: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Exception en GetAllPsicologosAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] StackTrace: {ex.StackTrace}");
            }
            
            return new List<PsicologoModel>();
        }

        /// <summary>
        /// Obtiene un psicólogo por ID
        /// GET /api/v1/empleados/{empleado_id}
        /// </summary>
        public async Task<PsicologoModel> GetPsicologoByIdAsync(int idEmpleado)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[PsicologoApiService] ? Token no disponible");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] >>> Obteniendo empleado/psicólogo ID: {idEmpleado}");
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] URL: {_httpClient.BaseAddress}api/v1/empleados/{idEmpleado}");

                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/empleados/{idEmpleado}");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response Headers: {response.Headers}");
                
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] ? Response Body (primeros 200 chars): {responseContent.Substring(0, Math.Min(200, responseContent.Length))}");
                    
                    try
                    {
                        var psicologo = JsonConvert.DeserializeObject<PsicologoModel>(responseContent);
                        
                        if (psicologo != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] ? Psicólogo deserializado correctamente:");
                            System.Diagnostics.Debug.WriteLine($"[PsicologoApiService]    IdEmpleado: {psicologo.IdEmpleado}");
                            System.Diagnostics.Debug.WriteLine($"[PsicologoApiService]    NombreCompleto: {psicologo?.NombreCompleto}");
                            System.Diagnostics.Debug.WriteLine($"[PsicologoApiService]    TituloProfesional: {psicologo?.TituloProfesional}");
                            System.Diagnostics.Debug.WriteLine($"[PsicologoApiService]    Email: {psicologo?.EmailPersonal}");
                            return psicologo;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] ?? Deserialización devolvió NULL");
                            return null;
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] ? ERROR de deserialización JSON: {jsonEx.Message}");
                        System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response Body completo: {responseContent}");
                        return null;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] ? Error HTTP {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response Body: {responseContent}");
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] ?? Empleado/Psicólogo ID {idEmpleado} NO EXISTE en la base de datos");
                    }
                    
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] ? Exception en GetPsicologoByIdAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Crea un nuevo empleado
        /// POST /api/v1/empleados/
        /// </summary>
        public async Task<PsicologoModel> CreateEmpleadoAsync(CreateEmpleadoModel empleado)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[PsicologoApiService] Token no disponible");
                    return null;
                }

                var json = JsonConvert.SerializeObject(empleado);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/empleados/");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = content;

                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] POST {_httpClient.BaseAddress}api/v1/empleados/");
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Request Body: {json}");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response: {responseJson}");
                    
                    var nuevoEmpleado = JsonConvert.DeserializeObject<PsicologoModel>(responseJson);
                    
                    if (nuevoEmpleado != null)
                    {
                        nuevoEmpleado.IdPsicologo = nuevoEmpleado.IdEmpleado;
                        System.Diagnostics.Debug.WriteLine($"? Empleado creado: {nuevoEmpleado.NombreCompleto} (ID: {nuevoEmpleado.IdEmpleado})");
                        return nuevoEmpleado;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? Error al crear empleado: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Exception en CreateEmpleadoAsync: {ex.Message}");
            }
            
            return null;
        }

        /// <summary>
        /// Actualiza un empleado existente
        /// PUT /api/v1/empleados/{empleado_id}
        /// </summary>
        public async Task<PsicologoModel> UpdateEmpleadoAsync(int idEmpleado, CreateEmpleadoModel empleado)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[PsicologoApiService] Token no disponible");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] >>> Actualizando empleado ID: {idEmpleado}");

                var json = JsonConvert.SerializeObject(empleado);
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Request Body: {json}");

                var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/empleados/{idEmpleado}");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] PUT {_httpClient.BaseAddress}api/v1/empleados/{idEmpleado}");

                var response = await _httpClient.SendAsync(request);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Response Body: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var empleadoActualizado = JsonConvert.DeserializeObject<PsicologoModel>(responseContent);
                    

                    if (empleadoActualizado != null)
                    {
                        empleadoActualizado.IdPsicologo = empleadoActualizado.IdEmpleado;
                        System.Diagnostics.Debug.WriteLine($"? Empleado actualizado exitosamente:");
                        System.Diagnostics.Debug.WriteLine($"   ID: {empleadoActualizado.IdEmpleado}");
                        System.Diagnostics.Debug.WriteLine($"   Nombre: {empleadoActualizado.NombreCompleto}");
                        System.Diagnostics.Debug.WriteLine($"   Rol: {empleadoActualizado.RolEmpleado}");
                        System.Diagnostics.Debug.WriteLine($"   Estado: {empleadoActualizado.Estado}");
                        return empleadoActualizado;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"? Error al actualizar empleado: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"   Detalle: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Exception en UpdateEmpleadoAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
            
            return null;
        }

        /// <summary>
        /// Elimina un empleado
        /// DELETE /api/v1/empleados/{empleado_id}
        /// </summary>
        public async Task<bool> DeleteEmpleadoAsync(int idEmpleado)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[PsicologoApiService] Token no disponible para DELETE");
                    return false;
                }

                var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/empleados/{idEmpleado}");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] DELETE {_httpClient.BaseAddress}api/v1/empleados/{idEmpleado}");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] DELETE Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"? [PsicologoApiService] Empleado ID {idEmpleado} eliminado exitosamente");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? [PsicologoApiService] Error al eliminar empleado ID {idEmpleado}: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? [PsicologoApiService] Exception en DeleteEmpleadoAsync({idEmpleado}): {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene solo psicólogos activos (útil para ComboBox en UI)
        /// </summary>
        public async Task<IEnumerable<PsicologoModel>> GetPsicologosActivosAsync()
        {
            try
            {
                var todosLosPsicologos = await GetAllPsicologosAsync();
                var psicologosActivos = new List<PsicologoModel>();
                
                foreach (var psicologo in todosLosPsicologos)
                {
                    if (psicologo.Estado == "activo")
                    {
                        psicologo.IdPsicologo = psicologo.IdEmpleado;
                        psicologosActivos.Add(psicologo);
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Empleados activos: {psicologosActivos.Count}");
                
                return psicologosActivos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PsicologoApiService] Exception en GetPsicologosActivosAsync: {ex.Message}");
                return new List<PsicologoModel>();
            }
        }
    }
}
