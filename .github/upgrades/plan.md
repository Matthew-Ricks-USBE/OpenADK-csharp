# .NET 10 Upgrade Plan - OpenADK-NET-US Solution

## Table of Contents

- [Executive Summary](#executive-summary)
- [Migration Strategy](#migration-strategy)
- [Detailed Dependency Analysis](#detailed-dependency-analysis)
- [Project-by-Project Plans](#project-by-project-plans)
  - [OpenADK-NET-Core](#openadk-net-core)
  - [OpenADK-US-SDO](#openadk-us-sdo)
- [Package Update Reference](#package-update-reference)
- [Breaking Changes Catalog](#breaking-changes-catalog)
- [Risk Management](#risk-management)
- [Testing & Validation Strategy](#testing--validation-strategy)
- [Complexity & Effort Assessment](#complexity--effort-assessment)
- [Source Control Strategy](#source-control-strategy)
- [Success Criteria](#success-criteria)

---

## Executive Summary

### Scenario Overview

| Attribute | Value |
|-----------|-------|
| **Solution** | OpenADK-NET-US.sln |
| **Current Framework** | .NET Framework 4.8 |
| **Target Framework** | .NET 10.0 |
| **Total Projects** | 2 |
| **Total Lines of Code** | 243,338 |
| **Estimated LOC to Modify** | ~1,947 (0.8% of codebase) |
| **Security Vulnerabilities** | None |
| **NuGet Package Updates** | None required |

### Discovered Metrics

| Metric | Value | Assessment |
|--------|-------|------------|
| Project Count | 2 | ? Simple |
| Dependency Depth | 1 | ? Shallow |
| Circular Dependencies | None | ? Clean |
| High-Risk Projects | 0 | ? Low risk |
| Binary Incompatible APIs | 8 | ?? Requires code changes |
| Source Incompatible APIs | 1,892 | ?? Mostly CAS removals |
| Behavioral Changes | 47 | ?? Requires testing |

### Complexity Classification

**Classification: Simple**

**Justification:**
- Only 2 projects with straightforward linear dependency
- Both projects rated as ?? Low Difficulty
- No NuGet package conflicts or updates needed
- No security vulnerabilities to address
- 94.9% of all issues are simple CAS attribute removals
- Clear dependency order: OpenADK-NET-Core (leaf) ? OpenADK-US-SDO (root)

### Critical Issues

**Primary Challenge: Code Access Security (CAS) Removal**
- 1,848 instances of `SecurityPermissionAttribute` must be removed
- CAS is not supported in .NET Core/.NET and these attributes simply need deletion
- This represents 94.9% of all migration issues

**Secondary Challenges:**
- SDK-style project conversion required for both projects
- 8 binary incompatible APIs (ICertificatePolicy, DirectoryInfo.FullName, ServicePointManager.CertificatePolicy)
- 7 BinaryFormatter usages requiring migration to modern serialization
- 5 legacy cryptography APIs requiring updates
- 4 ConfigurationManager usages (add NuGet package or modernize)

### Selected Strategy

**All-At-Once Strategy** - All projects upgraded simultaneously in a single coordinated operation.

**Rationale:**
- 2 projects (small solution well under 30-project threshold)
- Simple linear dependency structure
- Both projects currently on .NET Framework 4.8
- No NuGet package complications
- All issues have clear, known remediation paths
- Low overall risk profile

---

## Migration Strategy

### Approach Selection

**Selected: All-At-Once Strategy**

This solution is an ideal candidate for All-At-Once migration:

| Criterion | Status | Notes |
|-----------|--------|-------|
| Project Count | ? 2 projects | Well under 30-project threshold |
| Dependency Structure | ? Linear | Single dependency: SDO ? Core |
| Framework Homogeneity | ? All net48 | Consistent starting point |
| Package Complexity | ? None | No packages to update |
| Risk Profile | ? Low | Both projects rated low difficulty |

### Execution Approach

All project files and code changes will be applied as a **single atomic operation**:

1. **Convert both projects to SDK-style** simultaneously
2. **Update target frameworks** for both projects at once
3. **Restore dependencies** and build entire solution
4. **Fix all compilation errors** from API changes in one pass
5. **Verify solution builds** with 0 errors

### Dependency-Based Ordering

Even with All-At-Once approach, the logical order for understanding and addressing issues:

```
Phase 1 (Leaf Node):     OpenADK-NET-Core.csproj    ? net10.0-windows
                                ?
Phase 2 (Root Node):     OpenADK-US-SDO.csproj      ? net10.0
```

**OpenADK-NET-Core** is the foundation project (no dependencies, 1 dependant).
**OpenADK-US-SDO** depends on OpenADK-NET-Core.

### No Intermediate States

With All-At-Once strategy:
- ? No partial upgrades
- ? No multi-targeting
- ? Both projects upgraded together
- ? Single comprehensive testing phase
- ? Clean dependency resolution

---

## Detailed Dependency Analysis

### Dependency Graph

```
???????????????????????????????????????????????????????????????
?                    OpenADK-NET-US Solution                  ?
???????????????????????????????????????????????????????????????
?                                                             ?
?   ???????????????????????                                   ?
?   ?  OpenADK-US-SDO     ?  (Root - ClassicClassLibrary)     ?
?   ?  net48 ? net10.0    ?                                   ?
?   ?  167,819 LOC        ?                                   ?
?   ?  1,568 API issues   ?                                   ?
?   ???????????????????????                                   ?
?              ?                                              ?
?              ? depends on                                   ?
?              ?                                              ?
?   ???????????????????????                                   ?
?   ?  OpenADK-NET-Core   ?  (Leaf - ClassicWinForms)         ?
?   ?  net48 ? net10.0-win?                                   ?
?   ?  75,519 LOC         ?                                   ?
?   ?  379 API issues     ?                                   ?
?   ???????????????????????                                   ?
?                                                             ?
???????????????????????????????????????????????????????????????
```

### Project Groupings

| Group | Projects | Framework Target | Notes |
|-------|----------|------------------|-------|
| **Atomic Upgrade** | OpenADK-NET-Core, OpenADK-US-SDO | net10.0-windows, net10.0 | All projects upgraded simultaneously |

### Critical Path

The critical path is simple:
1. OpenADK-NET-Core must be functional for OpenADK-US-SDO to compile
2. Both are upgraded atomically, so dependency order is handled implicitly

### Circular Dependencies

**None detected** - Clean linear dependency structure

---

## Project-by-Project Plans

### OpenADK-NET-Core

#### Project Info

| Attribute | Value |
|-----------|-------|
| **Project Path** | `src/core/OpenADK-NET-Core.csproj` |
| **Project Kind** | ClassicWinForms |
| **Current Framework** | net48 |
| **Target Framework** | net10.0-windows |
| **SDK-style** | False ? True (conversion required) |
| **Lines of Code** | 75,519 |
| **Files** | 437 |
| **Files with Issues** | 162 |
| **Estimated LOC Impact** | 379+ |
| **Dependencies** | 0 |
| **Dependants** | 1 (OpenADK-US-SDO) |
| **Risk Level** | ?? Low |

#### Current State

- Classic WinForms project format (non-SDK style)
- Targets .NET Framework 4.8
- Contains Windows Forms components
- Uses legacy APIs: CAS, BinaryFormatter, ConfigurationManager, legacy cryptography

#### Target State

- SDK-style project format
- Targets net10.0-windows (Windows-specific for WinForms support)
- All CAS attributes removed
- Modern API equivalents for deprecated APIs

#### Migration Steps

**Step 1: SDK-Style Conversion**
- Convert classic `.csproj` to SDK-style format
- Remove legacy elements (AssemblyInfo.cs content moves to project properties)
- Enable Windows Forms: `<UseWindowsForms>true</UseWindowsForms>`

**Step 2: Target Framework Update**
- Update `<TargetFramework>` to `net10.0-windows`

**Step 3: Code Access Security Removal (280 instances)**
Remove all `SecurityPermissionAttribute` usages:
```csharp
// REMOVE lines like:
[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
```

**Step 4: BinaryFormatter Migration (7 instances)**
Replace `BinaryFormatter` with modern serialization:
```csharp
// FROM:
BinaryFormatter formatter = new BinaryFormatter();
formatter.Serialize(stream, obj);

// TO:
JsonSerializer.Serialize(stream, obj);
// Or use System.Text.Json / protobuf depending on use case
```

**Step 5: ICertificatePolicy Replacement (4 instances)**
Replace with `ServerCertificateCustomValidationCallback`:
```csharp
// FROM:
ServicePointManager.CertificatePolicy = new CustomPolicy();

// TO:
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
};
```

**Step 6: Legacy Cryptography Updates (5 instances)**
| Old API | Replacement |
|---------|-------------|
| `RC2CryptoServiceProvider` | `RC2.Create()` |
| `TripleDESCryptoServiceProvider` | `TripleDES.Create()` |
| `DESCryptoServiceProvider` | `DES.Create()` (or migrate to AES) |
| `MD5CryptoServiceProvider` | `MD5.Create()` |
| `SHA1Managed` | `SHA1.Create()` |

**Step 7: ConfigurationManager (4 instances)**
Add NuGet package reference:
```xml
<PackageReference Include="System.Configuration.ConfigurationManager" Version="9.0.0" />
```

**Step 8: Exception Serialization Constructors**
Mark as obsolete or remove serialization constructors:
```csharp
// Remove or mark obsolete:
[Obsolete("This API supports obsolete formatter-based serialization.")]
protected MyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
```

#### Validation Checklist

- [ ] Project converted to SDK-style format
- [ ] TargetFramework set to net10.0-windows
- [ ] UseWindowsForms enabled
- [ ] All SecurityPermissionAttribute removed (280 instances)
- [ ] BinaryFormatter replaced (7 instances)
- [ ] ICertificatePolicy replaced (4 instances)
- [ ] Legacy crypto APIs updated (5 instances)
- [ ] ConfigurationManager package added (4 usages)
- [ ] Project builds without errors
- [ ] Project builds without warnings

---

### OpenADK-US-SDO

#### Project Info

| Attribute | Value |
|-----------|-------|
| **Project Path** | `src/us/OpenADK-US-SDO.csproj` |
| **Project Kind** | ClassicClassLibrary |
| **Current Framework** | net48 |
| **Target Framework** | net10.0 |
| **SDK-style** | False ? True (conversion required) |
| **Lines of Code** | 167,819 |
| **Files** | 1,030 |
| **Files with Issues** | 786 |
| **Estimated LOC Impact** | 1,568+ |
| **Dependencies** | 1 (OpenADK-NET-Core) |
| **Dependants** | 0 |
| **Risk Level** | ?? Low |

#### Current State

- Classic Class Library project format (non-SDK style)
- Targets .NET Framework 4.8
- Depends on OpenADK-NET-Core
- Uses Code Access Security extensively (1,568 instances)

#### Target State

- SDK-style project format
- Targets net10.0
- All CAS attributes removed
- Reference to upgraded OpenADK-NET-Core

#### Migration Steps

**Step 1: SDK-Style Conversion**
- Convert classic `.csproj` to SDK-style format
- Remove legacy elements
- Maintain project reference to OpenADK-NET-Core

**Step 2: Target Framework Update**
- Update `<TargetFramework>` to `net10.0`

**Step 3: Code Access Security Removal (1,568 instances)**
Remove all `SecurityPermissionAttribute` usages across 786 files:
```csharp
// REMOVE lines like:
[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
```

This is the bulk of the work for this project - all 1,568 issues are CAS-related and simply need removal.

**Step 4: Update Project Reference**
Ensure project reference to OpenADK-NET-Core is correctly formatted for SDK-style:
```xml
<ProjectReference Include="..\core\OpenADK-NET-Core.csproj" />
```

#### Validation Checklist

- [ ] Project converted to SDK-style format
- [ ] TargetFramework set to net10.0
- [ ] All SecurityPermissionAttribute removed (1,568 instances)
- [ ] Project reference to OpenADK-NET-Core valid
- [ ] Project builds without errors
- [ ] Project builds without warnings

---

## Package Update Reference

### NuGet Package Status

**No NuGet package updates required.**

The assessment found 0 NuGet packages in the solution that need updating.

### Packages to Add

| Package | Version | Projects | Reason |
|---------|---------|----------|--------|
| `System.Configuration.ConfigurationManager` | 9.0.0+ | OpenADK-NET-Core | Required for `ConfigurationManager.AppSettings` usage |

---

## Breaking Changes Catalog

### Binary Incompatible APIs (8 instances)

These APIs have been removed and require code changes:

| API | Count | Project | Migration |
|-----|-------|---------|-----------|
| `System.Net.ICertificatePolicy` | 4 | Core | Replace with `HttpClientHandler.ServerCertificateCustomValidationCallback` |
| `System.IO.DirectoryInfo.FullName` | 3 | Core | Now returns normalized paths; verify path handling logic |
| `System.Net.ServicePointManager.CertificatePolicy` | 1 | Core | Use `HttpClientHandler` instead |

### Source Incompatible APIs (1,892 instances)

These APIs compile but produce errors/warnings requiring attention:

| API | Count | Category | Migration |
|-----|-------|----------|-----------|
| `SecurityPermissionAttribute` | 1,848 | CAS | **Remove entirely** - not supported in .NET Core |
| `BinaryFormatter` | 7 | Serialization | Replace with `System.Text.Json` or similar |
| `TimeSpan.FromMilliseconds(double)` | 4 | Precision | May have precision differences; verify behavior |
| `ApplicationException` serialization ctor | 4 | Serialization | Remove or mark obsolete |
| `Exception` serialization ctor | 4 | Serialization | Remove or mark obsolete |
| `FormatterAssemblyStyle` | 3 | Serialization | Remove with BinaryFormatter |
| `ConfigurationManager` | 4 | Configuration | Add NuGet package |
| `TimeZone` | 2 | DateTime | Replace with `TimeZoneInfo` |
| `TimeSpan.FromSeconds(double)` | 2 | Precision | Verify precision requirements |
| `SystemException` serialization ctor | 2 | Serialization | Remove or mark obsolete |
| `NameValueCollection` serialization ctor | 2 | Serialization | Remove or mark obsolete |
| `Exception.GetObjectData` | 1 | Serialization | Remove or mark obsolete |
| `Type.IsSerializable` | 1 | Reflection | Returns different values; check usage |
| `WebRequest.Create` | 1 | Networking | Replace with `HttpClient` |
| Legacy Crypto APIs | 5 | Cryptography | Use `*.Create()` factory methods |

### Behavioral Changes (47 instances)

These APIs work but may behave differently:

| API | Count | Behavior Change |
|-----|-------|-----------------|
| `System.Uri` | 37 | URI parsing/normalization differences |
| `Uri.AbsoluteUri` | 5 | May return different string for some URIs |
| `Uri.#ctor(string)` | 3 | Different handling of malformed URIs |
| `Uri.AbsolutePath` | 2 | Path encoding differences |

**Recommendation:** Test all URI handling thoroughly at runtime.

---

## Risk Management

### Risk Assessment Summary

| Risk Level | Project | Description |
|------------|---------|-------------|
| ?? Low | OpenADK-NET-Core | All issues have clear migration paths |
| ?? Low | OpenADK-US-SDO | Only CAS removal required |

### Identified Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| SDK-style conversion issues | Medium | Low | Tool-assisted conversion; manual verification |
| BinaryFormatter data compatibility | High | Low | Test serialization/deserialization with existing data |
| URI behavioral differences | Medium | Medium | Comprehensive testing of URI handling |
| WinForms compatibility | Low | Low | WinForms fully supported in .NET 10 |

### Contingency Plans

**If SDK-style conversion fails:**
- Manually create new SDK-style project
- Copy over source files and settings
- Verify all references and properties

**If BinaryFormatter replacement causes data issues:**
- Implement dual-read capability (old format + new format)
- Create data migration utility
- Consider temporary `EnableUnsafeBinaryFormatterSerialization` (not recommended for production)

**If build errors exceed expectations:**
- Address errors in dependency order (Core first, then SDO)
- Use `#if NET10_0_OR_GREATER` for conditional compilation if needed

---

## Testing & Validation Strategy

### Phase 1: Build Validation

After atomic upgrade operation completes:

- [ ] Both projects convert to SDK-style successfully
- [ ] Solution restores packages without errors
- [ ] Solution builds without errors
- [ ] Solution builds without warnings (or only expected warnings)

### Phase 2: Functional Validation

- [ ] All existing functionality works as expected
- [ ] Serialization/deserialization operations work
- [ ] Configuration reading works
- [ ] Cryptographic operations produce correct results
- [ ] URI handling behaves as expected

### Phase 3: Runtime Verification

Focus areas for behavioral changes:
- [ ] Test URI parsing with edge cases
- [ ] Verify path handling (DirectoryInfo.FullName)
- [ ] Test certificate validation scenarios
- [ ] Verify TimeSpan precision requirements

### No Test Projects

**Note:** The assessment did not identify any test projects in this solution. Consider adding unit tests post-migration for regression prevention.

---

## Complexity & Effort Assessment

### Per-Project Complexity

| Project | Complexity | Rationale |
|---------|------------|-----------|
| OpenADK-NET-Core | ?? Low | Most issues are mechanical (CAS removal); few complex changes |
| OpenADK-US-SDO | ?? Low | 100% CAS removal - simple find-and-delete |

### Overall Assessment

| Factor | Assessment |
|--------|------------|
| **Technical Complexity** | Low - All issues have documented solutions |
| **Risk Level** | Low - No security vulnerabilities, no complex packages |
| **Scope** | Moderate - 1,947 LOC changes but mostly repetitive |
| **Dependencies** | Simple - Linear dependency, no external complications |

### Effort Distribution (Relative)

| Activity | Relative Effort | Notes |
|----------|-----------------|-------|
| SDK-style conversion | Low | Tool-assisted |
| CAS attribute removal | Medium | High count but repetitive |
| API migrations | Low | Only 100 non-CAS issues |
| Build verification | Low | Straightforward |
| Testing | Medium | Behavioral changes need verification |

---

## Source Control Strategy

### Branch Strategy

| Branch | Purpose |
|--------|---------|
| `main` | Source branch (stable) |
| `upgrade-to-NET10` | Upgrade target branch (created) |

### Commit Strategy

**Single Commit Approach (Recommended for All-At-Once)**

All upgrade changes should be committed as a single atomic commit:

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

### Review and Merge Process

1. **Complete all changes** on `upgrade-to-NET10` branch
2. **Verify build** passes with 0 errors
3. **Run validation** tests
4. **Create Pull Request** to `main`
5. **Code Review** focusing on:
   - API replacement correctness
   - No unintended changes
   - Build configuration correctness
6. **Merge** after approval

---

## Success Criteria

### Technical Criteria

- [ ] All projects target .NET 10.0 (Core: net10.0-windows, SDO: net10.0)
- [ ] All projects use SDK-style format
- [ ] Solution builds with 0 errors
- [ ] Solution builds with 0 warnings (or documented acceptable warnings)
- [ ] All CAS attributes removed (1,848 instances)
- [ ] All binary incompatible APIs addressed (8 instances)
- [ ] All source incompatible APIs addressed (1,892 instances)
- [ ] No security vulnerabilities introduced

### Quality Criteria

- [ ] Code quality maintained (no regression in functionality)
- [ ] All API replacements use recommended modern equivalents
- [ ] Serialization compatibility verified
- [ ] URI handling verified

### Process Criteria

- [ ] All-At-Once strategy followed (single atomic upgrade)
- [ ] Single commit captures all changes
- [ ] Source control strategy followed
- [ ] Plan documentation complete and accurate

### Definition of Done

The .NET 10 upgrade is **complete** when:

1. ? Both projects successfully upgraded to .NET 10.0
2. ? Solution builds and all functionality verified
3. ? All changes committed to `upgrade-to-NET10` branch
4. ? Ready for pull request and code review
