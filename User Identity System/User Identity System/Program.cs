using Microsoft.Data.Sqlite;

// Inicializa el ayudante técnico de SQLite para evitar errores de comunicación
SQLitePCL.Batteries.Init();

// Nombre del archivo local para la base de datos de este proyecto
string datosConexion = "Data Source=SistemaUsuarios.db";

Console.WriteLine("=== PROYECTO 02: SISTEMA DE LOGIN ===");

try
{
    // 1. Conectamos con el motor de la base de datos
    using var conexion = new SqliteConnection(datosConexion);
    conexion.Open();

    // 2. DISEÑO SQL: Creamos la tabla de usuarios si no existe
    string scriptTabla = @"
        CREATE TABLE IF NOT EXISTS Usuarios (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL
        );";

    using var comandoTabla = new SqliteCommand(scriptTabla, conexion);
    comandoTabla.ExecuteNonQuery();

    // 3. MENÚ DE INTERFACCIÓN
    Console.WriteLine("\n1. Registrarse");
    Console.WriteLine("2. Iniciar Sesión");
    Console.Write("\nSelecciona una opción (1 o 2): ");

    // .Trim() borra automáticamente cualquier espacio accidental del teclado
    string opcion = Console.ReadLine().Trim();

    // ==========================================
    // SECCIÓN 1: REGISTRO DE USUARIOS
    // ==========================================
    if (opcion == "1")
    {
        Console.WriteLine("\n--- REGISTRO DE NUEVO USUARIO ---");
        Console.Write("Elige tu nombre de usuario: ");
        string nuevoUsuario = Console.ReadLine().Trim();

        Console.Write("Elige tu contraseña: ");
        string nuevaContrasena = Console.ReadLine().Trim();

        // SQL: Comando estructurado para insertar de forma segura
        string scriptInsertar = "INSERT INTO Usuarios (Username, Password) VALUES (@user, @pass);";
        using var comandoInsertar = new SqliteCommand(scriptInsertar, conexion);

        comandoInsertar.Parameters.AddWithValue("@user", nuevoUsuario);
        comandoInsertar.Parameters.AddWithValue("@pass", nuevaContrasena);

        comandoInsertar.ExecuteNonQuery();
        Console.WriteLine($"\n¡Éxito! El usuario '{nuevoUsuario}' ha sido registrado.");
    }
    // ==========================================
    // SECCIÓN 2: VALIDACIÓN DE INICIO DE SESIÓN
    // ==========================================
    else if (opcion == "2")
    {
        Console.WriteLine("\n--- INICIO DE SESIÓN ---");
        Console.Write("Introduce tu usuario: ");
        string loginUsuario = Console.ReadLine().Trim();

        Console.Write("Introduce tu contraseña: ");
        string loginContrasena = Console.ReadLine().Trim();

        // SQL: Cuenta cuántas filas coinciden exactamente con el usuario Y la contraseña ingresados
        string scriptBuscar = "SELECT COUNT(*) FROM Usuarios WHERE Username = @user AND Password = @pass;";
        using var comandoBuscar = new SqliteCommand(scriptBuscar, conexion);

        comandoBuscar.Parameters.AddWithValue("@user", loginUsuario);
        comandoBuscar.Parameters.AddWithValue("@pass", loginContrasena);

        long usuariosEncontrados = (long)comandoBuscar.ExecuteScalar();

        // Si el conteo es mayor a 0, significa que las credenciales existen y son correctas
        if (usuariosEncontrados > 0)
        {
            Console.WriteLine("\n¡BIENVENIDO! Has iniciado sesión correctamente. 🎉");
        }
        else
        {
            Console.WriteLine("\n❌ Acceso denegado. El usuario o la contraseña son incorrectos.");
        }
    }
    else
    {
        Console.WriteLine("\nOpción no válida.");
    }
}
catch (Exception error)
{
    Console.WriteLine($"Hubo un error inesperado: {error.Message}");
}

// Congelar la consola para revisar los resultados tranquilamente
Console.WriteLine("\nPresiona cualquier tecla para salir...");
Console.ReadKey();
