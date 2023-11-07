using System.Collections;
using System.Collections.Generic;
using Hado.ARFoundation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour
{
    [SerializeField] private GameObject ARSessionManagerGameObject;
    
    void Awake()
    {
        Debug.Log("===Init===");
        DontDestroyOnLoad(ARSessionManagerGameObject);

        SceneManager.LoadScene("Ready", LoadSceneMode.Single);
    }
}
