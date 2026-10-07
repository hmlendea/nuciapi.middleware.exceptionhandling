# Architecture — Detailed Component View

This document provides a detailed component-level view that complements the root `ARCHITECTURE.md`. It focuses on implementation structure, class responsibilities, data flows, and extension points.

## Component Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    Consumer ASP.NET Core App                      │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  IApplicationBuilder                                        │  │
│  │    └─ UseNuciApiExceptionHandling() ────────────────────┐  │  │
│  └───────────────────────────────────────────────────────────┘  │
│                              │                                    │
│                              ▼                                    │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  ExceptionHandlingMiddleware (internal sealed)              │  │
│  │    ├─ Inherits: NuciApiMiddleware (from NuciAPI.Middleware) │  │
│  │    ├─ Constructor: RequestDelegate next                     │  │
│  │    ├─ Override: InvokeAsync(HttpContext context)            │  │
│  │    └─ Private: WriteErrorResponseAsync (2 overloads)        │  │
│  └───────────────────────────────────────────────────────────┘  │
│                              │                                    │
│              ┌───────────────┼───────────────┐                   │
│              ▼               ▼               ▼                   │
│         Downstream       Exception       HttpContext.Response    │
│         RequestDelegate  Classification   (JSON write)           │
└─────────────────────────────────────────────────────────────────┘
```

## Public Composition API

### `ExceptionHandlingMiddlewareExtensions`

**Location:** `NuciAPI.Middleware.ExceptionHandling/ExceptionHandlingMiddlewareExtensions.cs`

```csharp
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseNuciApiExceptionHandling(
        this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
```

**Responsibilities:**
- Single public entry point for consumers.
- Delegates middleware construction to ASP.NET Core's `UseMiddleware<T>()`.
- No configuration parameters — behaviour is fixed at compile time.

**Dependencies:**
- `Microsoft.AspNetCore.Builder.IApplicationBuilder`
- Internal `ExceptionHandlingMiddleware` type (via `UseMiddleware`)

**Lifetime:** Static method; no instance state.

## Internal Middleware Implementation

### `ExceptionHandlingMiddleware`

**Location:** `NuciAPI.Middleware.ExceptionHandling/ExceptionHandlingMiddleware.cs`

**Inheritance:** `NuciApiMiddleware` (from `NuciAPI.Middleware` package)

**Constructor:**
```csharp
internal sealed class ExceptionHandlingMiddleware(RequestDelegate next)
    : NuciApiMiddleware(next)
```

**Primary Method — `InvokeAsync`:**
```csharp
public override async Task InvokeAsync(HttpContext context)
{
    try
    {
        await Next(context);  // Await downstream delegate
    }
    catch (Exception exception) when (/* mapped exception categories */)
    {
        await WriteErrorResponseAsync(context, statusCode, errorResponse);
    }
    // ... multiple catch blocks for each exception category ...
    catch
    {
        await WriteErrorResponseAsync(context, HttpStatusCode.InternalServerError,
            NuciApiErrorResponse.InternalServerError);
    }
}
```

**Exception Classification Logic (in catch block order):**

| Catch Block | Exception Types | HTTP Status | Response |
|-------------|-----------------|-------------|----------|
| 1 | `BadHttpRequestException`, `FormatException`, `ArgumentException`, `ValidationException` | 400 | `NuciApiErrorResponse(exception.Message, BadRequest)` |
| 2 | `SecurityException`, `UnauthorizedAccessException` | 403 | `NuciApiErrorResponse.Unauthorised` |
| 3 | `HttpRequestException`, `TaskCanceledException`, `TimeoutException` | 503 | `NuciApiErrorResponse.ServiceDependencyUnavailable` |
| 4 | `AuthenticationException` | 401 | `NuciApiErrorResponse.AuthenticationFailure` |
| 5 | `EntityNotFoundException`, `KeyNotFoundException` | 404 | `NuciApiErrorResponse.NotFound` |
| 6 | `EntityAlreadyExistsException` | 409 | `NuciApiErrorResponse.AlreadyExists` |
| 7 | `RequestAlreadyProcessedException` | 409 | `NuciApiErrorResponse.AlreadyProcessed` |
| 8 | `NotImplementedException` | 501 | `NuciApiErrorResponse(message, NotImplemented)` |
| 9 | `OperationCanceledException` | 499 | `NuciApiErrorResponse.ClientClosedTheRequest` |
| 10 (catch-all) | Any other exception | 500 | `NuciApiErrorResponse.InternalServerError` |

**Private Helper — `WriteErrorResponseAsync`:**
```csharp
private async Task WriteErrorResponseAsync(
    HttpContext context,
    int statusCode,
    NuciApiErrorResponse errorResponse)
{
    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(errorResponse);
}
```

**Key Implementation Details:**

1. **No shared mutable state** — Each invocation is independent; middleware is stateless.
2. **Base class `NuciApiMiddleware`** — Provides `Next` property (the downstream `RequestDelegate`) and possibly common middleware infrastructure.
3. **Exception filter patterns** — Uses C# `when` clauses for precise type matching without catching and re-throwing.
4. **Response writing** — Sets status code, content type, then serialises `NuciApiErrorResponse` via `WriteAsJsonAsync`.
5. **Short-circuiting** — Once an exception is caught and response written, the middleware completes without re-throwing.

## Data Flow

### Successful Request Path
```
HttpContext → InvokeAsync → await Next(context) → downstream completes → return
```

### Exception Path
```
HttpContext → InvokeAsync → await Next(context) → downstream throws
    → catch block matches exception type
    → WriteErrorResponseAsync(statusCode, NuciApiErrorResponse)
    → context.Response.StatusCode = statusCode
    → context.Response.ContentType = "application/json"
    → await context.Response.WriteAsJsonAsync(errorResponse)
    → return (exception consumed, not re-thrown)
```

### Data Transformations

| Stage | Input | Transformation | Output |
|-------|-------|----------------|--------|
| Classification | `Exception` | Pattern match against 10 categories | `(HttpStatusCode, NuciApiErrorResponse)` |
| Serialisation | `NuciApiErrorResponse` | `WriteAsJsonAsync` (System.Text.Json) | JSON bytes to response stream |
| HTTP Response | Status code + JSON body | ASP.NET Core response pipeline | HTTP response to client |

## Dependency Graph

```
ExceptionHandlingMiddlewareExtensions (public)
    └─ depends on ─► ExceptionHandlingMiddleware (internal)
                          ├─ inherits ─► NuciApiMiddleware (NuciAPI.Middleware)
                          ├─ uses ─► RequestDelegate, HttpContext (ASP.NET Core)
                          ├─ uses ─► NuciApiErrorResponse, NuciApiResponseCodes, NuciApiResponseMessages (NuciAPI.Responses)
                          ├─ catches ─► EntityNotFoundException, EntityAlreadyExistsException, RequestAlreadyProcessedException (NuciDAL.Repositories)
                          ├─ catches ─► SecurityException, UnauthorizedAccessException, AuthenticationException (System.Security, NuciAPI.Middleware.Security)
                          └─ catches ─► HttpRequestException, TaskCanceledException, TimeoutException, OperationCanceledException (System.Net.Http, System.Threading.Tasks)
```

## Extension Points

### For Consumers (Application Developers)

1. **Pipeline position** — Call `UseNuciApiExceptionHandling()` at the desired point in `Program.cs` or `Startup.Configure`. Earlier registration catches more downstream exceptions.

2. **Custom exception types** — Not directly extensible. Consumers must:
   - Map their exceptions to one of the handled types before they reach this middleware, OR
   - Register additional middleware upstream that translates custom exceptions.

### For Contributors (Package Maintainers)

1. **Add exception mappings** — Add new `catch` blocks in `ExceptionHandlingMiddleware.InvokeAsync` following the existing pattern.

2. **Modify response shapes** — Change `NuciApiErrorResponse` construction; requires `NuciAPI.Responses` package changes for new response types.

3. **Change HTTP status codes** — Modify the `HttpStatusCode` or `StatusCodes` values in catch blocks.

4. **Add configuration** — Would require:
   - Adding options class
   - Modifying `UseNuciApiExceptionHandling` to accept options
   - Passing options to middleware constructor
   - Updating `ExceptionHandlingMiddleware` to use configured behaviour

## Cross-Cutting Concerns

### Error Handling (Internal)
- **Owned by:** `ExceptionHandlingMiddleware`
- **Scope:** Only exceptions from the downstream `RequestDelegate` within the same pipeline segment.
- **Not handled:** Exceptions before this middleware, after response has started, or in other pipeline branches.

### Concurrency
- **Model:** Per-request async; no shared state.
- **Cancellation:** `OperationCanceledException` mapped to 499; other cancellation behaviours delegated to ASP.NET Core host.

### Serialisation
- **Mechanism:** `HttpContext.Response.WriteAsJsonAsync` (uses `System.Text.Json` with ASP.NET Core defaults).
- **Content-Type:** Explicitly set to `application/json` before writing.

## Compatibility Boundaries

| Boundary | Contract | Stability |
|----------|----------|-----------|
| `UseNuciApiExceptionHandling()` | `IApplicationBuilder` extension, no parameters | Stable — signature preserved |
| Exception-to-status mapping | 10 categories → specific status codes | Consumer-visible — changes are breaking |
| Response JSON shape | `NuciApiErrorResponse` from `NuciAPI.Responses` | Governed by `NuciAPI.Responses` package |
| Target framework | `net10.0` | Breaking if changed |

## Source Map

| Component | File |
|-----------|------|
| Public extension | `NuciAPI.Middleware.ExceptionHandling/ExceptionHandlingMiddlewareExtensions.cs` |
| Middleware implementation | `NuciAPI.Middleware.ExceptionHandling/ExceptionHandlingMiddleware.cs` |
| Unit tests | `NuciAPI.Middleware.ExceptionHandling.UnitTests/ExceptionHandlingMiddlewareTests.cs` |
| Project configuration | `NuciAPI.Middleware.ExceptionHandling/NuciAPI.Middleware.ExceptionHandling.csproj` |
| Test project configuration | `NuciAPI.Middleware.ExceptionHandling.UnitTests/NuciAPI.Middleware.ExceptionHandling.UnitTests.csproj` |
| Solution | `NuciAPI.Middleware.ExceptionHandling.sln` |
| CI workflow | `.github/workflows/dotnet.yml` |
| Release workflow | `.github/workflows/github-release.yml` |