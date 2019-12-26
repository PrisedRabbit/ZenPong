using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;

public sealed class AtomsReferences : MonoBehaviour
{
    [SerializeField] private VoidEvent countDownEndEvent;
    [SerializeField] private VoidEvent restartLevelEvent;
    [SerializeField] private Collider2DEvent collisionBallEvent;
    [SerializeField] private IntVariable scoreVariable;
    [SerializeField] private IntVariable highScoreVariable;

    public VoidEvent CountDownEndEvent { get => countDownEndEvent; }
    public VoidEvent RestartLevelEvent { get => restartLevelEvent; }
    public Collider2DEvent CollisionBallEvent { get => collisionBallEvent; }
    public IntVariable ScoreVariable { get => scoreVariable; }
    public IntVariable HighScoreVariable { get => highScoreVariable; }
}
