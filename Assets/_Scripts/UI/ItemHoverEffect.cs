using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image backgroundImage;
    private Color normalColor = new Color(1f, 1f, 1f, 0.5f);
    private Color hoverColor = new Color(1f, 1f, 1f, 0.8f);

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
} 