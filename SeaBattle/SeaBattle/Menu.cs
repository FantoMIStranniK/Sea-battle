﻿using System;
using System.Collections.Generic;
using System.IO;

namespace SeaBattle
{
    class Menu
    {
        private int gamemodeNumber = 0;
        private int profileNumber = 0;

        private GameHandler gameHandler = new GameHandler();

        private List<string> profileNames = new List<string>()
        {
            "Profile1",
            "Profile2",
            "Profile3",
        };

        private PlayerProfile currentProfile = null;

        public void LaunchMenu()
        {
            TryCreateProfiles();

            Console.WriteLine("Welcome to Sea battle!");
            Console.WriteLine("Choose your profile");
            Console.WriteLine("Available profiles:");

            WriteAvailableProfiles();

            GetProfileInput();

            ChooseProfile();

            Console.WriteLine("Choose gamemode:");
            Console.WriteLine("Write 1 - Player VS AI");
            Console.WriteLine("Write 2 - Player VS Player");
            Console.WriteLine("Write 3 - AI VS AI (Spectate battle)");

            GetGamemodeInput();

            HandleInput();
        }
        private void WriteAvailableProfiles()
        {
            for (int i = 0; i < profileNames.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {profileNames[i]}");
            }
        }
        private void TryCreateProfiles()
        {
            foreach (string profileName in profileNames) 
            {
                if (!File.Exists(XMLManager.pathToProfiles + @"\" + profileName + ".xml"))
                    CreateProfileXML(profileName);
            }

            if (!File.Exists(XMLManager.pathToProfiles + @"\" + "Ai1.xml"))
                CreateProfileXML("Ai1");
            if (!File.Exists(XMLManager.pathToProfiles + @"\" + "Ai2.xml"))
                CreateProfileXML("Ai2");
        }
        private void CreateProfileXML(string profileName)
        {
            if(!File.Exists(XMLManager.pathToProfiles + @"\" + profileName + ".xml"))
            {
                PlayerProfile profile = new PlayerProfile(profileName);

                XMLManager.SerializeXML(profile, profileName + ".xml");
            }
        }
        private void GetProfileInput()
        {
            bool isCorrectInput = false;

            do
            {
                profileNumber = 0;

                isCorrectInput = int.TryParse(Console.ReadLine(), out profileNumber);

                if (profileNumber > profileNames.Count || profileNumber <= 0)
                    isCorrectInput = false;

                if (!isCorrectInput)
                    Console.WriteLine("Wrong profile index");

            } while (!isCorrectInput);
        }
        private void ChooseProfile()
        {
            currentProfile = XMLManager.DeserializeXML(profileNames[profileNumber - 1] + ".xml");
        }
        private void GetGamemodeInput()
        {
            bool isCorrectInput = false;

            do
            {
                gamemodeNumber = 0;

                isCorrectInput = int.TryParse(Console.ReadLine(), out gamemodeNumber);

                if(gamemodeNumber > 3 || gamemodeNumber < 1) 
                    isCorrectInput = false;

                if (!isCorrectInput) 
                    Console.WriteLine("Wrong input");
            }
            while (!isCorrectInput);
        }
        private void HandleInput()
        {
            switch (gamemodeNumber) 
            {
                case 1:
                    gameHandler.LaunchGame(false, true, currentProfile);
                    break;
                case 2:
                    //gameHandler.LaunchGame(false, false, currentProfile);
                    Console.WriteLine("There is no multiplayer puk-puk :(");
                    break;
                case 3:
                    gameHandler.LaunchGame(true, true, currentProfile);
                    break;
            }
        }
    }
}
