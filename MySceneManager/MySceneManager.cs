using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public static class MySceneManager
{
    private static AsyncOperation _currentLoadingOperation;

    public static event UnityAction<float> OnLoadingProgress;   // Событие прогресса загрузки
    public static event UnityAction<Scene> OnSceneLoaded;       // Событие после загрузки сцены
    public static event UnityAction<Scene> OnSceneUnloaded;     // Событие после выгрузки сцены

    private static int _targetSceneId;
    private static string _targetSceneName;
    private static bool _useIdForLoading;

    /// <summary>
    /// Инициализация менеджера сцен (вызывать при старте игры)
    /// </summary>
    public static void Initialize()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        SceneManager.sceneUnloaded += HandleSceneUnloaded;
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnSceneLoaded?.Invoke(scene);
    }

    private static void HandleSceneUnloaded(Scene scene)
    {
        OnSceneUnloaded?.Invoke(scene);
    }

    /// <summary>
    /// Загружает сцену через сцену загрузки
    /// </summary>
    public static void LoadScene(int sceneId)
    {
        _targetSceneId = sceneId;
        _useIdForLoading = true;
        SceneManager.LoadScene("LoadingScene");
    }

    /// <summary>
    /// Загружает сцену через сцену загрузки
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        _targetSceneName = sceneName;
        _useIdForLoading = false;
        SceneManager.LoadScene("LoadingScene");
    }

    /// <summary>
    /// Асинхронно загружает сцену через сцену загрузки
    /// </summary>
    public static async Task LoadSceneAsync(int sceneId, bool activateOnLoad = true)
    {
        _targetSceneId = sceneId;
        _useIdForLoading = true;
        await LoadSceneInternal("LoadingScene", activateOnLoad);
    }

    /// <summary>
    /// Асинхронно загружает сцену через сцену загрузки
    /// </summary>
    public static async Task LoadSceneAsync(string sceneName, bool activateOnLoad = true)
    {
        _targetSceneName = sceneName;
        _useIdForLoading = false;
        await LoadSceneInternal("LoadingScene", activateOnLoad);
    }

    /// <summary>
    /// Загружает целевую сцену (вызывается из сцены загрузки)
    /// </summary>
    public static async Task LoadTargetSceneAsync(IProgress<float> progress = null)
    {
        _currentLoadingOperation = _useIdForLoading
       ? SceneManager.LoadSceneAsync(_targetSceneId)
       : SceneManager.LoadSceneAsync(_targetSceneName);

        _currentLoadingOperation.allowSceneActivation = false;

        while (!_currentLoadingOperation.isDone)
        {
            float loadProgress = Mathf.Clamp01(_currentLoadingOperation.progress / 0.9f);
            progress?.Report(loadProgress);
            OnLoadingProgress?.Invoke(loadProgress);

            if (_currentLoadingOperation.progress >= 0.9f)
            {
                // Сцена загружена на 90%, ждем ручной активации
                break;
            }

            await Task.Yield();
        }
    }

    /// <summary>
    /// Загружает сцену аддитивно
    /// </summary>
    public static async Task LoadSceneAdditiveAsync(string sceneName, bool activateOnLoad = true)
    {
        await LoadSceneInternal(sceneName, activateOnLoad, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Выгружает сцену
    /// </summary>
    public static async Task UnloadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            await Task.Yield();
        }
    }

    /// <summary>
    /// Перезагружает текущую сцену
    /// </summary>
    public static void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Асинхронно перезагружает текущую сцену
    /// </summary>
    public static async Task ReloadCurrentSceneAsync(bool activateOnLoad = true)
    {
        await LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, activateOnLoad);
    }

    /// <summary>
    /// Получает индекс текущей сцены
    /// </summary>
    public static int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    /// <summary>
    /// Получает имя текущей сцены
    /// </summary>
    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Возвращает текущую операцию загрузки сцены
    /// (используется для ручной активации сцены после загрузки)
    /// </summary>
    public static AsyncOperation GetCurrentLoadingOperation()
    {
        return _currentLoadingOperation;
    }

    private static async Task LoadSceneInternal(string sceneName, bool activateOnLoad, LoadSceneMode mode = LoadSceneMode.Single)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, mode);
        operation.allowSceneActivation = activateOnLoad;

        while (!operation.isDone)
        {
            if (!activateOnLoad && operation.progress >= 0.9f)
                break;

            await Task.Yield();
        }
    }
}
