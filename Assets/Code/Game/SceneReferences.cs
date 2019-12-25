using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SceneReferences : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint1, spawnPoint2;
    [SerializeField] private GameObject paddlePrefab;
    [SerializeField] private GameObject ballPrefab;

    public Transform SpawnPoint1 { get => spawnPoint1; }
    public Transform SpawnPoint2 { get => spawnPoint2; }
    public GameObject PaddlePrefab { get => paddlePrefab; }
    public GameObject BallPrefab { get => ballPrefab; }
}
