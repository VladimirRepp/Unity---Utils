using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MySceneManager
{
    /// <summary>
    /// Загрузка сцены по коду\индексу. 
    /// Имя сцены для загрузки - будет пустым, так как может быть только 
    /// одно поле которое отвечает за выбор необходимого уровня.
    /// </summary>
    public static void LoadScene(int id)
    {
        LoadingScene.SceneId = id;
        SceneManager.LoadScene("LoadingScene");
    }

    /// <summary>
    /// Загрузка сцены по названи. 
    /// Код сцены для загрузки - будет пустым, так как может быть только 
    /// одно поле которое отвечает за выбор необходимого уровня
    /// </summary>
    public static void LoadScene(string name)
    {
        LoadingScene.SceneName = name;
        SceneManager.LoadScene("LoadingScene");
    }
}
