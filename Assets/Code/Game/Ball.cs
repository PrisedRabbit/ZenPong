﻿using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;
using Zenject;

namespace PongGame
{
    public sealed class Ball : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer srenderere;

        public Vector2 direction;
        public float force;

        public void Push()
        {
            rb.AddForce(direction.normalized * force);
        }

        public class Pool : MonoMemoryPool<Vector2, float, float, Ball>
        {
            [Inject] IDatabase database;

            protected override void Reinitialize(Vector2 direction, float force, float scale, Ball ball)
            {
                ball.rb.velocity = Vector2.zero;
                ball.direction = direction;
                ball.force = force;
                ball.transform.position = Vector2.zero;
                ball.transform.localScale = new Vector3(scale, scale, 1f);
                ball.srenderere.color = database.GetBallColor();
            }
        }
    }
}