using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] ObstacleType type;

    public ObstacleType Type => type;
}

public enum ObstacleType
{
    JUMPING,
    SLIDING
}
