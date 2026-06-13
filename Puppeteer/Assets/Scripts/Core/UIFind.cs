// UIFind.cs  -  Assets/_Project/Scripts/Core
// Recursive find-by-name (includes inactive children). Used to bind UI cheaply.
using UnityEngine;

public static class UIFind
{
    public static Transform Deep(Transform root, string name)
    {
        if (root == null) return null;
        if (root.name == name) return root;
        for (int i = 0; i < root.childCount; i++)
        {
            var r = Deep(root.GetChild(i), name);
            if (r != null) return r;
        }
        return null;
    }
}
