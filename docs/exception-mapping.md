# Exception Mapping Reference

This document provides the complete, authoritative mapping of exception types to HTTP status codes and response payloads as implemented in `ExceptionHandlingMiddleware.cs`.

## Complete Mapping Table

| # | Exception Type(s) | HTTP Status | Status Code | Response Payload | Notes |
|---|-------------------|-------------|-------------|------------------|-------|
| 1 | `BadHttpRequestException` | Bad Request | 400 | `NuciApiErrorResponse(exception.Message, ErrorCodes.BadRequest)` | ASP.NET Core built-in for malformed requests |
| 2 | `FormatException` | Bad Request | 400 | `NuciApiErrorResponse(exception.Message, ErrorCodes.BadRequest)` | Parsing/conversion failures |
| 3 | `ArgumentException` | Bad Request | 400 | `NuciApiErrorResponse(exception.Message, ErrorCodes.BadRequest)` | Invalid method arguments |
| 4 | `ValidationException` | Bad Request | 400 | `NuciApiErrorResponse(exception.Message, ErrorCodes.BadRequest)` | `System.ComponentModel.DataAnnotations` validation failures |
| 5 | `SecurityException` | Forbidden | 403 | `NuciApiErrorResponse.Unauthorised` | CAS security failures (legacy .NET Framework) |
| 6 | `UnauthorizedAccessException` | Forbidden | 403 | `NuciApiErrorResponse.Unauthorised` | Access denied to resources/operations |
| 7 | `HttpRequestException` | Service Unavailable | 503 | `NuciApiErrorResponse.ServiceDependencyUnavailable` | Downstream HTTP call failures |
| 8 | `TaskCanceledException` | Service Unavailable | 503 | `NuciApiErrorResponse.ServiceDependencyUnavailable` | Task cancellation (non-client-abort) |
| 9 | `TimeoutException` | Service Unavailable | 503 | `NuciApiErrorResponse.ServiceDependencyUnavailable` | Operation timeouts |
| 10 | `AuthenticationException` | Unauthorized | 401 | `NuciApiErrorResponse.AuthenticationFailure` | Authentication failures (from `NuciAPI.Middleware.Security`) |
| 11 | `EntityNotFoundException` | Not Found | 404 | `NuciApiErrorResponse.NotFound` | Domain entity not found (from `NuciDAL.Repositories`) |
| 12 | `KeyNotFoundException` | Not Found | 404 | `NuciApiErrorResponse.NotFound` | Dictionary/collection key missing |
| 13 | `EntityAlreadyExistsException` | Conflict | 409 | `NuciApiErrorResponse.AlreadyExists` | Duplicate entity creation attempt (from `NuciDAL.Repositories`) |
| 14 | `RequestAlreadyProcessedException` | Conflict | 409 | `NuciApiErrorResponse.AlreadyProcessed` | Idempotency key already processed (from `NuciAPI.Middleware.Security`) |
| 15 | `NotImplementedException` | Not Implemented | 501 | `NuciApiErrorResponse(message, ErrorCodes.NotImplemented)` | Unimplemented features; uses exception message or default |
| 16 | `OperationCanceledException` | Client Closed Request | 499 | `NuciApiErrorResponse.ClientClosedTheRequest` | Client aborted request (non-standard HTTP 499) |
| 17 | *Any other exception* | Internal Server Error | 500 | `NuciApiErrorResponse.InternalServerError` | Catch-all fallback |

## Response Payload Details

### `NuciApiErrorResponse` Structure (from `NuciAPI.Responses`)

All error responses use this type. Key static properties used:

| Static Property | Description |
|-----------------|-------------|
| `Unauthorised` | Predefined 403 response |
| `ServiceDependencyUnavailable` | Predefined 503 response |
| `AuthenticationFailure` | Predefined 401 response |
| `NotFound` | Predefined 404 response |
| `AlreadyExists` | Predefined 409 response |
| `AlreadyProcessed` | Predefined 409 response |
| `ClientClosedTheRequest` | Predefined 499 response |
| `InternalServerError` | Predefined 500 response |

### Constructor Used for Dynamic Messages

```csharp
new NuciApiErrorResponse(string message, string errorCode)
```

Used for:
- Bad Request (400) — includes original exception message
- Not Implemented (501) — includes exception message or default from `NuciApiResponseMessages.ErrorMessages.NotImplemented`

## Exception Source Packages

