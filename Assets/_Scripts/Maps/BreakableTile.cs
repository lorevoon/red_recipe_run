using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

public class BreakableTile : MonoBehaviour
{
    private Tilemap _tilemap;
    private MapManager _mapManager;
    private PlayerController _playerController;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;
    private Vector3Int _position;
    private float _maxDurability;
    private float _currentDurability;

    [SerializeField] private GameObject _breakVFX;
    [SerializeField] private AudioClip[] _crackingSounds;
    [SerializeField] private AudioClip[] _destroyingSounds;
    
    private void Start()
    {
        _tilemap = GameObject.FindGameObjectWithTag("Bush").GetComponent<Tilemap>();
        _mapManager = MapManager.Instance;
        _playerController = PlayerController.Instance;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        
        var position = transform.position;
        _position = new Vector3Int((int)position.x, (int)position.y, 0);
        
        _maxDurability = _mapManager.BushDurabilityGrid[_position.x, _position.y];
        _currentDurability = _maxDurability;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!_playerController.IsInLastInputDirection(
                new Vector3(_position.x+0.5f, _position.y+0.5f, 0))) return;
        
        _currentDurability -= _playerController.ToolSpeed;
        if (_currentDurability > 0f)
        {
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f-_currentDurability/_maxDurability);
            PlayAudio(_crackingSounds[Random.Range(0, _crackingSounds.Length)]);
            Instantiate(_breakVFX, new Vector3(_position.x+0.5f, _position.y+0.5f), Quaternion.identity);
            _playerController.ResetBounce();
        }
        else // durability < 0
        {
            SpawnIngredient();
            PlayAudio(_destroyingSounds[Random.Range(0, _destroyingSounds.Length)], 0.3f);
            _mapManager.BushTypeGrid[_position.x, _position.y] = EGrid.Empty;
            _tilemap.SetTile(new Vector3Int(_position.x, _position.y, 0), null);
            Destroy(this);
        }
    }

    private void SpawnIngredient()
    {
        if (_mapManager.BushTypeGrid[_position.x, _position.y] != EGrid.Item) return;
        
        // 0.2 chance of spawning random, 0.8 chance of spawning in recipe
        EIngredient ingredient;
        // if (Random.Range(0f, 1f) < 0.2f)
        // {
        //     Debug.Log("dropping a random ingredient");
        //     Array values = Enum.GetValues(typeof(EIngredient));
        //     ingredient = (EIngredient)values.GetValue(Random.Range(0, values.Length));
        // }
        // else
        // {
        //     Debug.Log("dropping an ingredient from the recipe");
        //     ingredient = RecipeManager.Instance.GetRandomIngredientInRecipe();
        // }
        ingredient = RecipeManager.Instance.GetRandomIngredientInRecipe();
        IngredientManager.Instance.SpawnIngredients(_position, ingredient);
    }
    
    private void PlayAudio(AudioClip audioClip, float pitchRange = 0f, float volume = 0.5f)
    {
        _audioSource.pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
        _audioSource.volume = volume;
        _audioSource.PlayOneShot(audioClip);
    }
}
