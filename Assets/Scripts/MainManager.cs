using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class MainManager : MonoBehaviour
{
    public GameObject Lobby;
    public GameObject Choose;
    public InputField INP_IdleTime;
    public ArduinoInteractive arduinoInteractive;

    public List<Button> BTN_Books;
    public List<PageContent> Books;

    [Header("Config")]
    public float idleBackTime = 90f;

    int focusBTN = 0;

    public State currentStat = State.LOBBY;

    float ElapseIdle = 0;

    void Start()
    {
        PageContentBase.OnPageEnded += EnterChoose;

        for (int i = 0; i < BTN_Books.Count; i++)
        {
            int inloop = i;
            BTN_Books[inloop].onClick.AddListener(() => {
                Choose.SetActive(false);
                Books[inloop].EnterBook();
                focusBTN = inloop;
            });
        }

        INP_IdleTime.SetSavedDataFloat("IdleTime", 90, (x) => {
            idleBackTime = x;
        });

        arduinoInteractive.OnRecieveData += RecieveArduino;

        currentStat = State.LOBBY;
        StartCoroutine(LoopFocusBTN());
    }

    IEnumerator LoopFocusBTN(){
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if(Choose.activeSelf)
                EventSystem.current.SetSelectedGameObject(BTN_Books[focusBTN].gameObject);
        }
    }

    void RecieveArduino(string s){
        if(s == "h"){
            Button_Home();
        }
        if(s == "l"){
            Button_Left();
        }
        if(s == "r"){
            Button_Right();
        }
        if(s == "e"){
            Button_Enter();
        }
    }

    void EnterChoose(){
        Choose.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

        currentStat = State.CHOOSE;
    }

    void BackToLobby(){
        currentStat = State.LOBBY;
        Books[focusBTN].LeaveBook();
        Choose.SetActive(false);
    }
    
    public void Button_Home(){
        // if(Choose.activeSelf){
        //     Choose.SetActive(false);
        // } else {
        //     Books[focusBTN].LeaveBook();
        //     EnterChoose();
        // }

        switch (currentStat)
        {
            case State.LOBBY:
                break;
            case State.CHOOSE:
                Choose.SetActive(false);
                currentStat = State.LOBBY;
                break;
            case State.BOOK:
                Books[focusBTN].LeaveBook();
                EnterChoose();
                currentStat = State.CHOOSE;
                break;
        }
    }

    public void Button_Left(){
        if(!Choose.activeSelf) return;

        focusBTN = (focusBTN + BTN_Books.Count - 1) % BTN_Books.Count;
        EventSystem.current.SetSelectedGameObject(BTN_Books[focusBTN].gameObject);
    }

    public void Button_Right(){
        if(!Choose.activeSelf) return;

        focusBTN = (focusBTN + 1) % BTN_Books.Count;
        EventSystem.current.SetSelectedGameObject(BTN_Books[focusBTN].gameObject);
    }

    public void Button_Enter(){
        // if(Choose.activeSelf)
        //     BTN_Books[focusBTN].onClick.Invoke();
        // else 
        //     Books[focusBTN].NextPage();

        ElapseIdle = 0;

        switch (currentStat)
        {
            case State.LOBBY:
                EnterChoose();
                break;
            case State.CHOOSE:
                BTN_Books[focusBTN].onClick.Invoke();
                currentStat = State.BOOK;
                break;
            case State.BOOK:
                Books[focusBTN].NextPage();
                break;
        }
    }

    void Update()
    {
        ElapseIdle += Time.deltaTime;
        if(ElapseIdle > idleBackTime && currentStat != State.LOBBY){
            BackToLobby();
        }

        if(Input.GetKeyDown(KeyCode.Keypad9)){
            Debug.Log("Home");
            Button_Home();
        }
        if(Input.GetKeyDown(KeyCode.Keypad6)){
            Debug.Log("<");
            Button_Left();
        }
        if(Input.GetKeyDown(KeyCode.Keypad3)){
            Debug.Log(">");
            Button_Right();
        }
        if(Input.GetKeyDown(KeyCode.KeypadPeriod)){
            Debug.Log("OK");
            Button_Enter();
        }
    }
}

public enum State
{
    LOBBY = 0,
    CHOOSE = 1,
    BOOK = 2
}