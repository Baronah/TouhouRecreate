using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SpellcardBase;

public class HealthBarScript : MonoBehaviour
{
    public static Color Spellcard_Color = new Color(0.92f, 0.51f, 0.22f), NonspellColor = new(0.77f, 0.77f, 0.77f);
    [SerializeField] Image ColorFill;

    Slider slider;
    public Slider getHealthSlider => slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetColor(Color color) => ColorFill.color = color;

    public static Color[] GetHealthBarColorsForSpells(SpellData[] spellDatas)
    {
        Color[] colors = new Color[spellDatas.Length];
        float offsetR = 0, offsetG = 0, offsetB = 0;
        for (int i = 0; i < spellDatas.Length; i++)
        {
            SpellData spellData = spellDatas[i];
            if (spellData.SpellType == SpellType.NON_SPELL) colors[i] = NonspellColor;
            else
            {
                colors[i] = new (Spellcard_Color.r - offsetR, Spellcard_Color.g - offsetG, Spellcard_Color.b - offsetB);
                offsetR += 0.18f;
                offsetG += 0.125f;
                offsetB += 0.05f;
            }
        }

        return colors;
    }
}
