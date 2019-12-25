using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PongGame
{
    public class InputHandler : IFixedTickable, IDisposable
    {
        private readonly Controls controls;

        public InputHandler()
        {
            controls = new Controls();
            controls.Player.Enable();
            controls.Player.Move.started += _ => controlPerformed = true;
            controls.Player.Move.canceled += _ => controlPerformed = false;
        }

        public void AddMovable(IHorizontalMoveable moveable)
        {
            moveables.Add(moveable);
        }

        private readonly List<IHorizontalMoveable> moveables = new List<IHorizontalMoveable>();

        private bool controlPerformed;

        public void FixedTick()
        {
            if (controlPerformed)
            {
                var delta = controls.Player.Move.ReadValue<float>();
                for (int i = 0; i < moveables.Count; i++)
                {
                    moveables[i].MoveHorizontal(delta);
                }
            }
        }

        public void Dispose()
        {
            controls.Dispose();
        }
    }
}