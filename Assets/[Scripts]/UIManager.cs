using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class ToggleScreenButton
{
    public List<Button> buttons = new List<Button>();
    public List<GameObject> showGameObjects = new List<GameObject>();
    public List<GameObject> hideGameObjects = new List<GameObject>();
    public UnityEvent showEvents = new UnityEvent();
    public UnityEvent hideEvents = new UnityEvent();

    /// Functions ///

    public void SetupButtons()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(ToggleGameObjects);
        }
    }

    public void ToggleGameObjects()
    {
        if (showGameObjects.Count > 0)
            ShowGameObjects(!showGameObjects[0].activeSelf);
    }

    private void ShowGameObjects(bool show)
    {
        UnityEvent events = show ? showEvents : hideEvents;
        events.Invoke();

        foreach (GameObject go in showGameObjects)
        {
            go.SetActive(show);
        }

        foreach (GameObject go in hideGameObjects)
        {
            go.SetActive(!show);
        }
    }
}

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private List<ToggleScreenButton> toggleButtons = new List<ToggleScreenButton>();

    private void Awake()
    {
        foreach (ToggleScreenButton button in toggleButtons)
        {
            button.SetupButtons();
        }
    }

}
