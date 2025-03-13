using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBreaker : MonoBehaviour
{
    private Tilemap _tileMap;
    private MapManager _mapManager;
    private PlayerController _player;
    
    private void Awake()
    {
        _tileMap = GetComponent<Tilemap>();
        _mapManager = MapManager.Instance;
        _player = PlayerController.Instance;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        
        Vector3 hitPosition = Vector3.zero;
        foreach (ContactPoint2D hit in collision.contacts)
        {
            hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
            hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
            _tileMap.SetTile(_tileMap.WorldToCell(hitPosition), null);
            // TODO make it so that the tiles need to take x amount of damage before breaking
        }
    }
}
