using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageContent : PageContentBase
{
    public List<GameObject> pages;

    [Header("Runtime Data")]
    [SerializeField] int pageIndex = 0;
    [SerializeField] float elapseTime = 0;
    [SerializeField] Animator runAnimator;
    AnimatorStateInfo runAniInfo;

    void Update(){
        elapseTime += Time.deltaTime;

        if(runAnimator){
            // Debug.Log(runAniInfo.normalizedTime);
            // Debug.Log(runAniInfo.length);
            // Debug.Log(runAniInfo.loop);
            // Debug.Log(runAniInfo.ToString());

            if(elapseTime > MainManager.instance.pageWaiting + runAniInfo.length){
                NextPage();
            }

        } else {

            if(elapseTime > MainManager.instance.pageNoAnimWaiting){
                NextPage();
            }
        }
    }

    void CatchAnimatorForWait(GameObject obj){
        elapseTime = 0;

        var am = obj.GetComponent<Animator>();
        if(am){
            runAnimator = am;
            runAniInfo = runAnimator.GetCurrentAnimatorStateInfo(0);
        } else {
            runAnimator = null;
        }
    }

    public void EnterBook(){
        gameObject.SetActive(true);
        foreach (var item in pages)
        {
            item.SetActive(false);
        }

        pages[0].SetActive(true);
        pageIndex = 0;

        CatchAnimatorForWait(pages[0]);
    }

    public void NextPage(){
        if(!gameObject.activeSelf) return;

        pages[pageIndex].SetActive(false);

        pageIndex += 1;

        if(pageIndex >= pages.Count){
            pageIndex = 0;
            gameObject.SetActive(false);

            OnPageEnded?.Invoke();

            return;
        }

        pages[pageIndex].SetActive(true);
        CatchAnimatorForWait(pages[pageIndex]);
    }

    public void LeaveBook(){
        pageIndex = 0;
        gameObject.SetActive(false);
    }
}
