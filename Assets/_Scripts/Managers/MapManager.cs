using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    private MapGenerator _mapGenerator;
    private GameObject _player;
    private Dictionary<EBushLevel, SBushLevelData> _bushLevels;
    
    public EGrid[,] BushTypeGrid;
    public float[,] BushDurabilityGrid;
    public Vector3 PlayerSpawnPoint;

    protected override void Awake()
    {
        base.Awake();
        _mapGenerator = GetComponentInChildren<MapGenerator>();
        _player = GameObject.Find("Player");
        _bushLevels = new Dictionary<EBushLevel, SBushLevelData>();
        Init(Path.Combine(Application.streamingAssetsPath, "Tables/BushLevelTable.csv"));
    }
    
    private void Init(string dataPath)
    {
        Debug.Assert(dataPath != null && File.Exists(dataPath));

        using (var reader = new StreamReader(dataPath))
        using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                SBushLevelData data = new SBushLevelData
                {
                    Level = (EBushLevel)Enum.Parse(typeof(EBushLevel), csv.GetField("level")),
                    MaxDurability = float.Parse(csv.GetField("maxDurability")),
                    Percentage = float.Parse(csv.GetField("percentage")),
                    GapPercentage = float.Parse(csv.GetField("gapPercentage")),
                    ColorHex = csv.GetField("colorHex")
                };

                _bushLevels.Add(data.Level, data);
            }
        }
    }

    public SBushLevelData GetBushLevelData(EBushLevel bushLevel)
    {
        return _bushLevels[bushLevel];
    }

    public void SpawnPlayer()
    {
        _player.transform.position = PlayerSpawnPoint;
    }

}
