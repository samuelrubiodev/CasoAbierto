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
        messageText.fontSize = 60;
        messageText.fontStyle = FontStyles.Bold;
        messageText.characterSpacing = 1.0f;
        messageText.lineSpacing = -2f;

        messageText.faceColor = new Color32(156, 129, 63, 255); 
        messageText.outlineColor = new Color32(17,29,19,255);
        messageText.outlineWidth = 0.35f;

        messageText.fontMaterial.EnableKeyword("UNDERLAY_ON"); 
        messageText.fontMaterial.SetColor("_UnderlayColor", new Color32(0, 0, 0, 180));
        messageText.fontMaterial.SetFloat("_UnderlayOffsetX", 1.5f); 
        messageText.fontMaterial.SetFloat("_UnderlayOffsetY", -1.5f); 
        messageText.fontMaterial.SetFloat("_UnderlaySoftness", 0.2f); 

        messageText.alignment = TextAlignmentOptions.Center;
        messageText.textWrappingMode = TextWrappingModes.PreserveWhitespace;

        messageText.richText = true;
        messageText.enableAutoSizing = false;
        messageText.isOrthographic = true;                         
    }
}
