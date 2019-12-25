using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;

public sealed class AtomsReferences : MonoBehaviour
{
    [SerializeField] private VoidEvent countDownEndEvent;
    [SerializeField] private Collider2DEvent collisionBallEvent;
    [SerializeField] private IntVariable scoreVariable;

    public VoidEvent StartGameEvent { get => countDownEndEvent; }
    public Collider2DEvent CollisionBallEvent { get => collisionBallEvent; }
    public IntVariable ScoreVariable { get => scoreVariable; }
}
