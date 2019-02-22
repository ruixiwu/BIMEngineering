namespace BIM.Lmv.Processers.Helper
{
    using System;

    internal static class NormalsCalc
    {
        public static void Calc(int vertexCount, int triangleCount, float[] vertex, int[] indices, float[] normals)
        {
            int num;
            if ((normals == null) || (normals.Length < (vertexCount * 3)))
            {
                throw new ArgumentException("normals");
            }
            for (num = 0; num < (vertexCount * 3); num++)
            {
                normals[num] = 0f;
            }
            float[] vector = new float[3];
            float[] numArray2 = new float[3];
            float[] numArray3 = new float[3];
            float[] result = new float[3];
            float[] numArray5 = new float[3];
            float[] numArray6 = new float[3];
            for (num = 0; num < (triangleCount * 3); num += 3)
            {
                int index = indices[num] * 3;
                int num3 = indices[num + 1] * 3;
                int num4 = indices[num + 2] * 3;
                VectorFromArray(vertex, index, vector);
                VectorFromArray(vertex, num3, numArray2);
                VectorFromArray(vertex, num4, numArray3);
                VectorSub(result, numArray2, vector);
                VectorSub(numArray5, numArray3, vector);
                VectorCross(numArray6, result, numArray5);
                VectorNormalize(numArray6);
                VectorAddToArray(normals, index, numArray6);
                VectorAddToArray(normals, num3, numArray6);
                VectorAddToArray(normals, num4, numArray6);
            }
            for (num = 0; num < (vertexCount * 3); num++)
            {
                VectorFromArray(normals, num, numArray6);
                VectorNormalize(numArray6);
                VectorToArray(normals, num, numArray6);
            }
        }

        private static void VectorAddToArray(float[] buffer, int index, float[] vector)
        {
            buffer[index] += vector[0];
            buffer[index + 1] += vector[1];
            buffer[index + 2] += vector[2];
        }

        private static void VectorCross(float[] result, float[] a, float[] b)
        {
            float num = a[0];
            float num2 = a[1];
            float num3 = a[2];
            float num4 = b[0];
            float num5 = b[1];
            float num6 = b[2];
            result[0] = (num2 * num6) - (num3 * num5);
            result[1] = (num3 * num4) - (num * num6);
            result[2] = (num * num5) - (num2 * num4);
        }

        private static void VectorDividScalar(float[] v, float scalar)
        {
            if (scalar == 0f)
            {
                v[0] = 0f;
                v[1] = 0f;
                v[2] = 0f;
            }
            else
            {
                double num = 1.0 / ((double) scalar);
                v[0] = (float) (v[0] * num);
                v[1] = (float) (v[1] * num);
                v[2] = (float) (v[2] * num);
            }
        }

        private static void VectorFromArray(float[] buffer, int index, float[] vector)
        {
            vector[0] = buffer[index];
            vector[1] = buffer[index + 1];
            vector[2] = buffer[index + 2];
        }

        private static float VectorLength(float[] v)
        {
            float num = v[0];
            float num2 = v[1];
            float num3 = v[2];
            return (float) Math.Sqrt((double) (((num * num) + (num2 * num2)) + (num3 * num3)));
        }

        private static void VectorNormalize(float[] v)
        {
            VectorDividScalar(v, VectorLength(v));
        }

        private static void VectorSub(float[] result, float[] a, float[] b)
        {
            result[0] = a[0] - b[0];
            result[1] = a[1] - b[1];
            result[2] = a[2] - b[2];
        }

        private static void VectorToArray(float[] buffer, int index, float[] vector)
        {
            buffer[index] = vector[0];
            buffer[index + 1] = vector[1];
            buffer[index + 2] = vector[2];
        }
    }
}

