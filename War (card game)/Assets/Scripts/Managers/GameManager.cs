using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

namespace Managers
{
    internal class GameManager : MonoBehaviour
    {
        internal static GameManager Instance;
        private List<Card> _cardsGeneralDeck = new List<Card>();
        private Player User = new();
        private Player Computer = new();
        private Text _userText;
        private Text _computerText;
        public Button gameButton;
        private Image _userCards;
        private Image _computerCards;
        private GameObject _gamePanel;
        private Text _winnerText;
        private bool CanPlay => User.CurrentHand.Count > 0 && Computer.CurrentHand.Count > 0;
        public bool shouldStop = false;
        private bool _debuggedItOnce = false;
        void Awake()
        {
            if (Instance != null)
                return;
            Instance = this;    
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            AssignReferences();
            CreateDesk();

        }

        private void AssignReferences()
        {
            _userText = GameObject.Find("USER TEXT").GetComponent<Text>();             
            _computerText = GameObject.Find("COMPUTER TEXT").GetComponent<Text>();     
            gameButton = GameObject.Find("GameButton").GetComponent<Button>();         
            _userCards = GameObject.Find("UserCards").GetComponent<Image>();           
            _computerCards = GameObject.Find("ComputerCards").GetComponent<Image>();   
            _gamePanel = GameObject.Find("GamePanel");                                 
            _winnerText = GameObject.Find("Winner Text").GetComponent<Text>();    
        }
        
        private void CreateDesk()
        {
            for (int i = 0; i < 13; i++)
            {
                _cardsGeneralDeck.Add(new Card(i+1,Shape.Spade, "black"));
                _cardsGeneralDeck.Add(new Card(i+1,Shape.Club , "black"));
                _cardsGeneralDeck.Add(new Card(i+1,Shape.Heart , "red"));
                _cardsGeneralDeck.Add(new Card(i+1,Shape.Diamond , "red"));
            }

            DealCardsToPlayers();
        }

        private void DealCardsToPlayers()
        {
            ShuffleList(ref _cardsGeneralDeck);
            for (int i = 0; i < 52; i+= 2)
            {
                User.AddCardToQueue(_cardsGeneralDeck[i]);
                Computer.AddCardToQueue(_cardsGeneralDeck[i+1]);
            }
        }
        
        private void ShuffleList(ref List<Card> deck)
        {
            deck = deck.OrderBy(c => new Random().Next()).ToList();
        }

        private void Update()
        {
            if (!CanPlay && !_debuggedItOnce)
            {
                if (User.CardsDeck.Count > 0)
                {
                    CheckIfShouldTakeCardsFromDeck(User,false);
                    return;
                }
                if (Computer.CardsDeck.Count > 0)                  
                {                                              
                    CheckIfShouldTakeCardsFromDeck(Computer,false);
                    return;                                    
                }                                              
            }
            if (!shouldStop && CanPlay)
            {
                gameButton.interactable = true;
            }
        }
        private void CheckGameStatus()
        {
            _userCards.sprite = User.Field.Peek().Image;
            _userCards.enabled = true;
            _computerCards.sprite = Computer.Field.Peek().Image;
            _computerCards.enabled = true;
            switch (CheckRoundStatus())
            {
                case RoundStatus.Win:
                    Debug.Log($@"USER WINS - USER HAD - {User.Field.Peek().Number}, COMPUTER HAD - {Computer.Field.Peek().Number}");
                    AddCardsToWinner(User);
                    break;
                case RoundStatus.Lose:
                    Debug.Log($@"COMPUTER WINS - COMPUTER HAD - {Computer.Field.Peek().Number}, USER HAD - {User.Field.Peek().Number}");
                    AddCardsToWinner(Computer);
                    break;
                case RoundStatus.Tie:
                     HandleTie();
                    break;
            }

            _userText.text = $@"CARDS IN HAND - {User.CurrentHand.Count}      CARDS IN DECK - {User.CardsDeck.Count}";
            _computerText.text = $@"CARDS IN HAND - {Computer.CurrentHand.Count}      CARDS IN DECK - {Computer.CardsDeck.Count}";
        }

