# Privacy and Personal Data

NuciAPI.Middleware.ExceptionHandling is an ASP.NET Core middleware library that translates runtime exceptions into consistent JSON API error responses. It is distributed as a NuGet package and runs entirely within the host application's process. No personal data is collected, transmitted, or stored by this library.

**Information reviewed:** 2026-10-07

## Table of Contents

- [What This Document Covers](#what-this-document-covers)
- [Self-Hosted Deployments](#self-hosted-deployments)
- [Data We Handle](#data-we-handle)
- [Processing and Use](#processing-and-use)
- [Storage, Retention, and Deletion](#storage-retention-and-deletion)
- [External Processing and Integrations](#external-processing-and-integrations)
- [Document Changes](#document-changes)
- [Contact](#contact)

## What This Document Covers

This document describes how NuciAPI.Middleware.ExceptionHandling at https://github.com/hmlendea/nuciapi.middleware.exceptionhandling handles personal data. It covers the application behaviour and verified integrations described below. Where the software is self-hosted, the instance operator may have separate responsibilities described below.

## Self-Hosted Deployments

NuciAPI.Middleware.ExceptionHandling is a library that developers integrate into their own ASP.NET Core applications. The library runs entirely within the host application's process and does not communicate with any external services, project maintainers, or telemetry endpoints. Instance operators (application developers) control all configuration, local storage, logs, backups, access controls, retention, and request handling. No data is sent from the library to project maintainers or external services.

## Data We Handle

### Data Provided to the Application

No personal data is requested or accepted by this library. The library operates on exception objects thrown by the host application and does not process user-provided personal data.

### Data Generated or Collected by the Application

No personal data is generated or collected automatically by this library. The library catches exceptions and writes JSON error responses to the HTTP response stream. Exception messages may contain application-specific data, but the library itself does not generate, collect, or log personal data.

### Data Received from Integrations

No personal data is received from integrations or third parties. The library has no built-in integrations with external services.

## Processing and Use

The application processes the data described above for these verified functions:
- Exception handling — Exception objects thrown by the host application (no personal data processed by the library)
- HTTP response formatting — Standardised JSON error responses written to the HTTP response stream

## Storage, Retention, and Deletion

The library does not store any data. Exception information exists only transiently in memory during request processing and is written to the HTTP response stream. No databases, files, logs, caches, or backups are created or controlled by this library. For self-hosted deployments, the instance operator controls all storage, deletion, and backups for their application.

## External Processing and Integrations

The application has no built-in external data transfer. No external services, recipients, or integrations process or receive data from this library.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| None | N/A | N/A | N/A |

## Data Protection and Security

The library performs no data processing that requires protection. It catches exceptions and writes JSON responses. For self-hosted deployments, the instance operator is responsible for application-level security including updates, secrets management, access controls, backups, network exposure, and log protection. The library itself does not promise absolute security.

## Document Changes

Update this document when application data flows, storage, integrations, or deployment responsibilities change. The current version is published at https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/blob/main/PRIVACY.md.

## Contact

For questions about application data handling, contact the project maintainers via GitHub issues at https://github.com/hmlendea/nuciapi.middleware.exceptionhandling/issues. For a self-hosted instance, contact the instance operator (application developer), unless the project explicitly handles the request. Include the application name and deployment context if applicable; do not send passwords, access tokens, or other secrets.