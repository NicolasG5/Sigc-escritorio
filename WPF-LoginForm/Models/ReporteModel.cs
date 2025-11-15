using WPF_LoginForm.Models;
using WPF_LoginForm.Services;
using System.Collections.Generic;

namespace WPF_LoginForm.Models
{
    public class ReporteModel
    {
        public int Id { get; set; }
        public PacienteModel Paciente { get; set; }
        public TratamientoResponse Tratamiento { get; set; }
        public List<SeguimientoResponse> Seguimientos { get; set; }
        public List<MedicacionModel> Medicaciones { get; set; }
        public PsicologoModel Psicologo { get; set; }
        public string FechaReporte { get; set; }
        public string Estado { get; set; }
    }
}
