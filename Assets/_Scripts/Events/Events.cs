using UnityEngine.Events;

public static class PlayerEvents
{
    public static UnityAction<int> HpChanged;
    public static UnityAction Killed;
    public static UnityAction Spawned;
    
    // etc etc
}

public static class GameEvents
{
    public static UnityAction MainMenuLoaded;
    // etc etc
}