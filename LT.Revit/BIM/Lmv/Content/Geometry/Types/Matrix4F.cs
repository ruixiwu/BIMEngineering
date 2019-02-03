using System;
using System.Diagnostics;

namespace BIM.Lmv.Content.Geometry.Types
{
    public class Matrix4F
    {
        public float[] elements = {1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f};

        public Matrix4F clone()
        {
            return new Matrix4F().fromArray(elements);
        }

        public Matrix4F compose(Vector3D position, Vector4F quaternion, Vector3F scale)
        {
            makeRotationFromQuaternion(quaternion);
            this.scale(scale);
            setPosition(position);
            return this;
        }

        public Matrix4F compose(Vector3F position, Vector4F quaternion, Vector3F scale)
        {
            makeRotationFromQuaternion(quaternion);
            this.scale(scale);
            setPosition(position);
            return this;
        }

        public Matrix4F copy(Matrix4F m)
        {
            m.elements.CopyTo(elements, 0);
            return this;
        }

        public float determinant()
        {
            var elements = this.elements;
            var num = elements[0];
            var num2 = elements[4];
            var num3 = elements[8];
            var num4 = elements[12];
            var num5 = elements[1];
            var num6 = elements[5];
            var num7 = elements[9];
            var num8 = elements[13];
            var num9 = elements[2];
            var num10 = elements[6];
            var num11 = elements[10];
            var num12 = elements[14];
            var num13 = elements[3];
            var num14 = elements[7];
            var num15 = elements[11];
            var num16 = elements[15];
            return num13*
                   (num4*num7*num10 - num3*num8*num10 - num4*num6*num11 + num2*num8*num11 + num3*num6*num12 -
                    num2*num7*num12) +
                   num14*
                   (num*num7*num12 - num*num8*num11 + num4*num5*num11 - num3*num5*num12 + num3*num8*num9 -
                    num4*num7*num9) +
                   num15*
                   (num*num8*num10 - num*num6*num12 - num4*num5*num10 + num2*num5*num12 + num4*num6*num9 -
                    num2*num8*num9) +
                   num16*
                   (-num3*num6*num9 - num*num7*num10 + num*num6*num11 + num3*num5*num10 - num2*num5*num11 +
                    num2*num7*num9);
        }

        public float[] flattenToArrayOffset(float[] array, int offset)
        {
            var elements = this.elements;
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
            array.CopyTo(elements, 0);
            return this;
        }

        public Matrix4F getInverse(Matrix4F m, bool throwOnInvertible)
        {
            var elements = this.elements;
            var numArray2 = m.elements;
            var num = numArray2[0];
            var num2 = numArray2[4];
            var num3 = numArray2[8];
            var num4 = numArray2[12];
            var num5 = numArray2[1];
            var num6 = numArray2[5];
            var num7 = numArray2[9];
            var num8 = numArray2[13];
            var num9 = numArray2[2];
            var num10 = numArray2[6];
            var num11 = numArray2[10];
            var num12 = numArray2[14];
            var num13 = numArray2[3];
            var num14 = numArray2[7];
            var num15 = numArray2[11];
            var num16 = numArray2[15];
            elements[0] = num7*num12*num14 - num8*num11*num14 + num8*num10*num15 - num6*num12*num15 - num7*num10*num16 +
                          num6*num11*num16;
            elements[4] = num4*num11*num14 - num3*num12*num14 - num4*num10*num15 + num2*num12*num15 + num3*num10*num16 -
                          num2*num11*num16;
            elements[8] = num3*num8*num14 - num4*num7*num14 + num4*num6*num15 - num2*num8*num15 - num3*num6*num16 +
                          num2*num7*num16;
            elements[12] = num4*num7*num10 - num3*num8*num10 - num4*num6*num11 + num2*num8*num11 + num3*num6*num12 -
                           num2*num7*num12;
            elements[1] = num8*num11*num13 - num7*num12*num13 - num8*num9*num15 + num5*num12*num15 + num7*num9*num16 -
                          num5*num11*num16;
            elements[5] = num3*num12*num13 - num4*num11*num13 + num4*num9*num15 - num*num12*num15 - num3*num9*num16 +
                          num*num11*num16;
            elements[9] = num4*num7*num13 - num3*num8*num13 - num4*num5*num15 + num*num8*num15 + num3*num5*num16 -
                          num*num7*num16;
            elements[13] = num3*num8*num9 - num4*num7*num9 + num4*num5*num11 - num*num8*num11 - num3*num5*num12 +
                           num*num7*num12;
            elements[2] = num6*num12*num13 - num8*num10*num13 + num8*num9*num14 - num5*num12*num14 - num6*num9*num16 +
                          num5*num10*num16;
            elements[6] = num4*num10*num13 - num2*num12*num13 - num4*num9*num14 + num*num12*num14 + num2*num9*num16 -
                          num*num10*num16;
            elements[10] = num2*num8*num13 - num4*num6*num13 + num4*num5*num14 - num*num8*num14 - num2*num5*num16 +
                           num*num6*num16;
            elements[14] = num4*num6*num9 - num2*num8*num9 - num4*num5*num10 + num*num8*num10 + num2*num5*num12 -
                           num*num6*num12;
            elements[3] = num7*num10*num13 - num6*num11*num13 - num7*num9*num14 + num5*num11*num14 + num6*num9*num15 -
                          num5*num10*num15;
            elements[7] = num2*num11*num13 - num3*num10*num13 + num3*num9*num14 - num*num11*num14 - num2*num9*num15 +
                          num*num10*num15;
            elements[11] = num3*num6*num13 - num2*num7*num13 - num3*num5*num14 + num*num7*num14 + num2*num5*num15 -
                           num*num6*num15;
            elements[15] = num2*num7*num9 - num3*num6*num9 + num3*num5*num10 - num*num7*num10 - num2*num5*num11 +
                           num*num6*num11;
            var num17 = num*elements[0] + num5*elements[4] + num9*elements[8] + num13*elements[12];
            if (num17 == 0f)
            {
                var message = "Matrix4.getInverse(): can't invert matrix, determinant is 0";
                if (throwOnInvertible)
                {
                    throw new ApplicationException();
                }
                Debug.WriteLine(message);
                identity();
                return this;
            }
            multiplyScalar(1f/num17);
            return this;
        }

