using GestionMetasApp.Server.Context;
using GestionMetasApp.Server.Models;
using GestionMetasApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionMetasApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TareaController : ControllerBase
{
    public readonly GestionMetasAppContext _dbContext;

    public TareaController(GestionMetasAppContext _context)
    {
        _dbContext = _context;
    }

    //[HttpGet]
    //[Route("Lista/{idMeta:int}")]
    //public IActionResult Lista(int idMeta)
    //{
    //    List<TareaView> lista = new List<TareaView>();
    //    try
    //    {

    //        lista = _dbContext.Tareas.Where(x => x.IdMeta == idMeta).Select(tarea => new TareaView
    //        {
    //            IdTarea = tarea.IdTarea,
    //            Nombre = tarea.Nombre,
    //            Estado = tarea.Estado,
    //            FechaCreacion = tarea.FechaCreacion,
    //            Importante = tarea.Importante,
    //            IdMeta = tarea.IdMeta
    //        }).ToList();

    //        return StatusCode(StatusCodes.Status200OK, lista);

    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
    //    }
    //}

    [HttpGet]
    [Route("Lista/{idMeta:int}")]
    public IActionResult Lista(int idMeta, [FromQuery] int pagina = 1, [FromQuery] int tamanoPagina = 5, [FromQuery] string Tarea = null, [FromQuery] string Fecha = null, [FromQuery] string Estado = null, 
                                [FromQuery] int OrdenTarea = 1, [FromQuery] int OrdenFecha = 1, [FromQuery] int OrdenEstado = 1, [FromQuery] string TipoFiltro = null)
    {
        List<TareaView> lista = new List<TareaView>();
        try
        {
            // Calcula el número de resultados a omitir
            var skip = (pagina - 1) * tamanoPagina;

            // Convertir la fecha de string a DateTime si no está vacía
            DateTime? fechaBusqueda = null;
            if (!string.IsNullOrEmpty(Fecha) && DateTime.TryParse(Fecha, out DateTime fechaParsed))
            {
                fechaBusqueda = fechaParsed.Date;
            }


            // Obtén la lista paginada de tareas
            var listaMod = _dbContext.Tareas
                .Where(x => x.IdMeta == idMeta)
                .Where(t => string.IsNullOrEmpty(Tarea) || t.Nombre.Contains(Tarea))
                .Where(t => !fechaBusqueda.HasValue || (t.FechaCreacion != null && t.FechaCreacion.Value.Date == fechaBusqueda.Value.Date))
                .Where(t => string.IsNullOrEmpty(Estado) || t.Estado == Estado)
                .OrderByDescending(t => t.Importante)
                .ThenBy(t => t.FechaCreacion)
                .ToList();

            if (!string.IsNullOrEmpty(TipoFiltro))
            {
                listaMod = TipoFiltro switch
                {
                    "Tarea" => OrdenTarea switch
                    {
                        2 => listaMod.OrderBy(x => x.Nombre).ToList(), 
                        3 => listaMod.OrderByDescending(x => x.Nombre).ToList(),
                        _ => listaMod.ToList()
                    },
                    "Fecha" => OrdenFecha switch
                    {
                        2 => listaMod.OrderBy(x => x.FechaCreacion).ToList(),
                        3 => listaMod.OrderByDescending(x => x.FechaCreacion).ToList(),
                        _ => listaMod.ToList()
                    },
                    "Estado" => OrdenEstado switch
                    {
                        2 => listaMod.OrderBy(x => x.Estado).ToList(),
                        3 => listaMod.OrderByDescending(x => x.Estado).ToList(),
                        _ => listaMod.ToList()
                    },
                    _ => listaMod.ToList() 
                };
            }

            lista = listaMod.Skip(skip)
                .Take(tamanoPagina)
                .Select(tarea => new TareaView
                {
                    IdTarea = tarea.IdTarea,
                    Nombre = tarea.Nombre,
                    Estado = tarea.Estado,
                    FechaCreacion = tarea.FechaCreacion,
                    Importante = tarea.Importante,
                    IdMeta = tarea.IdMeta,
                    CantRegistros = _dbContext.Tareas
                        .Where(x => x.IdMeta == idMeta)
                        .Where(t => string.IsNullOrEmpty(Tarea) || t.Nombre.Contains(Tarea))
                        .Where(t => !fechaBusqueda.HasValue || (t.FechaCreacion != null && t.FechaCreacion.Value.Date == fechaBusqueda.Value.Date))
                        .Where(t => string.IsNullOrEmpty(Estado) || t.Estado == Estado)
                        .Count()
                })
                .ToList();

            // Opcional: Regresa también el total de elementos y el tamaño de la página para que el cliente pueda gestionar la paginación
            return StatusCode(StatusCodes.Status200OK, lista);

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    [HttpGet]
    [Route("Obtener/{idTarea:int}")]
    public IActionResult ObtenerPorId(int idTarea)
    {
        TareaMod meta = new TareaMod();

        meta = _dbContext.Tareas.Find(idTarea);

        if (meta == null)
        {
            throw new Exception("No existe la tarea.");
        }

        try
        {
            meta = _dbContext.Tareas.Where(x => x.IdTarea == idTarea).FirstOrDefault();

            return StatusCode(StatusCodes.Status200OK, meta);

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }


    [HttpPost]
    [Route("Guardar")]
    public IActionResult Guardar([FromBody] TareaMod tarea)
    {
        try
        {

            // Verificar si la tarea con el mismo nombre ya existe
            if (_dbContext.Tareas.Any(m => m.Nombre == tarea.Nombre && m.IdTarea != tarea.IdTarea && m.IdMeta == tarea.IdMeta))
            {
                throw new Exception("Ya existe una tarea con ese nombre.");
            }

            if (tarea.IdTarea == 0)
            {

                tarea.FechaCreacion = DateTime.Now;
                tarea.Importante = false;
                tarea.Estado = "Abierta";

                _dbContext.Tareas.Add(tarea);
                _dbContext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, "Tarea agregada exitosamente.");
            }
            else
            {
                TareaMod _tarea = _dbContext.Tareas.Find(tarea.IdTarea);

                if (_tarea == null)
                {
                    throw new Exception("No existe la tarea.");
                }

                _tarea.Nombre = tarea.Nombre;

                _dbContext.Update(_tarea);
                _dbContext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, "Tarea modificada exitosamente.");

            }

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    [HttpPost]
    [Route("Importante")]
    public IActionResult ModificarImportante([FromBody] TareaMod tarea)
    {
        try
        {

            TareaMod _tarea = _dbContext.Tareas.Find(tarea.IdTarea);

            if (_tarea == null)
            {
                throw new Exception("No existe la tarea.");
            }

            _tarea.Importante = tarea.Importante;

            _dbContext.Update(_tarea);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status200OK, "Tarea modificada exitosamente.");

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    [HttpPost]
    [Route("CompletarTareas")]
    public IActionResult ModificarCompletarTareas([FromBody] List<TareaMod> tareas)
    {
        try
        {
            if (tareas == null || !tareas.Any())
            {
                throw new Exception("La lista de tareas está vacía.");
            }

            // Obtener los IDs de las tareas a actualizar
            var idsTareas = tareas.Select(t => t.IdTarea).ToList();

            // Obtener las tareas existentes en la base de datos
            var tareasExistentes = _dbContext.Tareas
                .Where(t => idsTareas.Contains(t.IdTarea))
                .ToList();

            // Verificar que todas las tareas existan
            if (tareasExistentes.Count != tareas.Count)
            {
                throw new Exception("Algunas tareas no existen.");
            }

            // Actualizar el estado de las tareas a "Completada"
            foreach (var tarea in tareasExistentes)
            {
                tarea.Estado = "Completada";
            }

            // Guardar los cambios en la base de datos
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status200OK, "Tareas modificadas exitosamente.");

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    [HttpDelete]
    [Route("Eliminar")]
    public IActionResult Eliminar([FromBody] List<TareaMod> tareas)
    {
        try
        {

            if (tareas == null || !tareas.Any())
            {
                throw new Exception("La lista de tareas está vacía.");
            }

            // Obtener las tareas existentes en la base de datos
            var idsTareas = tareas.Select(t => t.IdTarea).ToList();
            var tareasExistentes = _dbContext.Tareas
                .Where(t => idsTareas.Contains(t.IdTarea))
                .ToList();

            // Verificar que todas las tareas existan
            if (tareasExistentes.Count != tareas.Count)
            {
                throw new Exception("Algunas tareas no existen.");
            }

            _dbContext.Tareas.RemoveRange(tareasExistentes);
            _dbContext.SaveChanges();

            int countEliminadas = tareasExistentes.Count;
            string mensaje = $"Se han eliminado {countEliminadas} tarea(s) exitosamente.";

            return StatusCode(StatusCodes.Status200OK, mensaje);

        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

}
