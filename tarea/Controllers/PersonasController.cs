using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tarea.Models;

namespace tarea.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonasController : ControllerBase
    {
        private readonly PruebaTecnicaContext _context;
        private readonly Ciudad ciudad;
        private readonly Comuna comuna;

        public PersonasController(PruebaTecnicaContext context)
        {
            _context = context;
        }

        // GET: api/Personas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Persona>>> GetPersonas()
        {
          if (_context.Personas == null)
          {
              return NotFound();
          }
            return await _context.Personas
                .Include(p=>p.Sexo)             //El include agrega los objetos anidados al json, asi despues podemos mostrar esos datos
                .Include(p=>p.Comuna)
                .ThenInclude(Comuna=>Comuna.Ciudad)
                .ThenInclude(Ciudad=>Ciudad.Region)
                .ToListAsync();
        }

        // GET: api/Personas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Persona>> GetPersona(Guid id)
        {
          if (_context.Personas == null)
          {
              return NotFound();
          }
            var persona = await _context.Personas
                .Include(p => p.Sexo)
                .Include(p => p.Comuna)
                .ThenInclude(Comuna => Comuna.Ciudad)
                .ThenInclude(Ciudad => Ciudad.Region)
                .FirstOrDefaultAsync(i => i.Id == id);
            ;

            if (persona == null)
            {
                return NotFound();
            }

            return persona;
        }

        // PUT: api/Personas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersona(Guid id, Persona persona)
        {
            if (id != persona.Id)
            {
                return BadRequest();
            }

            _context.Entry(persona).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //Se cambio por el return NoContent(); para visualizar algo en postman
            return Ok(new { message = "La persona se actualizó correctamente." });
        }

        // POST: api/Personas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Persona>> PostPersona(Persona persona)
        {
            /*  EJEMPLO PETICION POST PARA REGISTRAR NUEVA PERSONA
            {
            "run": "14.004.934-4",
            "runCuerpo": 14004934,
            "runDigito": "4",
            "nombre": "GARRIDO ARÁNGUIZ, GREGORY EVANS",
            "nombres": "GREGORY EVANS",
            "apellidoPaterno": "GARRIDO",
            "apellidoMaterno": "ARÁNGUIZ",
            "email": null,
            "sexoCodigo": 1,
            "fechaNacimiento": "1977-04-11T00:00:00",
            "regionCodigo": 4,
            "ciudadCodigo": 1,
            "comunaCodigo": 4,
            "direccion": "EL MOLLE S/N",
            "telefono": 967775851,
            "observaciones": "",
            "sexo": {
                    "nombre": "",
                "letra": ""
            }
            }*/


            if (_context.Personas == null)
            {
                return Problem("Entity set 'PruebaTecnicaContext.Personas' is null.");
            }

            // Buscamos el objeto sexo, segun el atributo SexoCodigo que viene en el json, esto dado que si intentamos agregarlo directamnete,
            // intenta insertar el objeto sexo a la tabla sexo dando como error que ya existe (primare key duplicated)
            //  (probalemente haya que hacer lo mismo con los demas objetos que pueden venir anidados en el json)

            // En la base de datos todos tienen region, ciudad y comuna, asi que se consideran datos obligatorios a la hora de registrar
            // Por tanto no se considera la posibilidad de registrar cuando vienen sin codigo de region o de ciudad
            if (persona.RegionCodigo != null && persona.CiudadCodigo != null && persona.ComunaCodigo != null )
            {


                persona.Comuna = await _context.Comunas.FindAsync(persona.RegionCodigo, persona.CiudadCodigo, persona.ComunaCodigo);
                // hay que darle mas de un dato en el findasync porque en el context y en la base de datos se definen con mas una primary keys
                persona.Comuna.Ciudad = await _context.Ciudads.FindAsync(persona.RegionCodigo, persona.CiudadCodigo);
                persona.Comuna.Ciudad.Region = await _context.Regions.FindAsync(persona.RegionCodigo);
            }



            persona.Sexo = await _context.Sexos.FindAsync(persona.SexoCodigo);

            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersona", new { id = persona.Id }, persona);
        }

        // DELETE: api/Personas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersona(Guid id)
        {
            if (_context.Personas == null)
            {
                return NotFound();
            }
            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
            {
                return NotFound();
            }

            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonaExists(Guid id)
        {
            return (_context.Personas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
