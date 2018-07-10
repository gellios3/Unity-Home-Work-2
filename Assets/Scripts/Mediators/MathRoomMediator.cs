﻿using Signals.multiplayer;
using UnityEngine;
using View;
using View.Multiplayer;

namespace Mediators
{
    public class MathRoomMediator : TargetMediator<MathRoomView>
    {
        /// <summary>
        /// Arena initialized signal
        /// </summary>
        [Inject]
        public ServerConnectedSignal ServerConnectedSignal { get; set; }

        /// <summary>
        /// On register mediator
        /// </summary>
        public override void OnRegister()
        {
            ServerConnectedSignal.AddListener(() => { View.OnServerConnected(); });
        }
    }
}