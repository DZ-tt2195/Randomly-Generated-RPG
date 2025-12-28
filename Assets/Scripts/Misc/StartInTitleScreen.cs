using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartInTitleScreen : MonoBehaviour
{
    void Awake()
    {
        if (Translator.inst == null)
            SceneManager.LoadScene(0);
    }

}
