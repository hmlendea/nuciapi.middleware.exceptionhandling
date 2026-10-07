# Testing Documentation

This document describes the testing strategy, test structure, execution, and coverage for NuciAPI.Middleware.ExceptionHandling.

## Test Project Structure

```
NuciAPI.Middleware.ExceptionHandling.UnitTests/
├── ExceptionHandlingMiddlewareTests.cs   # All unit tests
└── NuciAPI.Middleware.ExceptionHandling.UnitTests.csproj
```

## Test Framework

- **Framework:** NUnit 4.6.1
- **Test Adapter:** NUnit3TestAdapter 6.3.0
- **Test SDK:** Microsoft.NET.Test.Sdk 18.10.1
- **Mocking:** Moq 4.20.72 (available but not used in current tests)
- **Access:** `InternalsVisibleTo` in main project allows direct instantiation of `internal sealed ExceptionHandlingMiddleware`

## Test Categories

### 1. Exception Mapping Tests (Parameterised)

**Method:** `Given_DownstreamException_When_InvokeAsync_Then_WritesMappedStatusCode`

**Coverage:** 14 test cases covering all mapped exception categories.

```csharp
[TestCase(typeof(BadHttpRequestException), (int)HttpStatusCode.BadRequest)]
[TestCase(typeof(ArgumentException), (int)HttpStatusCode.BadRequest)]
[TestCase(typeof(FormatException), (int)HttpStatusCode.BadRequest)]
[TestCase(typeof(SecurityException), (int)HttpStatusCode.Forbidden)]
[TestCase(typeof(UnauthorizedAccessException), (int)HttpStatusCode.Forbidden)]
[TestCase(typeof(HttpRequestException), (int)HttpStatusCode.ServiceUnavailable)]
[TestCase(typeof(TaskCanceledException), (int)HttpStatusCode.ServiceUnavailable)]
[TestCase(typeof(TimeoutException), (int)HttpStatusCode.ServiceUnavailable)]
[TestCase(typeof(AuthenticationException), (int)HttpStatusCode.Unauthorized)]
[TestCase(typeof(KeyNotFoundException), (int)HttpStatusCode.NotFound)]
[TestCase(typeof(RequestAlreadyProcessedException), (int)HttpStatusCode.Conflict)]
[TestCase(typeof(NotImplementedException), (int)HttpStatusCode.NotImplemented)]
[TestCase(typeof(OperationCanceledException), StatusCodes.Status499ClientClosedRequest)]
[TestCase(typeof(Exception), (int)HttpStatusCode.InternalServerError)]
```

**Assertions per case:**
- `context.Response.StatusCode` equals expected status code
- `context.Response.ContentType` starts with `"application/json"`
- Response body is not empty

### 2. Successful Delegation Test

**Method:** `Given_NoException_When_InvokeAsync_Then_InvokesNextDelegate`

**Purpose:** Verifies the middleware correctly delegates to the downstream `RequestDelegate` when no exception occurs.

**Implementation:**
```csharp
bool wasInvoked = false;
ExceptionHandlingMiddleware middleware = new(_ =>
{
    wasInvoked = true;
    return Task.CompletedTask;
});

await middleware.InvokeAsync(CreateContext());
Assert.That(wasInvoked, Is.True);
```

## Test Infrastructure

### `CreateContext()` Helper

```csharp
private static DefaultHttpContext CreateContext()
{
    DefaultHttpContext context = new();
    context.Response.Body = new MemoryStream();
    return context;
}
```

- Creates a `DefaultHttpContext` with a `MemoryStream` response body
- Allows inspection of written response content

### `ReadResponseBody()` Helper

```csharp
private static string ReadResponseBody(DefaultHttpContext context)
{
    context.Response.Body.Position = 0;
    using StreamReader reader = new(context.Response.Body, leaveOpen: true);
    return reader.ReadToEnd();
}
```

- Reads the response body for content verification
- `leaveOpen: true` preserves stream for potential further reads

### `CreateException()` Factory

```csharp
private static Exception CreateException(Type exceptionType)
    => exceptionType == typeof(BadHttpRequestException) ? new BadHttpRequestException("Bad request")
    : exceptionType == typeof(ArgumentException) ? new ArgumentException("Invalid argument")
    : exceptionType == typeof(FormatException) ? new FormatException("Invalid format")
    : exceptionType == typeof(SecurityException) ? new SecurityException("Forbidden")
    : exceptionType == typeof(UnauthorizedAccessException) ? new UnauthorizedAccessException("Forbidden")
    : exceptionType == typeof(HttpRequestException) ? new HttpRequestException("Dependency unavailable")
    : exceptionType == typeof(TaskCanceledException) ? new TaskCanceledException("Cancelled")
    : exceptionType == typeof(TimeoutException) ? new TimeoutException("Timed out")
    : exceptionType == typeof(AuthenticationException) ? new AuthenticationException("Auth failed")
    : exceptionType == typeof(KeyNotFoundException) ? new KeyNotFoundException("Not found")
    : exceptionType == typeof(RequestAlreadyProcessedException) ? new RequestAlreadyProcessedException("REQ-1")
    : exceptionType == typeof(NotImplementedException) ? new NotImplementedException("Not implemented")
    : exceptionType == typeof(OperationCanceledException) ? new OperationCanceledException("Client closed")
    : new Exception("Unhandled");
```

