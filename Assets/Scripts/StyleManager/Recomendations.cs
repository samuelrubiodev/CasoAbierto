using TMPro;
using UnityEngine;

public class Recomendations : IStyleManger
{
    private TMP_Text _text;

    public Recomendations(TMP_Text text)
    {
        _text = text;
    }
    
    public void SetStyle()
    {
        _text.fontSize = 45;                             
        _text.fontStyle = FontStyles.Bold;       
        _text.characterSpacing = 1.0f;                   
        _text.lineSpacing = -2f;                

        _text.faceColor = new Color32(255, 255, 255, 255); 
        _text.outlineColor = new Color32(0, 0, 0, 255); 
        _text.outlineWidth = 0.5f;                   

        _text.fontMaterial.EnableKeyword("UNDERLAY_ON"); 
        _text.fontMaterial.SetColor("_UnderlayColor", new Color32(0, 0, 0, 180)); 
        _text.fontMaterial.SetFloat("_UnderlayOffsetX", 1.5f); 
        _text.fontMaterial.SetFloat("_UnderlayOffsetY", -1.5f); 
        _text.fontMaterial.SetFloat("_UnderlaySoftness", 0.2f); 


        _text.alignment = TextAlignmentOptions.Center;             
        _text.textWrappingMode = TextWrappingModes.PreserveWhitespace; 

        _text.richText = true;                      
        _text.enableAutoSizing = false;            
        _text.isOrthographic = true;                  
    }
}