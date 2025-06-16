using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// LoadingScene - самая первая сцена в списке для сборки
/// (так как в ней подгружаются данные и для главного меню)
/// </summary>
public class LoadingScene : MonoBehaviour
{
    private static int SCENE_ID;
    private static string SCENE_NAME;

    [SerializeField] private int _defaultSceneId = 1; // main menu

    private AsyncOperation _asyncOperation;

    /// <summary>
    /// SCENE_NAME будет пустым, так как может быть только 
    /// одно поле которое отвечает за выбор необходимого уровня
    /// </summary>
    public static int SceneId
    {
        get => SCENE_ID;
        set
        {
            SCENE_NAME = "";
            SCENE_ID = value;
        }
    }

    /// <summary>
    /// SCENE_ID будет пустым, так как может быть только 
    /// одно поле которое отвечает за выбор необходимого уровня
    /// </summary>
    public static string SceneName
    {
        get => SCENE_NAME;
        set
        {
            SCENE_NAME = value;
            SCENE_ID = -1;
        }
    }

    private void Start()
    {
        if (SCENE_ID == 0 || SCENE_NAME == "LoadingScene")
            SCENE_ID = _defaultSceneId;

        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine() 
    {
        yield return new WaitForSeconds(1);

        if (SCENE_ID != -1)
        {
            Debug.Log($"LoadingScene.LoadSceneRoutine(): Переход на уровень - {SCENE_ID}");
            _asyncOperation = SceneManager.LoadSceneAsync(SCENE_ID);
        }
        else if (SCENE_NAME != "")
        {
            Debug.Log($"LoadingScene.LoadSceneRoutine(): Переход на уровень - {SCENE_NAME}");
            _asyncOperation = SceneManager.LoadSceneAsync(SCENE_NAME);
        }
        else
        {
            Debug.Log($"LoadingScene.LoadSceneRoutine(): Переход на уровень - {_defaultSceneId}");
            _asyncOperation = SceneManager.LoadSceneAsync(_defaultSceneId);
        }

        // Здесь же можно добавить собственное условие для ожидания
        while (!_asyncOperation.isDone) 
        {
            float progress = _asyncOperation.progress;
            yield return 0;
        }
    }
}
