# Philiprehberger.DeepClone

[![CI](https://github.com/philiprehberger/dotnet-deep-clone/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-deep-clone/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.DeepClone.svg)](https://www.nuget.org/packages/Philiprehberger.DeepClone)
[![Last updated](https://img.shields.io/github/last-commit/philiprehberger/dotnet-deep-clone)](https://github.com/philiprehberger/dotnet-deep-clone/commits/main)

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

### Clone with Property Overrides

```csharp
var original = new Person { Name = "Alice", Age = 30 };
var modified = DeepClone.CloneWith(original, p => p.Name = "Bob");
// modified.Name is "Bob", modified.Age is 30
// original.Name is still "Alice"

// Or use the extension method
var modified2 = original.DeepCloneWith(p => p.Age = 25);
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
| `CloneWith<T>(T obj, Action<T> configure)` | Deep clone then apply property overrides |

### Extension Methods

| Method | Description |
|--------|-------------|
| `DeepClone<T>(this T obj)` | Extension method for deep cloning any object |
| `DeepCloneWith<T>(this T obj, Action<T> configure)` | Extension method for deep cloning with property overrides |

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

## Support

If you find this project useful:

⭐ [Star the repo](https://github.com/philiprehberger/dotnet-deep-clone)

🐛 [Report issues](https://github.com/philiprehberger/dotnet-deep-clone/issues?q=is%3Aissue+is%3Aopen+label%3Abug)

💡 [Suggest features](https://github.com/philiprehberger/dotnet-deep-clone/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)

❤️ [Sponsor development](https://github.com/sponsors/philiprehberger)

🌐 [All Open Source Projects](https://philiprehberger.com/open-source-packages)

💻 [GitHub Profile](https://github.com/philiprehberger)

🔗 [LinkedIn Profile](https://www.linkedin.com/in/philiprehberger)

## License

[MIT](LICENSE)
