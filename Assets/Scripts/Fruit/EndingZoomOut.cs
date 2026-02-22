using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingZoomOut : MonoBehaviour
{
    [SerializeField] private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) cam = GetComponentInChildren<Camera>();
    }

    public IEnumerator ZoomOut()
    {
        for (int i = 0; i < 100; i++)
        {
            cam.transform.position += -cam.transform.forward * 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        SceneManager.LoadScene(0);
        yield return null;
    }
}
