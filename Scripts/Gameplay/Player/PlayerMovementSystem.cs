using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        float moveSpeed = 3f;
        float rotationSpeed = 100f;
        float mouseSensitivity = 0.1f;

        foreach (var (input, transform) in SystemAPI.Query<RefRO<PlayerInput>, RefRW<LocalTransform>>()
            .WithAll<Simulate>())
        {
            float yaw = input.ValueRO.MouseDeltaX * mouseSensitivity;
            float pitch = input.ValueRO.MouseDeltaY * mouseSensitivity;

            var yawRotation = quaternion.Euler(0f, yaw * rotationSpeed * deltaTime, 0f);
            transform.ValueRW.Rotation = math.mul(transform.ValueRW.Rotation, yawRotation);

            var forward = math.forward(transform.ValueRO.Rotation);
            var right = math.cross(new float3(0f, 1f, 0f), forward);

            var moveInput = new float2(input.ValueRO.Horizontal, input.ValueRO.Vertical);
            moveInput = math.normalizesafe(moveInput);

            var moveDirection = forward * moveInput.y + right * moveInput.x;

            var currentSpeed = input.ValueRO.RunEvent.IsSet ? moveSpeed * 2f : moveSpeed;

            transform.ValueRW.Position += deltaTime * currentSpeed * moveDirection;
        }
    }
}