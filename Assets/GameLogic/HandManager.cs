using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Referências de Mãos")]
    public Transform playerHandContainer;
    public Transform enemyHandContainer;

    [Header("Prefabs e Artes")]
    public GameObject cardPrefab;
    public Sprite[] artworks;
    public Sprite cardBackSprite; // verso de Yu-Gi-Oh!

    [Header("Configurações")]
    public int playerHandSize = 5;
    public int enemyHandSize = 5;
    public bool hideEnemyCards = true;

    void Start()
    {
        GenerateHand(playerHandContainer, playerHandSize, isEnemy: false);
        GenerateHand(enemyHandContainer, enemyHandSize, isEnemy: true);
    }

    private void GenerateHand(Transform container, int count, bool isEnemy)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(cardPrefab, container);
            CardUI ui = go.GetComponent<CardUI>();

            Card c = new Card
            {
                Name = isEnemy ? "Carta Inimiga" : $"Dragão {i + 1}",
                Attack = 1000 + i * 500,
                Defense = 800 + i * 300,
                Artwork = artworks[i % artworks.Length]
            };

            ui.Setup(c);
            ui.cardBackSprite = cardBackSprite;

            if (isEnemy && hideEnemyCards)
            {
                ui.ShowBack(); // mostra o verso
            }
            else
            {
                ui.ShowFront(); // mostra a frente
            }
        }
    }
}
