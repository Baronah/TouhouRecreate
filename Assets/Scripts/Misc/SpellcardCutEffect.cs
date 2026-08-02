using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SpellcardCutEffect : MonoBehaviour
{
    public static SpellcardCutEffect _instance;

    [SerializeField] Image CardCasterSprite;
    [SerializeField] GameObject EffectContainer;
    [SerializeField] float holdDuration = 2f, inDuration = 1f, outDuration = 0.5f;

    CanvasGroup cg;
    Vector3 CasterSpritePosition, EffectContainerPostion;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            cg = GetComponent<CanvasGroup>();
            CasterSpritePosition = CardCasterSprite.transform.localPosition;
            EffectContainerPostion = EffectContainer.transform.localPosition;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSpriteAndFadeIn(Sprite sprite)
    {
        CardCasterSprite.sprite = sprite;
        IsPlaying = true;
        StartCoroutine(SpellcardEffectCoroutine());
    }

    public bool IsPlaying = false;

    IEnumerator SpellcardEffectCoroutine()
    {
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
}