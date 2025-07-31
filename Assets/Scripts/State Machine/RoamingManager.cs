using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace WaveSystem.StateMachine
{
    [BurstCompile]
    struct RoamingDestinationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> currentPositions;
        public NativeArray<Vector3> newDestinations;
        public NativeArray<uint> randomSeeds;

        public void Execute(int index)
        {
            var rand = new Unity.Mathematics.Random(randomSeeds[index]);
            float angle = rand.NextFloat(0f, math.PI * 2f);
            float distance = rand.NextFloat(5f, 15f);
            float2 direction = math.float2(math.cos(angle), math.sin(angle));
            float3 target = (float3)currentPositions[index] + new float3(direction.x * distance, 0f, direction.y * distance);
            newDestinations[index] = target;
        }
    }

    //Centralized manager for roaming states
    public class RoamingManager : MonoBehaviour
    {
        public static RoamingManager Instance { get; private set; }

        private readonly List<RoamingState> roamingStates = new();
        private readonly List<RoamingState> pendingDestination = new();

        void Awake()
        {
            Instance = this;
        }

        public void Register(RoamingState state)
        {
            // Register the state if not already registered

            if (!roamingStates.Contains(state))
                roamingStates.Add(state);
        }

        public void Unregister(RoamingState state)
        {
            // Unregister the state if it exists

            roamingStates.Remove(state);
            pendingDestination.Remove(state);
        }

        public void FlagForDestination(RoamingState state)
        {
            // Add the state to the pending destination list if not already present

            if (!pendingDestination.Contains(state))
                pendingDestination.Add(state);
        }

        void Update()
        {
            int count = pendingDestination.Count;
            if (count == 0) return;

            var positions = new NativeArray<Vector3>(count, Allocator.TempJob);
            var destinations = new NativeArray<Vector3>(count, Allocator.TempJob);
            var seeds = new NativeArray<uint>(count, Allocator.TempJob);

            // Prepare data for the job

            for (int i = 0; i < count; i++)
            {
                positions[i] = pendingDestination[i].GetPosition();
                seeds[i] = (uint)UnityEngine.Random.Range(1, int.MaxValue);
            }

            // Schedule the job to calculate new destinations

            var job = new RoamingDestinationJob
            {
                currentPositions = positions,
                newDestinations = destinations,
                randomSeeds = seeds
            };

            var handle = job.Schedule(count, 32);
            handle.Complete();

            // Set the new destinations for each pending state

            for (int i = 0; i < count; i++)
            {
                pendingDestination[i].SetDestination(destinations[i]);
            }

            positions.Dispose();
            destinations.Dispose();
            seeds.Dispose();

            pendingDestination.Clear();
        }
    }
}