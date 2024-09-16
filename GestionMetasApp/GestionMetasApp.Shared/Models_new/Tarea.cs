using System;
using System.Collections.Generic;

namespace GestionMetasApp.Server;

public partial class Tarea
{
    public int IdTarea { get; set; }

    public string? Nombre { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool? Importante { get; set; }

    public int? IdMeta { get; set; }

    public virtual Meta? IdMetaNavigation { get; set; }
}
