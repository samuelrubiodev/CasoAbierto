using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class AlertMessage : IStyleManger
{
    private TMP_Text _text;

    public AlertMessage(TMP_Text text)
    {
        _text = text;
    }

    public void SetStyle()
    {
        _text.fontSize        = 48;                   
        _text.fontStyle       = FontStyles.Bold;    
        _text.characterSpacing = 0.5f;             
        _text.lineSpacing     = 0f;             

        _text.faceColor       = new Color32(255, 215, 0, 255); 

        _text.outlineWidth    = 0.1f;   
        _text.outlineColor    = new Color32(0, 0, 0, 100);

        _text.fontMaterial.EnableKeyword("UNDERLAY_ON");
        _text.fontMaterial.SetColor("_UnderlayColor",    new Color32(0, 0, 0, 150)); 
        _text.fontMaterial.SetFloat("_UnderlayOffsetX",  1f);
        _text.fontMaterial.SetFloat("_UnderlayOffsetY", -1f);
        _text.fontMaterial.SetFloat("_UnderlaySoftness", 0.8f);

        _text.alignment         = TextAlignmentOptions.Center;
        _text.textWrappingMode  = TextWrappingModes.NoWrap;  

        _text.richText          = true;
        _text.enableAutoSizing  = false;
        _text.isOrthographic    = true;
    }
}