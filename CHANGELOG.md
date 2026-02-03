# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.0.0] - 2026-02-03

Major modernization of the codebase, bringing the application from .NET Framework to .NET 10 with extensive code and UI improvements.

### Added

- Light/dark theme support with restart-to-apply behaviour
- High-DPI support with configurable DPI mode setting:
  - **Per-Monitor** - Each monitor uses its own DPI scale; the application rescales when moved between monitors
  - **System** - Uses the DPI of the primary monitor for all displays
  - **Disabled** - No DPI scaling applied
- Option to hide the help text panel
- Dynamic listbox widths and aligned Settings dropdowns
- Unicode toolbar buttons

### Changed

- Retargeted from .NET Framework 4.0 to .NET 10 (`net10.0-windows`)
- Migrated all 3 projects (~31,000 LOC) from classic `.csproj` to SDK-style format
- All forms switched to `AutoScaleMode.Dpi` with 96 DPI baseline for consistent scaling
- Modern fonts: Segoe UI, Consolas, and Segoe UI Symbol
- Theme-aware system colors replacing hardcoded colour values
- C# records for data classes (`CellIdVobId`, `DemuxResult`, `EncodeMatch`, `CreateSubOptions`, `PartOfTitle`)
- `BinaryPrimitives` for big-endian binary reads/writes, replacing manual bit shifting
- `Span<T>` and managed arrays replacing `unsafe` pointer code
- Task-based `ReadAsync` replacing APM `BeginRead`/`EndRead`
- `using` declarations and `Dispose()` replacing manual `Close()`/`try-finally`
- File-scoped namespaces and collection expressions
- `FrozenDictionary` for static lookup tables
- `SortedSet` replacing `SortedDictionary` with null values
- Global usings to reduce per-file boilerplate
- Switch expressions, pattern matching, string interpolation, null-conditional event invocation

### Removed

- Legacy ClickOnce and bootstrapper properties
- Visual Studio installer project
- `GC.Collect()` calls and OOM-retry allocation patterns
- Dead code and unused project files
- OcrMap seed build steps that required a local `OcrMap.bin` file to build from source

### Fixed

- Various layout fixes and control anchoring improvements

## [1.0.3.2-beta] - 2013-01-26

Final release by Christopher R Meadowcroft on CodePlex. Targeted .NET Framework 4.0.

[Unreleased]: https://github.com/daisy-the-rabbit/SubExtractor/compare/v2.0.0...HEAD
[2.0.0]: https://github.com/daisy-the-rabbit/SubExtractor/releases/tag/v2.0.0
[1.0.3.2-beta]: https://web.archive.org/web/20171224054057/http://subextractor.codeplex.com/
