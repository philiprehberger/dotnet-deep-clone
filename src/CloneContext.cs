using System.Runtime.CompilerServices;

namespace Philiprehberger.DeepClone;

/// <summary>
/// Tracks visited objects during deep cloning to handle circular references.
/// Uses reference equality to detect objects that have already been cloned.
/// </summary>
public sealed class CloneContext
{
    private readonly Dictionary<object, object> _visited = new(ReferenceEqualityComparer.Instance);

    /// <summary>
    /// Attempts to get a previously cloned instance for the given source object.
    /// </summary>
    /// <param name="source">The original object to look up.</param>
    /// <param name="clone">The previously cloned instance, if found.</param>
    /// <returns><c>true</c> if the object has already been cloned; otherwise, <c>false</c>.</returns>
    public bool TryGetClone(object source, out object? clone)
    {
        return _visited.TryGetValue(source, out clone);
    }

    /// <summary>
    /// Registers a cloned instance for the given source object.
    /// </summary>
    /// <param name="source">The original object.</param>
    /// <param name="clone">The cloned instance.</param>
    public void Register(object source, object clone)
    {
        _visited[source] = clone;
    }
}