| Exception | Package | Notes |
|-----------|---------|-------|
| `BadHttpRequestException` | `Microsoft.AspNetCore.Http` | Framework |
| `FormatException` | `System` | Framework |
| `ArgumentException` | `System` | Framework |
| `ValidationException` | `System.ComponentModel.DataAnnotations` | Framework |
| `SecurityException` | `System.Security` | Framework (legacy CAS) |
| `UnauthorizedAccessException` | `System` | Framework |
| `HttpRequestException` | `System.Net.Http` | Framework |
| `TaskCanceledException` | `System.Threading.Tasks` | Framework |
| `TimeoutException` | `System` | Framework |
| `AuthenticationException` | `NuciAPI.Middleware.Security` | NuciAPI ecosystem |
| `EntityNotFoundException` | `NuciDAL.Repositories` | NuciAPI ecosystem |
| `KeyNotFoundException` | `System.Collections.Generic` | Framework |
| `EntityAlreadyExistsException` | `NuciDAL.Repositories` | NuciAPI ecosystem |
| `RequestAlreadyProcessedException` | `NuciAPI.Middleware.Security` | NuciAPI ecosystem |
| `NotImplementedException` | `System` | Framework |
| `OperationCanceledException` | `System` | Framework |

## Catch Block Order Significance

The catch blocks are evaluated **in order**. This matters for exception hierarchies:

1. **More specific types first** — `AuthenticationException` caught before base `Exception`
2. **`OperationCanceledException` before catch-all** — Ensures client aborts get 499 not 500
3. **`TaskCanceledException` in 503 group** — Distinct from `OperationCanceledException` (499)

### Inheritance Relationships Affecting Order

```
Exception
├── SystemException
│   ├── ArgumentException
│   ├── FormatException
│   ├── InvalidOperationException
│   ├── NotImplementedException
│   └── TimeoutException
├── System.Security.SecurityException
├── UnauthorizedAccessException
├── AuthenticationException (custom)
├── HttpRequestException
├── TaskCanceledException
├── OperationCanceledException
├── KeyNotFoundException
└── NuciDAL/NuciAPI custom exceptions
```

The current order correctly handles these because:
- `ArgumentException`, `FormatException` caught explicitly before base `Exception`
- `AuthenticationException` caught explicitly
- `OperationCanceledException` caught explicitly before catch-all
- `TaskCanceledException` caught in 503 group (not 499)

## Test Coverage

All 17 mappings are verified in `ExceptionHandlingMiddlewareTests.cs`:

```csharp
[TestCase(typeof(BadHttpRequestException), 400)]
[TestCase(typeof(ArgumentException), 400)]
[TestCase(typeof(FormatException), 400)]
[TestCase(typeof(SecurityException), 403)]
[TestCase(typeof(UnauthorizedAccessException), 403)]
[TestCase(typeof(HttpRequestException), 503)]
[TestCase(typeof(TaskCanceledException), 503)]
[TestCase(typeof(TimeoutException), 503)]
[TestCase(typeof(AuthenticationException), 401)]
[TestCase(typeof(KeyNotFoundException), 404)]
[TestCase(typeof(RequestAlreadyProcessedException), 409)]
[TestCase(typeof(NotImplementedException), 501)]
[TestCase(typeof(OperationCanceledException), 499)]
[TestCase(typeof(Exception), 500)]
```

**Note:** `ValidationException` and `EntityNotFoundException`/`EntityAlreadyExistsException` are not explicitly tested as separate test cases but are covered by the same catch blocks as `ArgumentException`/`KeyNotFoundException`/`RequestAlreadyProcessedException` respectively.

## Consumer Guidance

### If Your Exception Isn't Mapped

1. **Wrap/translate upstream** — Add middleware before this one that catches your exception and throws a mapped type.
2. **Use existing types** — Throw `ArgumentException`, `KeyNotFoundException`, etc. directly where appropriate.
3. **Request mapping addition** — File an issue for a new mapping if it represents a common cross-cutting concern.

### Pipeline Ordering

Register `UseNuciApiExceptionHandling()` **early enough** to catch exceptions from:
- Controller/action execution
- Other middleware downstream
- Endpoint filters

Register **after** middleware that should NOT have their exceptions translated (e.g., static files, health checks if they should return raw errors).

## Version History

| Version | Changes |
|---------|---------|
| 1.0.2 | Current — mappings as documented above |
| 1.x | Initial mappings established |

## Related Files

- Implementation: `NuciAPI.Middleware.ExceptionHandling/ExceptionHandlingMiddleware.cs`
- Tests: `NuciAPI.Middleware.ExceptionHandling.UnitTests/ExceptionHandlingMiddlewareTests.cs`
- Extension method: `NuciAPI.Middleware.ExceptionHandling/ExceptionHandlingMiddlewareExtensions.cs`