        public Matrix4F identity()
        {
            set(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeRotationAxis(Vector3F axis, double angle)
        {
            var num = (float) Math.Cos(angle);
            var num2 = (float) Math.Sin(angle);
            var num3 = 1f - num;
            var x = axis.x;
            var y = axis.y;
            var z = axis.z;
            var num7 = num3*x;
            var num8 = num3*y;
            set(num7*x + num, num7*y - num2*z, num7*z + num2*y, 0f, num7*y + num2*z, num8*y + num, num8*z - num2*x, 0f,
                num7*z - num2*y, num8*z + num2*x, num3*z*z + num, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeRotationFromQuaternion(Vector4F q)
        {
            var elements = this.elements;
            var x = q.x;
            var y = q.y;
            var z = q.z;
            var w = q.w;
            var num5 = x + x;
            var num6 = y + y;
            var num7 = z + z;
            var num8 = x*num5;
            var num9 = x*num6;
            var num10 = x*num7;
            var num11 = y*num6;
            var num12 = y*num7;
            var num13 = z*num7;
            var num14 = w*num5;
            var num15 = w*num6;
            var num16 = w*num7;
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
            var num = (float) Math.Cos(theta);
            var num2 = (float) Math.Sin(theta);
            set(1f, 0f, 0f, 0f, 0f, num, -num2, 0f, 0f, num2, num, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeRotationY(double theta)
        {
            var num = (float) Math.Cos(theta);
            var num2 = (float) Math.Sin(theta);
            set(num, 0f, num2, 0f, 0f, 1f, 0f, 0f, -num2, 0f, num, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeRotationZ(double theta)
        {
            var num = (float) Math.Cos(theta);
            var num2 = (float) Math.Sin(theta);
            set(num, -num2, 0f, 0f, num2, num, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeScale(float x, float y, float z)
        {
            set(x, 0f, 0f, 0f, 0f, y, 0f, 0f, 0f, 0f, z, 0f, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F makeTranslation(float x, float y, float z)
        {
            set(1f, 0f, 0f, x, 0f, 1f, 0f, y, 0f, 0f, 1f, z, 0f, 0f, 0f, 1f);
            return this;
        }

        public Matrix4F multiply(Matrix4F n)
        {
            return multiplyMatrices(this, n);
        }

        public Matrix4F multiplyMatrices(Matrix4F a, Matrix4F b)
        {
            var elements = a.elements;
            var numArray2 = b.elements;
            var numArray3 = this.elements;
            var num = elements[0];
            var num2 = elements[4];
            var num3 = elements[8];
            var num4 = elements[12];
            var num5 = elements[1];
            var num6 = elements[5];
            var num7 = elements[9];
            var num8 = elements[13];
            var num9 = elements[2];
            var num10 = elements[6];
            var num11 = elements[10];
            var num12 = elements[14];
            var num13 = elements[3];
            var num14 = elements[7];
            var num15 = elements[11];
            var num16 = elements[15];
            var num17 = numArray2[0];
            var num18 = numArray2[4];
            var num19 = numArray2[8];
            var num20 = numArray2[12];
            var num21 = numArray2[1];
            var num22 = numArray2[5];
            var num23 = numArray2[9];
            var num24 = numArray2[13];
            var num25 = numArray2[2];
            var num26 = numArray2[6];
            var num27 = numArray2[10];
            var num28 = numArray2[14];
            var num29 = numArray2[3];
            var num30 = numArray2[7];
            var num31 = numArray2[11];
            var num32 = numArray2[15];
            numArray3[0] = num*num17 + num2*num21 + num3*num25 + num4*num29;
            numArray3[4] = num*num18 + num2*num22 + num3*num26 + num4*num30;
            numArray3[8] = num*num19 + num2*num23 + num3*num27 + num4*num31;
            numArray3[12] = num*num20 + num2*num24 + num3*num28 + num4*num32;
            numArray3[1] = num5*num17 + num6*num21 + num7*num25 + num8*num29;
            numArray3[5] = num5*num18 + num6*num22 + num7*num26 + num8*num30;
            numArray3[9] = num5*num19 + num6*num23 + num7*num27 + num8*num31;
            numArray3[13] = num5*num20 + num6*num24 + num7*num28 + num8*num32;
            numArray3[2] = num9*num17 + num10*num21 + num11*num25 + num12*num29;
            numArray3[6] = num9*num18 + num10*num22 + num11*num26 + num12*num30;
            numArray3[10] = num9*num19 + num10*num23 + num11*num27 + num12*num31;
            numArray3[14] = num9*num20 + num10*num24 + num11*num28 + num12*num32;
            numArray3[3] = num13*num17 + num14*num21 + num15*num25 + num16*num29;
            numArray3[7] = num13*num18 + num14*num22 + num15*num26 + num16*num30;
            numArray3[11] = num13*num19 + num14*num23 + num15*num27 + num16*num31;
            numArray3[15] = num13*num20 + num14*num24 + num15*num28 + num16*num32;
            return this;
        }

        public Matrix4F multiplyScalar(float s)
        {
            var elements = this.elements;
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
            var elements = this.elements;
            multiplyMatrices(a, b);
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
            var elements = this.elements;
            var x = v.x;
            var y = v.y;
            var z = v.z;
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

        public Matrix4F set(float n11, float n12, float n13, float n14, float n21, float n22, float n23, float n24,
            float n31, float n32, float n33, float n34, float n41, float n42, float n43, float n44)
        {
            var elements = this.elements;
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
            var elements = this.elements;
            elements[12] = (float) v.x;
            elements[13] = (float) v.y;
            elements[14] = (float) v.z;
            return this;
        }

        public Matrix4F setPosition(Vector3F v)
        {
            var elements = this.elements;
            elements[12] = v.x;
            elements[13] = v.y;
            elements[14] = v.z;
            return this;
        }

        public float[] toArray()
        {
            var elements = this.elements;
            return new[]
            {
                elements[0], elements[1], elements[2], elements[3], elements[4], elements[5], elements[6], elements[7],
                elements[8], elements[9], elements[10], elements[11], elements[12], elements[13], elements[14],
                elements[15]
            };
        }

        public string toString()
        {
            var elements = this.elements;
            return
                string.Format(
                    "{0:F3},{1:F3},{2:F3},{3:F3},{4:F3},{5:F3},{6:F3},{7:F3},{8:F3},{9:F3},{10:F3},{11:F3},{12:F3},{13:F3},{14:F3},{15:F3}",
                    elements[0], elements[1], elements[2], elements[3], elements[4], elements[5], elements[6],
                    elements[7], elements[8], elements[9], elements[10], elements[11], elements[12], elements[13],
                    elements[14], elements[15]);
        }

        public Vector3F transformDirection(Vector3F v)
        {
            var x = v.x;
            var y = v.y;
            var z = v.z;
            var elements = this.elements;
            v.x = elements[0]*x + elements[4]*y + elements[8]*z;
            v.y = elements[1]*x + elements[5]*y + elements[9]*z;
            v.z = elements[2]*x + elements[6]*y + elements[10]*z;
            var num4 = Math.Sqrt(v.x*v.x + v.y*v.y + v.z*v.z);
            if (num4 > 0.0)
            {
                var num5 = (float) (1.0/num4);
                v.x *= num5;
                v.y *= num5;
                v.z *= num5;
            }
            return v;
        }

        public Vector3F transformPoint(Vector3F pt)
        {
            var x = pt.x;
            var y = pt.y;
            var z = pt.z;
            var elements = this.elements;
            pt.x = elements[0]*x + elements[4]*y + elements[8]*z + elements[12];
            pt.y = elements[1]*x + elements[5]*y + elements[9]*z + elements[13];
            pt.z = elements[2]*x + elements[6]*y + elements[10]*z + elements[14];
            return pt;
        }

        public Matrix4F transpose()
        {
            var elements = this.elements;
            var num = elements[1];
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