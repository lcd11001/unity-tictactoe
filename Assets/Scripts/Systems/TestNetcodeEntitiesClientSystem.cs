using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
partial struct TestNetcodeEntitiesClientSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T was pressed");
            Entity rpcEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(rpcEntity, new SimpleRpc
            {
                Value = 42
            });
            state.EntityManager.AddComponentData(rpcEntity, new SendRpcCommandRequest());
        }
    }


}
