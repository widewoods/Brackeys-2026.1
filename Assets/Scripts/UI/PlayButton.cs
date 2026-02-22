using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class PlayButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private AudioSource pop;
    private RectTransform rectTransform;

    void Start()
    {
        pop = GetComponent<AudioSource>();
        rectTransform = GetComponent<RectTransform>();
    }
    
    // 마우스를 올렸을 때 실행
    public void OnPointerEnter(PointerEventData eventData)
    {
        pop.Play();
        rectTransform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
    }

    // 마우스가 나갔을 때 실행
    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
    }

    public void OnButtonPressed()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
