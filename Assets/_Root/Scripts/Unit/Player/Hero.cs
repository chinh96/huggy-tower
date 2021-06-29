using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    private Vector2 originPosition;
    private Vector2 offset;

    private void OnMouseDown()
    {
        originPosition = transform.position;
        offset = (Vector2)(transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    private void OnMouseUp()
    {
        var slots = VisitTower.Instance.slots;
        foreach (var slot in slots)
        {
            if (slot.GetComponent<RectTransform>().Contains(Input.mousePosition, Camera.main))
            {
                transform.SetParent(slot.transform, false);
                originPosition = slot.spawnPoint.position;
                break;
            }
        }

        transform.position = originPosition;
    }

    private void OnMouseDrag()
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
    }
}
