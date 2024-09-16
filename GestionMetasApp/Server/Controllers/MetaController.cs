using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionMetasApp.Shared.Models;
using GestionMetasApp.Server.Context;
using GestionMetasApp.Server.Models;

namespace GestionMetasApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MetaController : ControllerBase
{
    public readonly GestionMetasAppContext _dbContext;

    public MetaController(GestionMetasAppContext _context)
    {
        _dbContext = _context;
    }

    [HttpGet]
    [Route("Lista")]
    public IActionResult Lista()
    {
        List<MetaView> lista = new List<MetaView>();
        try
        {

            lista = _dbContext.Metas.Select(meta => new MetaView
            {
                IdMeta = meta.IdMeta,
                Nombre = meta.Nombre,
                Fechacreacion = meta.FechaCreacion,
                TotalTareas = _dbContext.Tareas.Count(t => t.IdMeta == meta.IdMeta),
                TareasCompletadas = _dbContext.Tareas.Count(t => t.IdMeta == meta.IdMeta && t.Estado == "Completada"),
                Porcentaje = _dbContext.Tareas
                    .Where(t => t.IdMeta == meta.IdMeta)
                    .GroupBy(t => t.IdMeta)
                    .Select(g => new
                    {
                        Total = (decimal)g.Count(),
                        Completadas = g.Count(t => t.Estado == "Completada")
                    })
                    .AsEnumerable() // Realizar la operación en memoria
                    .Select(g => g.Total > 0 ? (g.Completadas / g.Total) * 100 : 0) // Calcular porcentaje
                    .FirstOrDefault()
            }).ToList();

            return StatusCode(StatusCodes.Status200OK, lista);

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    [HttpGet]
    [Route("Obtener/{idMeta:int}")]
    public IActionResult ObtenerPorId(int idMeta)
    {
        MetaMod meta = new MetaMod();

        meta = _dbContext.Metas.Find(idMeta);

        if (meta == null)
        {
            throw new Exception("No existe la meta.");
        }

        try
        {
            meta = _dbContext.Metas.Where(x => x.IdMeta == idMeta).FirstOrDefault();

            return StatusCode(StatusCodes.Status200OK, meta);

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    [HttpPost]
    [Route("Guardar")]
    public IActionResult Guardar([FromBody] MetaMod meta)
    {
        try
        {

            // Verificar si la meta con el mismo nombre ya existe
            if (_dbContext.Metas.Any(m => m.Nombre == meta.Nombre && m.IdMeta != meta.IdMeta))
            {
                throw new Exception("Ya existe una meta con ese nombre.");
            }

            if (meta.IdMeta == 0)
            {

                meta.FechaCreacion = DateTime.Now;

                _dbContext.Metas.Add(meta);
                _dbContext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, "Meta agregada exitosamente.");
            }
            else
            {
                MetaMod _meta = _dbContext.Metas.Find(meta.IdMeta);

                if (_meta == null)
                {
                    throw new Exception("No existe la meta.");
                }

                _meta.Nombre = meta.Nombre;

                _dbContext.Update(_meta);
                _dbContext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, "Meta modificada exitosamente.");

            }

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    [HttpDelete]
    [Route("Eliminar/{idMeta:int}")]
    public IActionResult Eliminar(int idMeta)
    {
        try
        {

            MetaMod _meta = _dbContext.Metas.Find(idMeta);

            if (_meta == null)
            {
                throw new Exception("La meta no existe.");
            }

            var meta = _dbContext.Metas
            .Include(m => m.Tareas)
            .FirstOrDefault(m => m.IdMeta == idMeta);

            // Elimina todas las tareas asociadas a la meta
            _dbContext.Tareas.RemoveRange(meta.Tareas);

            _dbContext.Metas.Remove(_meta);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status200OK, "La meta ha sido eliminada.");

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

}
