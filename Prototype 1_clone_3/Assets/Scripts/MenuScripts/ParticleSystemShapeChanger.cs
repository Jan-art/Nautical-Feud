using UnityEngine;
using System.Collections;

public class ParticleSystemShapeChanger: MonoBehaviour
 {
     private ParticleSystem m_particleSystem;
 
     private void Start()
     {
         m_particleSystem = GetComponent<ParticleSystem>();
         ParticleSystem.ShapeModule _editableShape = m_particleSystem.shape;
         _editableShape.position = new Vector3(1f, 2f, 3f);
     }
 }