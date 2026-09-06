# Skipping Unavailable Fixtures

**Load when:** a sample set, folder, or Revit localization may be absent on the running machine, and the affected tests must skip, not fail.

A skipped test carries the missing precondition as its reason.
A failed test on the same condition is indistinguishable from a real regression.
Pick the narrowest guard that fits.
Requires the `Nice3point.TUnit.Revit` package.

## Empty data source — no cases, nothing to fail

When a sample folder is missing, return an empty set from the data source.
No cases are generated, and the parameterized tests are absent from the run.
The test body needs no guard code.
This is the default for sample-file fixtures (see `parameterized-fixtures`):

```csharp
public string[] DocumentPaths { get; } = Directory.Exists(samplesPath)
    ? Directory.EnumerateFiles(samplesPath, "*.rvt").ToArray()
    : [];
```

## Runtime guard in a hook

When the precondition is only known at run time, call `Skip.Test(...)` from a `[Before(Test)]` hook.
Every test in the class is skipped when the condition holds.

```csharp
[Before(Test)]
public void SkipWhenNotEnglish()
{
    if (Application.Language != LanguageType.English_USA)
    {
        Skip.Test("Only supported on the English localization");
    }
}
```

`Skip.Test(...)` also works inside a test body when only one case is conditional — for example, a sample that carries no matching elements.

## Attribute-driven skip

When the same condition guards many tests, encode it once as a `SkipAttribute` subclass and tag the tests.
The condition is read from the injected Revit environment.
The check needs no running document.

```csharp
public sealed class EnglishOnlyAttribute() : SkipAttribute("Only supported on the English localization")
{
    public override Task<bool> ShouldSkip(TestRegisteredContext context)
    {
        if (string.IsNullOrEmpty(RevitEnvironment.Language)) return Task.FromResult(false); // English is the default
        return Task.FromResult(RevitEnvironment.Language != nameof(LanguageType.English_USA));
    }
}

[Test]
[EnglishOnly]
public async Task Cities_English_HasExpectedName()
{
    // Arrange
    const string expectedName = "Aberdeen, MD";

    // Act
    var actualName = Application.Cities.Cast<City>().OrderBy(city => city.Name).First().Name;

    // Assert
    await Assert.That(actualName).IsEqualTo(expectedName);
}
```

## Global guard across the assembly

To apply one rule to the whole suite, use a static `[BeforeEvery(Test)]` hook.
For instance, skip any test whose name mentions a localization other than the current one:

```csharp
public sealed class LocalizationSkipConfiguration : RevitApiTest
{
    private static readonly string[] Languages = Enum.GetNames<LanguageType>();

    [BeforeEvery(Test)]
    public static void SkipUnmatchedLocalization(TestContext context)
    {
        var current = Application.Language.ToString();
        foreach (var language in Languages)
        {
            if (!context.Metadata.TestName.Contains(language, StringComparison.OrdinalIgnoreCase)) continue;
            if (!current.Equals(language, StringComparison.OrdinalIgnoreCase))
            {
                Skip.Test($"Only supported on the {language} localization");
            }
            return;
        }
    }
}
```

## Notes

- Prefer the empty-data-source approach for missing files; reserve `Skip.Test` for conditions discovered at run time.
- `RevitEnvironment.Language` reflects the language the injector started Revit with; an empty value means the default (English).
- A skip is not a pass.
  Keep the assertion after the guard.
  A test that runs past the guard verifies behavior.
