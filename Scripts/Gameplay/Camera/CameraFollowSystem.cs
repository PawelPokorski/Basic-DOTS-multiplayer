using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class CameraFollowSystem : SystemBase
{
    private float3 _cameraOffset = new float3(0f, 3f, -3f);
    private float _smoothSpeed = 10f;

    Camera camera;

    protected override void OnCreate()
    {
        base.OnCreate();
        camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("Main camera not found. Please ensure there is a camera tagged as 'MainCamera'.");
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected override void OnUpdate()
    {
        foreach (var (transform, input, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerInput>>()
            .WithAll<GhostOwnerIsLocal>().WithEntityAccess())
        {
            var transformRW = transform.ValueRW;

            float deltaTime = SystemAPI.Time.DeltaTime;
            float mouseX = input.ValueRO.MouseDeltaX;

            var yawRotation = quaternion.EulerXYZ(0f, math.radians(mouseX * 2f), 0f);
            transformRW.Rotation = math.mul(transformRW.Rotation, yawRotation);

            // Smoothly follow the player position with an offset
            float3 offset = math.mul(transformRW.Rotation, _cameraOffset);
            float3 offsetPosition = transformRW.Position + offset;

            camera.transform.position = offsetPosition;

            // Converting quaternion from math to UnityEngine
            Quaternion unityQuat = new Quaternion(transformRW.Rotation.value.x, transformRW.Rotation.value.y, transformRW.Rotation.value.z, transformRW.Rotation.value.w);
            float yAngle = unityQuat.eulerAngles.y;

            // Setting the camera rotation only on the Y-axis
            camera.transform.rotation = Quaternion.Euler(0f, yAngle, 0f);
        }
    }
}