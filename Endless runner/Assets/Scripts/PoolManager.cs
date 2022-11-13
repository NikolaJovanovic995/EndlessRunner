using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] List<Tile> tiles;
    [SerializeField] List<Obstacle> obstacles;
    [SerializeField] Transform poolParent;

    private Dictionary<TileType, Tile> tilePrefabs;
    private Dictionary<ObstacleType, Obstacle> obstaclePrefabs;
    private List<Tile> tilesPool; 
    private List<Obstacle> obstaclesPool;


    private void Awake()
    {
        tilesPool = new List<Tile>();
        obstaclesPool = new List<Obstacle>();
        tilePrefabs = new Dictionary<TileType, Tile>();
        obstaclePrefabs = new Dictionary<ObstacleType, Obstacle>();

        foreach (Tile tile in tiles)
        {
            tilePrefabs.Add(tile.Type, tile);
        }
        foreach (Obstacle obstacle in obstacles)
        {
            obstaclePrefabs.Add(obstacle.Type, obstacle);
        }
    }

    public Tile GetTile(TileType type)
    {
        for (int i = 0; i < tilesPool.Count; i++)
        {
            if (tilesPool[i].Type == type)
            {
                Tile tile = tilesPool[i];   
                tilesPool.RemoveAt(i);
                return tile;
            }
        }

        return Instantiate(tilePrefabs[type]);
    }

    public void ReturnTile(Tile tile)
    {
        tile.gameObject.SetActive(false);
        tile.Turned = false;
        tile.transform.parent = poolParent;
        tilesPool.Add(tile);
    }

    public Obstacle GetObstacle(ObstacleType type)
    {
        for (int i = 0; i < obstaclesPool.Count; i++)
        {
            if (obstaclesPool[i].Type == type)
            {
                Obstacle obstacle = obstaclesPool[i];
                obstaclesPool.RemoveAt(i);
                return obstacle;
            }
        }

        return Instantiate(obstaclePrefabs[type]);
    }

    public void ReturnObstacle(Obstacle obstacle)
    {
        obstacle.gameObject.SetActive(false);
        obstacle.transform.parent = poolParent;
        obstaclesPool.Add(obstacle);
    }

}
