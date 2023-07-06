using UnityEngine;

public class SceneTransitionHandler : MonoBehaviour
{
    public static SceneTransitionHandler Instance;
    public bool InitializeAsHost;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else if (Instance == null)
        {
            Instance = this;
        }
    }
}
