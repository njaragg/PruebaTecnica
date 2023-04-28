using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace tarea.Models;

public partial class Region
{
    public short Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public string NombreOficial { get; set; } = null!;

    public int CodigoLibroClaseElectronico { get; set; }
    [JsonIgnore]
    public virtual ICollection<Ciudad> Ciudads { get; } = new List<Ciudad>();
}
