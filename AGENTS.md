# Repository Overview

osu-sp is an osu! fork with a focus on an improved offline experience, coming with stripped online functionality and extra additions such as clientside medals and custom mods.

## Internals

The engine and main resources are NuGet packages, with local copies provided. In case you need to dissect any of them: refer to either `osu-framework/` or `osu-resources/` in the root directory.

## Coding

- Adhere to established conventions but avoid adding `#nullable disable`.
- Utilize modern C# features (not limited to the following):
  - `is null`/`is not null` for null checking outside of expression trees.
    ```csharp
    if (theVariable is null) return;
    ```
  - Collection expressions (C# 12.0).
    ```csharp
    byte[] numberVault = [];
    
    string[] strings =
    [
        @"123",
        @"777",
    ];
    ```
  - `System.Threading.Lock` objects (C# 13.0) for lock statements.
  - `field` keyword (C# 14.0) for field-backed properties.
    ```csharp
    public string TheString
    {
        get;
        set => field = value != @"NULL" ? value : throw new ArgumentNullException();
    } = string.Empty;
    ```
- Use verbatim strings to avoid unwanted `OLOC001: "string" can be localised` suggestions.
- If an injected dependency can be null (e.g., not set up in `OsuGameBase`/`OsuGame` but created at runtime), specify `canBeNull: true` in `ResolvedAttribute`.
- Prefer Realm's `Filter` method which is **less error-prone** than LINQ methods, especially regarding property access.
- Wrap multiple database operations in a single Realm transaction for **efficiency**.
- **Comments should explain the "why" process, not state the obvious.**

## Building

- The game is developed and tested on PC. Use `dotnet build osu.Desktop.slnf` to build the solution.
- Don't bother with the `Test` build configuration. While unit tests can be compiled, they went through quick and dirty fixes; testing is done manually anyways.
