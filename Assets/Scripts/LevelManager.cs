using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public GameObject ghostPrefab;
    public PathRecorder recorder;
    public MazeGenerator mazeGen;

    [Header("UI Setup")]
    public GameObject slotPrefab;
    public Transform slotContainer;

    [Header("Teleport Settings")]
    public Transform startPoint; 

    private int selectedSlot = 0; 
    private List<GhostSlotUI> uiSlots = new List<GhostSlotUI>();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        int count = mazeGen.buttonsToSpawn;
        recorder.InitializeSlots(count);

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotContainer);
            GhostSlotUI slotScript = go.GetComponent<GhostSlotUI>();
            slotScript.Init(i, this);
            uiSlots.Add(slotScript);
        }
        UpdateUI();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // F - Начать/Остановить запись (только если курсор захвачен игрой)
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (recorder.IsRecording)
            {
                recorder.StopRecording();
                ClearAllGhosts();
            }
            else
            {
                // Начинаем запись  только если не в меню
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    recorder.StartRecording(selectedSlot);
                    SpawnAllGhostsExcept(selectedSlot);
                    GameObject playerObj = GameObject.FindWithTag("Player");
                    if (playerObj != null)
                    {
                        var pm = playerObj.GetComponent<PlayerMovement>();
                        if (pm != null && startPoint != null)
                        {
                            pm.TeleportTo(startPoint);
                        }
                    }
                }
            }
            UpdateUI();
        }

        // R - Запустить всех клонов
        if (Keyboard.current.rKey.wasPressedThisFrame && !recorder.IsRecording)
        {
            PlayAll();
        }
    }

    public void SelectSlot(int index)
    {
        if (recorder.IsRecording) return; 
        selectedSlot = index;
        UpdateUI();
    }

    void PlayAll()
    {
        ClearAllGhosts();
        for (int i = 0; i < recorder.TotalSlots; i++)
        {
            SpawnGhost(i);
        }
    }

    void SpawnGhost(int slotIndex)
    {
        var path = recorder.GetPath(slotIndex);
        if (path != null && path.Count > 0)
        {
            GameObject ghost = Instantiate(ghostPrefab);
            ghost.GetComponent<GhostActor>().Init(path);
            ghost.tag = "GhostClone"; 
        }
    }

    void SpawnAllGhostsExcept(int excluded)
    {
        ClearAllGhosts();
        for (int i = 0; i < recorder.TotalSlots; i++)
        {
            if (i == excluded) continue;
            SpawnGhost(i);
        }
    }

    void ClearAllGhosts()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("GhostClone");
        foreach (GameObject g in ghosts)
        {
            Destroy(g);
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < uiSlots.Count; i++)
        {
            bool hasData = recorder.GetPath(i).Count > 0;
            bool recording = (recorder.IsRecording && selectedSlot == i);
            uiSlots[i].SetStatus(hasData, recording, selectedSlot == i);
        }
    }
}