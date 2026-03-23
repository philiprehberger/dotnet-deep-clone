using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Philiprehberger.DeepClone;

/// <summary>
/// Builds and caches compiled expression tree cloners per type.
/// Handles primitives, strings, arrays, lists, dictionaries, classes, and structs.
/// </summary>
public static class CloneCompiler
{
    private static readonly ConcurrentDictionary<Type, Delegate> Cache = new();

    /// <summary>
    /// Gets or compiles a cloner function for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to clone.</typeparam>
    /// <returns>A function that deep clones an instance of <typeparamref name="T"/> using the provided context.</returns>
    public static Func<T, CloneContext, T> GetCloner<T>()
    {
        var cloner = Cache.GetOrAdd(typeof(T), type => CompileCloner<T>());
        return (Func<T, CloneContext, T>)cloner;
    }

    /// <summary>
    /// Gets or compiles a cloner delegate for the specified type at runtime.
    /// </summary>
    /// <param name="type">The type to clone.</param>
    /// <returns>A delegate that accepts (object, CloneContext) and returns a cloned object.</returns>
    internal static Func<object, CloneContext, object> GetClonerForType(Type type)
    {
        if (IsPrimitiveOrImmutable(type))
        {
            return (obj, _) => obj;
        }

        if (type.IsArray)
        {
            return CloneArray(type);
        }

        if (IsGenericList(type))
        {
            return CloneList(type);
        }

        if (IsGenericDictionary(type))
        {
            return CloneDictionary(type);
        }

        return CloneObject(type);
    }

    private static Func<T, CloneContext, T> CompileCloner<T>()
    {
        var type = typeof(T);
        var objectCloner = GetClonerForType(type);
        return (obj, ctx) => (T)objectCloner(obj!, ctx);
    }

    private static bool IsPrimitiveOrImmutable(Type type)
    {
        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid);
    }

    private static bool IsGenericList(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    private static bool IsGenericDictionary(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }

    private static Func<object, CloneContext, object> CloneArray(Type type)
    {
        var elementType = type.GetElementType()!;
        var elementCloner = IsPrimitiveOrImmutable(elementType)
            ? null
            : GetClonerForType(elementType);

        return (obj, ctx) =>
        {
            var source = (Array)obj;
            var length = source.Length;
            var dest = Array.CreateInstance(elementType, length);

            if (elementCloner is null)
            {
                Array.Copy(source, dest, length);
            }
            else
            {
                ctx.Register(obj, dest);
                for (var i = 0; i < length; i++)
                {
                    var element = source.GetValue(i);
                    dest.SetValue(element is null ? null : elementCloner(element, ctx), i);
                }
            }

            return dest;
        };
    }

    private static Func<object, CloneContext, object> CloneList(Type type)
    {
        var elementType = type.GetGenericArguments()[0];
        var elementCloner = IsPrimitiveOrImmutable(elementType)
            ? null
            : GetClonerForType(elementType);
        var addMethod = type.GetMethod("Add")!;
        var countProperty = type.GetProperty("Count")!;
        var indexer = type.GetProperty("Item")!;

        return (obj, ctx) =>
        {
            var clone = Activator.CreateInstance(type)!;
            ctx.Register(obj, clone);
            var count = (int)countProperty.GetValue(obj)!;

            for (var i = 0; i < count; i++)
            {
                var element = indexer.GetValue(obj, [i]);
                var clonedElement = element is null || elementCloner is null
                    ? element
                    : elementCloner(element, ctx);
                addMethod.Invoke(clone, [clonedElement]);
            }

            return clone;
        };
    }

    private static Func<object, CloneContext, object> CloneDictionary(Type type)
    {
        var args = type.GetGenericArguments();
        var keyType = args[0];
        var valueType = args[1];
        var keyCloner = IsPrimitiveOrImmutable(keyType) ? null : GetClonerForType(keyType);
        var valueCloner = IsPrimitiveOrImmutable(valueType) ? null : GetClonerForType(valueType);

        var enumerableType = typeof(IEnumerable<>).MakeGenericType(
            typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType));
        var enumeratorType = typeof(IEnumerator<>).MakeGenericType(
            typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType));
        var kvpType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
        var keyProp = kvpType.GetProperty("Key")!;
        var valueProp = kvpType.GetProperty("Value")!;
        var addMethod = type.GetMethod("Add", [keyType, valueType])!;

        return (obj, ctx) =>
        {
            var clone = Activator.CreateInstance(type)!;
            ctx.Register(obj, clone);

            var getEnumerator = enumerableType.GetMethod("GetEnumerator")!;
            var enumerator = getEnumerator.Invoke(obj, null)!;
            var moveNext = typeof(System.Collections.IEnumerator).GetMethod("MoveNext")!;
            var current = enumeratorType.GetProperty("Current")!;

            try
            {
                while ((bool)moveNext.Invoke(enumerator, null)!)
                {
                    var kvp = current.GetValue(enumerator)!;
                    var key = keyProp.GetValue(kvp);
                    var value = valueProp.GetValue(kvp);

                    var clonedKey = key is null || keyCloner is null ? key : keyCloner(key, ctx);
                    var clonedValue = value is null || valueCloner is null ? value : valueCloner(value, ctx);

                    addMethod.Invoke(clone, [clonedKey, clonedValue]);
                }
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }

            return clone;
        };
    }

    private static Func<object, CloneContext, object> CloneObject(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite)
            .ToArray();

        var propertyCloners = properties.Select(p =>
        {
            var cloner = IsPrimitiveOrImmutable(p.PropertyType)
                ? null
                : GetClonerForType(p.PropertyType);
            return (Property: p, Cloner: cloner);
        }).ToArray();

        return (obj, ctx) =>
        {
            if (ctx.TryGetClone(obj, out var existing))
            {
                return existing!;
            }

            var clone = Activator.CreateInstance(type)!;
            ctx.Register(obj, clone);

            foreach (var (property, cloner) in propertyCloners)
            {
                var value = property.GetValue(obj);
                if (value is null || cloner is null)
                {
                    property.SetValue(clone, value);
                }
                else
                {
                    if (ctx.TryGetClone(value, out var clonedValue))
                    {
                        property.SetValue(clone, clonedValue);
                    }
                    else
                    {
                        property.SetValue(clone, cloner(value, ctx));
                    }
                }
            }

            return clone;
        };
    }
}
