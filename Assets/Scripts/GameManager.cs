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
    public TextMeshProUGUI nextRoundButtonLabel;
    int currentRound = 0;
    
    // Partner UI: player buttons
    public Button rockPlayerButton;
    public Button paperPlayerButton;
    public Button scissorsPlayerButton;

    // Partner UI: player outlines
    public GameObject rockPlayerOutline;
    public GameObject paperPlayerOutline;
    public GameObject scissorsPlayerOutline;

    // Partner UI: opponent cards (whole object)
    public GameObject rockOpponentCard;
    public GameObject paperOpponentCard;
    public GameObject scissorsOpponentCard;
    
    public GameObject rockPlayerCard;
    public GameObject paperPlayerCard;
    public GameObject scissorsPlayerCard;
    
    //integrating - picking two
    bool pickingTwo = false;
    bool pickingFinal = false;

    System.Collections.Generic.List<CardType> selectedTwo = new System.Collections.Generic.List<CardType>();

    //action choice logic - new 
    public Button confirmPickTwoButton;
    public TMP_Text playerChosenText;
    public TMP_Text aiChosenText;

    List<CardType> aiChosenTwo = new List<CardType>();
    CardType aiFinalChoice;
    
    //game done?
    bool gameOver = false;
    
    //audio
    public UISFX audioSfx;
    
    void Start()
    {
        nextRoundButtonLabel.text = "Start Game";
        nextRoundButton.onClick.AddListener(StartNextRound);
        //resultText.text = "Click Start Game to begin";
        
        //integrated ones
        rockPlayerButton.onClick.AddListener(() => OnPartnerCardClicked(CardType.Rock));
        paperPlayerButton.onClick.AddListener(() => OnPartnerCardClicked(CardType.Paper));
        scissorsPlayerButton.onClick.AddListener(() => OnPartnerCardClicked(CardType.Scissors));
        UpdateScoreUI();
        confirmPickTwoButton.onClick.AddListener(ConfirmPickTwo);
        confirmPickTwoButton.interactable = false;
        // Start ready state
        SetPartnerButtonsInteractable(false);
        HideAllPlayerOutlines();
        HideAllOpponentCards();
    }
    
    void ResetGame()
    {
        currentRound = 0;
        playerScore = 0;
        aiScore = 0;
        gameOver = false;

        nextRoundButtonLabel.text = "Start Game";
        nextRoundButton.interactable = true;
        confirmPickTwoButton.interactable = false;

        // reset UI
        roundText.text = "Round 0 / 3";
        resultText.text = "Click Start Game to begin";
        playerChosenText.text = "Player chose: -";
        aiChosenText.text = "AI chose: -";

        UpdateScoreUI();

        // reset visuals
        SetPartnerButtonsInteractable(false);
        HideAllPlayerOutlines();
        HideAllOpponentCards();

        // show all player cards again
        if (rockPlayerCard) rockPlayerCard.SetActive(true);
        if (paperPlayerCard) paperPlayerCard.SetActive(true);
        if (scissorsPlayerCard) scissorsPlayerCard.SetActive(true);
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
    
    void StartNextRound()
    {
        if (gameOver)
        {
            ResetGame();
            return;
        }
        nextRoundButtonLabel.text = "Next Round";
        //re-show the damn cards
        if (rockPlayerCard) rockPlayerCard.SetActive(true);
        if (paperPlayerCard) paperPlayerCard.SetActive(true);
        if (scissorsPlayerCard) scissorsPlayerCard.SetActive(true);
        
        if (currentRound >= 3)
        {
            if (playerScore > aiScore) resultText.text = "YOU WIN!";
            else resultText.text = "AW, NEXT TIME!";

            nextRoundButton.interactable = false;
            confirmPickTwoButton.interactable = false;

            SetPartnerButtonsInteractable(false);
            HideAllPlayerOutlines();
            HideAllOpponentCards();
            return;
        }

        currentRound++;
        pickingTwo = true;
        pickingFinal = false;

        selectedTwo.Clear();
        HideAllPlayerOutlines();
        HideAllOpponentCards();

        SetPartnerButtonsInteractable(true);

        roundText.text = $"Round {currentRound} / 3";
        UpdateScoreUI();
        resultText.text = "Pick 2 cards";

        playerChosenText.text = "Player chose: -";
        aiChosenText.text = "AI chose: -";

        confirmPickTwoButton.interactable = false;
        nextRoundButton.interactable = false;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
    
    void OnPartnerCardClicked(CardType card)
    {
        // Pick 2 phase
        if (pickingTwo)
        {
            if (selectedTwo.Contains(card))
            {
                selectedTwo.Remove(card);
                SetOutline(card, false);
            }
            else
            {
                // ENFORCE MAX 2
                if (selectedTwo.Count >= 2) return;

                selectedTwo.Add(card);
                SetOutline(card, true);
            }

            confirmPickTwoButton.interactable = (selectedTwo.Count == 2);
            return;
        }

        // Final pick phase
        if (pickingFinal)
        {
            // only allow choosing one of the two you picked earlier
            if (!selectedTwo.Contains(card)) return;

            ResolveFinalPick(card);
            return;
        }
    }
   
    void ConfirmPickTwo()
    {
        if (!pickingTwo) return;
        if (selectedTwo.Count != 2) return;

        pickingTwo = false;
        confirmPickTwoButton.interactable = false;

        // After confirming, only allow the chosen two to be clicked for final pick
        pickingFinal = true;
        // Hide the unchosen player card (show only the chosen two)
        HideAllPlayerCards();
        ShowPlayerCard(selectedTwo[0], true);
        ShowPlayerCard(selectedTwo[1], true);

        // AI chooses 2 out of {Rock, Paper, Scissors} randomly
        List<CardType> all = new List<CardType> { CardType.Rock, CardType.Paper, CardType.Scissors };
        Shuffle(all); // reuse your Shuffle(List<T>) method
        aiChosenTwo.Clear();
        aiChosenTwo.Add(all[0]);
        aiChosenTwo.Add(all[1]);

        // Reveal text
        playerChosenText.text = $"Player chose: {selectedTwo[0]}, {selectedTwo[1]}";
        aiChosenText.text = $"AI chose: {aiChosenTwo[0]}, {aiChosenTwo[1]}";

        // Show opponent chosen cards visually
        HideAllOpponentCards();
        ShowOpponentCard(aiChosenTwo[0], true);
        ShowOpponentCard(aiChosenTwo[1], true);

        // UI instruction
        resultText.text = "Pick your final card";

        // Only allow clicking the two chosen cards now
        rockPlayerButton.interactable = selectedTwo.Contains(CardType.Rock);
        paperPlayerButton.interactable = selectedTwo.Contains(CardType.Paper);
        scissorsPlayerButton.interactable = selectedTwo.Contains(CardType.Scissors);
    }
    
    void ResolveFinalPick(CardType playerFinal)
    {
        if (!pickingFinal) return;
        pickingFinal = false;

        aiFinalChoice = aiChosenTwo[Random.Range(0, aiChosenTwo.Count)];

        // Hide all cards first
        HideAllPlayerCards();
        HideAllOpponentCards();

        // Show only final cards
        ShowPlayerCard(playerFinal, true);
        ShowOpponentCard(aiFinalChoice, true);

        int result = Compare(playerFinal, aiFinalChoice);

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

        UpdateScoreUI();
        if (currentRound >= 3)
        {
            // show final message immediately
            if (playerScore > aiScore) resultText.text = "YOU WIN!";
            else if (playerScore < aiScore) resultText.text = "AW, NEXT TIME!";
            else resultText.text = "OU, TIE!";

            gameOver = true;
            nextRoundButtonLabel.text = "Start Game";
            nextRoundButton.interactable = true;
            confirmPickTwoButton.interactable = false;

            // lock partner buttons since game ended
            SetPartnerButtonsInteractable(false);
            return;
        }
        nextRoundButton.interactable = true;
    }
    
    void UpdateScoreUI()
    {
        playerText.text = $"Player: {playerScore}";
        aiText.text = $"AI: {aiScore}";
    }
    
    void SetPartnerButtonsInteractable(bool on)
    {
        if (rockPlayerButton) rockPlayerButton.interactable = on;
        if (paperPlayerButton) paperPlayerButton.interactable = on;
        if (scissorsPlayerButton) scissorsPlayerButton.interactable = on;
    }

    void HideAllPlayerOutlines()
    {
        if (rockPlayerOutline) rockPlayerOutline.SetActive(false);
        if (paperPlayerOutline) paperPlayerOutline.SetActive(false);
        if (scissorsPlayerOutline) scissorsPlayerOutline.SetActive(false);
    }

    void HideAllOpponentCards()
    {
        if (rockOpponentCard) rockOpponentCard.SetActive(false);
        if (paperOpponentCard) paperOpponentCard.SetActive(false);
        if (scissorsOpponentCard) scissorsOpponentCard.SetActive(false);
    }
    
    void ShowOpponentCard(CardType type, bool on)
    {
        switch (type)
        {
            case CardType.Rock: if (rockOpponentCard) rockOpponentCard.SetActive(on); break;
            case CardType.Paper: if (paperOpponentCard) paperOpponentCard.SetActive(on); break;
            case CardType.Scissors: if (scissorsOpponentCard) scissorsOpponentCard.SetActive(on); break;
        }
    }
    
    void SetOutline(CardType type, bool on)
    {
        switch (type)
        {
            case CardType.Rock: if (rockPlayerOutline) rockPlayerOutline.SetActive(on); break;
            case CardType.Paper: if (paperPlayerOutline) paperPlayerOutline.SetActive(on); break;
            case CardType.Scissors: if (scissorsPlayerOutline) scissorsPlayerOutline.SetActive(on); break;
        }
    }
    
    void HideAllPlayerCards()
    {
        if (rockPlayerCard) rockPlayerCard.SetActive(false);
        if (paperPlayerCard) paperPlayerCard.SetActive(false);
        if (scissorsPlayerCard) scissorsPlayerCard.SetActive(false);
    }

    void ShowPlayerCard(CardType t, bool on)
    {
        switch (t)
        {
            case CardType.Rock: if (rockPlayerCard) rockPlayerCard.SetActive(on); break;
            case CardType.Paper: if (paperPlayerCard) paperPlayerCard.SetActive(on); break;
            case CardType.Scissors: if (scissorsPlayerCard) scissorsPlayerCard.SetActive(on); break;
        }
    }
}