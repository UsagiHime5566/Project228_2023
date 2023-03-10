using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class MainManager : HimeLib.SingletonMono<MainManager>
{
    public GameObject Lobby;
    public GameObject Choose;
    public InputField INP_IdleTime;
    public InputField INT_H;
    public InputField INT_L;
    public InputField INT_R;
    public InputField INT_E;
    public Toggle TG_MouseBlocker;
    public GameObject OBJ_MouseBlocker;
    public ArduinoInteractive arduinoInteractive;

    public List<Button> BTN_Books;
    public List<PageContent> Books;

    [Header("Config")]
    public float idleBackTime = 90f;
    public float pageWaiting = 3;
    public float pageNoAnimWaiting = 10;

    int focusBTN = 0;

    public State currentStat = State.LOBBY;

    float ElapseIdle = 0;

    [SerializeField] string signal_h;
    [SerializeField] string signal_l;
    [SerializeField] string signal_r;
    [SerializeField] string signal_e;

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

        INT_H.SetSavedDataString("H", "h", s => {
            signal_h = s;
        });
        INT_L.SetSavedDataString("L", "l", s => {
            signal_l = s;
        });
        INT_R.SetSavedDataString("R", "r", s => {
            signal_r = s;
        });
        INT_E.SetSavedDataString("C", "c", s => {
            signal_e = s;
        });
        TG_MouseBlocker.onValueChanged.AddListener(x => {
            OBJ_MouseBlocker.SetActive(x);
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
        if(s == signal_h){
            Button_Home();
        }
        if(s == signal_l){
            Button_Left();
        }
        if(s == signal_r){
            Button_Right();
        }
        if(s == signal_e){
            Button_Enter();
        }
    }

    void EnterChoose(){
        Choose.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);

        currentStat = State.CHOOSE;
        ElapseIdle = 0;
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

        ElapseIdle = 0;

        focusBTN = (focusBTN + BTN_Books.Count - 1) % BTN_Books.Count;
        EventSystem.current.SetSelectedGameObject(BTN_Books[focusBTN].gameObject);
    }

    public void Button_Right(){
        if(!Choose.activeSelf) return;

        ElapseIdle = 0;

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
        if(ElapseIdle > idleBackTime && currentStat == State.CHOOSE){
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