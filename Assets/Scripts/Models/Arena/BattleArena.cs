﻿using System;
using System.Collections.Generic;
using Services;
using Signals;
using Random = UnityEngine.Random;

namespace Models.Arena
{
    [Serializable]
    public class BattleArena
    {
        /// <summary>
        /// State service
        /// </summary>
        [Inject]
        public StateService StateService { get; set; }

        /// <summary>
        /// Add history log signal
        /// </summary>
        [Inject]
        public AddHistoryLogSignal AddHistoryLogSignal { get; set; }

        /// <summary>
        /// Batle turn
        /// </summary>
        [Inject]
        public BattleTurnService ActiveBattleTurnService { get; set; }

        /// <summary>
        /// Active state
        /// </summary>
        public BattleSide ActiveSide { get; set; }

        /// <summary>
        /// Battle history
        /// </summary>
        public readonly List<HistoryTurn> History = new List<HistoryTurn>();  

        /// <summary>
        /// Init history
        /// </summary>
        public void InitHistory()
        {
            StateService.InitActiveHistoryTurn();
            History.Add(StateService.ActiveHistotyTurn);
            AddHistoryLogSignal.Dispatch(new[] {"INIT '", StateService.TurnCount.ToString(), "' TURN!"},
                LogType.General);
        }

        /// <summary>
        /// Get arena player
        /// </summary>
        /// <returns></returns>
        public ArenaPlayer GetActivePlayer()
        {
            return StateService.ActiveArenaPlayer;
        }

        /// <summary>
        /// Init battle turn
        /// </summary>
        public void InitActiveTurn()
        {
//            // On 2 Turn add more carts 
//            var addCartcount = Arena.CartToAddCount;
//            if (StateService.TurnCount == 2)
//            {
//                addCartcount++;
//            }
//
//
//            // Add 3 item to hand
//            if (StateService.ActiveArenaPlayer.CardBattlePull.Count > 0)
//            {
//                if (!AddCartToPlayerHand())
//                {
//                    // @todo call not enough space in hand
//                }
//            }
//
//            for (var i = 1; i < addCartcount; i++)
//            {
//                if (Random.Range(0, 2) == 0)
//                {
//                    if (StateService.ActiveArenaPlayer.CardBattlePull.Count <= 0) continue;
//                    if (!AddCartToPlayerHand())
//                    {
//                        // @todo call not enough space in hand
//                    }
//                }
//                else
//                {
//                    if (StateService.ActiveArenaPlayer.TrateBattlePull.Count <= 0) continue;
//                    if (!AddTrateToPlayerHand())
//                    {
//                        // @todo call not enough space in hand            
//                    }
//                }
//            }
        }

        /// <summary>
        /// Fill Battle hand
        /// </summary>
        private bool AddCartToPlayerHand()
        {
            var status = true;

            if (StateService.ActiveArenaPlayer.BattleHand.Count < Arena.HandLimitCount)
            {
                StateService.ActiveArenaPlayer.BattleHand.Add(StateService.ActiveArenaPlayer.CardBattlePull[0]);

                var card = StateService.ActiveArenaPlayer.CardBattlePull[0];
                if (card != null)
                {
                    AddHistoryLogSignal.Dispatch(new[]
                    {
                        "PLAYER '", StateService.ActiveArenaPlayer.Name, "' Add '", card.SourceCard.name,
                        "' Card to Hand"
                    }, LogType.Hand);
                }
            }
            else
            {
                status = false;

                AddHistoryLogSignal.Dispatch(
                    new[] {"PLAYER '", StateService.ActiveArenaPlayer.Name, "' has add Card to Hand ERROR! "},
                    LogType.Hand);
            }

            StateService.ActiveArenaPlayer.CardBattlePull.RemoveAt(0);


            return status;
        }

        /// <summary>
        /// ФAdd trate to player hand
        /// </summary>
        /// <returns></returns>
        private bool AddTrateToPlayerHand()
        {
            var status = true;

            if (StateService.ActiveArenaPlayer.BattleHand.Count < Arena.HandLimitCount)
            {
                StateService.ActiveArenaPlayer.BattleHand.Add(StateService.ActiveArenaPlayer.TrateBattlePull[0]);

                var trate = StateService.ActiveArenaPlayer.TrateBattlePull[0];
                if (trate != null)
                {
                    AddHistoryLogSignal.Dispatch(new[]
                    {
                        "PLAYER '", StateService.ActiveArenaPlayer.Name, "' Add '", trate.SourceTrate.name,
                        "' Trate To Hand"
                    }, LogType.Hand);
                }
            }
            else
            {
                status = false;

                AddHistoryLogSignal.Dispatch(
                    new[] {"PLAYER '", StateService.ActiveArenaPlayer.Name, "' has add Trate to Hand ERROR! "},
                    LogType.Hand);
            }

            StateService.ActiveArenaPlayer.TrateBattlePull.RemoveAt(0);

            return status;
        }

        /// <summary>
        /// End turn
        /// </summary>
        public void EndTurn()
        {
            // Activate all cards and remove dead carts
            foreach (var arenaCard in StateService.ActiveArenaPlayer.ArenaCards)
            {
                if (arenaCard.Status != BattleStatus.Moving) continue;
                arenaCard.Status = BattleStatus.Active;
                AddHistoryLogSignal.Dispatch(new[]
                {
                    "PLAYER '", StateService.ActiveArenaPlayer.Name, "' Activate Moving '", arenaCard.SourceCard.name,
                    "' battle card!"
                }, LogType.Battle);
            }

            // Set active all not dead areana cards 
            foreach (var card in StateService.ActiveArenaPlayer.ArenaCards)
            {
                if (card.Status != BattleStatus.Wait) continue;
                card.Status = BattleStatus.Active;
                // 
                AddHistoryLogSignal.Dispatch(new[]
                {
                    "PLAYER '", StateService.ActiveArenaPlayer.Name, "' Activate sleep '", card.SourceCard.name,
                    "' battle card!"
                }, LogType.Battle);
            }

            // remove all dead carts
            StateService.ActiveArenaPlayer.ArenaCards = StateService.ActiveArenaPlayer.ArenaCards.FindAll(
                card => card.Status == BattleStatus.Active
            );

            // Switch active state
            ActiveSide = ActiveSide == BattleSide.Player ? BattleSide.Opponent : BattleSide.Player;
        }

        /// <summary>
        /// Is game over
        /// </summary>
        /// <param name="arenaPlayer"></param>
        /// <returns></returns>
        public bool IsGameOver(ArenaPlayer arenaPlayer)
        {
            return arenaPlayer.CardBattlePull.Count == 0 &&
                   arenaPlayer.BattleHand.FindAll(item =>
                   {
                       var card = item as BattleCard;
                       return card != null;
                   }).Count == 0 &&
                   arenaPlayer.ArenaCards.FindAll(card => card.Status != BattleStatus.Dead).Count == 0;
        }
    }
    
    public enum BattleSide
    {
        Player,
        Opponent
    }
}