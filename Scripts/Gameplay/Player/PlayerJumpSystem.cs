using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedFixedStepSimulationSystemGroup))]
public partial struct PlayerJumpSystem : ISystem
{
    private const float _jumpForce = 5f;

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (input, physics, mass, transform) in
            SystemAPI.Query<RefRO<PlayerInput>, RefRW<PhysicsVelocity>, RefRW<PhysicsMass>, RefRW<LocalTransform>>()
            .WithAll<Simulate>())
        {
            var playerInput = input.ValueRO;

            if(playerInput.JumpEvent.IsSet && transform.ValueRO.Position.y <= 0.01f)
            {
                Debug.Log("Jump");
                physics.ValueRW.Linear += new float3(0f, _jumpForce * mass.ValueRW.InverseMass, 0f);
            }
        }
    }

}