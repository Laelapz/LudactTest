using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SaveSo", menuName = "DataSaving")]
public class SaveSO : ScriptableObject
{
    public float TimeBetweenSpawn;
    public float FPSUpdateInterval;
    public bool IsPooling;
}
