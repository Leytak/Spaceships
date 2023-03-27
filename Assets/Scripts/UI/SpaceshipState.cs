using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SpaceshipState : MonoBehaviour
{
    [SerializeField] private Spaceship spaceship;

    private Text label;
    
    private void Awake()
    {
        label = GetComponent<Text>();
        spaceship.StateUpdated.AddListener(OnStateUpdated);
    }

    private void OnStateUpdated(string text) => label.text = text;
}