using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct GoInGameServerSystem : ISystem
{
    private ComponentLookup<NetworkId> _clients;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawner>();
        var builder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GoInGameRpcRequest>()
            .WithAll<ReceiveRpcCommandRequest>();

        state.RequireForUpdate(state.GetEntityQuery(builder));

        _clients = state.GetComponentLookup<NetworkId>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var spawner = SystemAPI.GetSingleton<PlayerSpawner>();

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        _clients.Update(ref state);

        foreach (var (request, entity) in
            SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>()
            .WithAll<GoInGameRpcRequest>().WithEntityAccess())
        {
            commandBuffer.AddComponent<NetworkStreamInGame>(request.ValueRO.SourceConnection);

            // Retrieving the network ID of the player
            var networkId = _clients[request.ValueRO.SourceConnection];

            // Setting the players prefab based on the network ID
            var prefab = (networkId.Value % 2 == 0) ? spawner.EvenPlayerPrefab : spawner.OddPlayerPrefab;

            // Instantiate the player entity from the prefab
            var player = commandBuffer.Instantiate(prefab);

            // Setting up the player's initial transform
            var randomPosition = new LocalTransform
            {
                Position = new Unity.Mathematics.float3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f)),
                Rotation = Quaternion.identity,
                Scale = 1f
            };
            commandBuffer.SetComponent(player, randomPosition);
            // Setting the player as a ghost entity
            commandBuffer.SetComponent(player, new GhostOwner { NetworkId = networkId.Value });

            // Adding the player to the LinkedEntityGroup for network synchronization
            commandBuffer.AppendToBuffer(request.ValueRO.SourceConnection, new LinkedEntityGroup { Value = player });
            commandBuffer.DestroyEntity(entity);
        }

        commandBuffer.Playback(state.EntityManager);
    }
}