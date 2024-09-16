using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace GestionMetasApp.Server.Models;

public partial class MetaMod
{
    public int IdMeta { get; set; }

    [Required(ErrorMessage = "El Nombre es requerido.")]
    [MaxLength(80, ErrorMessage = "El nombre no puede exceder los 80 caracteres.")]
    public string? Nombre { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public int? TotalTareas { get; set; }

    public decimal? Porcentaje { get; set; }

    public virtual ICollection<TareaMod> Tareas { get; set; } = new List<TareaMod>();
}
