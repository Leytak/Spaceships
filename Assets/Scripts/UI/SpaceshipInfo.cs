using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SpaceshipInfo : MonoBehaviour
{
    [SerializeField] private Spaceship spaceship;

    private Text label;
    
    private void Awake()
    {
        label = GetComponent<Text>();
        spaceship.ConfigurationUpdated.AddListener(OnConfigurationUpdated);
    }

    private void OnConfigurationUpdated(string text) => label.text = text;
}