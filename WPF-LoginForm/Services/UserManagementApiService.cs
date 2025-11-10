using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.Services
{
    public class UserManagementApiService
    {
        private readonly HttpClient _httpClient;

        public UserManagementApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        /// <summary>
        /// Obtiene todos los usuarios del sistema
        /// GET /api/v1/users/
        /// </summary>
        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[UserManagementApiService] Token no disponible");
                    return new List<UserModel>();
                }

                var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/users/?skip=0&limit=1000");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] GET {_httpClient.BaseAddress}api/v1/users/");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Response JSON: {json.Substring(0, Math.Min(200, json.Length))}...");
                    
                    // La API devuelve un objeto con propiedad "data" que contiene la lista
                    var apiResponse = JsonConvert.DeserializeObject<UsuariosPublicResponse>(json);
                    System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Usuarios deserializados: {apiResponse?.data?.Count ?? 0}");
                    
                    return apiResponse?.data ?? new List<UserModel>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Error: {response.StatusCode} - {errorContent}");
                }
                
                return new List<UserModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Exception en GetAllUsersAsync: {ex.Message}");
                return new List<UserModel>();
            }
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema (Signup)
        /// POST /api/v1/users/signup
        /// Este endpoint NO requiere autenticación
        /// RESPUESTA: Devuelve UserPublic (sin contraseña)
        /// </summary>
        public async Task<UserModel> SignupUserAsync(CreateUserModel newUser)
        {
            try
            {
                var json = JsonConvert.SerializeObject(newUser, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                System.Diagnostics.Debug.WriteLine("===========================================");
                System.Diagnostics.Debug.WriteLine("[UserManagementApiService] === SIGNUP REQUEST ===");
                System.Diagnostics.Debug.WriteLine($"URL: {_httpClient.BaseAddress}api/v1/users/signup");
                System.Diagnostics.Debug.WriteLine($"JSON enviado:");
                System.Diagnostics.Debug.WriteLine(json);
                System.Diagnostics.Debug.WriteLine("===========================================");

                var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/users/signup");
                request.Headers.Add("accept", "application/json");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Response Body:");
                System.Diagnostics.Debug.WriteLine(responseContent);
                System.Diagnostics.Debug.WriteLine("===========================================");

                if (response.IsSuccessStatusCode)
                {
                    // La API devuelve un objeto UserPublic que NO tiene todos los campos
                    // Vamos a deserializar flexiblemente
                    try
                    {
                        var usuario = JsonConvert.DeserializeObject<UserModel>(responseContent, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        });

                        if (usuario != null)
                        {
                            // Si no tiene Username, intentar obtenerlo del request
                            if (string.IsNullOrEmpty(usuario.Username))
                            {
                                usuario.Username = newUser.NombreUsuario;
                            }

                            // Si no tiene Email, intentar obtenerlo del request
                            if (string.IsNullOrEmpty(usuario.Email))
                            {
                                usuario.Email = newUser.Email;
                            }

                            // Si no tiene Estado, usar el del request
                            if (string.IsNullOrEmpty(usuario.Estado))
                            {
                                usuario.Estado = newUser.Estado;
                            }

                            System.Diagnostics.Debug.WriteLine($"? Usuario registrado exitosamente:");
                            System.Diagnostics.Debug.WriteLine($"   Username: {usuario.Username}");
                            System.Diagnostics.Debug.WriteLine($"   Email: {usuario.Email}");
                            System.Diagnostics.Debug.WriteLine($"   ID: {usuario.Id}");
                            System.Diagnostics.Debug.WriteLine($"   Estado: {usuario.Estado}");

                            return usuario;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("?? Deserialización retornó NULL pero status 200");
                            System.Diagnostics.Debug.WriteLine("Creando usuario manualmente con datos del request...");

                            // Si la deserialización falla, crear el usuario manualmente
                            // Intentar extraer el ID del response
                            dynamic responseObj = JsonConvert.DeserializeObject(responseContent);
                            string userId = responseObj?.id_usuario?.ToString() ?? 
                                          responseObj?.id?.ToString() ?? 
                                          "0";

                            return new UserModel
                            {
                                Id = userId,
                                Username = newUser.NombreUsuario,
                                Email = newUser.Email,
                                Estado = newUser.Estado,
                                Name = "",
                                LastName = ""
                            };
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"? Error JSON: {jsonEx.Message}");
                        System.Diagnostics.Debug.WriteLine("Intentando crear usuario con datos básicos...");

                        // Fallback: crear usuario con los datos del request
                        dynamic responseObj = JsonConvert.DeserializeObject(responseContent);
                        string userId = responseObj?.id_usuario?.ToString() ?? 
                                      responseObj?.id?.ToString() ?? 
                                      "0";

                        return new UserModel
                        {
                            Id = userId,
                            Username = newUser.NombreUsuario,
                            Email = newUser.Email,
                            Estado = newUser.Estado,
                            Name = "",
                            LastName = ""
                        };
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"? ERROR HTTP {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Response: {responseContent}");
                    
                    try
                    {
                        dynamic errorObj = JsonConvert.DeserializeObject(responseContent);
                        string errorDetail = errorObj?.detail?.ToString() ?? "Error desconocido";
                        System.Diagnostics.Debug.WriteLine($"Error Detail: {errorDetail}");
                    }
                    catch { }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? EXCEPTION en SignupUserAsync:");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                return null;
            }
        }

        /// <summary>
        /// Crea un nuevo usuario en el sistema (requiere autenticación de admin)
        /// POST /api/v1/users/
        /// </summary>
        public async Task<UserModel> CreateUserAsync(CreateUserModel newUser)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[UserManagementApiService] Token no disponible para crear usuario");
                    return null;
                }

                var json = JsonConvert.SerializeObject(newUser);
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] JSON a enviar: {json}");

                var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/users/");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] POST {_httpClient.BaseAddress}api/v1/users/");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Usuario creado: {responseJson}");
                    
                    var usuario = JsonConvert.DeserializeObject<UserModel>(responseJson);
                    return usuario;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Error al crear usuario: {response.StatusCode} - {errorContent}");
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Exception en CreateUserAsync: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Actualiza el usuario actual (me)
        /// PATCH /api/v1/users/me
        /// </summary>
        public async Task<bool> UpdateCurrentUserAsync(UpdateUserModel updatedUser)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[UserManagementApiService] Token no disponible para actualizar usuario");
                    return false;
                }

                var json = JsonConvert.SerializeObject(updatedUser);

                var request = new HttpRequestMessage(new HttpMethod("PATCH"), "api/v1/users/me");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] PATCH {_httpClient.BaseAddress}api/v1/users/me");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Usuario actualizado exitosamente");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Error al actualizar usuario: {response.StatusCode} - {errorContent}");
                }
                
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Exception en UpdateCurrentUserAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Elimina el usuario actual (me)
        /// DELETE /api/v1/users/me
        /// </summary>
        public async Task<bool> DeleteCurrentUserAsync()
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[UserManagementApiService] Token no disponible para eliminar usuario");
                    return false;
                }

                var request = new HttpRequestMessage(HttpMethod.Delete, "api/v1/users/me");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] DELETE {_httpClient.BaseAddress}api/v1/users/me");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Usuario eliminado exitosamente");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Error al eliminar usuario: {response.StatusCode} - {errorContent}");
                }
                
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Exception en DeleteCurrentUserAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por ID
        /// GET /api/v1/users/{id}
        /// </summary>
        public async Task<UserModel> GetUserByIdAsync(int idUsuario)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                    return null;

                var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/users/{idUsuario}");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<UserModel>(json);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UserManagementApiService] Exception en GetUserByIdAsync: {ex.Message}");
                return null;
            }
        }
    }
}
