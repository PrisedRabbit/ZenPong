using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityAtoms.SceneMgmt;
using UnityEngine;

// just inject this and use 
public sealed class AtomsReferences : MonoBehaviour
{
    [SerializeField] private VoidEvent countDownEndEvent;
    [SerializeField] private VoidEvent restartLevelEvent;
    [SerializeField] private BoolEvent pauseGameEvent;
    [SerializeField] private Collider2DEvent collisionBallEvent;
    [SerializeField] private IntVariable scoreVariable;
    [SerializeField] private IntVariable scorePlayer2Variable;
    [SerializeField] private IntVariable highScoreVariable;
    [SerializeField] private StringVariable networkGameStatusVariable;
    [SerializeField] private StringVariable networkAddressVariable;
    [SerializeField] private ChangeScene quitToMainMenuAction;

    public VoidEvent CountDownEndEvent { get => countDownEndEvent; }
    public VoidEvent RestartLevelEvent { get => restartLevelEvent; }
    public BoolEvent PauseGameEvent { get => pauseGameEvent; }
    public Collider2DEvent CollisionBallEvent { get => collisionBallEvent; }
    public IntVariable ScoreVariable { get => scoreVariable; }
    public IntVariable ScorePlayer2Variable { get => scorePlayer2Variable; }
    public IntVariable HighScoreVariable { get => highScoreVariable; }
    public StringVariable NetworkAddressVariable { get => networkAddressVariable; }
    public StringVariable NetworkGameStatusVariable { get => networkGameStatusVariable; }
    public ChangeScene QuitToMainMenuAction { get => quitToMainMenuAction; }
}