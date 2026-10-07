# NuciAPI.Middleware.ExceptionHandling — Overview

## Purpose

NuciAPI.Middleware.ExceptionHandling is an ASP.NET Core middleware library that translates selected runtime exceptions from the downstream request pipeline into consistent JSON HTTP error responses. It is distributed as a NuGet package (`NuciAPI.Middleware.ExceptionHandling`) targeting `net10.0`.

## Scope

| Aspect | Description |
|--------|-------------|
| **Package type** | Library (NuGet), not an executable service |
| **Target framework** | `net10.0` with `Microsoft.AspNetCore.App` framework reference |
| **Public API surface** | Single extension method: `UseNuciApiExceptionHandling()` on `IApplicationBuilder` |
| **Internal implementation** | One sealed middleware class: `ExceptionHandlingMiddleware` |
| **Dependencies** | `NuciAPI.Middleware` (base class), `NuciAPI.Middleware.Security`, `NuciDAL`, `NuciAPI.Responses`, `NuciLog.Core` |
| **Testing** | NUnit project with `InternalsVisibleTo` access to internal middleware |

## Key Capabilities

- **Pipeline registration** — `app.UseNuciApiExceptionHandling()` registers the middleware at the chosen pipeline position.
- **Exception classification** — Maps 10 exception categories to specific HTTP status codes (400, 401, 403, 404, 409, 499, 500, 501, 503).
- **Consistent JSON responses** — All error responses use `NuciApiErrorResponse` from `NuciAPI.Responses` with `Content-Type: application/json`.
- **Client-abort representation** — `OperationCanceledException` maps to HTTP 499 (non-standard, client-closed request).
- **Fallback handling** — Any unmapped exception produces HTTP 500 with `NuciApiErrorResponse.InternalServerError`.

## What This Package Does Not Do

- Does not host a process, listener, or background service.
- Does not persist, cache, or migrate data.
- Does not perform authentication, authorisation, or logging (delegates to consumer application and other NuciAPI middleware).
- Does not impose pipeline ordering — the consumer decides where the middleware executes.
- Does not communicate with external services, telemetry endpoints, or project maintainers.

## Intended Consumers

- Application developers building NuciAPI-based services who need standardised error responses.
- Contributors modifying response semantics, exception mappings, or HTTP contracts.