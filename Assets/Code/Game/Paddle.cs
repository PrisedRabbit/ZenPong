using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PongGame
{
    public class Paddle : MonoBehaviour, IHorizontalMoveable
    {
        public float speed;

        [SerializeField] private Rigidbody2D rb = default;

        public void MoveHorizontal(float step)
        {
            var pos = rb.position;
            pos.x = Mathf.Lerp(pos.x, pos.x + step, Time.fixedDeltaTime * speed);
            rb.MovePosition(pos);
        }
    }
}