using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    enum CardType { Rock, Paper, Scissors }

    int playerScore = 0;
    int aiScore = 0;

    //texts
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI aiText;
    public TextMeshProUGUI resultText;
    
    //next round logic
    public Button nextRoundButton;
    int currentRound = 0;
    
    void Start()
    {
        Debug.Log("Game Start");
        nextRoundButton.onClick.AddListener(PlayNextRound);
    }

    void PlayRound(int roundNumber)
    {
        // 1. Build deck
        List<CardType> deck = new List<CardType>()
        {
            CardType.Rock, CardType.Rock, CardType.Rock,
            CardType.Paper, CardType.Paper, CardType.Paper,
            CardType.Scissors, CardType.Scissors, CardType.Scissors
        };

        // 2. Shuffle
        Shuffle(deck);

        // 3. Deal
        List<CardType> playerHand = deck.GetRange(0, 3);
        List<CardType> aiHand = deck.GetRange(3, 3);

        // 4. Random pick
        CardType playerPick = playerHand[Random.Range(0, playerHand.Count)];
        CardType aiPick = aiHand[Random.Range(0, aiHand.Count)];

        roundText.text = $"Round {roundNumber} / 3";
        playerText.text = $"Player: {playerPick}";
        aiText.text = $"AI: {aiPick}";

        // 5. Decide winner
        int result = Compare(playerPick, aiPick);

        if (result == 1)
        {
            playerScore++;
            resultText.text = "Result: Player Wins";
        }
        else if (result == -1)
        {
            aiScore++;
            resultText.text = "Result: AI Wins";
        }
        else
        {
            resultText.text = "Result: Tie";
        }
    }

    void Shuffle(List<CardType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            CardType temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    int Compare(CardType player, CardType ai)
    {
        if (player == ai) return 0;

        if (
            (player == CardType.Rock && ai == CardType.Scissors) ||
            (player == CardType.Paper && ai == CardType.Rock) ||
            (player == CardType.Scissors && ai == CardType.Paper)
        )
            return 1;

        return -1;
    }
    
    void PlayNextRound()
    {
        if (currentRound >= 3)
        {
            Debug.Log($"Game Over -> Player: {playerScore}, AI: {aiScore}");
            resultText.text = "Game Over";
            nextRoundButton.interactable = false;
            return;
        }

        currentRound++;
        PlayRound(currentRound);
    }
}