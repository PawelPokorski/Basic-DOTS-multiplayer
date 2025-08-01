using Unity.Entities;
using UnityEngine;

public class PlayerSpawnerAuthoring : MonoBehaviour
{
    public GameObject EvenPlayerPrefab;
    public GameObject OddPlayerPrefab;

    class Baker : Baker<PlayerSpawnerAuthoring>
    {
        public override void Bake(PlayerSpawnerAuthoring authoring)
        {
            PlayerSpawner spawner = default(PlayerSpawner);
            spawner.EvenPlayerPrefab = GetEntity(authoring.EvenPlayerPrefab, TransformUsageFlags.Dynamic);
            spawner.OddPlayerPrefab = GetEntity(authoring.OddPlayerPrefab, TransformUsageFlags.Dynamic);
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, spawner);
        }
    }
}

public struct PlayerSpawner : IComponentData
{
    public Entity EvenPlayerPrefab;
    public Entity OddPlayerPrefab;
}

