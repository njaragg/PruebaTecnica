using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace tarea.Models;

public partial class Ciudad
{
    public short RegionCodigo { get; set; }

    public short Codigo { get; set; }

    public string Nombre { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Comuna> Comunas { get; } = new List<Comuna>();

    public virtual Region Region { get; set; } = null!;
}
