# api_coqon
Desarrollo de API para integración de camaras de videvigilinacia
Aquí tienes una explicación detallada de los endpoints de la API, incluyendo los parámetros necesarios y las respuestas esperadas:

# GET `/api/camera/stream/{cameraId}`
Descripción: Obtiene un stream de video en tiempo real de la cámara especificada.
- Parámetros:
  - `cameraId` (string): El identificador único de la cámara de la que se desea obtener el stream.
- Respuesta:
  - Código `200 OK`: Devuelve un stream de video en formato `video/MP2T`.
  - Código `404 Not Found`: Si la cámara con el `cameraId` especificado no existe.
  - Código `500 Internal Server Error`: Si hay un problema en la transmisión del video.

# 2.POST `/api/camera/start/{cameraId}`
Descripción: Inicia la transmisión de video de la cámara especificada.
- Parámetros:
  - `cameraId` (string): El identificador único de la cámara.
- Respuesta:
  - Código `200 OK`: Un objeto JSON confirmando el inicio de la transmisión (`{ "success": true }`).
  - Código `404 Not Found`: Si la cámara con el `cameraId` especificado no existe.
  - Código `400 Bad Request`: Si la transmisión ya está activa para la cámara.
  - Código `500 Internal Server Error`: Si hay un problema al iniciar la transmisión.

# 3. POST `/api/camera/stop/{cameraId}`
Descripción: Detiene la transmisión de video activa de la cámara especificada.
- Parámetros:
  - `cameraId` (string): El identificador único de la cámara.
- Respuesta:
  - Código `200 OK`: Un objeto JSON confirmando la detención de la transmisión (`{ "success": true }`).
  - Código `404 Not Found`: Si no hay transmisión activa para la cámara especificada.
  - Código `500 Internal Server Error`: Si hay un problema al detener la transmisión.

# 4. POST `/api/camera`
Descripción: Agrega una nueva cámara al sistema.
- Cuerpo de la solicitud:
  ```json
  {
    "name": "string",
    "ipAddress": "string",
    "username": "string",
    "password": "string",
    "resolution": "string",
    "frameRate": int,
    "isEnabled": true/false
  }
  ```
- Respuesta:
  - Código `200 OK`: Un objeto JSON confirmando la creación de la cámara (`{ "success": true }`).
  - Código `400 Bad Request`: Si falta algún campo obligatorio o hay un error de validación.
  - Código `500 Internal Server Error`: Si hay un problema al agregar la cámara.

# 5. PUT `/api/camera/{cameraId}/settings`
Descripción: Actualiza la configuración de la cámara especificada.
- Parámetros:
  - `cameraId` (string): El identificador único de la cámara.
- Cuerpo de la solicitud:
  ```json
  {
    "name": "string",
    "ipAddress": "string",
    "username": "string",
    "password": "string",
    "resolution": "string",
    "frameRate": int,
    "isEnabled": true/false
  }
  ```
- Respuesta:
  - Código `200 OK`: Un objeto JSON confirmando la actualización de la configuración (`{ "success": true }`).
  - Código `404 Not Found`: Si la cámara con el `cameraId` especificado no existe.
  - Código `500 Internal Server Error`: Si hay un problema al actualizar la configuración.

# 6. GET `/api/camera`
Descripción: Obtiene una lista de todas las cámaras registradas en el sistema.
- Respuesta:
  - Código `200 OK`: Una lista de objetos `Camera` en formato JSON.
  ```json
  [
    {
      "id": "string",
      "name": "string",
      "ipAddress": "string",
      "username": "string",
      "password": "string",
      "resolution": "string",
      "frameRate": int,
      "isEnabled": true/false
    }
  ]
  ```
  - Código `500 Internal Server Error`: Si hay un problema al obtener la lista de cámaras.

# 7. DELETE `/api/camera/{cameraId}`
Descripción: Elimina la cámara especificada y detiene la transmisión si está activa.
- Parámetros:
  - `cameraId` (string): El identificador único de la cámara.
- Respuesta:
  - Código `200 OK`: Un objeto JSON confirmando la eliminación de la cámara (`{ "success": true }`).
  - Código `404 Not Found`: Si la cámara con el `cameraId` especificado no existe.
  - Código `500 Internal Server Error`: Si hay un problema al eliminar la cámara.

# 8. POST `/api/camera/saveToCloud/{cameraId}`
Descripción**: Guarda un stream de video de la cámara especificada en pCloud.
- Parámetros:
  - `cameraId` (string): El identificador único de la cámara.
- Respuesta:
  - Código `200 OK`: Un objeto JSON confirmando que el video se guardó en pCloud (`{ "success": true/false }`).
  - Código `404 Not Found`: Si la cámara con el `cameraId` especificado no existe.
  - Código `500 Internal Server Error`: Si hay un problema al guardar el video en la nube.
