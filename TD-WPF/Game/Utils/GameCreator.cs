﻿using System;
using System.Collections.Generic;
using TD_WPF.Game.Objects.RoundObjects;
using TD_WPF.Game.Objects.StaticGameObjects;

namespace TD_WPF.Game.Utils
{
    public class GameCreator // equivalent zur Spielfeldklasse
    {
        public GameCreator(GameControl gameControl)
        {
            GameControl = gameControl;
        }

        public void InitilizeRandomGame()
        {
            var random = new Random();
            Health = random.Next(50, 101);
            Money = 50;
            InitilizeRandomPath();
            InitializeRandomWaves();
        }

        private void InitilizeRandomPath()
        {
            Paths = GameUtils.GenerateRandomPath(GameControl.Canvas.ActualWidth,
                GameControl.Canvas.ActualHeight, X, Y);
            Ground = GameUtils.GenerateRandomGround(Paths, GameControl.Canvas.ActualWidth,
                GameControl.Canvas.ActualHeight, X, Y);
        }

        private void InitializeRandomWaves()
        {
            Waves = GameUtils.GenerateRandomWaves(5000, 2000, (Spawn) Paths[0]);
        }

        #region attributes

        // base and spawn will be stored in paths too
        public List<Path> Paths { get; private set; } = new List<Path>();

        // we dont need towers because we will get them by iterationg the ground which referes to the tower
        public List<Ground> Ground { get; private set; } = new List<Ground>();

        public Waves Waves { get; set; }
        private GameControl GameControl { get; }
        public int X { get; set; } = 20;
        public int Y { get; set; } = 15;
        public int Health { get; set; } = 100;
        public int Money { get; set; } = 50;

        #endregion
    }
}