using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public partial class MapGenerator : MonoBehaviour
{
    private MapManager _mapManager;

    [SerializeField] private GameObject _grandmasHouse;
    [SerializeField] private RuleTile _bushRuleTile;
    [SerializeField] private RuleTile _wallRuleTile;
    [SerializeField] private RuleTile _groundRuleTile;
    [SerializeField] private RuleTile _itemRuleTile;
    
    private Tilemap _bushTilemap;
    private Tilemap _groundTilemap;
    // private Tilemap _wallTilemap;
    
    // random walker algorithm
    private List<WalkerObject> _walkers;
    private int _mapWidth = 100;
    private int _mapHeight = 100;
    private int _maxWalkers = 10;
    private int _tileCount;
    private float _bushFillPercentage = 0.3f;
    private float _chanceToChange = 0.5f;

    private int _minYCoord = int.MaxValue;
    private int _maxYCoord = -1;
    private List<Vector3Int> _topWalls = new List<Vector3Int>();
    
    private void Start()
    {
        _mapManager = MapManager.Instance;
        _bushTilemap = GameObject.FindGameObjectWithTag("Bush").GetComponent<Tilemap>();
        _groundTilemap = GameObject.FindGameObjectWithTag("Ground").GetComponent<Tilemap>();
        // _wallTilemap = GameObject.FindGameObjectWithTag("Wall").GetComponent<Tilemap>();
        GenerateMap();
    }
    
    private void GenerateMap()
    {
        InitializeGrid();
        
        GenerateBushes();
        GenerateWalls();
        
        SetHardnessLevels();
        
        GenerateGaps();
        GenerateVeins();
        
        GeneratePlayerSpawn();
        GenerateGrandmasHouse();

        CreateNodes();
    }

    private void InitializeGrid()
    {
        _mapManager.BushTypeGrid = new EGrid[_mapWidth, _mapHeight];
        _mapManager.BushDurabilityGrid = new float[_mapWidth, _mapHeight];

        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                _mapManager.BushTypeGrid[x, y] = EGrid.Unreachable;
                _mapManager.BushDurabilityGrid[x, y] = -1f;
                _bushTilemap.SetTile(new Vector3Int(x, y, 0), _wallRuleTile);
                
                if (Random.Range(0f, 1f) <= 0.1f)
                    _groundTilemap.SetTile(new Vector3Int(x, y, 0), _groundRuleTile);
            }
        }

        _walkers = new List<WalkerObject>();

        Vector3Int tileCenter = new Vector3Int(_mapWidth / 2, 
            _mapHeight / 2, 0);

        WalkerObject currentWalker = 
            new WalkerObject(new Vector2(tileCenter.x, tileCenter.y), GetDirection(), _chanceToChange);
        _mapManager.BushTypeGrid[tileCenter.x, tileCenter.y] = EGrid.Bush;
        _bushTilemap.SetTile(tileCenter, _bushRuleTile);
        _walkers.Add(currentWalker);

        _tileCount++;
    }

    private void GenerateBushes()
    {
        while ((float)_tileCount / (float)_mapManager.BushTypeGrid.Length < _bushFillPercentage)
        {
            foreach (WalkerObject currentWalker in _walkers)
            {
                Vector3Int currentPos = new Vector3Int((int)currentWalker.Position.x, (int)currentWalker.Position.y, 0);
                
                for (int x = -1; x <= 1; x++)
                {
                    if (currentPos.x + x >= _mapWidth-1 || currentPos.x + x < 1) continue;
                    
                    for (int y = -1; y <= 1; y++)
                    {
                        if (currentPos.y + y >= _mapHeight-2 || currentPos.y + y < 1) continue;
                        if (_mapManager.BushTypeGrid[currentPos.x + x, currentPos.y + y] == EGrid.Bush) continue;
                        
                        _bushTilemap.SetTile(new Vector3Int(currentPos.x + x, currentPos.y + y), _bushRuleTile);
                        _tileCount++;
                        _mapManager.BushTypeGrid[currentPos.x + x, currentPos.y + y] = EGrid.Bush;

                        if (currentPos.y+y < _minYCoord) _minYCoord = currentPos.y;
                        if (currentPos.y+y > _maxYCoord) _maxYCoord = currentPos.y;
                    }
                }
            }
            
            // update Walker
            RemoveWalkerByChance();
            RedirectWalkerByChance();
            CreateWalkerByChance();
            UpdateWalkerPosition();
        }
    }

    private void GenerateWalls()
    {
        for (int x = 0; x < _mapWidth - 1; x++)
        {
            for (int y = 0; y < _mapHeight - 1; y++)
            {
                if (_mapManager.BushTypeGrid[x, y] != EGrid.Bush) continue;
                
                if (_mapManager.BushTypeGrid[x + 1, y] == EGrid.Unreachable)
                {
                    _bushTilemap.SetTile(new Vector3Int(x+1, y, 0), _wallRuleTile);
                    _mapManager.BushTypeGrid[x + 1, y] = EGrid.Wall;
                }

                if (_mapManager.BushTypeGrid[x - 1, y] == EGrid.Unreachable)
                {
                    _bushTilemap.SetTile(new Vector3Int(x-1, y, 0), _wallRuleTile);
                    _mapManager.BushTypeGrid[x - 1, y] = EGrid.Wall;
                }
                    
                if (_mapManager.BushTypeGrid[x, y+1] == EGrid.Unreachable)
                {
                    _bushTilemap.SetTile(new Vector3Int(x, y+1, 0), _wallRuleTile);
                    _mapManager.BushTypeGrid[x, y+1] = EGrid.Wall;
                    
                    _topWalls.Add(new Vector3Int(x, y+1, 0));
                }
                    
                if (_mapManager.BushTypeGrid[x, y-1] == EGrid.Unreachable)
                {
                    _bushTilemap.SetTile(new Vector3Int(x, y-1, 0), _wallRuleTile);
                    _mapManager.BushTypeGrid[x, y-1] = EGrid.Wall;
                }
            }
        }
    }

    private void SetHardnessLevels()
    {
        // set grid durability
        // set tile map color

        for (int x = 0; x < _mapWidth - 1; x++)
        {
            for (int y = _mapHeight - 2; y >= 0; y--)
            {
                if (_mapManager.BushTypeGrid[x, y] != EGrid.Bush) continue;

                float cumulative = 0;
                foreach (EBushLevel level in Enum.GetValues(typeof(EBushLevel)))
                {
                    float percentage = _mapManager.GetBushLevelData(level).Percentage;
                    if ((float)(_maxYCoord - y) / (_maxYCoord - _minYCoord) - cumulative > percentage)
                    {
                        cumulative += percentage;
                        if (level != EBushLevel.Level4) continue;
                    }
                    
                    _mapManager.BushDurabilityGrid[x, y] = _mapManager.GetBushLevelData(level).MaxDurability;
                    
                    if (ColorUtility.TryParseHtmlString(_mapManager.GetBushLevelData(level).ColorHex, out Color color))
                        _bushTilemap.SetColor(new Vector3Int(x, y), color);

                    break;
                }
            }
        }
    }

    private void GenerateGaps()
    {
        float gapPercentage = 0.075f; // TEMP
        int sizeOfGap = 5;
        
        int totalTiles = (int)(_mapWidth * _mapHeight * _bushFillPercentage);
        int totalGaps = Mathf.RoundToInt(totalTiles * gapPercentage);
        
        while (totalGaps > 0)
        {
            int startX = Random.Range(1, _mapWidth-1);
            int startY = Random.Range(1, _mapHeight-1);
            if (_mapManager.BushTypeGrid[startX, startY] != EGrid.Bush) continue;

            ConvertRandomSection(startX, startY, sizeOfGap, EGrid.Empty, null);
            totalGaps -= sizeOfGap;
        }
    }
    
    private void GenerateVeins()
    {
        float veinPercentage = 0.075f; // TEMP
        int sizeOfVein = 5;
        
        int totalTiles = (int)(_mapWidth * _mapHeight * _bushFillPercentage);
        int totalGaps = Mathf.RoundToInt(totalTiles * veinPercentage);
        
        while (totalGaps > 0)
        {
            int startX = Random.Range(1, _mapWidth-1);
            int startY = Random.Range(1, _mapHeight-1);
            if (_mapManager.BushTypeGrid[startX, startY] != EGrid.Bush) continue;

            ConvertRandomSection(startX, startY, sizeOfVein, EGrid.Item, _itemRuleTile);
            totalGaps -= sizeOfVein;
        }
    }

    private void ConvertRandomSection(int startX, int startY, int size, EGrid type, RuleTile tile)
    {
        int currentSize = 1;
        _walkers = new List<WalkerObject>();
        for (int i = 0; i < 2; i++)
        {
            WalkerObject walker = new WalkerObject(new Vector2(startX, startY), GetDirection(), _chanceToChange);
            _walkers.Add(walker);
        }
        _mapManager.BushTypeGrid[startX, startY] = type;
        _bushTilemap.SetTile(new Vector3Int(startX, startY), tile);
        currentSize++;
        
        while (currentSize <= size)
        {
            foreach (WalkerObject walker in _walkers)
            {
                Vector3Int currentPos = new Vector3Int((int)walker.Position.x, (int)walker.Position.y, 0);
                if (_mapManager.BushTypeGrid[currentPos.x, currentPos.y] == EGrid.Bush)
                {
                    _mapManager.BushTypeGrid[currentPos.x, currentPos.y] = type;
                    _bushTilemap.SetTile(new Vector3Int(currentPos.x, currentPos.y), tile);
                    currentSize++;
                }
                
                if (currentSize > size) break;
            }

            _chanceToChange = 0.8f;
            RedirectWalkerByChance();
            UpdateWalkerPosition();
        }
    }

    private void GeneratePlayerSpawn()
    {
        List<Vector3Int> startableWalls = new List<Vector3Int>();
        
        foreach (Vector3Int wall in _topWalls)
        {
            if ((float)(_maxYCoord - wall.y) / (_maxYCoord - _minYCoord)
                < _mapManager.GetBushLevelData(EBushLevel.Level1).Percentage)
                startableWalls.Add(wall);
        }

        Vector3Int startPos = startableWalls[Random.Range(0, startableWalls.Count)];
        _mapManager.BushTypeGrid[startPos.x, startPos.y] = EGrid.Unreachable;
        _bushTilemap.SetTile(startPos, null);
        
        _mapManager.PlayerSpawnPoint = new Vector3(startPos.x + 0.5f, startPos.y + 0.5f);
        _mapManager.SpawnPlayer();
    }

    private void GenerateGrandmasHouse()
    {
        int houseWidth = 7;
        int houseHeight = 7;
        Vector3Int center = new Vector3Int(Mathf.FloorToInt(_mapManager.PlayerSpawnPoint.x), 
            Mathf.FloorToInt(_mapManager.PlayerSpawnPoint.y), 0);

        
        int startX = center.x - houseWidth / 2;
        int startY = center.y + 1;
        int endX = startX + houseWidth;
        int endY = startY + houseHeight;

        int gridWidth = _mapManager.BushTypeGrid.GetLength(0);
        int gridHeight = _mapManager.BushTypeGrid.GetLength(1);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
                {
                    _mapManager.BushTypeGrid[x, y] = EGrid.Unreachable;
                    _bushTilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
        
        Vector3 housePos = new Vector3(startX + houseWidth / 2f - 0.5f, startY + houseHeight / 2f, 0f);
        Instantiate(_grandmasHouse, housePos, Quaternion.identity);
    }
    
    # region random walker algorithm
    private Vector2 GetDirection()
    {
        int choice = Mathf.FloorToInt(Random.value * 3.99f);

        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }
    
    private void RemoveWalkerByChance()
    {
        int updatedCount = _walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (!(Random.value < _walkers[i].ChanceToChange) || _walkers.Count <= 1) continue;
            _walkers.RemoveAt(i);
            break;
        }
    }

    private void RedirectWalkerByChance()
    {
        for (int i = 0; i < _walkers.Count; i++)
        {
            if (!(Random.value < _walkers[i].ChanceToChange)) continue;
            WalkerObject currentWalker = _walkers[i];
            currentWalker.Direction = GetDirection();
            _walkers[i] = currentWalker;
        }
    }

    private void CreateWalkerByChance()
    {
        int updatedCount = _walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (!(Random.value < _walkers[i].ChanceToChange) || _walkers.Count >= _maxWalkers) continue;
            Vector2 newDirection = GetDirection();
            Vector2 newPosition = _walkers[i].Position;

            WalkerObject newWalker = new WalkerObject(newPosition, newDirection, _chanceToChange);
            _walkers.Add(newWalker);
        }
    }

    private void UpdateWalkerPosition()
    {
        for (int i = 0; i < _walkers.Count; i++)
        {
            WalkerObject foundWalker = _walkers[i];
            foundWalker.Position += foundWalker.Direction;
            foundWalker.Position.x = Mathf.Clamp(foundWalker.Position.x, 1, _mapWidth - 2);
            foundWalker.Position.y = Mathf.Clamp(foundWalker.Position.y, 1, _mapHeight - 2);
            _walkers[i] = foundWalker;
        }
    }
    
    # endregion
}
