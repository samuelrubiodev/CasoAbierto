using UnityEngine;

public class GenericHoverInteraction : MonoBehaviour, IHoverInteraction
{
    public string message;
    public float distance = 2f;
    private bool isActive;
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        OnHoverEnter();
    }

    public GenericHoverInteraction(string message)
    {
        this.message = message;
        this.isActive = false;
    }

    public void OnHoverEnter()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance))
        {
            isActive = hit.collider == boxCollider;
        }
        else
        {
            OnHoverExit();
        }
    }

    public void OnHoverExit()
    {
        isActive = false;
        HideMessage();
    }

    void OnGUI()
    {
        if (isActive)
        {
            ShowMessage();
        }
    }

    private void ShowMessage()
    {
        if (isActive)
        {
            GUIStyle customStyle = new(GUI.skin.label);
            customStyle.normal.textColor = Color.white; 
            customStyle.fontSize = 30; 
            customStyle.alignment = TextAnchor.MiddleCenter; 
            
            string message = this.message;
            
            float baseWidth = 500;
            float charWidth = 15;
            float contentWidth = Mathf.Max(baseWidth, message.Length * charWidth);
            float labelWidth = Mathf.Min(contentWidth, Screen.width * 0.9f);
            
            float labelHeight = 50;
            if (contentWidth > labelWidth)
            {
                float lines = Mathf.Ceil(contentWidth / labelWidth);
                labelHeight = labelHeight * lines;
            }

            float screenCenterX = Screen.width / 2;
            float screenCenterY = Screen.height / 2;

            Rect borderRect = new(
                screenCenterX - labelWidth / 2 - 2, 
                screenCenterY - labelHeight / 2 - 2, 
                labelWidth + 4, 
                labelHeight + 4 
            );

            GUI.color = new Color(0.5f, 0.5f, 0.5f, 1f); 
            GUI.Box(borderRect, "");

            Rect labelRect = new(
                screenCenterX - labelWidth / 2,
                screenCenterY - labelHeight / 2,
                labelWidth,
                labelHeight
            );

            GUI.color = Color.white; 
            GUI.Label(labelRect, message, customStyle);
        }
    }

    private void HideMessage()
    {
    }
}