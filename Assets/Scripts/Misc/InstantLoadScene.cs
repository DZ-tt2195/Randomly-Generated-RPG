using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;

public class InstantLoadScene : MonoBehaviour
{
    [Scene]
    [SerializeField] string scene;

    void Start()
    {
        SceneManager.LoadScene(scene);
    }
}
