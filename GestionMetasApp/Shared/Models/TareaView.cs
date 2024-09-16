using System.ComponentModel.DataAnnotations;

namespace GestionMetasApp.Shared.Models;

public class TareaView
{
    public int IdTarea { get; set; }

    public string? Nombre { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool Importante { get; set; }

    public int? IdMeta { get; set; }

    public bool seleccionado { get; set; }

    public int CantRegistros { get; set; }

}
