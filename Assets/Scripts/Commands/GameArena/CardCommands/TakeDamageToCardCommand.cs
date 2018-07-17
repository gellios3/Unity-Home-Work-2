﻿using Models;
using Models.Arena;
using strange.extensions.command.impl;
using Signals;
using UniRx.Triggers;
using UnityEngine;
using View.DeckItems;
using LogType = Models.LogType;

namespace Commands.GameArena.CardCommands
{
    public class TakeDamageToCardCommand : Command
    {
        [Inject] public DamageStruct DamageStruct { get; set; }

        /// <summary>
        /// Battle
        /// </summary>
        [Inject]
        public BattleArena BattleArena { get; set; }

        /// <summary>
        /// Add history log signal
        /// </summary>
        [Inject]
        public AddHistoryLogSignal AddHistoryLogSignal { get; set; }

        public override void Execute()
        {
            var activePlayer = BattleArena.GetActivePlayer();
            var sourceCard = DamageStruct.SourceCardView.Card;
            var damageCard = DamageStruct.DamageCardView.Card;

            Debug.Log(sourceCard.SourceCard.name + " " + damageCard.SourceCard.name);

            if (sourceCard.TakeDamage(damageCard))
            {
                AddHistoryLogSignal.Dispatch(new[]
                {
                    "Player '", activePlayer.Name, "' Use Card '", damageCard.SourceCard.name,
                    "' hit CRITICAL ememy Card '", sourceCard.SourceCard.name, "'"
                }, LogType.Battle);
            }
            else
            {
                AddHistoryLogSignal.Dispatch(new[]
                {
                    "Player '", activePlayer.Name, "' Use Card '", damageCard.SourceCard.name,
                    "' hit ememy Card '", sourceCard.SourceCard.name, "' take damage '", damageCard.Attack.ToString(),
                    "'"
                }, LogType.Battle);
            }

            // Emeny cart return attack
            damageCard.TakeDamage(sourceCard, false);
            AddHistoryLogSignal.Dispatch(new[]
            {
                "Enemy Card '", sourceCard.SourceCard.name,
                "' return  damage '", sourceCard.Attack.ToString(), "' to '", damageCard.SourceCard.name, "'"
            }, LogType.Battle);


            if (damageCard.Status == BattleStatus.Dead)
            {
                AddHistoryLogSignal.Dispatch(new[] {"Player Card '", damageCard.SourceCard.name, "' has dead!"},
                    LogType.Battle);
                DamageStruct.DamageCardView.DestroyView();
            }
            else
            {
                damageCard.Status = BattleStatus.Sleep;
                Debug.Log("Player card '" + damageCard.SourceCard.name + "' has '" + damageCard.Health +
                          "' Health and '" + damageCard.Defence + "' Defence");
                DamageStruct.DamageCardView.Init(damageCard);
            }

            if (sourceCard.Status == BattleStatus.Dead)
            {
                AddHistoryLogSignal.Dispatch(new[] {"Enemy Card '", sourceCard.SourceCard.name, "' has dead!"},
                    LogType.Battle);
                DamageStruct.SourceCardView.DestroyView();
            }
            else
            {
                Debug.Log("Enemy card '" + sourceCard.SourceCard.name + "' has '" + sourceCard.Health +
                          "' Health and '" + sourceCard.Defence + "' Defence");
                DamageStruct.SourceCardView.Init(sourceCard);
            }
        }
    }
}