using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GestionMetasApp.Server.Models;

public partial class TareaMod
{
    public int IdTarea { get; set; }

    [Required(ErrorMessage = "El Nombre es requerido.")]
    [MaxLength(80, ErrorMessage = "El nombre no puede exceder los 80 caracteres.")]
    public string? Nombre { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public bool Importante { get; set; }

    public int? IdMeta { get; set; }

    [JsonIgnore]
    public virtual MetaMod? meta{ get; set; }
}
