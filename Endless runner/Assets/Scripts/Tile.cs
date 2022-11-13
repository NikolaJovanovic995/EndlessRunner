using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] TileType type;
    [SerializeField] Transform pivot;
    [SerializeField] Vector3 initialRotation;
    [SerializeField] int offset;

    public TileType Type => type;
    public Transform Pivot => pivot;
    public Vector3 InitialRotation => initialRotation;
    public int Offset => offset;
    public bool Turned { set; get; }
}

public enum TileType
{
    STRAIGHT,
    LEFT,
    RIGHT,
    SIDEWAYS
}