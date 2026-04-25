using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostSlotUI : MonoBehaviour
{
    public TextMeshProUGUI slotNameText;
    public Image statusIndicator; 
    
    private int myIndex;
    private LevelManager manager;
    private Outline outline; 

    public void Init(int index, LevelManager mngr)
    {
        myIndex = index;
        manager = mngr;
        slotNameText.text = $"Клон #{index + 1}";
        outline = GetComponent<Outline>();
    }

    public void OnClickSelect()
    {
        manager.SelectSlot(myIndex);
    }

    public void SetStatus(bool hasData, bool isRecording, bool isSelected)
    {
        if (isRecording) statusIndicator.color = Color.red;
        else if (hasData) statusIndicator.color = Color.green;
        else statusIndicator.color = Color.gray;

        if (outline != null) outline.enabled = isSelected;
    }
}