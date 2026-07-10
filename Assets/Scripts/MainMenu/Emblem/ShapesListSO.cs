using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ShapesListSO", menuName = "Game/Shapes List")]
public class ShapesListSO : ScriptableObject
{
    public List<Shape> shapesList = new List<Shape>();
}
