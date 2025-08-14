using UnityEngine;

public class UIRootSingleton : MonoBehaviour
{
    static UIRootSingleton inst;
    void Awake()
    {
        if (inst != null) { Destroy(gameObject); return; } 
        inst = this;
        DontDestroyOnLoad(gameObject); 
    }
}
