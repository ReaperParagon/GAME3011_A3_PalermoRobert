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
    public bool IsShown;

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
        ShowGameObjects(!IsShown);
    }

    private void ShowGameObjects(bool show)
    {
        IsShown = show;

        UnityEvent events = show ? showEvents : hideEvents;
        events.Invoke();

        foreach (GameObject go in showGameObjects)
        {
            go.SetActive(IsShown);
        }

        foreach (GameObject go in hideGameObjects)
        {
            go.SetActive(!IsShown);
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
