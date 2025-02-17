using UnityEngine;

public static class DebugUtil
{
    /// <summary>
    /// Safely gets a component from a GameObject, logging an error if it's missing.
    /// </summary>
    public static bool SafeGetComponent<T>(GameObject gameObject, out T component, bool disableScript = true) where T : Component
    {
        if (gameObject.TryGetComponent(out component))
        {
            return true; // Component found
        }

        Debug.LogError($"[{gameObject.name}] Missing required component: {typeof(T).Name}");

        return false;
    }

    /// <summary>
    /// Safely gets a component from any parent in the hierarchy, logging an error if it's missing.
    /// </summary>
    public static bool SafeGetComponentInParent<T>(GameObject child, out T component) where T : Component
    {
        component = child.GetComponentInParent<T>();

        if (component != null)
        {
            return true; // Component found in the hierarchy
        }

        Debug.LogError($"[{child.name}] No parent with component {typeof(T).Name} found in the hierarchy.");
        return false;
    }

    /// <summary>
    /// Logs an error if the given component is null.
    /// </summary>
    public static void HandleErrorIfNull<T>(T component, Component source) where T : Component
    {
#if UNITY_EDITOR
        if (component == null)
        {
            Debug.LogError($"[{source.gameObject.name}] Expected component of type {typeof(T).Name}, but none were found.");
        }
#endif
    }

    /// <summary>
    /// Recursively finds the first parent of type T (not just the direct parent).
    /// </summary>
    public static T GetFirstParentOfType<T>(GameObject child) where T : Component
    {
        Transform parent = child.transform.parent;

        while (parent != null)
        {
            if (parent.TryGetComponent(out T component))
            {
                return component; // Found the parent with the component
            }
            parent = parent.parent; // Move up the hierarchy
        }

        Debug.LogWarning($"[{child.name}] No parent of type {typeof(T).Name} found.");
        return null; // Return null if no matching parent was found
    }

    /// <summary>
    /// Logs an error if no objects of type T are found in the scene.
    /// </summary>
    public static void HandleErrorIfNullFindObject<T>(T obj, Component source) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (obj == null)
        {
            Debug.LogError($"[{source.gameObject.name}] Expected to find an object of type {typeof(T).Name} in the scene, but none were found.");
        }
#endif
    }

    /// <summary>
    /// Logs an error if no components of type T exist on the specified GameObject.
    /// </summary>
    public static void HandleErrorIfNoComponentFound<T>(int count, Component source, GameObject onObject)
    {
#if UNITY_EDITOR
        if (count == 0)
        {
            Debug.LogError($"[{source.gameObject.name}] Expected at least one component of type {typeof(T).Name} on {onObject.name}, but none were found.");
        }
#endif
    }

    /// <summary>
    /// Logs a warning if multiple components of type T exist on a GameObject.
    /// </summary>
    public static void HandleWarningIfDuplicateObjects<T>(int count, Component source, GameObject onObject)
    {
#if UNITY_EDITOR
        if (count > 1)
        {
            Debug.LogWarning($"[{source.gameObject.name}] Expected only one component of type {typeof(T).Name} on {onObject.name}, but found {count}. Using the first one.");
        }
#endif
    }
}
