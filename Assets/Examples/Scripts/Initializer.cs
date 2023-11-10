﻿using System;
using System.Collections;
using System.Collections.Generic;
using Hado.ARFoundation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour
{
    [SerializeField] private GameObject ARSessionManagerGameObject;
    
    private void Awake()
    {
        Debug.Log("Load Ready Scene");
        SceneManager.LoadScene("Ready", LoadSceneMode.Single);
    }
}
