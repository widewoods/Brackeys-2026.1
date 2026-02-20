using TMPro;
using UnityEngine;

public class FruitStateLabel : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Vector3 offset = new Vector3(0, 1.5f, 0);
    [SerializeField] bool showDistanceToHome;

    FruitController brain;
    Camera cam;

    void Awake()
    {
        brain = GetComponentInParent<FruitController>();
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (brain == null) return;

        // position above head
        Transform root = brain.transform;
        transform.position = root.position + offset;

        // face camera
        if (cam != null)
        {
            Vector3 forward = transform.position - cam.transform.position;
            transform.rotation = Quaternion.LookRotation(forward);
        }

        // text
        if (text != null)
        {
            text.text = brain.currentState.ToString();
        }
    }
}
