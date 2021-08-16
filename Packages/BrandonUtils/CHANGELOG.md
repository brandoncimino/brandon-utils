# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres
to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.8.1] - 8/15/2021

### Added

- `PrimitiveUtils`:
  - `IsPositive`
  - `IsStrictlyPositive`
  - `IsNegative`

## [2.8.0] - 8/8/2021

### Changed

## [2.7.2] - 8/1/2021

### Added

- `CollectionUtils.AddIfMissing()`

## [2.7.1] - 7/31/2021

### Changed

- Switched `SaveData.JsonSerializationSettings` from a `readonly` field to a `{get; protected set;}` property

## [2.7.0] - 7/7/2021

### Added

- `Guardian` and `Dependant`
- Some fancy NuGet packages!
    - `JetBrains.Annotations.dll`
    - `Humanizer.dll`

## [2.6.2] - 7/5/2021

### Added

- Finally committed to `CloseTo` aliases for `Approximately`

## [2.6.1] - 7/5/2021

### Added

- `Clusivity` enum
- `ClusivityUtils`
- `Is`, which is an "extension" of NUnit's `Is` but includes my custom assertions

### Changed

- Improved `ApproximationConstraint`
    - Moved default value setting into `ApproximationConstraint` constructors
    - Removed generic factory methods
    - Used new fancy new `Coercively` class
    - Added ability to specify the `Clusivity` of the range

## [2.6.0] - 7/4/2021

### Added

- The sexy new `Coercively` class, which replaces `CoercionUtils`!

### Deprecated

- `CoercionUtils`

## [2.5.3] - 7/4/2021

### Added

- `CollectionUtils.ContainsAll()`
- `CollectionUtils.ContainsAny()`
- `CollectionUtils.ContainsNone()`
- `CollectionUtils.SupersetOf()`
- `CollectionUtils.SubsetOf()`

### Changed

- Improved stack trace display in `AssertAll.Of()`

## [2.5.2] - 6/30/2021

### Changed

- Added some additional `[NotNull]` attributes

## [2.5.1] - 6/30/2021

### Added

- `IOptional<T>` / `Nullable<T>` extension methods:
    - `.IsEmpty()`
    - `.OrElseThrow()`

## [2.5.0] - 6/29/2021

### Added

- Generic `ISaveData<T>` interface
- Ability to reset `SaveData` values using some fancy `JsonConvert` shenanigans

### Changed

- `ReflectionUtils.Construct()` can now find private constructors
- `ISaveData` timestamps are now `DateTime?`
- `SaveData` no longer initializes timestamps

## [2.4.0] - 6/29/2021

### Added

- `RegexUtils`
- `CollectionUtils.None()`
- `EnumUtils.Parse()`
- `Optional.IfOrElse()`

## [2.3.1] - 6/29/2021

### Added

- Trimmed stack traces to `AssertAll.Of()`
- Words to the dictionary

## [2.3.0] - 6/29/2021

### Added

- `PrimitiveUtils` - bold extensions for "primitive" types like `int`

## [2.2.0] - 6/29/2021

### Added

- New `StringUtils`
    - `SplitLines()` - splits a string by line breaks
    - `CollapseLines()` - replaces groups of lines matching a predicate with `...`
    - `TruncateLines()`
    - `StringFilter` - a simple holder for multiple `Contains()` substrings and `Regex` patterns
    - `ContainsAll()`
    - `ContainsAny()`
    - `ContainsNone()`

## [2.1.0] - 6/26/2021

### Changed

- `SaveData.Reload()` became `virtual`

## [2.0.0] - 6/26/2021

### Added

- `ReflectionUtils.Construct` methods
- Ability to modify messages of existing exceptions while maintaining their types & inner exceptions
    - `ExceptionUtils.ModifyMessage()`
    - `ExceptionUtils.PrependMessage()`
- Easier-to-read alternatives for `.Any()`
    - `CollectionUtils.IsEmpty()`
    - `CollectionUtils.IsNotEmpty()`
- Metric butt-fuckloads of tests
- `Optional<T>`, a horrific parody
  of [Java's Optional](https://docs.oracle.com/javase/8/docs/api/java/util/Optional.html)
    - Base interface, `IOptional<T>`
    - Specialized version, `Failable<T>`
- `Refreshing<T>`, an overly-complicated way to update things once per frame*
    - *Capable of doing other stuff too, but mostly that
- `Mathb.cs`, I don't remember why, but it's probably got some cool stuff in it
- `Bloop.cs`, because I no longer feel anything if it isn't in a lambda expression
- `Brandom.cs`, which lets you pass randomized values in a way that seems more semantic, I guess, but mostly it has a
  cute name
- `AssertAll.Of()`, which works like JUnit 5 / NUnit (the updated version that Unity _doesn't use yet_)'s
  multi-lambda-assertions.

### Changed

- Major changes to `SaveData.cs`:
    - Moved most of the static fields from `SaveData<T>` to a non-generic `SaveData` base class. They can still be
      accessed from the child classes, and some, like `SaveDataFolder`, **must** be, but this cleans up `SaveData<T>` a
      bit
    - Added `ISaveData`, which makes generic (heh) usages of `SaveData.cs` inheritors like `SaveDataException.cs` not
      need to be generic
        - Removed type parameters from `SaveDataException.cs` and `ReSaveDelayException.cs`
    - Made constructing `SaveData<T>` children more strict
        - Removed `new()` constraint from `SaveData<T>`
        - Added a constructor to `SaveData` that requires a `nickname`
        - Constructed children of `SaveData.cs` via reflection with `SaveData.Construct()` instead of with `new()`