namespace Philiprehberger.DeepClone;

/// <summary>
/// Provides fast deep cloning using compiled expression trees.
/// Supports nested objects, collections, and circular references.
/// </summary>
public static class DeepClone
{
    /// <summary>
    /// Creates a deep clone of the specified object.
    /// </summary>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <param name="obj">The object to clone.</param>
    /// <returns>A deep clone of the object, or the default value if the input is <c>null</c>.</returns>
    public static T? Clone<T>(T? obj)
    {
        if (obj is null)
        {
            return default;
        }

        var context = new CloneContext();
        var cloner = CloneCompiler.GetCloner<T>();
        return cloner(obj, context);
    }

    /// <summary>
    /// Creates a deep clone of the specified object and applies the configure action on the clone.
    /// </summary>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <param name="obj">The object to clone.</param>
    /// <param name="configure">An action to apply property overrides on the cloned object.</param>
    /// <returns>A deep clone of the object with the configure action applied.</returns>
    public static T CloneWith<T>(T obj, Action<T> configure) where T : class
    {
        var clone = Clone(obj);
        if (clone is not null)
            configure(clone);
        return clone!;
    }
}

/// <summary>
/// Extension methods for deep cloning objects.
/// </summary>
public static class DeepCloneExtensions
{
    /// <summary>
    /// Creates a deep clone of this object using compiled expression trees.
    /// Handles nested objects, collections, and circular references.
    /// </summary>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <param name="obj">The object to clone.</param>
    /// <returns>A deep clone of the object, or the default value if the input is <c>null</c>.</returns>
    public static T? DeepClone<T>(this T? obj)
    {
        return Philiprehberger.DeepClone.DeepClone.Clone(obj);
    }

    /// <summary>
    /// Creates a deep clone of this object and applies the configure action on the clone.
    /// </summary>
    /// <typeparam name="T">The type of the object to clone.</typeparam>
    /// <param name="obj">The object to clone.</param>
    /// <param name="configure">An action to apply property overrides on the cloned object.</param>
    /// <returns>A deep clone of the object with the configure action applied.</returns>
    public static T DeepCloneWith<T>(this T obj, Action<T> configure) where T : class
    {
        return Philiprehberger.DeepClone.DeepClone.CloneWith(obj, configure);
    }
}
