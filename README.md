# 🛠️ ProductCatalog API

API RESTful desarrollada en **.NET 8** con **Entity Framework Core** y **SQL Server**.  
Permite administrar productos y categorías con relación **muchos a muchos**.  
Está diseñada con arquitectura limpia (**Domain / Infrastructure / API**) y documentada con **Swagger**.

---

## 🚀 Tecnologías utilizadas

- **.NET 8** (ASP.NET Core Web API)
- **Entity Framework Core 8** (con migraciones a SQL Server)
- **SQL Server** (persistencia relacional)
- **Swashbuckle / Swagger** (documentación automática)
- **CORS habilitado** para `http://localhost:4200` (frontend Angular)
- **xUnit + Moq + FluentAssertions + EF Core InMemory** (testing unitario)

---

## 📂 Estructura de proyectos

ProductCatalog/
├─ ProductCatalog.Api/ → API (controllers, endpoints, Swagger, CORS)
├─ ProductCatalog.Infrastructure/ → EF Core DbContext, migraciones
├─ ProductCatalog.Domain/ → Entidades de dominio (Product, Category, ProductCategory)
├─ ProductCatalog.Tests/ → Pruebas unitarias
└─ db.sql → Script SQL de creación inicial


---

## 🗄️ Modelo de datos

Tablas principales:

- **Product** → Productos (ID, nombre, descripción, imagen)  
- **Category** → Categorías (ID, nombre)  
- **ProductCategory** → Relación N:N entre productos y categorías  

---

## ⚙️ Configuración y ejecución

### 1. Base de datos

SQL Server local

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=ProductCatalogDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
}

### 2. Migraciones

dotnet ef database update -p ProductCatalog.Infrastructure -s ProductCatalog.Api

### 3. Ejecutar API
dotnet run --project ProductCatalog.Api

## 🧪 Pruebas

### Ejecutar los tests unitarios:

dotnet test

### Incluyen:

Crear producto y validación de categorías.

Obtener producto por ID.

Caso NotFound.

### Frameworks usados: xUnit, FluentAssertions, Moq, EF Core InMemory.
