# Backend de Portafolio Profesional con ASP.NET Core

## Introducción

Este proyecto es el motor backend de un portafolio profesional, concebido para ofrecer una solución completa y personalizable para que desarrolladores, diseñadores y otros profesionales puedan gestionar el contenido de su sitio web de manera centralizada. La motivación principal fue crear un sistema CMS (Content Management System) a medida que no solo permitiera administrar artículos de blog o proyectos, sino que también ofreciera un control granular sobre la estructura y diseño de la página principal, algo que las soluciones genéricas no suelen proveir.

El resultado es una potente API RESTful que gestiona usuarios, posts, categorías, biografías, redes sociales y, lo más importante, un sistema de layouts dinámicos.

## Stack Tecnológico

*   **Lenguaje:** C# 10
*   **Framework:** ASP.NET Core 6
*   **Arquitectura:** N-Tier (Capas), Patrón Repositorio, Inyección de Dependencias.
*   **Base de Datos:** Microsoft SQL Server
*   **Mapeo Objeto-Relacional (ORM):** Dapper (implícito en los repositorios, por la naturaleza de las consultas).
*   **Contenedores:** Docker y Docker Compose para un entorno de desarrollo y despliegue consistente.
*   **Autenticación:** ASP.NET Core Identity (basada en cookies).

## Flujo de Lógica y Arquitectura

El sistema sigue una arquitectura de N-Capas bien definida que garantiza la separación de responsabilidades, la modularidad y la facilidad de mantenimiento.

1.  **Controladores (Controllers):** Actúan como el punto de entrada de las solicitudes HTTP. Validann los datos de entrada y orquestan la respuesta. No contienen lógica de negocio; su única responsabilidad es comunicar el front-end con la lógica de la aplicación.
2.  **Servicios (Services):** Aquí reside toda la lógica de negocio. Los servicios son invocados por los controladores y coordinan el trabajo necesario para cumplir con una solicitud. Por ejemplo, el `LayoutsService` contiene la lógica para reordenar las secciones de la página de un usuario.
3.  **Repositorios (Repositories):** Esta capa abstrae el acceso a los datos. Los servicios utilizan los repositorios para consultar o modificar información en la base de datos (SQL Server) sin necesidad de conocer los detalles de la implementación del acceso a datos.
4.  **Entidades y Modelos (Entities & Models):** Las `Entities` representan los objetos de la base de datos. Se utilizan `ViewModels` y AutoMapper para exponer a los clientes solo los datos que necesitan, evitando así la sobreexposición de la estructura interna y facilitando la validación.

## Implementaciones Destacadas

### 1. Gestión Dinámica de Layouts de Usuario

Más allá de un CMS tradicional, una de las características más potentes de este backend es la capacidad de permitir a los usuarios construir dinámicamente la estructura de su página principal.

*   **Diseño:** A través de la entidad `UserHomeLayout` y el servicio `LayoutsService`, el sistema permite a los usuarios agregar, eliminar y, lo más importante, reordenar diferentes tipos de bloques de contenido (Biografía, Redes Sociales, secciones de posts personalizados, etc.). El campo `DisplayOrder` es clave para esta funcionalidad, y el backend recalcula el orden de todos los elementos de forma atómica cuando uno es eliminado o movido, manteniendo la integridad de la interfaz.
*   **Decisión Técnica:** Esta implementación dota al portafolio de una flexibilidad inmensa, convirtiéndolo en un producto mucho más personalizable y potente que un simple blog. Demuestra la capacidad de diseñar e implementar lógica de negocio compleja más allá de las operaciones CRUD estándar.

### 2. Arquitectura Robusta con Patrón Repositorio y Servicios

El proyecto es un excelente ejemplo de una arquitectura de software limpia y escalable.

*   **Diseño:** La estricta separación entre Controladores (orquestación de HTTP), Servicios (lógica de negocio) y Repositorios (acceso a datos) hace que el código sea fácil de entender, depurar y extender. El uso intensivo de la Inyección de Dependencias (registrado en `Program.cs`) permite que los componentes estén débilmente acoplados, lo que facilita las pruebas unitarias y la sustitución de implementaciones.
*   **Decisión Técnica:** Adoptar esta arquitectura desde el principio, aunque requiere una configuración inicial más elaborada, es una decisión profesional que garantiza la mantenibilidad y escalabilidad del proyecto a largo plazo.

## Instalación y Uso

El proyecto está completamente contenedorizado, lo que simplifica enormemente su puesta en marcha.

1.  **Prerrequisitos:**
    *   Tener instalado [Docker](https://www.docker.com/get-started) y Docker Compose.

2.  **Clonar el repositorio:**
    ```bash
    git clone <URL-DEL-REPOSITORIO>
    cd portfolio_back
    ```

3.  **Configuración del Entorno:**
    *   Renombrá el archivo `.env.development.example` a `.env.development` (o crealo si no existe) y completá las variables de entorno, especialmente `SA_PASSWORD` para la base de datos.

4.  **Ejecutar el proyecto:**
    *   Para una mayor fiabilidad y para asegurar que la base de datos se inicie y esté lista antes que la aplicación, se recomienda levantar los servicios en dos pasos.

    *   **Paso 1: Iniciar la base de datos y esperar a que esté completamente operativa.**
        ```bash
        docker-compose --env-file .env.development up -d sqlserver_db
        ```
        *Espera unos 60 segundos para que SQL Server se inicie por primera vez.*

    *   **Paso 2: Iniciar el backend.**
        ```bash
        docker-compose --env-file .env.development up -d --build backend_portafolio
        ```
        *El flag `--build` asegura que la imagen de Docker para el backend se construya con los últimos cambios en el código.*

5.  **Acceso:**
    *   La API estará disponible en `http://localhost:8080`.
    *   La base de datos estará accesible en `localhost:1433`.

