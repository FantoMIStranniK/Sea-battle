﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SeaBattle
{
    public class Player
    {
        //Random
        private Random rand = new Random();

        //Score
        public int Score { get; private set; } = 0;

        //Bools
        public bool IsAI { get; private set; } = false;
        public bool IsStreak { get; private set; } = false;

        //Cells
        public Map Cells { get; private set; } = new Map();
        public Map VisibleCells { get; private set; } = new Map();

        //Enemy
        private Player enemy;
        
        //Tuples
        private static (int?, int?) aiMemoryCords = (null, null);

        private static (int?, int?) lastShootCords;

        public Player(bool isAi) 
        {
            IsAI = isAi;
        }
        public void SetEnemy(Player enm)
        {
            enemy = enm;
        }
        public void DoTurn()
        {
            if (IsAI)
            {
                AIShoot();
            }
            else
            {
                HumanInput();

                HumanShoot();
            }
        }
        public void AddScore()
        {
            Score++;
        }
        public void ResetScore()
        { 
            Score = 0; 
        }

        #region Human
        private void HumanInput()
        {
            int? Y = null;
            int? X = null;

            do
            {
                Console.WriteLine("Write your Y shoot coordinates");
                Y = GetInput();

                Console.WriteLine("Write your X shoot coordinates");
                X = GetInput();

            } while (enemy.Cells.field[(int)X, (int)Y].IsBombed);

            lastShootCords = (X, Y);
        }
        private int? GetInput()
        {
            string input = Console.ReadLine();

            int number;

            bool result = int.TryParse(input, out number);

            if (!result)
            {
                 ThrowWrongInputException();
                    
                 return null;
            }

            if (Tools.IsValidCordinate(number)) return number;

            ThrowWrongInputException();

            return null;
        }
        private void ThrowWrongInputException()
        {
            Console.WriteLine("Wrong input you dumb fuck");
        }
        private void HumanShoot()
        {
            if (lastShootCords.Item1 is null || lastShootCords.Item2 is null) return;

            int x = (int)lastShootCords.Item1;
            int y = (int)lastShootCords.Item2;

            if (enemy.Cells.field[x, y].IsBombed) return;

            if (enemy.Cells.field[x, y].IsShip) VisibleCells.field[x, y].RevealShip();

            IsStreak = enemy.Cells.field[x, y].IsShip;

            enemy.Cells.field[x, y].DestroyCell();
            VisibleCells.field[x, y].DestroyCell();
        }
        #endregion

        #region AI
        public void ResetMemory()
        {
            aiMemoryCords = (null, null);
        }
        private void AIShoot()
        {
            if (!IsStreak) ShootRandomPoint();
            else GetNearPoints();
        }
        private void ShootRandomPoint()
        {
            int x = rand.Next(0, Tools.fieldSide);
            int y = rand.Next(0, Tools.fieldSide);

            while (enemy.Cells.field[x, y].IsBombed)
            {
                x = rand.Next(0, Tools.fieldSide);
                y = rand.Next(0, Tools.fieldSide);
            }

            CheckIfPlayerShip(x, y);

            enemy.Cells.field[x, y].DestroyCell();
        }
        private void GetNearPoints()
        {
            (int, int)[] points = GetAvailablePoints();

            List<(int, int)> validPoints = ValidatePoints(points);

            if (validPoints.Count is 0) ShootRandomPoint();
            else ShootNearPoints(validPoints);
        }
        private void ShootNearPoints(List<(int, int)> validPoints)
        {
            int randomNearX = validPoints[rand.Next(0, validPoints.Count)].Item1;
            int randomNearY = validPoints[rand.Next(0, validPoints.Count)].Item2;

            CheckIfPlayerShip(randomNearX, randomNearY);

            enemy.Cells.field[randomNearX, randomNearY].DestroyCell();
        }
        private List<(int, int)> ValidatePoints((int, int)[] points)
        {
            List<(int, int)> validPoints = new List<(int, int)>();

            foreach (var point in points)
            {
                if (Tools.IsValidPoint(point) && !IsBombedPosition(point)) validPoints.Add(point);
            }

            return validPoints;
        }
        private void CheckIfPlayerShip(int x, int y)
        {
            if (enemy.Cells.field[x, y].IsShip) MemberPosition(x, y);
            else IsStreak = false;
        }
        private bool IsBombedPosition((int, int) point)
        {
            if (!Tools.IsValidPoint(point)) return true;

            return enemy.Cells.field[point.Item1, point.Item2].IsBombed;
        }
        private void MemberPosition(int x, int y)
        {
            IsStreak = true;

            aiMemoryCords = (x, y);
        }
        private (int, int)[] GetAvailablePoints()
        {
            int startX = (int)aiMemoryCords.Item1;
            int startY = (int)aiMemoryCords.Item2;

            (int, int)[] points = new (int, int)[]
            {
                (startX, startY + 1),
                (startX, startY - 1),
                (startX + 1, startY),
                (startX - 1, startY),
            };

            return points;
        }

        #endregion
    }
}