using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SpellcardManager : MonoBehaviour
{
    public static SpellcardManager _instance;

    [SerializeField] Image CardCasterSprite;
    [SerializeField] GameObject EffectContainer;
    [SerializeField] float holdDuration = 2f, inDuration = 1f, outDuration = 0.5f;

    CanvasGroup cg;
    Vector3 CasterSpritePosition, EffectContainerPostion;
    AudioSource[] audioSources;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            cg = GetComponent<CanvasGroup>();
            audioSources = GetComponents<AudioSource>();
            CasterSpritePosition = CardCasterSprite.transform.localPosition;
            EffectContainerPostion = EffectContainer.transform.localPosition;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterSpellcard(SpellcardBase spellcardBase, Sprite sprite)
    {
        CurrentSpellcard = spellcardBase;
        CardCasterSprite.sprite = sprite;

        if (spellcardBase.spellType == SpellcardBase.SpellType.SPELLCARD)
        {
            IsPlaying = true;
            StartCoroutine(SpellcardEffectCoroutine());
        }
    }

    public bool IsPlaying = false;

    IEnumerator SpellcardEffectCoroutine()
    {
        audioSources[0].Play();
        IsPlaying = true;
        StartCoroutine(FadeIn());
        StartCoroutine(SpellcardWarningsMoveInCoroutine());
        yield return StartCoroutine(SpriteMoveInCoroutine());
        StartCoroutine(FadeOutAndDestroy());
        IsPlaying = false;
    }

    IEnumerator FadeIn()
    {
        float c = 0, d = 0.2f;
        while (c < d)
        {
            cg.alpha = Mathf.Lerp(0, 1, c * 1.0f / d);
            yield return null;
            c += Time.deltaTime;
        }

        cg.alpha = 1;
    }

    Vector3 MoveVelocity = new Vector3(15, -15);
    IEnumerator SpellcardWarningsMoveInCoroutine()
    {
        while (true)
        {
            MoveWarning(EffectContainer.transform, MoveVelocity);
            yield return null;
        }
    }

    void MoveWarning(Transform target, Vector3 velocity)
    {
        target.position += velocity * Time.deltaTime;
    }

    IEnumerator SpriteMoveInCoroutine()
    {
        Vector3 target = Vector3.zero;

        Vector3 finalPosition = CardCasterSprite.transform.localPosition * -1;

        float c = 0, d = inDuration;
        while (c < d)
        {
            CardCasterSprite.transform.localPosition = Vector3.Lerp(CardCasterSprite.transform.localPosition, target, c * 1.0f / d);
            yield return null;
            c += Time.deltaTime;
        }

        CardCasterSprite.transform.localPosition = target;

        float thisVelocity = Random.Range(5, 15);
        Vector3 randomVelocity = new Vector3(thisVelocity, -thisVelocity);

        c = 0;
        d = holdDuration;

        while (c < d)
        {
            CardCasterSprite.transform.localPosition += randomVelocity * Time.deltaTime;
            yield return null;
            c += Time.deltaTime;
        }

        c = 0;
        d = outDuration;
        Vector3 current = CardCasterSprite.transform.localPosition;

        while (c < d)
        {
            CardCasterSprite.transform.localPosition = Vector3.Lerp(current, finalPosition, c * 1.0f / d);
            yield return null;
            c += Time.deltaTime;
        }

        CardCasterSprite.transform.localPosition = finalPosition;
    }

    IEnumerator FadeOutAndDestroy()
    { 
        float c = 0, d = 0.25f;
        while (c < d)
        {
            cg.alpha = Mathf.Lerp(1, 0, c * 1.0f / d);
            yield return null; 
            c += Time.deltaTime;
        }

        cg.alpha = 0;
        OnFinish();
    }


    void OnFinish()
    {
        IsPlaying = false;
        StopAllCoroutines();
        cg.alpha = 0;
        CardCasterSprite.transform.localPosition = CasterSpritePosition;
        EffectContainer.transform.localPosition = EffectContainerPostion;
    }

    public enum SpellcardFinishType
    {
        CAPTURED,
        FAILED
    }
    public void OnSpellcardFinish(SpellcardFinishType spellcardFinishType)
    {
        if (spellcardFinishType == SpellcardFinishType.CAPTURED)
        {
            audioSources[1].Play();
        }
        else if (spellcardFinishType == SpellcardFinishType.FAILED)
        {
            audioSources[2].Play();
        }
    }

    SpellcardBase CurrentSpellcard = null;
    public SpellcardBase GetCurrentSpell()
    {
        return CurrentSpellcard;
    }
    public void MakeSpellcardCaptureInvalid()
    {
        if (!CurrentSpellcard) return;
        CurrentSpellcard.PlayerCheatedThroughSpell();
    }

    [SerializeField] GameObject SpellcardCaptureMessage;
    [SerializeField] Image SpellCardCaptureStatusImg;
    [SerializeField] Sprite CaptureSuccessSprite, CaptureFailedSprite;
    [SerializeField] TMP_Text CapturePointTxt, SpellPointTxt, HistoryTxt;
    [SerializeField] Image[] BonusAndHistory;
    public void ShowSpellcardCaptureMessage(int score)
    {
        SpellCardCaptureStatusImg.sprite = CaptureSuccessSprite;
        CapturePointTxt.text = string.Format($"+{score:N0}");
        StartCoroutine(TurnMessageOffAfter(2));
    }

    public void ShowSpellcardFailMessage()
    {
        SpellCardCaptureStatusImg.sprite = CaptureFailedSprite;
        CapturePointTxt.text = string.Empty;
        StartCoroutine(TurnMessageOffAfter(1.5f));
    }

    IEnumerator TurnMessageOffAfter(float c)
    {
        SpellcardCaptureMessage.SetActive(true);
        yield return new WaitForSeconds(c);
        SpellcardCaptureMessage.SetActive(false);
    }

    public void SetSpellPoint(int pt) => SpellPointTxt.text = string.Format($"{pt:N0}");
    public void SetSpellFailed() => SpellPointTxt.text = "Failed";
    public void SetHistory(string text) => HistoryTxt.text = text;

    [SerializeField] GameObject SpellBox;
    [SerializeField] Image SpellBar;
    [SerializeField] TMP_Text SpellName;

    Vector3 SpellBoxInitPos = new(100, 430);
    public void DoSpellAnimation()
    {
        SpellBox.transform.localPosition = SpellBoxInitPos - new Vector3(0, 750);
        
        SpellBar.transform.localScale *= 2;
        SpellBar.color = ColorUtil.clearWhite;

        SpellName.transform.localScale *= 2;
        SpellName.color = ColorUtil.clearWhite;

        SpellPointTxt.color = ColorUtil.clearWhite;
        HistoryTxt.color = ColorUtil.clearWhite;
        foreach (var item in BonusAndHistory) item.color = ColorUtil.clearWhite;

        StartCoroutine(PlaySpellAnimation());
    }

    IEnumerator PlaySpellAnimation()
    {
        // scale to normal
        float c = 0, d = 1f;
        Vector3 scaleFrom = Vector3.one * 2, scaleTo = Vector3.one;
        Color colorFrom = ColorUtil.clearWhite, colorTo = Color.white;
        while (c < d)
        {
            float t = c * 1.0f / d;
            SpellName.transform.localScale = SpellBar.transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, t * t);
            SpellName.color = SpellBar.color = Color.Lerp(colorFrom, colorTo, t * t);
            c += Time.deltaTime;
            yield return null;
        }

        SpellName.transform.localScale = SpellBar.transform.localScale = scaleTo;
        SpellName.color = SpellBar.color = colorTo;

        yield return new WaitForSeconds(0.67f);

        // move bar to top
        c = 0;
        d = 0.4f;
        Vector3 moveFrom = SpellBox.transform.localPosition, moveTo = SpellBoxInitPos;
        while (c < d)
        {
            float t = c * 1.0f / d;
            SpellBox.transform.localPosition = Vector3.Lerp(moveFrom, moveTo, t);
            c += Time.deltaTime;
            yield return null;
        }

        SpellBox.transform.localPosition = moveTo;
        
        // show bonus and history
        c = 0;
        d = 0.25f;
        while (c < d)
        {
            float t = c * 1.0f / d;
            foreach (var item in BonusAndHistory) item.color = Color.Lerp(colorFrom, colorTo, t);
            SpellPointTxt.color = HistoryTxt.color = Color.Lerp(colorFrom, colorTo, t);
            c += Time.deltaTime;
            yield return null;
        }

        foreach (var item in BonusAndHistory) item.color = colorTo;
        SpellPointTxt.color = HistoryTxt.color = colorTo;
    }
}