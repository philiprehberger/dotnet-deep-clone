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
}
