using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public enum PlayerTurn { Player, Enemy }
    public PlayerTurn currentTurn = PlayerTurn.Player;

    [Header("Referências UI")]
    public Button endPhaseButton;
    public Text turnText;

    void Start()
    {
        UpdateTurnUI();
        endPhaseButton.onClick.AddListener(EndPhase);
    }

    public void EndPhase()
    {
        // Aqui você pode colocar efeitos de fim de turno (ex: limpar buffs temporários)
        Debug.Log($"Fim da fase do {currentTurn}");

        // Troca de turno
        currentTurn = (currentTurn == PlayerTurn.Player) ? PlayerTurn.Enemy : PlayerTurn.Player;
        UpdateTurnUI();

        // Caso o turno do inimigo seja automático
        if (currentTurn == PlayerTurn.Enemy)
        {
            StartCoroutine(EnemyTurn());
        }
    }

    void UpdateTurnUI()
    {
        if (turnText != null)
            turnText.text = $"Turno: {currentTurn}";

        // Desativa o botão no turno do inimigo
        endPhaseButton.interactable = (currentTurn == PlayerTurn.Player);
    }

    System.Collections.IEnumerator EnemyTurn()
    {
        Debug.Log("Turno do inimigo começou");

        // Exemplo de ações automáticas do inimigo
        yield return new WaitForSeconds(2f); // Simula jogada
        Debug.Log("Inimigo finalizou o turno");

        EndPhase(); // Passa de volta pro jogador
    }
}
