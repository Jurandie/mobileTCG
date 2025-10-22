using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CardUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Dados da Carta")]
    public Card cardData;
    public TMP_Text nameText;
    public TMP_Text statsText;
    public Image artImage;

    [Header("Verso da Carta")]
    public Sprite cardBackSprite; // verso para quando estiver oculta
    public bool isBackVisible = false; // controla se está mostrando o verso

    [Header("Configurações de Exibição")]
    [Range(0.5f, 1f)] public float fitScale = 0.9f;
    [Range(0.05f, 0.3f)] public float snapDuration = 0.1f;
    [Range(1f, 1.2f)] public float bounceScale = 1.08f;
    [Range(0.05f, 0.2f)] public float bounceDuration = 0.08f;

    private Canvas rootCanvas;
    private RectTransform rect;
    private CanvasGroup group;
    private Transform originalParent;
    private Vector2 originalPos;
    private Vector2 dragOffset;
    private MonsterZoneSlot currentSlot;
    private Coroutine moveCoroutine;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void Setup(Card data)
    {
        cardData = data;
        ShowFront();
    }

    // Mostra a frente da carta
    public void ShowFront()
    {
        isBackVisible = false;
        if (cardData == null) return;

        nameText.text = cardData.Name;
        statsText.text = $"ATK {cardData.Attack} / DEF {cardData.Defense}";
        artImage.sprite = cardData.Artwork;
        artImage.color = Color.white;
    }

    // Mostra o verso da carta
    public void ShowBack()
    {
        isBackVisible = true;
        nameText.text = "";
        statsText.text = "";
        artImage.sprite = cardBackSprite;
        artImage.color = Color.white;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (isBackVisible) return; // impede arrastar cartas viradas

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        originalParent = transform.parent;
        originalPos = rect.anchoredPosition;
        group.blocksRaycasts = false;

        transform.SetParent(rootCanvas.transform, true);
        transform.SetAsLastSibling();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform, e.position, rootCanvas.worldCamera, out var localPoint);
        dragOffset = rect.anchoredPosition - localPoint;

        if (currentSlot != null)
        {
            currentSlot.ClearSlot();
            currentSlot = null;
        }
    }

    public void OnDrag(PointerEventData e)
    {
        if (isBackVisible) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform, e.position, rootCanvas.worldCamera, out var pos))
        {
            rect.anchoredPosition = pos + dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (isBackVisible) return;

        group.blocksRaycasts = true;

        if (currentSlot != null)
        {
            SmoothSnapToSlot(currentSlot);
        }
        else
        {
            moveCoroutine = StartCoroutine(SmoothMove(rect.anchoredPosition, originalPos, snapDuration, () =>
            {
                transform.SetParent(originalParent);
                rect.anchoredPosition = originalPos;
            }));
        }
    }

    public void SnapToSlot(MonsterZoneSlot slot)
    {
        currentSlot = slot;
        SmoothSnapToSlot(slot);
    }

    private void SmoothSnapToSlot(MonsterZoneSlot slot)
    {
        currentSlot = slot;
        transform.SetParent(slot.transform, false);

        RectTransform slotRect = slot.transform as RectTransform;

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Vector2 targetSize = slotRect.rect.size * fitScale;

        moveCoroutine = StartCoroutine(SmoothMove(rect.anchoredPosition, Vector2.zero, snapDuration, () =>
        {
            rect.sizeDelta = targetSize;
            rect.localScale = Vector3.one;
            rect.anchoredPosition = Vector2.zero;
            StartCoroutine(BounceEffect());
        }));
    }

    private IEnumerator SmoothMove(Vector2 from, Vector2 to, float duration, System.Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - Mathf.Pow(1f - t, 3f); // EaseOutCubic
            rect.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
            yield return null;
        }

        rect.anchoredPosition = to;
        onComplete?.Invoke();
    }

    private IEnumerator BounceEffect()
    {
        Vector3 startScale = Vector3.one;
        Vector3 peakScale = Vector3.one * bounceScale;

        float elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            t = 1f - Mathf.Pow(1f - t, 3f);
            rect.localScale = Vector3.Lerp(startScale, peakScale, t);
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            t = 1f - Mathf.Pow(1f - t, 3f);
            rect.localScale = Vector3.Lerp(peakScale, startScale, t);
            yield return null;
        }

        rect.localScale = Vector3.one;
    }
}
