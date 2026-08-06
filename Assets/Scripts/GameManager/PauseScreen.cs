using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[Singleton]
public class PauseScreen : MonoBehaviour
{
    public static PauseScreen _instance;
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            texts = new List<TMP_Text> { ResumeTxt, RestartTxt, MenuTxt, QuitTxt };
            
            underlines = new();
            for (int i = 0; i < texts.Count; i++)
            {
                underlines.Add(GetUnderline(texts[i]));
            }

            gameObject.SetActive(false);
        }
    }

    GameObject GetUnderline(TMP_Text obj) => obj.transform.Find("Underline").gameObject;

    private void OnEnable()
    {
        selectedIndex = 0;
        UpdateTexts();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
            UpdateTexts();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--; 
            UpdateTexts();
        }
        else if (Input.GetKeyDown(InputManager.ShootKey))
        {
            ConfirmChoice();
        }
    }

    void UnPause()
    {
        isPaused = true;
        TogglePause();
    }

    void ForcePause()
    {
        isPaused = false;
        TogglePause();
    }

    bool isPaused = false;
    public bool IsGamePaused => isPaused;
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }

    [SerializeField] TMP_Text ResumeTxt, RestartTxt, MenuTxt, QuitTxt;
    [SerializeField] float defaultSize = 36, highlightSize = 42;
    [SerializeField] Color defaultColor = Color.white, highlightColor = new(1, 0.83f, 0.3f);

    List<TMP_Text> texts;
    List<GameObject> underlines;

    int selectedIndex = 0;
    int GetIndex() => (selectedIndex % texts.Count + texts.Count) % texts.Count;
    void UpdateTexts()
    {
        selectedIndex = GetIndex();
        for (int i = 0; i < texts.Count; ++i)
        {
            TMP_Text text = texts[i];
            if (i == selectedIndex)
            {
                text.fontSize = highlightSize;
                text.fontStyle = FontStyles.Bold;
                text.color = highlightColor;
                underlines[i].SetActive(true);
            }
            else
            {
                text.fontSize = defaultSize;
                text.fontStyle = FontStyles.Normal;
                text.color = defaultColor;
                underlines[i].SetActive(false);
            }
        }
    }

    private int ResumeIndex = 0, RestartIndex = 1, MenuIndex = 2, QuitIndex = 3;
    void ConfirmChoice()
    {
        selectedIndex = GetIndex();
        if (selectedIndex == ResumeIndex)
        {
            UnPause();
        }
        else if (selectedIndex == RestartIndex)
        {
            UnPause();
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        else if (selectedIndex == MenuIndex)
        {

        }
        else 
        { 
            Application.Quit();
        }
    }

    bool over = false;
    public void SetGameOverScreen()
    {
        if (over) return;
        over = true;
        ResumeIndex = -1;
        RestartIndex = 0;
        MenuIndex = 1;
        QuitIndex = 2;
        texts.Remove(ResumeTxt);
        underlines.Remove(GetUnderline(ResumeTxt));
        selectedIndex = 0;

        RestartTxt.text = "Restart";
        QuitTxt.text = "Quit";
        MenuTxt.text = "Menu";
        ResumeTxt.gameObject.SetActive(false);

        DelegateWaiting delegateWaiting = new(OnWaitingFinish);
        GameManager._instance.WaitForSecondThenShowMenu(1.5f, delegateWaiting);
    }

    public delegate void DelegateWaiting();

    void OnWaitingFinish()
    {
        ForcePause();
        UpdateTexts();
    }
}