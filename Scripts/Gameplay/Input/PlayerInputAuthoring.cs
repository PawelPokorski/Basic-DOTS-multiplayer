using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerInputAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerInputAuthoring>
    {
        public override void Bake(PlayerInputAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerInput>(entity);
        }
    }
}

public struct PlayerInput : IInputComponentData
{
    public float Horizontal;
    public float Vertical;
    public float MouseDeltaX;
    public float MouseDeltaY;
    public InputEvent JumpEvent;
    public InputEvent RunEvent;
}