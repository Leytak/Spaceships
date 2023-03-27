using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleScreen : MonoBehaviour
{
    [Header("Game Components")]
    [SerializeField] private Spaceship spaceship1;
    [SerializeField] private Spaceship spaceship2;
    [SerializeField] private SlotSelector slotSelector;

    [Header("UI Components")]
    [SerializeField] private Button enterBattleButton;
    [SerializeField] private Button exitBattleButton;
    [SerializeField] private Text resultsLabel;
    
    private Gamestate state = Gamestate.None;
    private UnityEvent<Gamestate> stateChanged = new ();

    private void Awake()
    {
        spaceship1.Initialize(slotSelector, spaceship2, stateChanged);
        spaceship2.Initialize(slotSelector, spaceship1, stateChanged);
        
        spaceship1.Died.AddListener(() => DeclareVictory(spaceship2.name));
        spaceship2.Died.AddListener(() => DeclareVictory(spaceship1.name));
        
        enterBattleButton.onClick.AddListener(StartBattleState);
        exitBattleButton.onClick.AddListener(StartSetupState);
        
        stateChanged.AddListener((gameState) => resultsLabel.gameObject.SetActive(gameState == Gamestate.Finished));

        StartSetupState();
    }

    private void DeclareVictory(string winnerName)
    {
        resultsLabel.text = $"{winnerName} won!";
        FinishBattleState();
    }
    
    private void StartSetupState() => SetState(Gamestate.Setup);

    private void StartBattleState()
    {
        slotSelector.Hide();
        SetState(Gamestate.Battle);
    }
    
    private void FinishBattleState() => SetState(Gamestate.Finished);

    private void SetState(Gamestate newState)
    {
        state = newState;
        stateChanged.Invoke(state);
    }
}