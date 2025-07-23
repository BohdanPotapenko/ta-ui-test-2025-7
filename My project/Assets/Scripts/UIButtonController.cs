using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI References")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject autoSpinOverlay;

    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("Settings")]
    [SerializeField] private float holdTimeToAutoSpin = 1.0f;
    [SerializeField] private float fillDelay = 0.2f;

    [SerializeField] private ParticleSystem buttonGlow;

    private Vector3 defaultScale;
    private Tween currentTween;
    private float holdTimer;
    private float fillDelayTimer;
    private bool isAutoSpin = false;
    private bool isHolding = false;
    private bool fillStarted = false;

    private void Start()
    {
        defaultScale = transform.localScale;
        SetNormalState();
    }

    private void Update()
    {
        if (isHolding && !isAutoSpin)
        {
            fillDelayTimer += Time.deltaTime;

            if (!fillStarted && fillDelayTimer >= fillDelay)
            {
                fillStarted = true;
                fillImage.fillAmount = 0f;
            }

            if (fillStarted)
            {
                holdTimer += Time.deltaTime;
                fillImage.fillAmount = holdTimer / holdTimeToAutoSpin;

                if (holdTimer >= holdTimeToAutoSpin)
                {
                    ActivateAutoSpin();
                }
            }
        }
    }

    public void SetNormalState()
    {
        isAutoSpin = false;
        isHolding = false;
        fillStarted = false;
        holdTimer = 0f;
        fillDelayTimer = 0f;

        KillCurrentTween();

        targetImage.sprite = normalSprite;
        fillImage.fillAmount = 0f;
        autoSpinOverlay.SetActive(false);
        transform.localScale = defaultScale;
    }

    public void SetPressedState()
    {
        targetImage.sprite = pressedSprite;

        transform.DOKill();
        transform.DOScale(defaultScale * 0.95f, 0.1f).SetEase(Ease.OutSine);
    }

    public void ActivateAutoSpin()
    {
        isAutoSpin = true;
        isHolding = false;
        fillStarted = false;
        holdTimer = 0f;
        fillDelayTimer = 0f;

        autoSpinOverlay.SetActive(true);
        fillImage.fillAmount = 0f;
        targetImage.sprite = normalSprite;
        transform.localScale = defaultScale;

        transform.DOKill();
        transform.DOScale(defaultScale * 1.05f, 0.8f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isAutoSpin)
        {
            SetNormalState();
        }
        else
        {
            isHolding = true;
            fillStarted = false;
            fillDelayTimer = 0f;
            holdTimer = 0f;
            buttonGlow.Play();
            SetPressedState();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isAutoSpin)
        {
            SetNormalState();
        }
    }

    private void KillCurrentTween()
    {
        if (currentTween != null && currentTween.IsActive())
        {
             currentTween.Kill();
        }
           
        transform.DOKill();
    }
}
