using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

/*
 * Skrivet av Amanda Hwatz Björkholm
 * Kurs: DT071G - Programmering i C# .NET
 * Mittuniveristetet 2021
 */

namespace hangman_project
{
    class SinglePlayer
    {
        private List<string> _wordList;
        private List<string> _guesses;
        
        protected string _word;
        protected string _wordStatus;
        protected int _tries;
        protected int _failedAttempts;

        // Konstruktor
        public SinglePlayer()
        {
            // Läs ord från textfil och spara i en lista
            try
            {
                // Hämta ordlista
                using (StreamReader sr = new StreamReader("wordList.txt"))
                {
                    string line = "";
                    this._wordList = new List<string>();

                    // Loopa igenom alla rader och spara i lista
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
            
            // Hämta slumpat ord
            int idx = new Random().Next(this._wordList.Count);
            this._word = this._wordList[idx];

            // Sätt array med gissade bokstäver
            this._guesses = new List<string>();
            
            // Sätt status på ordet
            this._wordStatus = "";
            // Lägg in ett "_" för varje bokstav i ordet
            for (int i = 0; i < this._word.Length; i++)
            {
                this._wordStatus = this._wordStatus.Insert(0,"_");
            }

            // Sätt hur många försök
            this._tries = 8;
            
            // Sätt misslyckade gissningar
            this._failedAttempts = 0;
        }

        // Skriv ut status på ordet, (tex a_a__a)
        public void PrintStatus()
        {
            Console.Write("Guessed letters: ");
            foreach (var guess in this._guesses)
            {
                Console.Write($"{guess}, ");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Secret word:");
            Console.WriteLine(this._wordStatus);
        }

        // Kontrollera gissad bokstav
        public string GuessLetter(string letter, string gameMode)
        {
            string message = "";
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Hangman | {gameMode}");
            Console.ForegroundColor = ConsoleColor.Blue;

            // Om bokstav redan gissad 
            if (this._guesses.Contains(letter))
            {
                Console.WriteLine();
                Console.WriteLine($"You have already guessed letter {letter}. Try again.");
            }
            else if(this._word.Contains(letter))
            {
                // Lägg till bokstav bland gissade bokstäver
                this._guesses.Add(letter);
                
                // Gör om till char-array för att kunna jämföra med bokstav i ordet
                var character = letter.ToCharArray();
                var statusChar = this._wordStatus.ToCharArray();

                // Ersätt _ med bokstav på rätt index
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
                            string fails = this._failedAttempts.ToString();
                            message = "winner " + fails;
                            Console.WriteLine();
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
                    // Räkna misslyckad gissning
                    this._failedAttempts += 1;
                    
                    Console.WriteLine();
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

    // Multiplayer klass ärver från single player
    class Multiplayer : SinglePlayer
    {
        public Multiplayer(string word)
        {
            this._word = word;
            this._tries = 8;
            this._wordStatus = "";
            
            // Sätt ord med det som matats in från spelare 1
            for (int i = 0; i < this._word.Length; i++)
            {
                this._wordStatus = this._wordStatus.Insert(0,"_");
            }
        }
    }

    // Ett Highscore
    class Highscore
    {
        private string _nickname;
        private string _failedAttempts;
    
        // Sätt namn och spelresultat
        public Highscore(string nickname, string failedAttempts)
        {
            this._nickname = nickname;
            this._failedAttempts = failedAttempts;
        }

        public string GetNickname()
        {
            return this._nickname;
        }

        public string GetFailedAttempts()
        {
            return this._failedAttempts;
        }
    }

    // Alla highscores
    class Highscores
    {
        private List<Highscore> _highscores;

        public Highscores()
        {
            try
            {
                // Hämta hela listan med highscores om filen finns
                using (StreamReader sr = new StreamReader("highscores.txt"))
                {
                    string line = "";
                    this._highscores = new List<Highscore>();

                    // Loopa igenom alla rader i textfilen
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "")
                        {
                            continue;
                        }
                        
                        // Separera highscore på namn och spelresultat. Separeras med :
                        // Skapa objekt av varje highscore
                        string[] lineArr = line.Split(':');
                        Highscore readHighscore = new Highscore(lineArr[0], lineArr[1]);
                        
                        // Lägg till alla objekt i listan
                        this._highscores.Add(readHighscore);
                    }
                    sr.Close(); // Stäng streamreader
                }
            }
            catch
            {
                // Tom lista om filen inte finns
                this._highscores = new List<Highscore>();
            }
        }
        
        public void WriteHighscores()
        {
            // Skriv ut allt i listan
            int i = 1;
            if (this._highscores != null)
            {
                Console.Clear();
                Console.WriteLine("HIGHSCORES");
                Console.WriteLine();
                // Sortera lista med spelaren med färst felgissningar högst upp
                IOrderedEnumerable<Highscore> orderedList = this._highscores.OrderBy(highscore => highscore.GetFailedAttempts());
                this._highscores = orderedList.ToList();
                if (this._highscores.Count() > 10)
                {
                    Console.WriteLine(this._highscores.Count());
                    
                    this._highscores.RemoveAt(10);

                    // Spara hela listan på nytt. Separera namn och spelresultat med :
                
                    using (StreamWriter sw = new StreamWriter("highscores.txt"))
                    {
                        foreach (var highscore in this._highscores)
                        {
                            sw.WriteLine($"{highscore.GetNickname()}:{highscore.GetFailedAttempts()}");
                        }
                        sw.Close(); // Stäng streamwriter
                    }
                }
                // Skriv ut hela listan
                foreach (var highscore in this._highscores)
                {
                    // int score = Int32.Parse(highscore.GetFailedAttempts());
                    // score = 8 - score;
                    Console.WriteLine($"{i}. {highscore.GetNickname()} - {highscore.GetFailedAttempts()} failed attempts");
                    i += 1;
                } 
            }
        }

        public void AddHighscore(Highscore highscore)
        {
            // Lägg till highscore i filen. Om filen inte finns skapas den
            using (StreamWriter sw = new StreamWriter("highscores.txt", true))
            {
                sw.WriteLine($"{highscore.GetNickname()}:{highscore.GetFailedAttempts()}");
                sw.Close();
                
                this._highscores.Add(highscore);
            }
        }
    }

    internal static class Program
    {
        public static void PlaySinglePlayer()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Hangman | Single player");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine("Tries left: 8");
            
            SinglePlayer singlePlayer = new SinglePlayer();

            string gameStatus = "";
            
            while (true)
            {
                singlePlayer.PrintStatus();
            
                Console.WriteLine();
                Console.WriteLine("Guess a letter");
                string guess = Console.ReadLine();
                
                // Kontrollera att det endast är en bokstav och inte null
                Regex rx = new Regex(@"^[a-zåäö]$");
                if (guess != null && rx.IsMatch(guess))
                {

                    // Skicka gissad bokstav samt om multi- eller single player
                    gameStatus = singlePlayer.GuessLetter(guess, "Single player");

                    // Kontrollera om spelaren vunnit
                    if (gameStatus.Contains("winner"))
                    {
                        RegisterHighscore(gameStatus);
                        break;
                    }

                    // Kontrollera om spelaren förlorat
                    if (gameStatus.Contains("GAME"))
                    {
                        Console.WriteLine(gameStatus);
                        PrintRedoMenu();
                        break;
                    }
                }
                else
                {
                    // Om regex inte matchar
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Hangman | Single player");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine();
                    Console.WriteLine("You can only type one (1) letter in lowercase.");
                }
            }
        }

        // Multiplayer
        public static void PlayMultiplayer()
        {
            // Sätt färgtema samt ta emot ord från spelare 1
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Hangman | Multiplayer");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine("Player 1, type a word for player 2 to guess.");
            string word = Console.ReadLine();
            
            // Kontrollera att inmatat värde är minst 4 bokstäver i gemener
            Regex rxWord = new Regex(@"^[a-zåäö]{4,}$");

            // Om inmatat värde inte matchar regex, be om ord tills det är korrekt
            while (!rxWord.IsMatch(word))
            {
                Console.WriteLine("You must type a word with at least 4 letters in lowercase and no symbols or numbers.");
                Console.WriteLine();
                Console.WriteLine("Type a word.");
                word = Console.ReadLine();
            }

            // Nytt objekt
            Multiplayer multiplayer = new Multiplayer(word);
            
            // Spelare 2 ska gissa
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Hangman | Multiplayer");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine("Tries left: 8");
            
            string gameStatus = "";
            
            // Låt spelare 2 gissa tills gamestatus är winner eller GAME OVER
            while (true)
            {
                multiplayer.PrintStatus();
            
                Console.WriteLine();
                Console.WriteLine("Player 2, guess a letter");
                string guess = Console.ReadLine();

                // Kontrollera att det endast är en bokstav och inte null
                Regex rx = new Regex(@"^[a-zåäö]$");
                if (guess != null && rx.IsMatch(guess))
                {
                    // Skicka gissad bokstav
                    gameStatus = multiplayer.GuessLetter(guess, "Multiplayer");

                    if (gameStatus.Contains("winner"))
                    {
                        RegisterHighscore(gameStatus);
                        break;
                    }

                    if (gameStatus.Contains("GAME"))
                    {
                        Console.WriteLine(gameStatus);
                        PrintRedoMenu();
                        break;
                    }
                }
                else // Om regex inte matchar
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Hangman | Multiplayer");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine();
                    Console.WriteLine("You can only type one (1) letter in lowercase.");
                }
            }
        }

        // Spara highscore och spelresultat
        public static void RegisterHighscore(string result)
        {
            Console.WriteLine("YOU WON!");
            bool redo = true;
            while (redo)
            {
                // Meny
                Console.WriteLine();
                Console.WriteLine("[r] Register highscore");
                Console.WriteLine("[x] Don't register highscore");
                Console.WriteLine();
                string choise = Console.ReadLine();

                if (choise == "r")
                {
                    // LÄGG TILL HIGHSCORE
                    Console.WriteLine("Enter a nickname");
                    string nickname = Console.ReadLine();
                    
                    // Kontrollera nickname. Minst 3 bokstäver, stora eller små.
                    Regex rx = new Regex(@"^([A-ZÅÄÖa-zåäö]{3,})$");
                    if (nickname != null && rx.IsMatch(nickname))
                    {
                        // Plocka ut namn och spelresultatet och skapa objekt
                        string[] partsArr = result.Split(' ');

                        Highscore highscore = new Highscore(nickname, partsArr[1]);
                        Highscores obj = new Highscores();
                        
                        // Spara highscore
                        obj.AddHighscore(highscore);
                        
                        redo = false;
                        
                        Console.Clear();
                        Console.WriteLine("Highscore registered.");
                        PrintRedoMenu();
                    }
                    else // Om regex inte matchar
                    {
                        // Om regex inte matchar
                        Console.Clear();
                        Console.WriteLine("You can only type letters in lowercase or uppercase. No symbols allowed.");
                    }
                }

                // Skriv ut liten meny
                if (choise == "x")
                {
                    redo = false;
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("HANGMAN");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    PrintRedoMenu();
                }

                // Om användare matar in annat än r eller x
                if (choise != "r" && choise != "x")
                {
                    Console.WriteLine("You can choose r or x from the menu.");
                }
            }
        }

        // Skriv highscore-lista
        public static void PrintHighscore()
        {
            // Nytt objekt, skriv ut lista
            Highscores obj = new Highscores();
            obj.WriteHighscores();
            bool redo = true;

            while (redo)
            {
                Console.WriteLine();
                Console.WriteLine("[x] Return to menu");
                string exit = Console.ReadLine();

                if (exit == "x")
                {
                    redo = false;
                    PrintMenu();
                }
                else
                {
                    Console.WriteLine("Press x to return to menu");
                }
            }
        }

        // Huvudmeny
        public static void PrintMenu()
        {
            bool redo = true;
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("HANGMAN");
            Console.ForegroundColor = ConsoleColor.Blue;
            

            // Loopa menyn tills ett korrekt värde matas in
            while (redo)
            {
                // Meny
                Console.WriteLine("How do you want to play?");
                Console.WriteLine();
                Console.WriteLine("[1] Single player");
                Console.WriteLine("[2] Multiplayer");
                Console.WriteLine("[3] View Highscores");
                Console.WriteLine("[x] Exit Game");

                Console.WriteLine();
                string menuChoise = Console.ReadLine();

                // Hantera menyval
                switch (menuChoise)
                {
                    case "1" :
                        PlaySinglePlayer();
                        redo = false;
                        break;
                    case "2":
                        PlayMultiplayer();
                        redo = false;
                        break;
                    case "3":
                        PrintHighscore();
                        redo = false;
                        break;
                    case "x":
                        Console.WriteLine("Shutting off.");
                        redo = false;
                        break;
                    default:
                        Console.WriteLine("You can choose 1, 2, 3, or x from the menu.");
                        break;
                }
            }
        }

        // Skriv ut meny efter avslutad omgång
        public static void PrintRedoMenu()
        {
            bool redo = true;
            while (redo)
            {
                Console.WriteLine();
                Console.WriteLine("[1] Return to menu");
                Console.WriteLine("[x] Exit game");
                Console.WriteLine();
                string menuChoise = Console.ReadLine();

                if (menuChoise == "1")
                {
                    PrintMenu();
                    redo = false;
                }
                if(menuChoise == "x")
                {
                    Console.WriteLine("Shutting off.");
                    redo = false;
                }
                if(menuChoise != "1" && menuChoise != "x")
                {
                    Console.WriteLine("You can choose 1 or x from the menu.");
                }
            }
        }
        
        // Main
        public static void Main(string[] args)
        {
            Console.Clear();
            PrintMenu(); // Huvudmeny
        }
    }
}