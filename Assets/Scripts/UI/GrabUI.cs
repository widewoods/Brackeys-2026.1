using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class GrabUI : MonoBehaviour
{
    [SerializeField] Image image;

    public void ChangeFillAmount(float a)
    {
        image.fillAmount = a;
    }
}
