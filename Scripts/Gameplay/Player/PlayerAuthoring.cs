using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<NetworkStreamConnection>(entity);
            AddComponent(entity, new PlayerHealth()
            {
                Value = 100
            });

            AddComponent<PlayerTagComponent>(entity);
            AddComponent<Player>(entity);
        }
    }
}

public struct Player : IComponentData { }

public struct PlayerTagComponent : IComponentData { }

public struct PlayerHealth : IComponentData
{
    public int Value;
}