- Creates appropriate exception instances with meaningful messages
- Handles constructor differences (e.g., `RequestAlreadyProcessedException` takes a string ID)

## Running Tests

### Command Line

```bash
# Run all tests in solution
dotnet test NuciAPI.Middleware.ExceptionHandling.sln

# Run only unit test project
dotnet test NuciAPI.Middleware.ExceptionHandling.UnitTests/NuciAPI.Middleware.ExceptionHandling.UnitTests.csproj

# Run with verbose output
dotnet test --verbosity normal

# Run specific test method
dotnet test --filter "FullyQualifiedName~Given_DownstreamException_When_InvokeAsync_Then_WritesMappedStatusCode"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### CI Execution

The `.github/workflows/dotnet.yml` workflow runs tests on every push and PR to `master`:

```yaml
- name: Test
  run: dotnet test --no-build --verbosity normal
```

## Test Coverage Analysis

### Currently Covered

| Area | Coverage | Notes |
|------|----------|-------|
| All 14 parameterised exception mappings | ✅ | Status code, content type, non-empty body |
| Successful downstream delegation | ✅ | Verifies `Next` delegate invoked |
| Response JSON serialisation | ✅ | Implicit via non-empty body check |

### Not Explicitly Covered (Gaps)

| Area | Gap Description | Risk |
|------|-----------------|------|
| `ValidationException` mapping | Not a separate test case (covered by `ArgumentException` catch block) | Low — same catch block |
| `EntityNotFoundException` mapping | Not a separate test case (covered by `KeyNotFoundException` catch block) | Low — same catch block |
| `EntityAlreadyExistsException` mapping | Not a separate test case (covered by `RequestAlreadyProcessedException` catch block) | Low — same catch block |
| Response payload content validation | Only checks non-empty; doesn't verify JSON structure or error codes | Medium — could miss serialisation regressions |
| `NotImplementedException` message handling | Doesn't test fallback to default message when `exception.Message` is null | Low |
| Multiple exceptions in sequence | No test for middleware reuse across requests | Low — stateless design |
| Response already started scenario | No test for behaviour when `context.Response.HasStarted == true` | Medium — ASP.NET Core may throw |
| Concurrent requests | No concurrency/parallelism tests | Low — stateless, but worth verifying |

## Adding New Tests

### For New Exception Mappings

1. Add test case to `Given_DownstreamException_When_InvokeAsync_Then_WritesMappedStatusCode`:
```csharp
[TestCase(typeof(YourNewException), (int)HttpStatusCode.YourStatus)]
```

2. Add case to `CreateException()` factory:
```csharp
: exceptionType == typeof(YourNewException) ? new YourNewException("message")
```

### For Response Content Validation

Add assertions to verify JSON structure:
```csharp
var body = ReadResponseBody(context);
var response = JsonSerializer.Deserialize<NuciApiErrorResponse>(body);
Assert.That(response.ErrorCode, Is.EqualTo(expectedCode));
Assert.That(response.Message, Is.EqualTo(expectedMessage));
```

### For Edge Cases

Create new test methods:
```csharp
[Test]
public async Task Given_ResponseAlreadyStarted_When_ExceptionThrown_Then_HandlesGracefully()
{
    // Arrange: context with Response.HasStarted = true
    // Act & Assert: verify no exception thrown from middleware
}
```

## Test Design Principles

1. **Direct instantiation** — Tests instantiate `ExceptionHandlingMiddleware` directly using `InternalsVisibleTo`, avoiding full ASP.NET Core host startup.

2. **Minimal context** — `DefaultHttpContext` with `MemoryStream` response body is sufficient; no need for `TestServer` or `WebApplicationFactory`.

3. **Parameterised tests** — Single test method covers all exception mappings, reducing duplication.

4. **Focused assertions** — Each test verifies the essential contract: status code, content type, response written.

5. **No mocking needed** — The downstream delegate is a simple lambda; `Moq` is available but unused.

## Continuous Integration

| Trigger | Workflow | Command |
|---------|----------|---------|
| Push to master | `.github/workflows/dotnet.yml` | `dotnet test --no-build --verbosity normal` |
| PR to master | `.github/workflows/dotnet.yml` | `dotnet test --no-build --verbosity normal` |
| Release published | `.github/workflows/github-release.yml` | Pack only (tests run in CI beforehand) |

## Related Files

- Test implementation: `NuciAPI.Middleware.ExceptionHandling.UnitTests/ExceptionHandlingMiddlewareTests.cs`
- Test project: `NuciAPI.Middleware.ExceptionHandling.UnitTests/NuciAPI.Middleware.ExceptionHandling.UnitTests.csproj`
- Main project (with InternalsVisibleTo): `NuciAPI.Middleware.ExceptionHandling/NuciAPI.Middleware.ExceptionHandling.csproj`
- CI workflow: `.github/workflows/dotnet.yml`