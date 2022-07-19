using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaBuilder
{
    public static List<Vector2> GetParabolaPoints(Vector2 p1, Vector2 p2, Vector2 p3, int precision = 10)
    {
        //y = a * x^2 + b * x + c
        var matrix = Matrix<float>.Build.Dense(3,3);
        matrix[0, 0] = Mathf.Pow(p1.x, 2);
        matrix[0, 1] = p1.x;
        matrix[0, 2] = 1f;

        matrix[1, 0] = Mathf.Pow(p2.x, 2);
        matrix[1, 1] = p2.x;
        matrix[1, 2] = 1f;

        matrix[2, 0] = Mathf.Pow(p3.x, 2);
        matrix[2, 1] = p3.x;
        matrix[2, 2] = 1f;
        var secondMatrix = Matrix<float>.Build.Dense(3, 1);
        secondMatrix[0, 0] = p1.y;
        secondMatrix[1, 0] = p2.y;
        secondMatrix[2, 0] = p3.y;

        var result = matrix.Inverse().Multiply(secondMatrix);
        //calculate step
        float step = Mathf.Abs(p1.x - p3.x) / (float)precision;
        List<Vector2> points = new List<Vector2>();
        for (int i = 1; i <= precision; i++)
        {
            float x = p1.x + i * step;
            float y = x * x * result[0, 0] + x * result[1, 0] + result[2, 0];
            points.Add(new Vector2(x, y));
        }
        return points;
    }
}
