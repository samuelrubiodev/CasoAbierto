using TMPro;
using UnityEngine;

public class MessageStyleManager : IStyleManger
{
    public TMP_Text messageText;

    public MessageStyleManager(TMP_Text text)
    {
        messageText = text;
    }

    public void SetStyle()
    {
        messageText.fontSize = 50;                     
        messageText.fontStyle = FontStyles.Bold;       
        messageText.characterSpacing = 2f;                 
        messageText.lineSpacing = -10f;                


        messageText.faceColor = new Color32(255, 255, 255, 255); 
        messageText.outlineColor = new Color32(20, 20, 20, 255); 
        messageText.outlineWidth = 0.8f;          


        messageText.alignment = TextAlignmentOptions.Center;  
        messageText.textWrappingMode = TextWrappingModes.PreserveWhitespace; 
        messageText.textWrappingMode = TextWrappingModes.Normal;        


        messageText.fontMaterial.EnableKeyword("GLOW_ON"); 
        messageText.fontMaterial.SetColor("_GlowColor", new Color32(50, 50, 50, 128));
        messageText.fontMaterial.SetFloat("_GlowPower", 0.5f); 


        messageText.richText = true;                     
        messageText.enableAutoSizing = false;             
        messageText.isOrthographic = true;                 
    }
}
