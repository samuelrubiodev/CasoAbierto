using System.Collections.Generic;

public class ErrorsMessage  
{
    public Conexion conexion { get; set; } 
    public Audio audio { get; set; } 
    public Ia ia { get; set; } 
    public BaseDatos baseDatos { get; set; } 
    public Rendimiento rendimiento { get; set; } 
    public Generico generico { get; set; } 
}

public class Conexion {
    public IList<string> titulos { get; set; }
    public IList<string> mensajes { get; set; }

}
public class Audio {
    public IList<string> titulos { get; set; }
    public IList<string> mensajes { get; set; }

}
public class Ia {
    public IList<string> titulos { get; set; }
    public IList<string> mensajes { get; set; }

}
public class BaseDatos {
    public IList<string> titulos { get; set; }
    public IList<string> mensajes { get; set; }

}
public class Rendimiento {
    public IList<string> titulos { get; set; }
    public IList<string> mensajes { get; set; }
}
public class Generico {
    public IList<string> titulos { get; set; }
    public IList<string> mensajes { get; set; }
}