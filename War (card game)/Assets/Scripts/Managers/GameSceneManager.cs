using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    internal static GameSceneManager Instance;

    private void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;    
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
}
