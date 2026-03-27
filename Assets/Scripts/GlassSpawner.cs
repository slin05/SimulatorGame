using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlassSpawner : MonoBehaviour, IPointerClickHandler
{
    [Header("Assign in Inspector")]
    public GameObject glassPrefab;        // the draggable glass prefab
    public Transform canvasTransform;     // drag Canvas here

    public void OnPointerClick(PointerEventData eventData)
    {
        // spawn a draggable glass copy at the same position
        GameObject newGlass = Instantiate(glassPrefab, canvasTransform);
        newGlass.transform.position = transform.position;
        Debug.Log("Glass picked up!");
    }
}