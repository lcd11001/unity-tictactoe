using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class GridPosition : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int x;
    [SerializeField]
    private int y;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.ClickedOnCellRpc(x, y, GameManager.Instance.GetLocalPlayerType());
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

}
