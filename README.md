<p align="center">
  <img width="970" height="280" alt="Logo EduConnect" src="https://github.com/user-attachments/assets/5c0861de-90c4-4e83-9aec-47a9f76a6961" />
</p>

<h1 align="center"><b>EDUCONNECT FRONT</b></h1>

<p align="center">
  <i>Interfaz web del sistema de tutorías académicas EduConnect, desarrollada con .NET Core MVC</i>
</p>

---

## <b>Descripción general</b>

**EduConnect_Front** constituye la capa de presentación del sistema de tutorías académicas **EduConnect**, permitiendo la interacción visual y funcional de los usuarios con los servicios del backend (**EduConnect_API**).  
Está desarrollado con **ASP.NET Core MVC**, empleando controladores, vistas y modelos para gestionar perfiles, tutorías, reportes y comunicación entre tutores, tutorados y coordinadores.

La aplicación se comunica con la API mediante **servicios HTTP** autenticados por **JWT** y controlados por políticas **CORS**.

---

## <b>Objetivos del proyecto</b>

- Facilitar la gestión de usuarios (tutores, tutorados, coordinadores y administradores).  
- Permitir la reserva, seguimiento y visualización de tutorías académicas.  
- Implementar un flujo de autenticación seguro mediante **JWT**.  
- Proporcionar una interfaz intuitiva, moderna y accesible para todos los perfiles.  
- Integrarse de forma segura con la API **EduConnect_API**.

---

## <b>Estructura del proyecto</b>
- Controllers: Controladores MVC para flujo de vistas y peticiones HTTP
- Models: Modelos de datos para intercambio con la API
- Views: Vistas Razor (.cshtml) organizadas por módulo (carpeta para cada rol)
- wwwroot: Archivos estáticos (CSS, JS, imágenes)
- appsettings.json: Configuración general (conexión a API, JWT, etc.)
- Program.cs: Punto de entrada de la aplicación
- EduConnect_Front.sln: Solución del proyecto
- EduConnect_Front.csproj: Archivo de proyecto

---

## <b>Tecnologías utilizadas</b>

| Componente | Tecnología |
|-------------|-------------|
| Lenguaje principal | C# |
| Framework | ASP.NET Core MVC (.NET 8) |
| Motor de vistas | Razor |
| Estilos y diseño | HTML5, CSS3, Bootstrap |
| Comunicación con backend | HTTP Client + JSON |
| Autenticación | JSON Web Tokens (JWT) |
| Control de acceso | Políticas CORS |
| Servidor | Kestrel / IIS Express |

---

## <b>Requisitos previos</b>

- [.NET SDK 8.0](https://dotnet.microsoft.com/download)  
- [Visual Studio 2022](https://visualstudio.microsoft.com/es/) o [VS Code](https://code.visualstudio.com/)  
- Navegador actualizado (Edge, Chrome o Firefox)  
- Proyecto backend **EduConnect_API** configurado y ejecutándose en `https://localhost:7003`

---

## <b>Configuración inicial</b>

<h3>1️Clonado del repositorio</h3>

```bash
git clone https://github.com/lauraJimena/EduConnect_Front.git
cd EduConnect_Front
```

<h3>2️Configuración del archivo <code>appsettings.json</code></h3>

En este archivo se define la URL base del backend y otros parámetros de conexión:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7003/api"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
<h3>Ejecución del proyecto</h3>

Desde Visual Studio:

- Seleccionar el perfil EduConnect_Front
- Ejecutar con F5 o Ctrl + F5

Desde la terminal:

```bash
dotnet restore
dotnet build
dotnet run
```
La aplicación quedará disponible en:
```arduino
https://localhost:7270
```
<b>Integración con EduConnect_API</b>

El frontend se comunica con el backend EduConnect_API consumiendo sus endpoints REST.
Los servicios se gestionan desde clases como GeneralService, TutorService, TutoradoService, entre otros.
Ejemplo de integración HTTP con autenticación JWT:

```csharp
_httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

var response = await _httpClient.PostAsJsonAsync("Tutor/SolicitudesTutorias", filtro, ct);
```
Si el token es válido → la API responde con los datos solicitados.
Si el token expira o es inválido → la API responde con 401 Unauthorized o Acceso Bloqueado.

<b>Diseño y experiencia de usuario</b>

- Interfaz intuitiva.
- Diseño adaptable a pantallas móviles y escritorio.
- Navegación por roles (administrador, tutor, tutorado, coordinador).
. Colores institucionales inspirados en la Universidad de Cundinamarca.
- Componentes dinámicos con validaciones y retroalimentación visual.

<b>Seguridad</b>

- Autenticación: Implementada mediante JWT Bearer Tokens emitidos por la API.
- Autorización por rol:
Cada usuario autenticado accede únicamente a las vistas y funcionalidades correspondientes a su rol. Se validan los roles almacenados en el token JWT antes de renderizar vistas o permitir el acceso a controladores.
- Protección de rutas:
El sistema redirige automáticamente a los usuarios no autorizados hacia una vista de “Acceso denegado”.
- Restricción de orígenes: Configuración CORS que únicamente permite el acceso desde el dominio oficial (https://localhost:7270).
- Validación de entrada: Se realiza sanitización y validación de datos en formularios y vistas.

<b>Autoría</b>
- Laura Jimena Herreño Rubiano
- Andrés Mateo Morales Gonzalez
- Juan Sebastián Moreno
- Edwin Felipe Garavito Izquierdo
<br>Estudiantes de Ingeniería de Sistemas – Universidad de Cundinamarca.
<br>Correo: 📧<a href="mailto:notificaciones.educonnect@gmail.com">notificaciones.educonnect@gmail.com
</a>
<b>Licencia</b>

Proyecto académico con fines educativos.
La reutilización o modificación del código requiere citar a la autora y el proyecto EduConnect.
