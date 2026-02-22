using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GroceryRowUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text label;

    public void Set(Sprite icon, string name, int count)
    {
        iconImage.sprite = icon;
        iconImage.preserveAspect = true;

        // Example: "Pineapple x 2" (or just "x 2" if you want only icon + count)
        label.text = $"x {count}";
    }
}