using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionMetasApp.Shared.Models;

public class MetaView
{
    public int IdMeta { get; set; }

    public string? Nombre { get; set; }

    public DateTime? Fechacreacion { get; set; }
    public int? TotalTareas { get; set; }
    public int? TareasCompletadas{ get; set; }

    public decimal? Porcentaje { get; set; }
}

