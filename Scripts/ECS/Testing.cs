using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;


public class Testing : MonoBehaviour
{
    [SerializeField]
    private int number;
    public Mesh mesh;
    public Material mat;
    // Start is called before the first frame update
    private EntityManager entityManager;
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        AddEntity();
    }
    private void AddEntity()
    {
        EntityArchetype entityArchetype = entityManager.CreateArchetype(typeof(Translation),
            typeof(RenderMesh),
            typeof(RenderBounds), 
            typeof(LocalToWorld),
            typeof(Scale), 
            typeof(Rotation), 
            typeof(PerInstanceCullingTag)
            );
        NativeArray<Entity> entities = entityManager.CreateEntity(entityArchetype, number, Allocator.Temp);
        for (int i = 0; i < entities.Length; i++)
        {
            var position = new float3(100*UnityEngine.Random.Range(0f, entities.Length) / entities.Length, 4, UnityEngine.Random.Range(0f, entities.Length) / entities.Length);
            entityManager.SetComponentData<Translation>(entities[i], new Translation { Value = position });
            entityManager.SetSharedComponentData<RenderMesh>(entities[i], new RenderMesh { material = mat, mesh = mesh });
            entityManager.SetComponentData<Scale>(entities[i], new Scale { Value = 1f});
            entityManager.SetComponentData<RenderBounds>(entities[i], new RenderBounds { Value= mesh.bounds.ToAABB() });
        }
        entities.Dispose();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
