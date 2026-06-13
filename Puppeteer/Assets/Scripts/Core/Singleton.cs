// Singleton.cs  -  Assets/_Project/Scripts/Core
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T I { get; private set; }

    protected virtual void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = (T)this;
    }

    protected virtual void OnDestroy() { if (I == this) I = null; }
}
