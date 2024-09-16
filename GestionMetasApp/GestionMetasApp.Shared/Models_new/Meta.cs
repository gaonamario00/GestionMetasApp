using System;
using System.Collections.Generic;

namespace GestionMetasApp.Server;

public partial class Meta
{
    public int IdMeta { get; set; }

    public string? Nombre { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public int? TotalTareas { get; set; }

    public decimal? Porcentaje { get; set; }

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
