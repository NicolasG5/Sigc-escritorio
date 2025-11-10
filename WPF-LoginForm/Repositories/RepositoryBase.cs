using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace WPF_LoginForm.Repositories
{
    /// <summary>
    /// ⚠️ DEPRECADO: Este archivo usa conexión SQL directa
    /// 
    /// TODO: ELIMINAR cuando todas las vistas migren a API REST
    /// 
    /// Usar ApiServiceBase en su lugar para todas las nuevas implementaciones.
    /// Este archivo se mantiene temporalmente por compatibilidad con código legacy.
    /// 
    /// Fecha de deprecación: 3 de Enero de 2025
    /// Versión objetivo de eliminación: 2.0.0
    /// </summary>
    [Obsolete("Usar ApiServiceBase en lugar de RepositoryBase. Esta clase será eliminada en la versión 2.0.0")]
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;
        
        public RepositoryBase()
        {
            _connectionString = "Server=NICOLASGONZALEZ\\NICOLASGONZALEZ; Database=OKCASA7; Integrated Security=true";
        }
        
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
