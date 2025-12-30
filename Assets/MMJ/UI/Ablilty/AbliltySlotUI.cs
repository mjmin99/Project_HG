using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using static CharacterInstance;

public enum SetBonusLevel // 세트 강도
{
    None,
    Pair,       // 2
    Triple,     // 3
    Quad,       // 4+
}


public class AbilitySlotUI : MonoBehaviour
{
    public TMP_Text nameText;
    public Button lockButton;

    [Header("Lock Button Sprites")]
    [SerializeField] private Sprite unlockSprite; 
    [SerializeField] private Sprite lockSprite;

    [SerializeField] private Image backgroundImage;

    private Image lockButtonImage;

    private int slotIndex;
    private CharacterInstance inst;
    private CharacterModel model;
    private System.Action onChanged;

    private static readonly Color LOCKED_COLOR = new Color(1f, 0.55f, 0f); // 주황색
    private static readonly Color UNLOCKED_COLOR = Color.white;
    private static readonly Color TIER1_COLOR = new Color(0.3f, 0.9f, 0.3f); // 초록
    private static readonly Color TIER2_COLOR = new Color(0.3f, 0.5f, 1f);   // 파랑
    private static readonly Color TIER3_COLOR = new Color(0.7f, 0.3f, 1f);   // 보라
    private static readonly Color EMPTY_COLOR = new Color(0.2f, 0.2f, 0.2f); // 빈 슬롯
    private static readonly Color TEXT_TIER1 = new Color(0.9f, 1f, 0.9f);
    private static readonly Color TEXT_TIER2 = new Color(0.9f, 0.95f, 1f);
    private static readonly Color TEXT_TIER3 = new Color(0.95f, 0.9f, 1f);
    private static readonly Color TEXT_EMPTY = Color.gray;


    [SerializeField] private CanvasGroup canvasGroup;

    private Tween setBonusTween;

    [SerializeField] private Image glowImage; // 슬롯 뒤에 깔아둘 이미지

    // 메인 연출의 진입점
    private Tween alphaTween;
    private Tween scaleTween;
    private Tween glowTween;

    private void Awake()
    {
        lockButtonImage = lockButton.GetComponent<Image>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Bind(
        CharacterModel model,
        CharacterInstance inst,
        int index,
        System.Action onChanged)
    {
        this.model = model;
        this.inst = inst;
        this.slotIndex = index;
        this.onChanged = onChanged;

        lockButton.onClick.RemoveAllListeners();
        lockButton.onClick.AddListener(OnClickLock);

        Refresh();
    }

    public void Refresh()
    {
        int unlocked = inst.GetUnlockedAbilitySlotCount(model);

        // 아직 해방되지 않은 슬롯
        if (slotIndex >= unlocked)
        {
            nameText.text = "Locked";
            lockButton.interactable = false;
            backgroundImage.color = Color.black;
            return;
        }

        var slot = inst.abilitySlots[slotIndex];

        lockButton.interactable = true;

        RefreshLockButtonImage(slot.isLocked);

        if (slot.ability == null)
        {
            nameText.text = "Empty";
            backgroundImage.color = EMPTY_COLOR;
            nameText.color = TEXT_EMPTY;
        }
        else
        {
            nameText.text = AbilityNameProvider.GetName(slot.ability.abilityId);
            backgroundImage.color = GetColorByRarity(slot.ability.rarity);
            nameText.color = GetTextColorByRarity(slot.ability.rarity);
        }
    }

    private void RefreshLockButtonImage(bool isLocked)
    {
        lockButtonImage.sprite = isLocked ? lockSprite : unlockSprite;
        lockButtonImage.color = isLocked ? LOCKED_COLOR : UNLOCKED_COLOR;
    }

    private void OnClickLock()
    {
        var slot = inst.abilitySlots[slotIndex];
        slot.isLocked = !slot.isLocked;

        onChanged?.Invoke(); // → 상위에서 Refresh 다시 호출
    }

    private Color GetColorByRarity(AbilityRarity rarity)
    {
        return rarity switch
        {
            AbilityRarity.Tier1 => TIER1_COLOR,
            AbilityRarity.Tier2 => TIER2_COLOR,
            AbilityRarity.Tier3 => TIER3_COLOR,
            _ => Color.white
        };
    }

    private Color GetTextColorByRarity(AbilityRarity rarity)
    {
        return rarity switch
        {
            AbilityRarity.Tier1 => TEXT_TIER1,
            AbilityRarity.Tier2 => TEXT_TIER2,
            AbilityRarity.Tier3 => TEXT_TIER3,
            _ => Color.white
        };
    }


    // ------------------------- 추가중

    public void SetSetBonusEffect(SetBonusLevel level, Color glowColor)
    {
        StopAllSetBonusEffects();

        if (level == SetBonusLevel.None)
            return;

        PlayAlphaPulse(level);
        PlayScalePulse(level);
        PlayGlowPulse(level, glowColor);
    }

    private void PlayAlphaPulse(SetBonusLevel level)
    {
        float minAlpha = level switch
        {
            SetBonusLevel.Pair => 0.75f,
            SetBonusLevel.Triple => 0.6f,
            _ => 0.45f
        };

        alphaTween = canvasGroup
            .DOFade(minAlpha, 0.35f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void PlayScalePulse(SetBonusLevel level)
    {
        float scale = level switch
        {
            SetBonusLevel.Pair => 1.02f,
            SetBonusLevel.Triple => 1.04f,
            _ => 1.06f
        };

        scaleTween = transform
            .DOScale(scale, 0.35f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void PlayGlowPulse(SetBonusLevel level, Color color)
    {
        glowImage.gameObject.SetActive(true);
        glowImage.color = new Color(color.r, color.g, color.b, 0f);

        float maxAlpha = level switch
        {
            SetBonusLevel.Pair => 0.3f,
            SetBonusLevel.Triple => 0.5f,
            _ => 0.75f
        };

        glowTween = glowImage
            .DOFade(maxAlpha, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void PlaySetBonusEffect()
    {
        StopSetBonusEffect();

        canvasGroup.alpha = 1f;

        setBonusTween = canvasGroup
            .DOFade(0.4f, 0.6f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopSetBonusEffect()
    {
        if (setBonusTween != null && setBonusTween.IsActive())
            setBonusTween.Kill();

        canvasGroup.alpha = 1f;
    }

    private void OnDestroy() 
    {
        StopAllSetBonusEffects();
    } // 두 트윈 버그 해결용 코드

    private void StopAllSetBonusEffects()
    {
        alphaTween?.Kill();
        scaleTween?.Kill();
        glowTween?.Kill();

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        transform.localScale = Vector3.one;

        if (glowImage != null)
            glowImage.gameObject.SetActive(false);
    }
}
