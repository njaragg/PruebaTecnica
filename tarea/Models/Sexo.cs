using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace tarea.Models;

public partial class Sexo
{
    public short Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public string Letra { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Persona> Personas { get; } = new List<Persona>();
}
