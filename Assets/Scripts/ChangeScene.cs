﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void ChangeToScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
