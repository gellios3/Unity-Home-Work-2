﻿using Models.Arena;
using strange.extensions.command.impl;
using Services;
using Signals.GameArena;
using Signals.GameArena.CardSignals;
using Signals.GameArena.TrateSignals;

namespace Commands.GameArena
{
    public class InitBattleTurnCommand : Command
    {
        /// <summary>
        /// Init card deck signal
        /// </summary>
        [Inject]
        public InitCardDeckSignal InitCardDeckSignal { get; set; }

        /// <summary>
        /// Init trate deck signal
        /// </summary>
        [Inject]
        public InitTrateDeckSignal InitTrateDeckSignal { get; set; }

        /// <summary>
        /// Init mana signal
        /// </summary>
        [Inject]
        public InitManaSignal InitManaSignal { get; set; }

        /// <summary>
        /// Init battle arena signal
        /// </summary>
        [Inject]
        public InitBattleArenaSignal InitBattleArenaSignal { get; set; }

        /// <summary>
        /// Init hand pull signal
        /// </summary>
        [Inject]
        public InitHandPullSignal InitHandPullSignal { get; set; }

        /// <summary>
        /// Battle Arena
        /// </summary>
        [Inject]
        public BattleArena BattleArena { get; set; }

        /// <summary>
        /// State service
        /// </summary>
        [Inject]
        public StateService StateService { get; set; }

        /// <summary>
        /// Show turn popup signal
        /// </summary>
        [Inject]
        public ShowTurnPopupSignal ShowTurnPopupSignal { get; set; }

        /// <summary>
        /// Arena
        /// </summary>
        [Inject]
        public Arena Arena { get; set; }

        /// <summary>
        /// Execute event init arena
        /// </summary>
        public override void Execute()
        {
            // Init Active player 
            StateService.InitActivePlayer(BattleArena.ActiveSide == BattleSide.Player
                ? Arena.Player
                : Arena.Opponent);

            // init active turn
            // Increase turn count
            StateService.IncreaseTurnCount();

            // init turn history
            BattleArena.InitHistory();

            // On 2 Turn add more carts 
            BattleArena.CountOfCardsAddingToHand = Arena.CartToAddCount;
            if (StateService.TurnCount == 2)
            {
                BattleArena.CountOfCardsAddingToHand++;
            }

            // Init card desk
            InitCardDeckSignal.Dispatch();

            // Init trate deck signal
            InitTrateDeckSignal.Dispatch();

            // Init mana pull
            BattleArena.GetActivePlayer().InitManaPull();

            // Init mana view
            InitManaSignal.Dispatch();

            // Init hand panel
            InitHandPullSignal.Dispatch();

            //Init battle arena
            InitBattleArenaSignal.Dispatch();

            // show new turn popup
            ShowTurnPopupSignal.Dispatch();
        }
    }
}