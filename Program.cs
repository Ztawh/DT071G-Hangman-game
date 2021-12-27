using System;
using System.IO;
using System.Collections.Generic;

namespace hangman_project
{
    class SinglePlayer
    {
        private List<string> _wordList;
        
        protected string _word;
        protected List<string> _guesses;
        protected string _wordStatus;
        protected int _tries;

        // Konstruktor
        public SinglePlayer()
        {
            /* Läs ord från textfil och spara i en lista */
            try
            {
                /*Hämta ordlista*/
                using (StreamReader sr = new StreamReader("wordList.txt"))
                {
                    string line = "";
                    this._wordList = new List<string>();

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "") { continue; }
                        this._wordList.Add(line);
                    }

                    sr.Close();
                }
            }
            catch // Om textfilen skulle vara tom
            {
                this._wordList = new List<string>();
            }
            
            /* Hämta slumpat ord */
            int idx = new Random().Next(this._wordList.Count);
            this._word = this._wordList[idx];

            /* Sätt array med gissade bokstäver */
            this._guesses = new List<string>();
            
            /* Sätt status på ordet */
            this._wordStatus = "";
            
            for (int i = 0; i < this._word.Length; i++)
            {
                this._wordStatus = this._wordStatus.Insert(0,"_");
            }

            /* Sätt hur många försök */
            this._tries = 8;
        }

        public void printStatus()
        {
            Console.WriteLine(this._wordStatus);
        }

        public string guessLetter(string letter, string gameMode)
        {
            string message = "";
            Console.Clear();
            Console.WriteLine($"Hangman | {gameMode}");
            Console.WriteLine();
            
            /* Om bokstav redan gissad */
            if (this._guesses.Contains(letter))
            {
                 //message = "guessed";
                 Console.WriteLine($"You have already guessed letter {letter}. Try again.");
            }
            else if(this._word.Contains(letter))
            {
                // Lägg till bokstav bland gissade bokstäver
                this._guesses.Add(letter);
                
                /* Ersätt _ med bokstav på rätt index */

                /* Gör om till char-array för att kunna jämföra med bokstav i ordet */
                var character = letter.ToCharArray();
                var statusChar = this._wordStatus.ToCharArray();

                for (int i = 0; i < this._word.Length; i++)
                {
                    if (this._word[i] == character[0])
                    {
                        // Jämför gissade bokstaven med varje bokstav i ordet. Uppdatera sen wordStatus
                        statusChar[i] = character[0];
                        this._wordStatus = new string(statusChar);
                        
                        // Kontrollera om hela ordet är gissat
                        if (this._wordStatus == this._word)
                        {
                            message = "winner";
                            Console.WriteLine($"The word was: {this._word}");
                        }
                    }
                }
            }
            else
            {
                this._tries -= 1;
                if (this._tries > 0)
                {
                    // Lägg till bokstav bland gissade bokstäver
                    this._guesses.Add(letter);
                    
                    string triesLeft = this._tries.ToString();
                    Console.WriteLine($"Letter '{letter}' is wrong, try again.");
                }
                else
                {
                    message = "GAME OVER. The word was: " + this._word;
                }
            }

            if (message.Contains("GAME OVER") || message.Contains("winner"))
            {
                return message;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"Tries left: " + this._tries);
                return message;
            }
        }
    }

    class Multiplayer : SinglePlayer
    {
        public Multiplayer(string word)
        {
            this._word = word;
            this._tries = 8;
            this._wordStatus = "";
            
            for (int i = 0; i < this._word.Length; i++)
            {
                this._wordStatus = this._wordStatus.Insert(0,"_");
            }
        }
        
        
        
    }

    internal static class Program
    {
        public static void PlaySinglePlayer()
        {
            Console.Clear();
            Console.WriteLine("Hangman | Single player");
            Console.WriteLine();
            Console.WriteLine("Tries left: 8");
            
            SinglePlayer singlePlayer = new SinglePlayer();

            string gameStatus = "";
            
            // while (gameStatus != "game over")
            while (true)
            {
                Console.WriteLine("Secret word:");
                singlePlayer.printStatus();
            
                Console.WriteLine("Guess a letter");
                string guess = Console.ReadLine();

                // Skicka gissad bokstav
                gameStatus = singlePlayer.guessLetter(guess, "Single player");

                if (gameStatus == "winner")
                {
                    Console.WriteLine("YOU WON!");
                    break;
                }

                if (gameStatus.Contains("GAME"))
                {
                    Console.WriteLine(gameStatus);
                    break;
                }
            }
        }

        public static void PlayMultiplayer()
        {
            Console.WriteLine("Hangman | Multiplayer");
            Console.WriteLine();
            Console.WriteLine("Player 1, type a word for player 2 to guess.");
            string word = Console.ReadLine();

            Multiplayer multiplayer = new Multiplayer(word);
            
            Console.Clear();
            Console.WriteLine("Hangman | Multiplayer");
            Console.WriteLine();
            Console.WriteLine("Tries left: 8");
            
            string gameStatus = "";
            
            // while (gameStatus != "game over")
            while (true)
            {
                Console.WriteLine("Secret word:");
                multiplayer.printStatus();
            
                Console.WriteLine("Player 2, guess a letter");
                string guess = Console.ReadLine();

                // Skicka gissad bokstav
                gameStatus = multiplayer.guessLetter(guess, "Multiplayer");

                if (gameStatus == "winner")
                {
                    Console.WriteLine("YOU WON!");
                    break;
                }

                if (gameStatus.Contains("GAME"))
                {
                    Console.WriteLine(gameStatus);
                    break;
                }
            }
        }
        
        // Main
        public static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Play a game of Hangman");
            Console.WriteLine();
            
            // Meny
            Console.WriteLine("How do you want to play?");
            Console.WriteLine("Single player: press 1");
            Console.WriteLine("Multiplayer: press 2");
            Console.WriteLine("Exit game: press x");

            string menuChoise = Console.ReadLine();

            // Hantera menyval
            switch (menuChoise)
            {
                case "1" :
                    PlaySinglePlayer();
                    break;
                case "2":
                    PlayMultiplayer();
                    break;
                case "x":
                    Console.WriteLine("Shutting off.");
                    break;
            }

        }
    }
}