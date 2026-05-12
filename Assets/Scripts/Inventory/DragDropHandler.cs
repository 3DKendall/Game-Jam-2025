using UnityEngine;
using UnityEngine.UI;

public class DragDropHandler : MonoBehaviour
{
    [HideInInspector] public bool isDragging;

    public Slot slotDraggedFrom;
    public Slot slotDraggedTo;
    [Space]
    public Image dragDropIconImage;

    private void Update()
    {
        if (isDragging && slotDraggedFrom != null)
        {
            //Show what the asset is been dragged
            dragDropIconImage.sprite = slotDraggedFrom.icon.sprite;

            //The icon moves with players mouse when they are dragging the asset
            dragDropIconImage.transform.position = Input.mousePosition;
        }
        else
        {
            dragDropIconImage.transform.position = new Vector3(-10000, 0, 0);
        }
    }
}