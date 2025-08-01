using Unity.Entities;
using Unity.NetCode;

public partial class NetworkManager : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<NetworkId>();
        RequireForUpdate<NetworkStreamInGame>();
    }

    protected override void OnUpdate() { }
}