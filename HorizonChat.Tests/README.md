# HorizonChat.Tests

Unit tests for the HorizonChat application.

## Test Coverage

### GuestNameGeneratorTests (25 tests)
Tests for the guest name generation functionality:
- ✅ Name format validation (AdjectiveNoun#### or Guest####)
- ✅ Uniqueness verification across multiple generations
- ✅ Character validation (capital letters, no spaces)
- ✅ Length constraints (≤20 characters)
- ✅ Pattern matching with Theory tests for various formats
- ✅ Stress test (1000 generations)
- ✅ Multiple instance coordination

### UsernameServiceTests (14 tests)
Tests for username storage and management:
- ✅ Setting and retrieving usernames from localStorage
- ✅ Caching behavior
- ✅ Input validation (empty/null checks)
- ✅ Clear username functionality
- ✅ Event notifications (OnUsernameChanged)
- ✅ Error handling for JavaScript exceptions
- ✅ Mock verification of localStorage interactions

## Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with coverage (requires coverlet)
dotnet test /p:CollectCoverage=true
```

## Test Results

**Total Tests:** 39  
**Passed:** 39 ✅  
**Failed:** 0  
**Skipped:** 0

## Dependencies

- xUnit 2.9.2
- Moq 4.20.72 (for mocking IJSRuntime)
- Microsoft.NET.Test.Sdk 17.11.1

## Test Structure

```
HorizonChat.Tests/
├── Services/
│   ├── GuestNameGeneratorTests.cs
│   └── UsernameServiceTests.cs
└── HorizonChat.Tests.csproj
```
