using System.Collections;
using UnityEngine;

// A class to manage ImpactVFX on an active object to allow coroutines when the parent object is disabled
public class ImpactVFXManager : MonoBehaviour
{
    public static ImpactVFXManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Release an object from the pool after a delay
    public void ReleaseAfterTime(GameObject obj, float delay)
    {
        StartCoroutine(ReleaseCoroutine(obj, delay));
    }

    // Apply delay and release the object at the end
    private IEnumerator ReleaseCoroutine(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPoolingManager.Instance.GetOrCreatePool(obj).Release(obj);
    }
}
