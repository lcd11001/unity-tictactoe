using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct TestNetcodeEntitiesServerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach ((RefRO<SimpleRpc> rpc, RefRO<ReceiveRpcCommandRequest> request, Entity entity)
        in SystemAPI.Query<RefRO<SimpleRpc>, RefRO<ReceiveRpcCommandRequest>>().WithEntityAccess())
        {
            if (request.ValueRO.SourceConnection != null)
            {
                Debug.Log($"Received RPC {rpc.ValueRO.Value} :: {entity.ToString()} :: {request.ValueRO.SourceConnection.ToString()}");
                commandBuffer.DestroyEntity(entity);
            }
        }

        commandBuffer.Playback(state.EntityManager);
    }
}
