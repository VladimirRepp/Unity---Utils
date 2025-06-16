using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// LoadingScene - сцена загрузки, которая использует MySceneManager для асинхронной загрузки
/// </summary>
public class LoadingScene : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Минимальное время показа экрана загрузки (секунды)")]
    [SerializeField] private float _minLoadingTime = 2f;

    [Header("UI Elements")]
    [SerializeField] private Image _progressImg;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private GameObject _loadingCompleteHint;

    private float _loadingStartTime;
    private bool _isLoadingComplete;

    private void Start()
    {
        _loadingStartTime = Time.time;
        _isLoadingComplete = false;
        _loadingCompleteHint.SetActive(false);

        if (_progressBar != null) _progressBar.value = 0f;
        if (_progressText != null) _progressText.text = "0%";

        StartLoading();
    }

    private async void StartLoading()
    {
        // Ждем минимальное время загрузки и саму загрузку параллельно
        var loadingTask = LoadTargetSceneAsync();
        var minTimeTask = Task.Delay((int)(_minLoadingTime * 1000));

        await Task.WhenAll(loadingTask, minTimeTask);

        _isLoadingComplete = true;
        _loadingCompleteHint.SetActive(true);
    }

    private async Task LoadTargetSceneAsync()
    {
        var progress = new System.Progress<float>(UpdateProgressUI);
        await MySceneManager.LoadTargetSceneAsync(progress);
    }

    private void UpdateProgressUI(float progress)
    {
        if (_progressBar != null)
            _progressBar.value = progress;

        if (_progressText != null)
            _progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }

    private void Update()
    {
        // Пример: переход после завершения загрузки по нажатию клавиши или нажатия на экран
        if (_isLoadingComplete && (Input.anyKeyDown || Input.GetMouseButton(0)))
        {
            // Активируем сцену (если еще не активирована)
            var currentOperation = MySceneManager.GetCurrentLoadingOperation();
            if (currentOperation != null && !currentOperation.allowSceneActivation)
            {
                currentOperation.allowSceneActivation = true;
            }
        }
    }
}
