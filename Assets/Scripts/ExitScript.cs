using UnityEngine;
using UnityEngine.VFX; 
using TMPro; 
using System.Collections;

public class ExitScript : MonoBehaviour
{
    [Header("Links to Portal Components")]
    [SerializeField] private MeshRenderer portalRenderer; 
    [SerializeField] private Collider portalCollider;     
    [SerializeField] private Light mainBrightLight; 
    [SerializeField] private Light beaconLight;      

    [Header("Logic")]
    public ButtonScript[] logicButtons;
    private bool isActive = false;
    private GameObject cachedWinText;

    void Start()
    {
        cachedWinText = GameObject.FindGameObjectWithTag("Finish"); 

        if (cachedWinText != null) 
        {
            cachedWinText.SetActive(false);
            // Debug.Log("WinText found and hidden");
        }
        else 
        {
            Debug.LogError("No Finish-tag text found");
        }
        
        SetExitState(false);
    }

    void Update()
    {
        if (logicButtons == null || logicButtons.Length == 0) return;

        bool allPressed = true;
        foreach (ButtonScript btn in logicButtons)
        {
            if (btn != null && !btn.isPressed)
            {
                allPressed = false;
                break;
            }
        }

        if (allPressed && !isActive) SetExitState(true);
        else if (!allPressed && isActive) SetExitState(false);
    }

    void SetExitState(bool state)
    {
        isActive = state;
        if (portalRenderer != null) portalRenderer.enabled = state;
        if (portalCollider != null) portalCollider.enabled = state;
        if (mainBrightLight != null) mainBrightLight.enabled = state;
        if (beaconLight != null) beaconLight.enabled = !state; 

        var vfx = GetComponentInChildren<VisualEffect>();
        if (vfx != null)
        {
            if (state) vfx.Play(); 
            else vfx.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.CompareTag("Player"))
        {
            if (cachedWinText != null)
            {
                cachedWinText.SetActive(true);
                Debug.Log("LEVEL COMPLETE!");
            }
            else 
            {
                Debug.LogError("WinText not found");
            }
        }
    }
}