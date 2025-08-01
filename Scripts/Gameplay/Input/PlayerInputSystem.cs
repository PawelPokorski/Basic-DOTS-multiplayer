using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct PlayerInputSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GhostOwnerIsLocal>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);

        foreach (var (input, transform) in SystemAPI.Query<RefRW<PlayerInput>, RefRO<LocalTransform>>().WithAll<GhostOwnerIsLocal>())
        {
            var currentInput = default(PlayerInput);

            currentInput.Horizontal = horizontal;
            currentInput.Vertical = vertical;

            currentInput.MouseDeltaX = mouseX;
            currentInput.MouseDeltaY = mouseY;

            if(jumpPressed)
                currentInput.JumpEvent.Set();

            if (runPressed)
                currentInput.RunEvent.Set();
            
            input.ValueRW = currentInput;
        }
    }
}