# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres
to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.17.2] - 9/2/2021

### Added

- `CollectionUtils.FirstIndexOf`

## [2.17.1] - 9/2/2021

### Added

- **Prettification!** Which includes:
  - `IPrettifier`
  - `Prettifier`
  - `PrettificationSettings`
  - `Prettification`

  And currently supports prettification of:
  - `Type`
  - `IDictionary`
  - `KeyedList`
  - `IEnumerable`
  - `Tuple<T1,T2>`

## [2.16.2] - 9/2/2021

### Changed

- Changed types of `Optional.FirstWithValue` functions from `IOptional<T>` -> `Optional<T>`

## [2.16.1] - 9/2/2021

### Changed

- Renamed `IgnoreAll` -> `Ignore`

## [2.15.1] - 9/2/2021

### Changed

- Renamed `KeyedList` -> `PrimaryKeyedList`
- `KeyedList` is now a `KeyedCollection` implementation that doesn't require `IPrimaryKeyed` members

## [2.14.3] - 9/2/2021

### Fixed

- Corrected missing implementation of `IReadOnlyCollection` in `FailableFunc<TValue>`

## [2.14.2] - 9/2/2021

### Added

- `CollectionUtils.Find()` methods that return `Optional<T>`s

## [2.14.1] - 9/2/2021

### Changed

- Added `IReadOnlyCollection<T>` to `IOptional<T>`
- Added `[CanBeNull]` to `IOptional<T>.Value`

### Fixed

- Equality comparisons for `null` `IOptional<T>`s and `IOptional<T>`s _containing_ `null`

## [2.13.4] - 8/31/2021

### Changed

- Renamed `Optional.FirstOf` -> `Optional.FirstWithValue`

## [2.13.3] - 8/31/2021

### Added

- `Optional.FirstOf` methods that try a bunch of `Func<>`s until one of returns an `IOptional` that `.HasValue`

## [2.13.2] - 8/31/2021

### Added

- `ReflectionUtils.IsTupleType`
- `ReflectionUtils.IsGenericTypeOrDefinition`

## [2.13.1] - 8/24/2021

### Added

- `Assertable`, an `IFailable` implementation with some extra bits for help with testing
- Builder-style classes for executing multiple assertions:
  - `Asserter`
  - `Assumer`
  - `Ignorer`
- Equivalents of `AssertAll` with different result types - `AssumeAll` and `IgnoreAll`

## [2.12.2] - 8/24/2021

### Added

- Proper versions of `CollectionUtils.AddNonNull` that match `ICollection.Add` instead of `Enumerable.Concat`

### Changed

- Renamed old `CollectionUtils.AddNonNull` to `CollectionUtils.AppendNonNull`

## [2.12.1] - 8/24/2021

### Added

- Extension methods for `Math.Min` and `Math.Max`

### Changed

- Several of the `Mathb.Clamp` extension methods to use `Math` instead of Unity's `Mathf`

## [2.11.1] - 8/24/2021

### Changed

- Moved `Failable` implementations into separate `FailableFunc` files
- Added a no-return-value, non-generic `Failable` implementation as the new `Failable.cs`
- Split `IFailable` apart from `IOptional`
  - `IFailable` doesn't have a return value
  - `IFailableFunc` is the equivalent of the old `IFailable`
- Separated and clarified different `IFailable` interfaces
- Took the `Optional<T>` class out of the `Optional.cs` file and put it into a separate `Optional.T.cs` file

## [2.10.1] - 8/24/2021

### Added

- `StringUtils`
  - A version of `SplitLines()` that operates on `IEnumerable<string>`s
  - A `ToString()` extension method that takes a "null placeholder" value
  - `ToStringLines()`, which recursively converts things into their `ToString()` representations, splits each string
    line-by-line, and flattens the result
- `CollectionUtils`
  - `JoinLinesFlatly()`, which is a recursive equivalent to `JoinLines()`
  - `AddNonNull()`, which adds things that are non-`null`
  - `NonNull()`, which returns things that are non-`null`

### Changed

- `StringUtils.SplitLines()` no longer "collapses" repeated white space

## [2.9.6] - 8/24/2021

### Fixed

- Set `MSBuild` version in `brandon-utils.sln.DotSettings`, which, for some reason, fixes
  the [invalid /langversion '8.0'](https://forum.unity.com/threads/issues-with-running-code-in-rider-and-long-enter-play-mode-times.1114033/#post-7443683)
  issue that prevented executing NUnit standalone tests

## [2.9.5] - 8/23/2021

### Added

- A version of `Bloop.ForEach()` processes each entry AND its index

## [2.9.4] - 8/23/2021

### Added

- `int` extension methods for `Enumerable.Repeat`

## [2.9.3] - 8/23/2021

### Added

- Words to the dictionary

## [2.9.2] - 8/18/2021

### Added

- A ton of documentation to `IOptional` extensions
- `ContractAnnotation` attributes to `IOptional.OrElseThrow()` methods
- `DayOfWeekExtensions`, which includes completely useless methods like `IsSchoolNight()`
-

### Changed

- Pulled `IFailable` into its own class
- Renamed the bizarre `IOptional.IfOrElse` -> `IOptional.IfPresentOrElse`

### Fixed

- Properly put `Failable` implementations into the `Optional` namespace, I think?

## [2.9.1] - 8/16/2021

### Added

- `TimeUnitExtensions`
- `TimeSpanExtensions`, but I dunno if I'm really committed to it yet

### Changed

- Moved `TimeUtils` into the new `BrandonUtils.Standalone.Chronic` package
- Replaced `TimeUtils.DateTimeIntervalType` enum with `TimeUnit`

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