# .NET 10 Upgrade Tasks

## Progress Dashboard

| Metric | Status |
|--------|--------|
| **Total Tasks** | 8 |
**Progress**: 7/8 tasks complete (88%) ![88%](https://progress-bar.xyz/88)
| **In Progress** | 0 |
| **Failed** | 0 |
**Remaining** | 6 |

**Legend:** `[ ]` Not Started | `[?]` In Progress | `[?]` Complete | `[?]` Failed | `[?]` Skipped

---

## Tasks

### [?] TASK-001: Verify Prerequisites *(Completed: 2026-04-07 14:13)*
**Scope**: Validate environment readiness for .NET 10 upgrade
**References**: Plan: §Executive Summary, §Migration Strategy

**Actions:**
- [?] (1) Verify .NET 10 SDK is installed on the machine
- [?] (2) Verify global.json (if present) is compatible with .NET 10
- [?] (3) Confirm working branch is `upgrade-to-NET10`

**Validation:**
- .NET 10 SDK available
- No blocking global.json constraints
- On correct branch

---

### [?] TASK-002: Convert OpenADK-NET-Core to SDK-style *(Completed: 2026-04-07 14:17)*
**Scope**: Convert the leaf project to SDK-style format
**References**: Plan: §Project-by-Project Plans > OpenADK-NET-Core

**Actions:**
- [?] (1) Convert `src/core/OpenADK-NET-Core.csproj` from classic format to SDK-style
- [?] (2) Set `<TargetFramework>` to `net10.0-windows`
- [?] (3) Enable Windows Forms: add `<UseWindowsForms>true</UseWindowsForms>`
- [?] (4) Reload the project in the IDE

**Validation:**
- Project file is SDK-style format
- TargetFramework is net10.0-windows
- UseWindowsForms is enabled

---

### [?] TASK-003: Convert OpenADK-US-SDO to SDK-style *(Completed: 2026-04-07 14:19)*
**Scope**: Convert the root project to SDK-style format
**References**: Plan: §Project-by-Project Plans > OpenADK-US-SDO

**Actions:**
- [?] (1) Convert `src/us/OpenADK-US-SDO.csproj` from classic format to SDK-style
- [?] (2) Set `<TargetFramework>` to `net10.0`
- [?] (3) Verify project reference to OpenADK-NET-Core is correct
- [?] (4) Reload the project in the IDE

**Validation:**
- Project file is SDK-style format
- TargetFramework is net10.0
- Project reference intact

---

### [?] TASK-004: Add Required NuGet Packages *(Completed: 2026-04-07 14:30)*
**Scope**: Add packages needed for .NET 10 compatibility
**References**: Plan: §Package Update Reference

**Actions:**
- [?] (1) Add `System.Configuration.ConfigurationManager` package (version 9.0.0+) to OpenADK-NET-Core
- [?] (2) Restore all NuGet packages for the solution

**Validation:**
- Package successfully added
- All packages restore without errors

---

### [?] TASK-005: Remove Code Access Security (CAS) Attributes *(Completed: 2026-04-07 14:51)*
**Scope**: Remove all SecurityPermissionAttribute usages (1,848 instances)
**References**: Plan: §Breaking Changes Catalog

**Actions:**
- [?] (1) Remove all `[SecurityPermission(...)]` attributes from OpenADK-NET-Core (280 instances in 162 files)
- [?] (2) Remove all `[SecurityPermission(...)]` attributes from OpenADK-US-SDO (1,568 instances in 786 files)
- [?] (3) Remove any `using System.Security.Permissions;` statements that become unused

**Validation:**
- No SecurityPermissionAttribute remains in codebase
- No unused using statements

---

### [?] TASK-006: Fix Binary and Source Incompatible APIs *(Completed: 2026-04-07 14:56)*
**Scope**: Update deprecated/removed APIs to modern equivalents
**References**: Plan: §Breaking Changes Catalog, §Project-by-Project Plans > OpenADK-NET-Core

**Actions:**
- [?] (1) Replace `ICertificatePolicy` with `HttpClientHandler.ServerCertificateCustomValidationCallback` (4 instances)
- [?] (2) Replace `ServicePointManager.CertificatePolicy` usage (1 instance)
- [?] (3) Replace `BinaryFormatter` with modern serialization approach (7 instances)
- [?] (4) Update legacy cryptography APIs to use factory methods (5 instances):
        - `RC2CryptoServiceProvider` ? `RC2.Create()`
        - `TripleDESCryptoServiceProvider` ? `TripleDES.Create()`
        - `DESCryptoServiceProvider` ? `DES.Create()`
        - `MD5CryptoServiceProvider` ? `MD5.Create()`
        - `SHA1Managed` ? `SHA1.Create()`
- [?] (5) Update or remove serialization constructors from exception classes (mark obsolete or remove)
- [?] (6) Replace `TimeZone` with `TimeZoneInfo` (2 instances)
- [?] (7) Replace `WebRequest.Create` with `HttpClient` (1 instance)

**Validation:**
- All binary incompatible APIs replaced
- All source incompatible APIs addressed
- Code compiles without API-related errors

---

### [?] TASK-007: Build and Fix Remaining Errors *(Completed: 2026-04-07 14:59)*
**Scope**: Build solution and resolve any remaining compilation errors
**References**: Plan: §Testing & Validation Strategy

**Actions:**
- [?] (1) Build the entire solution
- [?] (2) Review and fix any remaining compilation errors
- [?] (3) Review and address any significant warnings
- [?] (4) Verify both projects build successfully

**Validation:**
- Solution builds with 0 errors
- No critical warnings remain

---

### [?] TASK-008: Commit Changes and Finalize
**Scope**: Commit all upgrade changes to source control
**References**: Plan: §Source Control Strategy

**Actions:**
- [?] (1) Stage all modified files
- [ ] (2) Commit with message:
        ```
        feat: Upgrade solution from .NET Framework 4.8 to .NET 10.0
        
        - Convert OpenADK-NET-Core to SDK-style, target net10.0-windows
        - Convert OpenADK-US-SDO to SDK-style, target net10.0
        - Remove all CAS SecurityPermissionAttribute usages (1,848 instances)
        - Replace BinaryFormatter with modern serialization
        - Update legacy cryptography APIs
        - Replace ICertificatePolicy with HttpClientHandler
        - Add System.Configuration.ConfigurationManager package
        ```
- [ ] (3) Verify commit was successful

**Validation:**
- All changes committed
- No uncommitted changes remain
- Upgrade branch ready for pull request

---

## Execution Log

*Execution log entries will be appended here as tasks are completed.*
