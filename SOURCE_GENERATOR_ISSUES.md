# Source generator follow-up notes

These are the issues identified while validating the enum extension generator changes. They were recorded separately because the generator itself should be fixed in its own follow-up commit rather than bundled with the regression tests.

## 1) Mixed enum discovery can be inconsistent

The generator currently combines:
- source-discovered enums from the syntax tree
- externally configured enums from `build_property.EnumExtensionTypes`

This creates a risk of duplicate processing or silent skipping when the same enum is discovered in both ways, when metadata names contain whitespace, or when a configured enum is not accessible from the assembly.

Relevant code:
- `Neme.Extensions.SourceGenerators/Neme.Extensions.SourceGenerators/EnumExtensionsGenerator.cs`

Recommended follow-up:
- normalize and de-dupe metadata names before generation
- validate access and type-kind checks consistently for both code paths
- add a regression test that includes a combination of discovered and configured enums

## 2) Generated formatting is framework-sensitive

The generated `ToStringGenerated` implementation contains conditional output based on target framework and compiler/runtime capabilities. This is especially visible in the flag-formatting path, which uses interpolation and string builder logic with framework-specific branches.

This makes the generator more sensitive to SDK-version and target-TFM differences than a pure generator should be, and it increases the chance of invalid or subtly changing output across multi-target builds.

Relevant code:
- `Neme.Extensions.SourceGenerators/Neme.Extensions.SourceGenerators/EnumGenerators/EnumToStringGenerator.cs`
- `Neme.Extensions.SourceGenerators/Neme.Extensions.SourceGenerators/Internal/DefaultInterpolatedStringHandlerExtensions.cs`
- `Neme.Extensions.SourceGenerators/Neme.Extensions.SourceGenerators/Internal/DefaultInterpolatedStringHandlerCompat.cs`

Recommended follow-up:
- isolate the framework-specific branches behind explicit compatibility helpers
- verify the generated output against the supported target frameworks in a dedicated compatibility test suite
- include a regression test for both flags and non-flags enums across the target matrix

## 3) Compatibility shims should be handled as a distinct change

The compatibility layer added around the interpolated string handling indicates this was not just a logic change; it was a compatibility fix for the compiler/analyzer runtime surface.

That kind of fix should not be mixed into the test-only validation work. It needs a dedicated commit so the behavioral change and the compatibility workaround can be reviewed independently.

Recommended follow-up:
- land the compatibility fix in a separate commit from the test additions
- document the supported SDK/framework matrix in the generator README or analyzer docs
- keep the generator output contract stable across the supported targets

## Suggested split for the eventual fix

1. Keep the regression tests as the accepted part of this work.
2. Revert or isolate the generator behavior changes separately.
3. Land a dedicated generator bug-fix PR that includes:
   - normalization and deduplication for configured enums
   - explicit compatibility shims for target-framework differences
   - regression tests for the discovered issues above

These findings should be treated as a follow-up, not as part of the current test-only branch.
