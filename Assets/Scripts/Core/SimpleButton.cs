using UnityEngine;
using UnityEngine.Events;

public class SimpleButton : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent Clicked = new ();

    private void OnMouseDown()
    {
        Clicked?.Invoke();
    }
}