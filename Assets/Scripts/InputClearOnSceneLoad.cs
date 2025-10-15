using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InputClearOnSceneLoad : MonoBehaviour
{
    void OnEnable()  => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EventSystem.current?.SetSelectedGameObject(null);
        Input.ResetInputAxes();
    }
}