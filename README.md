# Trabajo Práctico Integrador
## Desarrollo de Software
### Backend

## Introducción
Se desea desarrollar una plataforma de comercio electrónico (E-commerce). 
En esta primera etapa el objetivo es construir el módulo de Órdenes, permitiendo la gestión completa de éstas.

## Visión General del Producto
Del relevamiento preliminar se identificaron los siguientes requisitos:
- Los visitantes pueden consultar los productos sin necesidad de estar registrados o iniciar sesión.
- Para realizar un pedido se requiere el inicio de sesión.
- Una orden, para ser aceptada, debe incluir la información básica del cliente, envío y facturación.
- Antes de registrar la orden se debe verificar la disponibilidad de stock (o existencias) de los productos.
- Si la orden es exitosa hay que actualizar el stock de cada producto.
- Se deben poder consultar órdenes individuales o listar varias con posibilidad de filtrado.
- Será necesario el cambio de estado de una orden a medida que avanza en su ciclo de vida.
- Los administradores solo pueden gestionar los productos (alta, modificación y baja) y actualizar el estado de la orden.
- Los clientes pueden crear y consultar órdenes.

[Documento completo](https://frtutneduar.sharepoint.com/:b:/s/DSW2025/ETueAd4rTe1Gilj_Yfi64RYB5oz9s2dOamxKSfMFPREbiA?e=azZcwg) 

## Alcance para el Primer Parcial
> [!IMPORTANT]
> Del apartado `IMPLEMENTACIÓN` (Pag. 7), completo hasta el punto `6` (inclusive)


### Características de la Solución

- Lenguaje: C# 12.0
- Plataforma: .NET 8

Integrantes:
Bianchi Chaban Lisandro Eres 57943
Garcia Carlos Agustin 58063
Gil Fadda Angel 58051


Endpoints

Endpoints disponibles
* GET /api/products
Lista todos los productos activos.

Respuesta: 200 OK con lista de productos o 204 No Content.

* GET /api/products/{id}
Obtiene un producto por su ID.

Respuesta: 200 OK con datos del producto o 404 Not Found.

* POST /api/products
Agrega un nuevo producto.

Cuerpo esperado:

json
{
  "sku": "ABC123",
  "name": "Producto X",
  "descripcion": "Descripción opcional",
  "currectUnitPrice": 100.00,
  "stockQuantity": 50
}
Respuesta: 201 Created con el recurso creado o 400/409 si hay errores.

* PUT /api/products/{id}
Actualiza un producto existente por ID.

Cuerpo igual al POST

Respuesta: 200 OK o 404 Not Found o 400/409.

* PATCH /api/products/{id}
Desactiva (inhabilita) un producto.

Respuesta: 204 No Content o 404 Not Found.

* POST /api/orders
Registra una nueva orden de compra con items.

Cuerpo esperado:

json
{
  "customerId": "guid-del-cliente",
  "shippingAddress": "Calle 123",
  "billingAddress": "Calle 456",
  "notes": "Orden urgente",
  "items": [
{
"productId": "guid-del-producto",
"quantity":2}
]
}
Respuesta: 201 Created con los detalles de la orden o 400 BadRequest.
