using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GridPosition : MonoBehaviour, IPointerClickHandler
{
    public int x;
    public int y;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on cell " + y + ", " + x);
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

}
