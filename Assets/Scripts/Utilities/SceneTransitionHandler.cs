using System;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
