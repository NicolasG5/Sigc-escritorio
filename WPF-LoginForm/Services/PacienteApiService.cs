using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_LoginForm.Models;

namespace WPF_LoginForm.Services
{
    /// <summary>
    /// Servicio completo para CRUD de Pacientes
    /// </summary>
    public class PacienteApiService : ApiServiceBase
    {
        private const string BASE_ENDPOINT = "/api/v1/pacientes";

        /// <summary>
        /// GET /api/v1/pacientes/?skip=0&limit=1000
        /// Obtiene lista completa de pacientes
        /// </summary>
        public async Task<IEnumerable<PacienteModel>> GetAllPacientesAsync(int skip = 0, int limit = 1000)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GET {BASE_ENDPOINT}/ ===");
                System.Diagnostics.Debug.WriteLine($"Parámetros: skip={skip}, limit={limit}");

                var url = $"{BASE_ENDPOINT}/?skip={skip}&limit={limit}";
                var response = await _httpClient.GetAsync(url);

                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Respuesta JSON: {json.Substring(0, Math.Min(200, json.Length))}...");

                    var pacientesResponse = JsonConvert.DeserializeObject<PacientesResponse>(json);
                    
                    if (pacientesResponse?.Data != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"? Pacientes obtenidos: {pacientesResponse.Count}");
                        return pacientesResponse.Data;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("?? Respuesta sin datos");
                        return Enumerable.Empty<PacienteModel>();
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? Error: {error}");
                    throw new Exception($"Error al obtener pacientes: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Excepción: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// GET /api/v1/pacientes/{id}
        /// Obtiene un paciente por ID
        /// </summary>
        public async Task<PacienteModel> GetPacienteByIdAsync(int id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GET {BASE_ENDPOINT}/{id} ===");

                var url = $"{BASE_ENDPOINT}/{id}";
                var response = await _httpClient.GetAsync(url);

                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Respuesta JSON: {json}");

                    // CORRECCIÓN: La API devuelve el paciente DIRECTAMENTE, NO envuelto en {data: [], count: 1}
                    // Intentar deserializar directamente como PacienteModel
                    try
                    {
                        var paciente = JsonConvert.DeserializeObject<PacienteModel>(json);
                        
                        if (paciente != null && paciente.IdPaciente > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"? Paciente obtenido: {paciente.NombreCompleto} (ID: {paciente.IdPaciente})");
                            return paciente;
                        }
                    }
                    catch (Exception exDirecto)
                    {
                        System.Diagnostics.Debug.WriteLine($"?? No se pudo deserializar como PacienteModel directo: {exDirecto.Message}");
                        
                        // Plan B: Intentar como PacientesResponse (formato con data/count)
                        try
                        {
                            var pacientesResponse = JsonConvert.DeserializeObject<PacientesResponse>(json);
                            
                            if (pacientesResponse?.Data != null && pacientesResponse.Data.Length > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"? Paciente obtenido (formato array): {pacientesResponse.Data[0].NombreCompleto}");
                                return pacientesResponse.Data[0];
                            }
                        }
                        catch (Exception exArray)
                        {
                            System.Diagnostics.Debug.WriteLine($"?? Tampoco se pudo deserializar como array: {exArray.Message}");
                        }
                    }
                    
                    // Si llegamos aquí, no se pudo deserializar de ninguna forma
                    throw new Exception($"No se pudo deserializar la respuesta del paciente. JSON: {json.Substring(0, Math.Min(200, json.Length))}");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    System.Diagnostics.Debug.WriteLine($"? Paciente ID {id} no encontrado (404)");
                    throw new Exception($"Paciente con ID {id} no encontrado");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? Error: {error}");
                    throw new Exception($"Error al obtener paciente: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Excepción: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// POST /api/v1/pacientes/
        /// Crea un nuevo paciente
        /// </summary>
        public async Task<PacienteModel> CreatePacienteAsync(PacienteUpdateModel paciente)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== POST {BASE_ENDPOINT}/ ===");

                var json = JsonConvert.SerializeObject(paciente, Formatting.Indented);
                System.Diagnostics.Debug.WriteLine($"Datos a enviar:\n{json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(BASE_ENDPOINT + "/", content);

                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Respuesta: {responseJson}");

                    // La API podría devolver el paciente creado o {data: [...], count: 1}
                    try
                    {
                        var pacientesResponse = JsonConvert.DeserializeObject<PacientesResponse>(responseJson);
                        if (pacientesResponse?.Data != null && pacientesResponse.Data.Length > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"? Paciente creado: ID {pacientesResponse.Data[0].IdPaciente}");
                            return pacientesResponse.Data[0];
                        }
                    }
                    catch
                    {
                        // Si no es PacientesResponse, intentar deserializar directamente
                        var pacienteCreado = JsonConvert.DeserializeObject<PacienteModel>(responseJson);
                        System.Diagnostics.Debug.WriteLine($"? Paciente creado: ID {pacienteCreado.IdPaciente}");
                        return pacienteCreado;
                    }

                    throw new Exception("Respuesta inesperada del servidor");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? Error: {error}");
                    throw new Exception($"Error al crear paciente: {response.StatusCode}\n{error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Excepción: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// PUT /api/v1/pacientes/{id}
        /// Actualiza un paciente existente
        /// </summary>
        public async Task<bool> UpdatePacienteAsync(int id, PacienteUpdateModel paciente)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== PUT {BASE_ENDPOINT}/{id} ===");

                var json = JsonConvert.SerializeObject(paciente, Formatting.Indented);
                System.Diagnostics.Debug.WriteLine($"Datos a enviar:\n{json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{BASE_ENDPOINT}/{id}", content);

                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Respuesta: {responseJson}");
                    System.Diagnostics.Debug.WriteLine($"? Paciente actualizado exitosamente");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? Error: {error}");
                    throw new Exception($"Error al actualizar paciente: {response.StatusCode}\n{error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Excepción: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DELETE /api/v1/pacientes/{id}
        /// Elimina un paciente
        /// </summary>
        public async Task<bool> DeletePacienteAsync(int id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== DELETE {BASE_ENDPOINT}/{id} ===");

                var response = await _httpClient.DeleteAsync($"{BASE_ENDPOINT}/{id}");

                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Respuesta: {responseJson}");
                    System.Diagnostics.Debug.WriteLine($"? Paciente eliminado exitosamente");
                    return true;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? Error: {error}");
                    throw new Exception($"Error al eliminar paciente: {response.StatusCode}\n{error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Excepción: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// GET /api/v1/pacientes/?skip=0&limit=1000&estado=activo
        /// Obtiene solo pacientes activos (para compatibilidad con CrearCita)
        /// </summary>
        public async Task<IEnumerable<PacienteModel>> GetPacientesActivosAsync()
        {
            try
            {
                var todosPacientes = await GetAllPacientesAsync();
                return todosPacientes.Where(p => p.Estado?.ToLower() == "activo");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error al obtener pacientes activos: {ex.Message}");
                throw;
            }
        }
    }
}