            private void CheckTotalGameWinner()
            {
                if (User.CurrentHand.Count == 0 && User.CardsDeck.Count == 0)
                {
                    Debug.Log("COMPUTER WON THE TOTAL GAME");
                    HandleWin("You Win !!");
                }
                else if (Computer.CurrentHand.Count == 0 && Computer.CardsDeck.Count == 0)
                {
                    Debug.Log("USER WON THE TOTAL GAME");
                    HandleWin("Computer Wins !!");
                }
                else if(!CanPlay)
                    Debug.LogWarning("SOMETHING IS FUCKED UP WITH THE CODE");
            }

            private void HandleWin(string winnerText)
            {
                _gamePanel.SetActive(false);
                _winnerText.text = winnerText;
            }

            private void AddCardsToWinner(Player player)
            {
                player.AddCardsToList(MergeStacks(User.Field, Computer.Field));
                StartCoroutine(Sleep());
            }
        private void HandleTie()
        {
            Debug.Log("TIE !!");
            if (User.CurrentHand.Count > 0 && User.CurrentHand.Count < 3)
            {
                CheckIfShouldTakeCardsFromDeck(User, true);
            }
            if (Computer.CurrentHand.Count > 0 && Computer.CurrentHand.Count < 3)
            {
                CheckIfShouldTakeCardsFromDeck(Computer, true);
            }
            
            if (User.CurrentHand.Count >= 3 && Computer.CurrentHand.Count >= 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    User.Field.Push(User.CurrentHand.Dequeue());
                    Computer.Field.Push(Computer.CurrentHand.Dequeue());
                }
                CheckGameStatus();
            }
            else
            {
                FlipCoinDecideWinner();
            }
        }

        private void FlipCoinDecideWinner()
        {
            int Total = User.CurrentHand.Count + Computer.CurrentHand.Count;
            Random rnd = new Random();
            int FlipResult = rnd.Next(1, Total+1);
            if (User.CurrentHand.Count <= FlipResult)
                AddCardsToWinner(User);
            else
                AddCardsToWinner(Computer);
            StartCoroutine(Sleep());
        }
        private IEnumerator Sleep()
        {
            yield return new WaitForSeconds(2);
            shouldStop = false;
        }
        private RoundStatus CheckRoundStatus()
        {
            CheckIfShouldTakeCardsFromDeck(User, false);
            CheckIfShouldTakeCardsFromDeck(Computer, false);
            if (User.Field.Peek().Number  > Computer.Field.Peek().Number) 
                return Computer.Field.Peek().Number == 1 ? RoundStatus.Lose : RoundStatus.Win;
            if (User.Field.Peek().Number < Computer.Field.Peek().Number) 
                return User.Field.Peek().Number == 1 ? RoundStatus.Win : RoundStatus.Lose;
            return RoundStatus.Tie;
        }

        private void CheckIfShouldTakeCardsFromDeck(Player player, bool fromTie)
        {
            int maxValue = fromTie ? 3 : 1;
            if (player.CurrentHand.Count <= maxValue)
            {
                if (player.CardsDeck.Count > 0)
                {
                    ShuffleList(ref player.CardsDeck);
                    player.CardsDeck.ForEach(c => player.AddCardToQueue(c));
                    player.CardsDeck.Clear();
                }
                else
                {
                    CheckTotalGameWinner();
                }
            }
        }
        public Stack<Card> MergeStacks(Stack<Card> s1, Stack<Card> s2)
        {
            while (s2.Count > 0)
                s1.Push(s2.Pop());
            return s1;
        }

        public void ClickToPlayButton()
        {
            gameButton.interactable = false;
            shouldStop = true;
            User.Field.Push(User.CurrentHand.Dequeue());
            Computer.Field.Push(Computer.CurrentHand.Dequeue());
            CheckGameStatus();
        }

        public void PlayAgain()
        {
            _gamePanel.SetActive(true);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}