using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIClickDebugger : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(eventSystem);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                Debug.Log($"[UI Hit] {result.gameObject.name}");
            }

            if (results.Count == 0)
            {
                Debug.Log("UI 클릭 안 됨!");
            }
        }
    }
}
