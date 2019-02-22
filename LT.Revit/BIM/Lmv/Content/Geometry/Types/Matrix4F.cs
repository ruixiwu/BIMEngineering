namespace BIM.Lmv.Content.Geometry.Types
{
    using System;
    using System.Diagnostics;

    public class Matrix4F
    {
        public float[] elements = new float[] { 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f };

        public Matrix4F clone() => 
            new Matrix4F().fromArray(this.elements);

        public Matrix4F compose(Vector3D position, Vector4F quaternion, Vector3F scale)
        {
            this.makeRotationFromQuaternion(quaternion);
            this.scale(scale);
            this.setPosition(position);
            return this;
        }

        public Matrix4F compose(Vector3F position, Vector4F quaternion, Vector3F scale)
        {
            this.makeRotationFromQuaternion(quaternion);
            this.scale(scale);
            this.setPosition(position);
            return this;
        }

        public Matrix4F copy(Matrix4F m)
        {
            m.elements.CopyTo(this.elements, 0);
            return this;
        }

        public float determinant()
        {
            float[] elements = this.elements;
            float num = elements[0];
            float num2 = elements[4];
            float num3 = elements[8];
            float num4 = elements[12];
            float num5 = elements[1];
            float num6 = elements[5];
            float num7 = elements[9];
            float num8 = elements[13];
            float num9 = elements[2];
            float num10 = elements[6];
            float num11 = elements[10];
            float num12 = elements[14];
            float num13 = elements[3];
            float num14 = elements[7];
            float num15 = elements[11];
            float num16 = elements[15];
            return ((((num13 * (((((((num4 * num7) * num10) - ((num3 * num8) * num10)) - ((num4 * num6) * num11)) + ((num2 * num8) * num11)) + ((num3 * num6) * num12)) - ((num2 * num7) * num12))) + (num14 * (((((((num * num7) * num12) - ((num * num8) * num11)) + ((num4 * num5) * num11)) - ((num3 * num5) * num12)) + ((num3 * num8) * num9)) - ((num4 * num7) * num9)))) + (num15 * (((((((num * num8) * num10) - ((num * num6) * num12)) - ((num4 * num5) * num10)) + ((num2 * num5) * num12)) + ((num4 * num6) * num9)) - ((num2 * num8) * num9)))) + (num16 * (((((((-num3 * num6) * num9) - ((num * num7) * num10)) + ((num * num6) * num11)) + ((num3 * num5) * num10)) - ((num2 * num5) * num11)) + ((num2 * num7) * num9))));
        }

        public float[] flattenToArrayOffset(float[] array, int offset)
        {
            float[] elements = this.elements;
            array[offset] = elements[0];
            array[offset + 1] = elements[1];
            array[offset + 2] = elements[2];
            array[offset + 3] = elements[3];
            array[offset + 4] = elements[4];
            array[offset + 5] = elements[5];
            array[offset + 6] = elements[6];
            array[offset + 7] = elements[7];
            array[offset + 8] = elements[8];
            array[offset + 9] = elements[9];
            array[offset + 10] = elements[10];
            array[offset + 11] = elements[11];
            array[offset + 12] = elements[12];
            array[offset + 13] = elements[13];
            array[offset + 14] = elements[14];
            array[offset + 15] = elements[15];
            return array;
        }

        public Matrix4F fromArray(float[] array)
        {
            array.CopyTo(this.elements, 0);
            return this;
        }

        public Matrix4F getInverse(Matrix4F m, bool throwOnInvertible)
        {
            float[] elements = this.elements;
            float[] numArray2 = m.elements;
            float num = numArray2[0];
            float num2 = numArray2[4];
            float num3 = numArray2[8];
            float num4 = numArray2[12];
            float num5 = numArray2[1];
            float num6 = numArray2[5];
            float num7 = numArray2[9];
            float num8 = numArray2[13];
            float num9 = numArray2[2];
            float num10 = numArray2[6];
            float num11 = numArray2[10];
            float num12 = numArray2[14];
            float num13 = numArray2[3];
            float num14 = numArray2[7];
            float num15 = numArray2[11];
            float num16 = numArray2[15];
            elements[0] = ((((((num7 * num12) * num14) - ((num8 * num11) * num14)) + ((num8 * num10) * num15)) - ((num6 * num12) * num15)) - ((num7 * num10) * num16)) + ((num6 * num11) * num16);
            elements[4] = ((((((num4 * num11) * num14) - ((num3 * num12) * num14)) - ((num4 * num10) * num15)) + ((num2 * num12) * num15)) + ((num3 * num10) * num16)) - ((num2 * num11) * num16);
            elements[8] = ((((((num3 * num8) * num14) - ((num4 * num7) * num14)) + ((num4 * num6) * num15)) - ((num2 * num8) * num15)) - ((num3 * num6) * num16)) + ((num2 * num7) * num16);
            elements[12] = ((((((num4 * num7) * num10) - ((num3 * num8) * num10)) - ((num4 * num6) * num11)) + ((num2 * num8) * num11)) + ((num3 * num6) * num12)) - ((num2 * num7) * num12);
            elements[1] = ((((((num8 * num11) * num13) - ((num7 * num12) * num13)) - ((num8 * num9) * num15)) + ((num5 * num12) * num15)) + ((num7 * num9) * num16)) - ((num5 * num11) * num16);
            elements[5] = ((((((num3 * num12) * num13) - ((num4 * num11) * num13)) + ((num4 * num9) * num15)) - ((num * num12) * num15)) - ((num3 * num9) * num16)) + ((num * num11) * num16);
            elements[9] = ((((((num4 * num7) * num13) - ((num3 * num8) * num13)) - ((num4 * num5) * num15)) + ((num * num8) * num15)) + ((num3 * num5) * num16)) - ((num * num7) * num16);
            elements[13] = ((((((num3 * num8) * num9) - ((num4 * num7) * num9)) + ((num4 * num5) * num11)) - ((num * num8) * num11)) - ((num3 * num5) * num12)) + ((num * num7) * num12);
            elements[2] = ((((((num6 * num12) * num13) - ((num8 * num10) * num13)) + ((num8 * num9) * num14)) - ((num5 * num12) * num14)) - ((num6 * num9) * num16)) + ((num5 * num10) * num16);
            elements[6] = ((((((num4 * num10) * num13) - ((num2 * num12) * num13)) - ((num4 * num9) * num14)) + ((num * num12) * num14)) + ((num2 * num9) * num16)) - ((num * num10) * num16);
            elements[10] = ((((((num2 * num8) * num13) - ((num4 * num6) * num13)) + ((num4 * num5) * num14)) - ((num * num8) * num14)) - ((num2 * num5) * num16)) + ((num * num6) * num16);
            elements[14] = ((((((num4 * num6) * num9) - ((num2 * num8) * num9)) - ((num4 * num5) * num10)) + ((num * num8) * num10)) + ((num2 * num5) * num12)) - ((num * num6) * num12);
            elements[3] = ((((((num7 * num10) * num13) - ((num6 * num11) * num13)) - ((num7 * num9) * num14)) + ((num5 * num11) * num14)) + ((num6 * num9) * num15)) - ((num5 * num10) * num15);
            elements[7] = ((((((num2 * num11) * num13) - ((num3 * num10) * num13)) + ((num3 * num9) * num14)) - ((num * num11) * num14)) - ((num2 * num9) * num15)) + ((num * num10) * num15);
            elements[11] = ((((((num3 * num6) * num13) - ((num2 * num7) * num13)) - ((num3 * num5) * num14)) + ((num * num7) * num14)) + ((num2 * num5) * num15)) - ((num * num6) * num15);
            elements[15] = ((((((num2 * num7) * num9) - ((num3 * num6) * num9)) + ((num3 * num5) * num10)) - ((num * num7) * num10)) - ((num2 * num5) * num11)) + ((num * num6) * num11);
            float num17 = (((num * elements[0]) + (num5 * elements[4])) + (num9 * elements[8])) + (num13 * elements[12]);
            if (num17 == 0f)
            {
                string message = "Matrix4.getInverse(): can't invert matrix, determinant is 0";
                if (throwOnInvertible)
                {
                    throw new ApplicationException();
                }
                Debug.WriteLine(message);
                this.identity();
                return this;
            }
            this.multiplyScalar(1f / num17);
            return this;
        }

        public Matrix4F identity()
        {
            this.set(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeRotationAxis(Vector3F axis, double angle)
        {
            float num = (float) Math.Cos(angle);
            float num2 = (float) Math.Sin(angle);
            float num3 = 1f - num;
            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            float num7 = num3 * x;
            float num8 = num3 * y;
            this.set((num7 * x) + num, (num7 * y) - (num2 * z), (num7 * z) + (num2 * y), 0f, (num7 * y) + (num2 * z), (num8 * y) + num, (num8 * z) - (num2 * x), 0f, (num7 * z) - (num2 * y), (num8 * z) + (num2 * x), ((num3 * z) * z) + num, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeRotationFromQuaternion(Vector4F q)
        {
            float[] elements = this.elements;
            float x = q.x;
            float y = q.y;
            float z = q.z;
            float w = q.w;
            float num5 = x + x;
            float num6 = y + y;
            float num7 = z + z;
            float num8 = x * num5;
            float num9 = x * num6;
            float num10 = x * num7;
            float num11 = y * num6;
            float num12 = y * num7;
            float num13 = z * num7;
            float num14 = w * num5;
            float num15 = w * num6;
            float num16 = w * num7;
            elements[0] = 1f - (num11 + num13);
            elements[4] = num9 - num16;
            elements[8] = num10 + num15;
            elements[1] = num9 + num16;
            elements[5] = 1f - (num8 + num13);
            elements[9] = num12 - num14;
            elements[2] = num10 - num15;
            elements[6] = num12 + num14;
            elements[10] = 1f - (num8 + num11);
            elements[3] = 0f;
            elements[7] = 0f;
            elements[11] = 0f;
            elements[12] = 0f;
            elements[13] = 0f;
            elements[14] = 0f;
            elements[15] = 1f;
            return this;
        }

        public Matrix4F makeRotationX(double theta)
        {
            float num = (float) Math.Cos(theta);
            float num2 = (float) Math.Sin(theta);
            this.set(1f, 0f, 0f, 0f, 0f, num, -num2, 0f, 0f, num2, num, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeRotationY(double theta)
        {
            float num = (float) Math.Cos(theta);
            float num2 = (float) Math.Sin(theta);
            this.set(num, 0f, num2, 0f, 0f, 1f, 0f, 0f, -num2, 0f, num, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeRotationZ(double theta)
        {
            float num = (float) Math.Cos(theta);
            float num2 = (float) Math.Sin(theta);
            this.set(num, -num2, 0f, 0f, num2, num, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeScale(float x, float y, float z)
        {
            this.set(x, 0f, 0f, 0f, 0f, y, 0f, 0f, 0f, 0f, z, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeTranslation(float x, float y, float z)
        {
            this.set(1f, 0f, 0f, x, 0f, 1f, 0f, y, 0f, 0f, 1f, z, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F multiply(Matrix4F n) => 
            this.multiplyMatrices(this, n);

        public Matrix4F multiplyMatrices(Matrix4F a, Matrix4F b)
        {
            float[] elements = a.elements;
            float[] numArray2 = b.elements;
            float[] numArray3 = this.elements;
            float num = elements[0];
            float num2 = elements[4];
            float num3 = elements[8];
            float num4 = elements[12];
            float num5 = elements[1];
            float num6 = elements[5];
            float num7 = elements[9];
            float num8 = elements[13];
            float num9 = elements[2];
            float num10 = elements[6];
            float num11 = elements[10];
            float num12 = elements[14];
            float num13 = elements[3];
            float num14 = elements[7];
            float num15 = elements[11];
            float num16 = elements[15];
            float num17 = numArray2[0];
            float num18 = numArray2[4];
            float num19 = numArray2[8];
            float num20 = numArray2[12];
            float num21 = numArray2[1];
            float num22 = numArray2[5];
            float num23 = numArray2[9];
            float num24 = numArray2[13];
            float num25 = numArray2[2];
            float num26 = numArray2[6];
            float num27 = numArray2[10];
            float num28 = numArray2[14];
            float num29 = numArray2[3];
            float num30 = numArray2[7];
            float num31 = numArray2[11];
            float num32 = numArray2[15];
            numArray3[0] = (((num * num17) + (num2 * num21)) + (num3 * num25)) + (num4 * num29);
            numArray3[4] = (((num * num18) + (num2 * num22)) + (num3 * num26)) + (num4 * num30);
            numArray3[8] = (((num * num19) + (num2 * num23)) + (num3 * num27)) + (num4 * num31);
            numArray3[12] = (((num * num20) + (num2 * num24)) + (num3 * num28)) + (num4 * num32);
            numArray3[1] = (((num5 * num17) + (num6 * num21)) + (num7 * num25)) + (num8 * num29);
            numArray3[5] = (((num5 * num18) + (num6 * num22)) + (num7 * num26)) + (num8 * num30);
            numArray3[9] = (((num5 * num19) + (num6 * num23)) + (num7 * num27)) + (num8 * num31);
            numArray3[13] = (((num5 * num20) + (num6 * num24)) + (num7 * num28)) + (num8 * num32);
            numArray3[2] = (((num9 * num17) + (num10 * num21)) + (num11 * num25)) + (num12 * num29);
            numArray3[6] = (((num9 * num18) + (num10 * num22)) + (num11 * num26)) + (num12 * num30);
            numArray3[10] = (((num9 * num19) + (num10 * num23)) + (num11 * num27)) + (num12 * num31);
            numArray3[14] = (((num9 * num20) + (num10 * num24)) + (num11 * num28)) + (num12 * num32);
            numArray3[3] = (((num13 * num17) + (num14 * num21)) + (num15 * num25)) + (num16 * num29);
            numArray3[7] = (((num13 * num18) + (num14 * num22)) + (num15 * num26)) + (num16 * num30);
            numArray3[11] = (((num13 * num19) + (num14 * num23)) + (num15 * num27)) + (num16 * num31);
            numArray3[15] = (((num13 * num20) + (num14 * num24)) + (num15 * num28)) + (num16 * num32);
            return this;
        }

        public Matrix4F multiplyScalar(float s)
        {
            float[] elements = this.elements;
            elements[0] *= s;
            elements[4] *= s;
            elements[8] *= s;
            elements[12] *= s;
            elements[1] *= s;
            elements[5] *= s;
            elements[9] *= s;
            elements[13] *= s;
            elements[2] *= s;
            elements[6] *= s;
            elements[10] *= s;
            elements[14] *= s;
            elements[3] *= s;
            elements[7] *= s;
            elements[11] *= s;
            elements[15] *= s;
            return this;
        }

        public Matrix4F multiplyToArray(Matrix4F a, Matrix4F b, float[] r)
        {
            float[] elements = this.elements;
            this.multiplyMatrices(a, b);
            r[0] = elements[0];
            r[1] = elements[1];
            r[2] = elements[2];
            r[3] = elements[3];
            r[4] = elements[4];
            r[5] = elements[5];
            r[6] = elements[6];
            r[7] = elements[7];
            r[8] = elements[8];
            r[9] = elements[9];
            r[10] = elements[10];
            r[11] = elements[11];
            r[12] = elements[12];
            r[13] = elements[13];
            r[14] = elements[14];
            r[15] = elements[15];
            return this;
        }

        public Matrix4F scale(Vector3F v)
        {
            float[] elements = this.elements;
            float x = v.x;
            float y = v.y;
            float z = v.z;
            elements[0] *= x;
            elements[4] *= y;
            elements[8] *= z;
            elements[1] *= x;
            elements[5] *= y;
            elements[9] *= z;
            elements[2] *= x;
            elements[6] *= y;
            elements[10] *= z;
            elements[3] *= x;
            elements[7] *= y;
            elements[11] *= z;
            return this;
        }

        public Matrix4F set(float n11, float n12, float n13, float n14, float n21, float n22, float n23, float n24, float n31, float n32, float n33, float n34, float n41, float n42, float n43, float n44)
        {
            float[] elements = this.elements;
            elements[0] = n11;
            elements[4] = n12;
            elements[8] = n13;
            elements[12] = n14;
            elements[1] = n21;
            elements[5] = n22;
            elements[9] = n23;
            elements[13] = n24;
            elements[2] = n31;
            elements[6] = n32;
            elements[10] = n33;
            elements[14] = n34;
            elements[3] = n41;
            elements[7] = n42;
            elements[11] = n43;
            elements[15] = n44;
            return this;
        }

        public Matrix4F setPosition(Vector3D v)
        {
            float[] elements = this.elements;
            elements[12] = (float) v.x;
            elements[13] = (float) v.y;
            elements[14] = (float) v.z;
            return this;
        }

        public Matrix4F setPosition(Vector3F v)
        {
            float[] elements = this.elements;
            elements[12] = v.x;
            elements[13] = v.y;
            elements[14] = v.z;
            return this;
        }

        public float[] toArray()
        {
            float[] elements = this.elements;
            return new float[] { elements[0], elements[1], elements[2], elements[3], elements[4], elements[5], elements[6], elements[7], elements[8], elements[9], elements[10], elements[11], elements[12], elements[13], elements[14], elements[15] };
        }

        public string toString()
        {
            float[] elements = this.elements;
            return $"{elements[0]:F3},{elements[1]:F3},{elements[2]:F3},{elements[3]:F3},{elements[4]:F3},{elements[5]:F3},{elements[6]:F3},{elements[7]:F3},{elements[8]:F3},{elements[9]:F3},{elements[10]:F3},{elements[11]:F3},{elements[12]:F3},{elements[13]:F3},{elements[14]:F3},{elements[15]:F3}";
        }

        public Vector3F transformDirection(Vector3F v)
        {
            float x = v.x;
            float y = v.y;
            float z = v.z;
            float[] elements = this.elements;
            v.x = ((elements[0] * x) + (elements[4] * y)) + (elements[8] * z);
            v.y = ((elements[1] * x) + (elements[5] * y)) + (elements[9] * z);
            v.z = ((elements[2] * x) + (elements[6] * y)) + (elements[10] * z);
            double num4 = Math.Sqrt((double) (((v.x * v.x) + (v.y * v.y)) + (v.z * v.z)));
            if (num4 > 0.0)
            {
                float num5 = (float) (1.0 / num4);
                v.x *= num5;
                v.y *= num5;
                v.z *= num5;
            }
            return v;
        }

        public Vector3F transformPoint(Vector3F pt)
        {
            float x = pt.x;
            float y = pt.y;
            float z = pt.z;
            float[] elements = this.elements;
            pt.x = (((elements[0] * x) + (elements[4] * y)) + (elements[8] * z)) + elements[12];
            pt.y = (((elements[1] * x) + (elements[5] * y)) + (elements[9] * z)) + elements[13];
            pt.z = (((elements[2] * x) + (elements[6] * y)) + (elements[10] * z)) + elements[14];
            return pt;
        }

        public Matrix4F transpose()
        {
            float[] elements = this.elements;
            float num = elements[1];
            elements[1] = elements[4];
            elements[4] = num;
            num = elements[2];
            elements[2] = elements[8];
            elements[8] = num;
            num = elements[6];
            elements[6] = elements[9];
            elements[9] = num;
            num = elements[3];
            elements[3] = elements[12];
            elements[12] = num;
            num = elements[7];
            elements[7] = elements[13];
            elements[13] = num;
            num = elements[11];
            elements[11] = elements[14];
            elements[14] = num;
            return this;
        }
    }
}

