# Philiprehberger.DeepClone

[![CI](https://github.com/philiprehberger/dotnet-deep-clone/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-deep-clone/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.DeepClone.svg)](https://www.nuget.org/packages/Philiprehberger.DeepClone)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-deep-clone)](LICENSE)
[![Sponsor](https://img.shields.io/badge/sponsor-GitHub%20Sponsors-ec6cb9)](https://github.com/sponsors/philiprehberger)

Fast deep cloning using compiled expression trees for nested objects, collections, and circular references.

## Installation

```bash
dotnet add package Philiprehberger.DeepClone
```

## Usage

### Basic Cloning

```csharp
using Philiprehberger.DeepClone;

var original = new Person { Name = "Alice", Age = 30 };
var clone = DeepClone.Clone(original);

// Or use the extension method
var clone2 = original.DeepClone();
```

### Nested Objects and Collections

```csharp
var team = new Team
{
    Name = "Engineering",
    Members = new List<Person>
    {
        new() { Name = "Alice", Age = 30 },
        new() { Name = "Bob", Age = 25 }
    }
};

var clonedTeam = team.DeepClone();
clonedTeam.Members[0].Name = "Charlie";
// original.Members[0].Name is still "Alice"
```

### Circular References

```csharp
var node = new TreeNode { Value = "root" };
node.Children.Add(new TreeNode { Value = "child", Parent = node });

// Circular references are handled automatically
var clonedTree = node.DeepClone();
```

## API

### `DeepClone`

| Method | Description |
|--------|-------------|
| `Clone<T>(T obj)` | Create a deep clone of the given object |

### Extension Methods

| Method | Description |
|--------|-------------|
| `DeepClone<T>(this T obj)` | Extension method for deep cloning any object |

### Supported Types

| Type | Behavior |
|------|----------|
| Primitives | Passed through (value types) |
| `string` | Passed through (immutable) |
| Arrays | Element-wise deep clone |
| `List<T>` | Element-wise deep clone |
| `Dictionary<K, V>` | Key/value deep clone |
| Classes | Property-by-property deep clone |
| Structs | Property-by-property deep clone |

## Development

```bash
dotnet build src/Philiprehberger.DeepClone.csproj --configuration Release
```

## License

[MIT](LICENSE)
