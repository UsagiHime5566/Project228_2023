using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageContent : PageContentBase
{
    public List<GameObject> pages;
    int pageIndex = 0;

    public void EnterBook(){
        gameObject.SetActive(true);
        foreach (var item in pages)
        {
            item.SetActive(false);
        }

        pages[0].SetActive(true);
        pageIndex = 0;
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
    }

    public void LeaveBook(){
        pageIndex = 0;
        gameObject.SetActive(false);
    }
}
