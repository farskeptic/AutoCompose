# Test Code Snippets

This directory contains test input files for the AutoCompose source generator.

## File Naming Convention

- **M##**: Method-related tests
- **P##**: Property-related tests  
- **C##**: Class-related tests (multiple classes, multiple interfaces)
- **X##**: Edge cases and special scenarios
- **A##**: Attribute-related tests

## Quick Reference

When reviewing snapshot test results (`.received.cs` files), use this guide:

### Should Generate Code (NOT Empty)
- ✅ Method/property doesn't exist in class
- ✅ Method/property exists but has wrong signature (parameters, return type)
- ✅ Method/property exists but has wrong access modifier (e.g., `protected` instead of `public`)
- ✅ Method/property exists but doesn't satisfy interface contract

### Should NOT Generate Code (Empty File)
- ❌ All interface members are already properly implemented
- ❌ Class doesn't have `[AutoCompose]` attribute
- ❌ Class uses a different attribute (e.g., `[Decoy]`)

## Detailed Expectations

See `../TEST_EXPECTATIONS.md` for detailed explanations of what each test should generate.

