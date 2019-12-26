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

    // It's anoing to create events for variables.
    // Dont forget to change execution order to prior
    private void Awake()
    {
        scoreVariable.Changed = scoreVariable.Changed ?? ScriptableObject.CreateInstance<IntEvent>();
        HighScoreVariable.Changed = HighScoreVariable.Changed ?? ScriptableObject.CreateInstance<IntEvent>();
    }

    private void OnDestroy() 
    {
        GameObject.Destroy(scoreVariable.Changed);
        GameObject.Destroy(HighScoreVariable.Changed);
    }
}