using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    
    //curr round logic
    bool roundInProgress = false;
    
    //action choice logic - old
    CardType? playerChoice = null;
    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;
    
    //dragging cards
    public Transform playerHandPanel;   // drag PlayerHandPanel here
    public Button cardButtonTemplate;   // drag CardButtonTemplate here
    List<CardType> currentPlayerHand = new List<CardType>();
    //action choice logic - new 
    public Button confirmPickTwoButton;
    public TMP_Text playerChosenText;
    public TMP_Text aiChosenText;

    List<CardType> playerHand = new List<CardType>();
    List<CardType> aiHand = new List<CardType>();

    List<int> selectedIndices = new List<int>();   // which 2 cards player selected
    List<CardType> playerChosenTwo = new List<CardType>();
    List<CardType> aiChosenTwo = new List<CardType>();

    bool pickingTwo = false;
    
    bool pickingFinal = false;
    
    void Start()
    {
        Debug.Log("Game Start");

        nextRoundButton.onClick.AddListener(StartNextRound);

        rockButton.onClick.AddListener(() => OnPlayerChoose(CardType.Rock));
        paperButton.onClick.AddListener(() => OnPlayerChoose(CardType.Paper));
        scissorsButton.onClick.AddListener(() => OnPlayerChoose(CardType.Scissors));

        confirmPickTwoButton.onClick.AddListener(ConfirmPickTwo);
        confirmPickTwoButton.interactable = false;
        // Start ready state
        SetChoiceButtonsInteractable(false);
        resultText.text = "Click Next Round to begin";
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
    
    void OnPlayerChoose(CardType choice)
    {
        if (!roundInProgress) return; // must have started the round

        // Lock input immediately
        roundInProgress = false;
        SetChoiceButtonsInteractable(false);

        CardType playerPick = choice;
        CardType aiPick = (CardType)Random.Range(0, 3); // Rock/Paper/Scissors

        playerText.text = $"Player: {playerPick}";
        aiText.text = $"AI: {aiPick}";

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

        Debug.Log($"Round {currentRound}: Player {playerPick} vs AI {aiPick} -> {resultText.text}");

        // Allow next round
        nextRoundButton.interactable = true;
    }
    
    void SetChoiceButtonsInteractable(bool on)
    {
        rockButton.interactable = on;
        paperButton.interactable = on;
        scissorsButton.interactable = on;
    }

    void ClearRoundUI()
    {
        playerText.text = "Player: -";
        aiText.text = "AI: -";
        resultText.text = "Pick Rock / Paper / Scissors";
    }
    
    void StartNextRound()
    {
        if (currentRound >= 3)
        {
            resultText.text = $"Game Over — Player: {playerScore}  AI: {aiScore}";
            nextRoundButton.interactable = false;
            confirmPickTwoButton.interactable = false;
            ClearHandUI();
            return;
        }

        currentRound++;
        roundInProgress = true;
        pickingTwo = true;

        roundText.text = $"Round {currentRound} / 3";
        playerText.text = "Player: -";
        aiText.text = "AI: -";
        resultText.text = "Pick 2 cards";

        playerChosenText.text = "Player chose: -";
        aiChosenText.text = "AI chose: -";

        // reset selection
        selectedIndices.Clear();
        playerChosenTwo.Clear();
        aiChosenTwo.Clear();

        confirmPickTwoButton.interactable = false;
        nextRoundButton.interactable = false;

        // Deal both hands
        List<CardType> deck = BuildDeck();
        Shuffle(deck);

        playerHand = deck.GetRange(0, 3);
        aiHand = deck.GetRange(3, 3);

        // Render player's 3 buttons
        ClearHandUI();

        for (int i = 0; i < playerHand.Count; i++)
        {
            int idx = i;
            CardType card = playerHand[i];

            Button b = Instantiate(cardButtonTemplate, playerHandPanel);
            b.gameObject.SetActive(true);

            TMP_Text label = b.GetComponentInChildren<TMP_Text>();
            if (label) label.text = card.ToString();

            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => ToggleSelect(idx, b));
        }
    }
    
    //clicking the cards
    List<CardType> BuildDeck()
    {
        return new List<CardType>()
        {
            CardType.Rock, CardType.Rock, CardType.Rock,
            CardType.Paper, CardType.Paper, CardType.Paper,
            CardType.Scissors, CardType.Scissors, CardType.Scissors
        };
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }

    void ClearHandUI()
    {
        // delete all spawned card buttons (but keep the hidden template)
        for (int i = playerHandPanel.childCount - 1; i >= 0; i--)
        {
            Transform child = playerHandPanel.GetChild(i);
            if (child.gameObject != cardButtonTemplate.gameObject)
                Destroy(child.gameObject);
        }
    }
    
    void ToggleSelect(int idx, Button button)
    {
        if (!pickingTwo) return;

        if (selectedIndices.Contains(idx))
        {
            selectedIndices.Remove(idx);
            // simple visual: reset color
            button.image.color = Color.white;
        }
        else
        {
            if (selectedIndices.Count >= 2) return; // keep it simple for now
            selectedIndices.Add(idx);
            // simple visual: highlight selected
            button.image.color = new Color(0.8f, 0.9f, 1f); // light tint
        }

        confirmPickTwoButton.interactable = (selectedIndices.Count == 2);
    }

    void ConfirmPickTwo()
    {
        if (!pickingTwo) return;
        if (selectedIndices.Count != 2) return;

        pickingTwo = false;
        confirmPickTwoButton.interactable = false;

        // Lock the hand UI (prevent more clicks)
        for (int i = 0; i < playerHandPanel.childCount; i++)
        {
            var child = playerHandPanel.GetChild(i).GetComponent<Button>();
            if (child && child.gameObject != cardButtonTemplate.gameObject)
                child.interactable = false;
        }

        // Player chosen 2
        playerChosenTwo.Clear();
        playerChosenTwo.Add(playerHand[selectedIndices[0]]);
        playerChosenTwo.Add(playerHand[selectedIndices[1]]);

        // AI chooses 2 randomly from its 3
        List<int> aiIdx = new List<int> { 0, 1, 2 };
        Shuffle(aiIdx);
        aiChosenTwo.Clear();
        aiChosenTwo.Add(aiHand[aiIdx[0]]);
        aiChosenTwo.Add(aiHand[aiIdx[1]]);

        // Reveal
        playerChosenText.text = $"Player chose: {playerChosenTwo[0]}, {playerChosenTwo[1]}";
        aiChosenText.text = $"AI chose: {aiChosenTwo[0]}, {aiChosenTwo[1]}";

        resultText.text = "Chosen cards revealed (Step 7 = pick final 1)";
        nextRoundButton.interactable = false;
        //till here is logic for picking two cards
        
        // Prepare for final pick
        ClearHandUI();
        pickingFinal = true;

        resultText.text = "Pick your final card";

        // Show only the chosen 2 as buttons
        for (int i = 0; i < playerChosenTwo.Count; i++)
        {
            CardType card = playerChosenTwo[i];

            Button b = Instantiate(cardButtonTemplate, playerHandPanel);
            b.gameObject.SetActive(true);

            TMP_Text label = b.GetComponentInChildren<TMP_Text>();
            if (label) label.text = card.ToString();

            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => OnFinalPick(card));
        }
    }
    
    void OnFinalPick(CardType playerFinal)
    {
        if (!pickingFinal) return;

        pickingFinal = false;

        // Lock buttons
        ClearHandUI();

        CardType aiFinal = aiChosenTwo[Random.Range(0, aiChosenTwo.Count)];

        playerText.text = $"Player: {playerFinal}";
        aiText.text = $"AI: {aiFinal}";

        int result = Compare(playerFinal, aiFinal);

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

        nextRoundButton.interactable = true;
    }
}