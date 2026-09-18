# Security Policy

This policy defines responsible vulnerability reporting for the currently maintained 1.x releases of NuciAPI.Middleware.ExceptionHandling distributed through NuGet.org and GitHub Releases.

## 📑 Table of Contents

- [Supported Versions](#supported-versions)
- [Reporting a Vulnerability](#reporting-a-vulnerability)
- [Scope](#scope)
- [Disclosure Policy](#disclosure-policy)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| 1.x | NuGet.org | ✅ |
| 1.x | GitHub Releases | ✅ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/security/advisories)
- Contact the maintainers directly

## 📌 Scope

The subsequent report categories are in scope for this repository:
- Exception-to-HTTP response mappings that expose sensitive information or permit security-control circumvention
- Vulnerabilities in package contents, the build process, or direct dependencies

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Application-specific API configuration and deployed service infrastructure
- Issues affecting unsupported package versions

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.