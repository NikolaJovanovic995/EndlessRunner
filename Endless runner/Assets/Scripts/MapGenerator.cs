using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] PoolManager poolManager;
    [SerializeField] Transform groundParent;

    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward;
    private Tile previousTile;
    private List<Tile> activeTiles;
    private List<Obstacle> activeObstacles;
    private List<TileType> turnTileTypes;
    private List<ObstacleType> obstacleTypes;

    private float spawnObstacleChance = 0.4f;
    private readonly int tileStartCount = 5;
    private readonly int minStraightTiles = 3;
    private readonly int maxStraightTiles = 10;

    private void Awake()
    {
        activeTiles = new List<Tile>();
        activeObstacles = new List<Obstacle>();
        turnTileTypes = new List<TileType>() { TileType.LEFT, TileType.RIGHT, TileType.SIDEWAYS };
        obstacleTypes = Enum.GetValues(typeof(ObstacleType)).Cast<ObstacleType>().ToList();
    }

    public void StartGeneratingMap()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        SpawnNumberOfStraightTiles(tileStartCount);
        SpawnTile(SelectRandomTurnType());
    }

    public void ResetMap()
    {
        currentTileLocation = Vector3.zero;
        currentTileDirection = Vector3.forward;
        previousTile = null;
        RemovePreviousElements(true);
    }

    public void AddNewDirection(Vector3 direction)
    {
        currentTileDirection = direction;
        RemovePreviousElements();
        currentTileLocation += CalculateTileOffset();
        SpawnNumberOfStraightTiles(Random.Range(minStraightTiles, maxStraightTiles), true);
        SpawnTile(SelectRandomTurnType());
    }

    private void RemovePreviousElements(bool removeAll = false)
    {
        int count = removeAll ? 0 : 1;

        while (activeTiles.Count != count)
        {
            Tile tile = activeTiles[0];
            activeTiles.RemoveAt(0);
            poolManager.ReturnTile(tile);
        }

        while (activeObstacles.Count != 0)
        {
            Obstacle obstacle = activeObstacles[0];
            activeObstacles.RemoveAt(0);
            poolManager.ReturnObstacle(obstacle);   
        }
    }

    private void SpawnNumberOfStraightTiles(int number, bool withObstacles = false)
    {
        for (int i = 0; i < number; i++)
        {
            SpawnTile(TileType.STRAIGHT, withObstacles && i != 0);
        }
    }

    private void SpawnTile(TileType type, bool spawnObstacle = false)
    {
        SetUpNewTile(type);

        if (spawnObstacle && Random.value > spawnObstacleChance)
            SpawnObstacle();

        if (type == TileType.STRAIGHT)
        {
            currentTileLocation += CalculateTileOffset();
        }
    }

    private void SetUpNewTile(TileType type)
    {
        Tile newTile = poolManager.GetTile(type);

        Quaternion newTileRotation = Quaternion.Euler(newTile.InitialRotation) * Quaternion.LookRotation(currentTileDirection, Vector3.up);

        newTile.transform.parent = groundParent;
        newTile.transform.SetPositionAndRotation(currentTileLocation, newTileRotation);
        newTile.gameObject.SetActive(true);
        previousTile = newTile;

        activeTiles.Add(previousTile);
    }

    private Vector3 CalculateTileOffset()
    {
        return previousTile.Offset * currentTileDirection;
    }

    private TileType SelectRandomTurnType()
    {
        if (turnTileTypes.Count == 0)
            return TileType.SIDEWAYS;

        return turnTileTypes[Random.Range(0, turnTileTypes.Count)];
    }

    private ObstacleType SelectRandomObstacleType()
    {
        if (obstacleTypes.Count == 0)
            return ObstacleType.JUMPING;

        return obstacleTypes[Random.Range(0, obstacleTypes.Count)];
    }

    private void SpawnObstacle()
    {
        Obstacle obstacle = poolManager.GetObstacle(SelectRandomObstacleType());

        Quaternion newObstRotation = Quaternion.identity * Quaternion.LookRotation(currentTileDirection, Vector3.up);
        obstacle.transform.SetParent(groundParent);
        obstacle.transform.SetPositionAndRotation(currentTileLocation, newObstRotation);
        obstacle.gameObject.SetActive(true);

        activeObstacles.Add(obstacle);
    }
}
