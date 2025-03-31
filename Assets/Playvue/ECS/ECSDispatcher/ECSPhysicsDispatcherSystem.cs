using Unity.Entities;
using Unity.Physics;
using System.Collections.Generic;

namespace Playvue.ECS {

public partial struct ECSPhysicsDispatcherSystem : ISystem {
    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state){
        foreach (var manager in ECSManagerRegistry.GetPhysicsManagers()){
            manager.ECSPhysicsUpdate(ref state);
        }

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();

        ECSPhysicsJobRegistry.ScheduleAll(ref state, simulation, ecb);

    }
}

public static class ECSPhysicsJobRegistry{
    public delegate void SchedulePhysicsJobs(
        ref SystemState state,
        SimulationSingleton simulation,
        EntityCommandBuffer.ParallelWriter ecb
    );

    private static readonly List<SchedulePhysicsJobs> jobs = new();

    public static void Register(SchedulePhysicsJobs job){
        if (!jobs.Contains(job))
            jobs.Add(job);
    }

    public static void ScheduleAll(ref SystemState state, SimulationSingleton simulation, EntityCommandBuffer.ParallelWriter ecb){
        foreach (var job in jobs)
            job(ref state, simulation, ecb);
    }

    public static void Clear() => jobs.Clear();
}

}