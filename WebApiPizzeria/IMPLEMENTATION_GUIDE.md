# 🍕 API Pizzería - Agregar Item a Pedido

## 📋 Descripción
Implementación de la funcionalidad "Agregar Item a un Pedido" siguiendo **Arquitectura N-Layer** (Controller → Service → Repository) en .NET Core 8 con **AutoMapper**.

---

## 🏗️ Arquitectura

### Estructura de Capas

```
┌─────────────────────────────────────┐
│      Controllers Layer              │
│   OrdersController.cs               │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│       Services Layer                │
│   IOrderService / OrderService      │
│   (Lógica de Negocio + AutoMapper)  │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│     Repositories Layer              │
│   IOrderRepository / OrderRepository│
│   (Acceso a Datos)                  │
└─────────────────────────────────────┘
              ↓
┌─────────────────────────────────────┐
│       PostgresContext               │
│   (Entity Framework Core)           │
└─────────────────────────────────────┘
```

---

## 📁 Archivos Creados

### 1. DTOs (`DTOs/`)
- **`AddItemRequest.cs`**: Request con OrderId, ProductId, Quantity, UserId
- **`AddItemResponse.cs`**: Response con Success, Message, NewTotal, Currency

### 2. Mappings (`Mappings/`)
- **`MappingProfile.cs`**: 
  - `AddItemRequest → OrderItem` (propiedades coincidentes)
  - `Order → AddItemResponse` (Total → NewTotal)

### 3. Repositories (`Repositories/`)
- **`IOrderRepository.cs`**: Interface con métodos async
- **`OrderRepository.cs`**: Implementación con EF Core
  - `GetProductByIdAsync`
  - `GetOrderByIdAsync`
  - `GetOrderItemAsync`
  - `AddOrderItemAsync`
  - `RemoveOrderItem`
  - `SaveChangesAsync`

### 4. Services (`Services/`)
- **`IOrderService.cs`**: Interface del servicio
- **`OrderService.cs`**: Lógica de negocio (Upsert)
  - Validación de producto y orden
  - **Caso A (Item existe)**: Suma Quantity manualmente
  - **Caso B (Item nuevo)**: Usa AutoMapper + asigna UnitPrice y ProductName
  - Recalcula Total y guarda cambios

### 5. Controllers (`Controllers/`)
- **`OrdersController.cs`**: 
  - Endpoint: `POST /api/orders/add-item`
  - Retorna `Ok(AddItemResponse)` o `BadRequest`

### 6. Configuración (`Program.cs`)
- Registro de AutoMapper
- Inyección de Dependencias (Repositories y Services)
- Configuración de DbContext

---

## 🔧 Configuración

### Paquetes NuGet Requeridos
```xml
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.1" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.1" />
```

### Inyección de Dependencias
```csharp
// Program.cs
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
```

---

## 📡 Uso del Endpoint

### Request
```http
POST /api/orders/add-item
Content-Type: application/json

{
  "orderId": 1,
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "quantity": 2,
  "userId": "user123"
}
```

### Response Exitosa (200 OK)
```json
{
  "success": true,
  "message": "Item agregado exitosamente",
  "newTotal": 25.50,
  "currency": "USD"
}
```

### Response de Error (400 Bad Request)
```json
{
  "success": false,
  "message": "Producto no encontrado",
  "newTotal": null,
  "currency": "USD"
}
```

---

## 🎯 Lógica de Negocio (Upsert)

1. **Validar Producto**: Verifica que el producto exista en BD
2. **Validar Orden**: Verifica que la orden exista en BD
3. **Buscar Item Existente**: Consulta si el producto ya está en la orden
4. **Caso A - Item Existe**:
   - Suma la `Quantity` manualmente (no usa AutoMapper)
   - Si `Quantity <= 0`, elimina el item
5. **Caso B - Item Nuevo**:
   - Usa `_mapper.Map<OrderItem>(request)` para crear entidad
   - Asigna `UnitPrice` y `ProductName` desde el producto
   - Agrega el item a la orden
6. **Guardar Cambios**: Persiste en BD
7. **Recalcular Total**: Suma todos los items (`Quantity * UnitPrice`)
8. **Retornar Respuesta**: Usa AutoMapper para mapear `Order → AddItemResponse`

---

## ✅ Características Implementadas

- ✅ **AutoMapper**: Transformación automática de objetos
- ✅ **Arquitectura N-Layer**: Separación de responsabilidades
- ✅ **Async/Await**: Operaciones asíncronas en toda la capa de datos
- ✅ **Dependency Injection**: IoC Container de .NET
- ✅ **SOLID Principles**: Código mantenible y escalable
- ✅ **Upsert Logic**: Manejo inteligente de items nuevos y existentes
- ✅ **Error Handling**: Validaciones y respuestas consistentes
- ✅ **Entity Framework Core**: ORM para PostgreSQL

---

## 🧪 Testing

### Pasos para Probar
1. Asegúrate de tener una orden existente en la BD
2. Asegúrate de tener productos disponibles en la BD
3. Usa Postman, Swagger o el archivo `.http` para hacer el request
4. Verifica que el total se actualiza correctamente

### Casos de Prueba
- ✅ Agregar item nuevo a una orden
- ✅ Incrementar cantidad de item existente
- ✅ Reducir cantidad de item existente
- ✅ Eliminar item (quantity <= 0)
- ✅ Validar producto inexistente
- ✅ Validar orden inexistente

---

## 📝 Notas Técnicas

### AutoMapper
- **Prohibido mapeo manual** para creación de objetos nuevos
- Solo se permite mapeo manual para actualización de entidades existentes (evitar sobrescribir IDs)

### Modelo OrderItem
Se agregaron dos propiedades al modelo:
- `UnitPrice`: Precio del producto al momento de agregar al pedido
- `ProductName`: Nombre del producto (desnormalización para historial)

---

## 🚀 Próximas Mejoras
- [ ] Validación de stock de productos
- [ ] Manejo de transacciones
- [ ] Logging con Serilog
- [ ] Unit Tests con xUnit
- [ ] Validación de datos con FluentValidation
- [ ] Caché con Redis
- [ ] Autenticación y Autorización

---

## 👤 Autor
Implementado por: **Arquitecto de Software .NET Senior**  
Fecha: Enero 2026
