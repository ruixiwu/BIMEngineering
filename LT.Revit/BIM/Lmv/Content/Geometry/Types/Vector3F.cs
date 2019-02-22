namespace BIM.Lmv.Content.Geometry.Types
{
    using System;
    using System.Runtime.InteropServices;

    public class Vector3F
    {
        public float x;
        public float y;
        public float z;

        public Vector3F()
        {
        }

        public Vector3F(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3F add(Vector3F v)
        {
            this.x += v.x;
            this.y += v.y;
            this.z += v.z;
            return this;
        }

        public Vector3F addScalar(float s)
        {
            this.x += s;
            this.y += s;
            this.z += s;
            return this;
        }

        public Vector3F addScaledVector(Vector3F v, float s)
        {
            this.x += v.x * s;
            this.y += v.y * s;
            this.z += v.z * s;
            return this;
        }

        public Vector3F addVectors(Vector3F a, Vector3F b)
        {
            this.x = a.x + b.x;
            this.y = a.y + b.y;
            this.z = a.z + b.z;
            return this;
        }

        public Vector3F applyMatrix3(Matrix4F m)
        {
            float x = this.x;
            float y = this.y;
            float z = this.z;
            float[] elements = m.elements;
            this.x = ((elements[0] * x) + (elements[3] * y)) + (elements[6] * z);
            this.y = ((elements[1] * x) + (elements[4] * y)) + (elements[7] * z);
            this.z = ((elements[2] * x) + (elements[5] * y)) + (elements[8] * z);
            return this;
        }

        public Vector3F applyMatrix4(Matrix4F m)
        {
            float x = this.x;
            float y = this.y;
            float z = this.z;
            float[] elements = m.elements;
            this.x = (((elements[0] * x) + (elements[4] * y)) + (elements[8] * z)) + elements[12];
            this.y = (((elements[1] * x) + (elements[5] * y)) + (elements[9] * z)) + elements[13];
            this.z = (((elements[2] * x) + (elements[6] * y)) + (elements[10] * z)) + elements[14];
            return this;
        }

        public Vector3F applyProjection(Matrix4F m)
        {
            float x = this.x;
            float y = this.y;
            float z = this.z;
            float[] elements = m.elements;
            float num4 = 1f / ((((elements[3] * x) + (elements[7] * y)) + (elements[11] * z)) + elements[15]);
            this.x = ((((elements[0] * x) + (elements[4] * y)) + (elements[8] * z)) + elements[12]) * num4;
            this.y = ((((elements[1] * x) + (elements[5] * y)) + (elements[9] * z)) + elements[13]) * num4;
            this.z = ((((elements[2] * x) + (elements[6] * y)) + (elements[10] * z)) + elements[14]) * num4;
            return this;
        }

        public Vector3F applyQuaternion(Vector4F q)
        {
            float x = this.x;
            float y = this.y;
            float z = this.z;
            float num4 = q.x;
            float num5 = q.y;
            float num6 = q.z;
            float w = q.w;
            float num8 = ((w * x) + (num5 * z)) - (num6 * y);
            float num9 = ((w * y) + (num6 * x)) - (num4 * z);
            float num10 = ((w * z) + (num4 * y)) - (num5 * x);
            float num11 = ((-num4 * x) - (num5 * y)) - (num6 * z);
            this.x = (((num8 * w) + (num11 * -num4)) + (num9 * -num6)) - (num10 * -num5);
            this.y = (((num9 * w) + (num11 * -num5)) + (num10 * -num4)) - (num8 * -num6);
            this.z = (((num10 * w) + (num11 * -num6)) + (num8 * -num5)) - (num9 * -num4);
            return this;
        }

        public Vector3F ceil()
        {
            this.x = (float) Math.Ceiling((double) this.x);
            this.y = (float) Math.Ceiling((double) this.y);
            this.z = (float) Math.Ceiling((double) this.z);
            return this;
        }

        public Vector3F clamp(Vector3F min, Vector3F max)
        {
            if (this.x < min.x)
            {
                this.x = min.x;
            }
            else if (this.x > max.x)
            {
                this.x = max.x;
            }
            if (this.y < min.y)
            {
                this.y = min.y;
            }
            else if (this.y > max.y)
            {
                this.y = max.y;
            }
            if (this.z < min.z)
            {
                this.z = min.z;
            }
            else if (this.z > max.z)
            {
                this.z = max.z;
            }
            return this;
        }

        public Vector3F clampScalar(float minVal, float maxVal)
        {
            Vector3F min = new Vector3F();
            Vector3F max = new Vector3F();
            min.set(minVal, minVal, minVal);
            max.set(maxVal, maxVal, maxVal);
            return this.clamp(min, max);
        }

        public Vector3F clone() => 
            new Vector3F(this.x, this.y, this.z);

        public Vector3F copy(Vector3F v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            return this;
        }

        public Vector3F cross(Vector3F v)
        {
            float x = this.x;
            float y = this.y;
            float z = this.z;
            this.x = (y * v.z) - (z * v.y);
            this.y = (z * v.x) - (x * v.z);
            this.z = (x * v.y) - (y * v.x);
            return this;
        }

        public Vector3F crossVectors(Vector3F a, Vector3F b)
        {
            float x = a.x;
            float y = a.y;
            float z = a.z;
            float num4 = b.x;
            float num5 = b.y;
            float num6 = b.z;
            this.x = (y * num6) - (z * num5);
            this.y = (z * num4) - (x * num6);
            this.z = (x * num5) - (y * num4);
            return this;
        }

        public float distanceTo(Vector3F v) => 
            ((float) Math.Sqrt((double) this.distanceToSquared(v)));

        public float distanceToSquared(Vector3F v)
        {
            float num = this.x - v.x;
            float num2 = this.y - v.y;
            float num3 = this.z - v.z;
            return (((num * num) + (num2 * num2)) + (num3 * num3));
        }

        public Vector3F divide(Vector3F v)
        {
            this.x /= v.x;
            this.y /= v.y;
            this.z /= v.z;
            return this;
        }

        public Vector3F divideScalar(float scalar)
        {
            if (!(scalar == 0f))
            {
                float num = 1f / scalar;
                this.x *= num;
                this.y *= num;
                this.z *= num;
            }
            else
            {
                this.x = 0f;
                this.y = 0f;
                this.z = 0f;
            }
            return this;
        }

        public float dot(Vector3F v) => 
            (((this.x * v.x) + (this.y * v.y)) + (this.z * v.z));

        public bool equals(Vector3F v) => 
            (((v.x == this.x) && (v.y == this.y)) && (v.z == this.z));

        public Vector3F floor()
        {
            this.x = (float) Math.Floor((double) this.x);
            this.y = (float) Math.Floor((double) this.y);
            this.z = (float) Math.Floor((double) this.z);
            return this;
        }

        public Vector3F fromArray(float[] array, int offset = 0)
        {
            this.x = array[offset];
            this.y = array[offset + 1];
            this.z = array[offset + 2];
            return this;
        }

        public Vector3F fromAttribute(float[] attributeArray, int attributeItemSize, int index, int offset)
        {
            if (offset < 0)
            {
                offset = 0;
            }
            index = (index * attributeItemSize) + offset;
            this.x = attributeArray[index];
            this.y = attributeArray[index + 1];
            this.z = attributeArray[index + 2];
            return this;
        }

        public Vector3F getColumnFromMatrix(int index, Matrix4F matrix) => 
            this.setFromMatrixColumn(index, matrix);

        public float getComponent(int index)
        {
            switch (index)
            {
                case 0:
                    return this.x;

                case 1:
                    return this.y;

                case 2:
                    return this.z;
            }
            throw new NotSupportedException("index is out of range: " + index);
        }

        public Vector3F getPositionFromMatrix(Matrix4F m) => 
            this.setFromMatrixPosition(m);

        public Vector3F getScaleFromMatrix(Matrix4F m) => 
            this.setFromMatrixScale(m);

        public float length() => 
            ((float) Math.Sqrt((double) (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z))));

        public float lengthManhattan() => 
            ((Math.Abs(this.x) + Math.Abs(this.y)) + Math.Abs(this.z));

        public float lengthSq() => 
            (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));

        public Vector3F lerp(Vector3F v, float alpha)
        {
            this.x += (v.x - this.x) * alpha;
            this.y += (v.y - this.y) * alpha;
            this.z += (v.z - this.z) * alpha;
            return this;
        }

        public Vector3F lerpVectors(Vector3F v1, Vector3F v2, float alpha)
        {
            this.subVectors(v2, v1).multiplyScalar(alpha).add(v1);
            return this;
        }

        public Vector3F max(Vector3F v)
        {
            if (this.x < v.x)
            {
                this.x = v.x;
            }
            if (this.y < v.y)
            {
                this.y = v.y;
            }
            if (this.z < v.z)
            {
                this.z = v.z;
            }
            return this;
        }

        public Vector3F min(Vector3F v)
        {
            if (this.x > v.x)
            {
                this.x = v.x;
            }
            if (this.y > v.y)
            {
                this.y = v.y;
            }
            if (this.z > v.z)
            {
                this.z = v.z;
            }
            return this;
        }

        public Vector3F multiply(Vector3F v, Vector3F w)
        {
            if (w != null)
            {
                return this.multiplyVectors(v, w);
            }
            this.x *= v.x;
            this.y *= v.y;
            this.z *= v.z;
            return this;
        }

        public Vector3F multiplyScalar(float scalar)
        {
            this.x *= scalar;
            this.y *= scalar;
            this.z *= scalar;
            return this;
        }

        public Vector3F multiplyVectors(Vector3F a, Vector3F b)
        {
            this.x = a.x * b.x;
            this.y = a.y * b.y;
            this.z = a.z * b.z;
            return this;
        }

        public Vector3F negate()
        {
            this.x = -this.x;
            this.y = -this.y;
            this.z = -this.z;
            return this;
        }

        public Vector3F normalize() => 
            this.divideScalar(this.length());

        public static explicit operator Vector3F(Vector3D v)
        {
            if (v == null)
            {
                return null;
            }
            return new Vector3F((float) v.x, (float) v.y, (float) v.z);
        }

        public Vector3F projectOnPlane(Vector3F planeNormal)
        {
            Vector3F v = new Vector3F();
            v.copy(this).projectOnVector(planeNormal);
            return this.sub(v);
        }

        public Vector3F projectOnVector(Vector3F vector)
        {
            Vector3F v = new Vector3F();
            v.copy(vector).normalize();
            float scalar = this.dot(v);
            return this.copy(v).multiplyScalar(scalar);
        }

        public Vector3F reflect(Vector3F normal)
        {
            Vector3F vectorf = new Vector3F();
            return this.sub(vectorf.copy(normal).multiplyScalar(2f * this.dot(normal)));
        }

        public Vector3F round()
        {
            this.x = (float) Math.Round((double) this.x);
            this.y = (float) Math.Round((double) this.y);
            this.z = (float) Math.Round((double) this.z);
            return this;
        }

        public Vector3F roundToZero()
        {
            this.x = (this.x < 0f) ? ((float) Math.Ceiling((double) this.x)) : ((float) Math.Floor((double) this.x));
            this.y = (this.y < 0f) ? ((float) Math.Ceiling((double) this.y)) : ((float) Math.Floor((double) this.y));
            this.z = (this.z < 0f) ? ((float) Math.Ceiling((double) this.z)) : ((float) Math.Floor((double) this.z));
            return this;
        }

        public Vector3F set(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            return this;
        }

        public void setComponent(int index, float value)
        {
            switch (index)
            {
                case 0:
                    this.x = value;
                    break;

                case 1:
                    this.y = value;
                    break;

                case 2:
                    this.z = value;
                    break;

                default:
                    throw new NotSupportedException("index is out of range: " + index);
            }
        }

        public Vector3F setFromMatrixColumn(int index, Matrix4F matrix)
        {
            int num = index * 4;
            float[] elements = matrix.elements;
            this.x = elements[num];
            this.y = elements[num + 1];
            this.z = elements[num + 2];
            return this;
        }

        public Vector3F setFromMatrixPosition(Matrix4F m)
        {
            this.x = m.elements[12];
            this.y = m.elements[13];
            this.z = m.elements[14];
            return this;
        }

        public Vector3F setFromMatrixScale(Matrix4F m)
        {
            float num = this.set(m.elements[0], m.elements[1], m.elements[2]).length();
            float num2 = this.set(m.elements[4], m.elements[5], m.elements[6]).length();
            float num3 = this.set(m.elements[8], m.elements[9], m.elements[10]).length();
            this.x = num;
            this.y = num2;
            this.z = num3;
            return this;
        }

        public Vector3F setLength(float l)
        {
            float num = this.length();
            if ((num != 0f) && (l != num))
            {
                this.multiplyScalar(l / num);
            }
            return this;
        }

        public Vector3F setX(float x)
        {
            this.x = x;
            return this;
        }

        public Vector3F setY(float y)
        {
            this.y = y;
            return this;
        }

        public Vector3F setZ(float z)
        {
            this.z = z;
            return this;
        }

        public Vector3F sub(Vector3F v)
        {
            this.x -= v.x;
            this.y -= v.y;
            this.z -= v.z;
            return this;
        }

        public Vector3F subScalar(float s)
        {
            this.x -= s;
            this.y -= s;
            this.z -= s;
            return this;
        }

        public Vector3F subVectors(Vector3F a, Vector3F b)
        {
            this.x = a.x - b.x;
            this.y = a.y - b.y;
            this.z = a.z - b.z;
            return this;
        }

        public float[] toArray(float[] array, int offset)
        {
            if (offset < 0)
            {
                offset = 0;
            }
            if (array == null)
            {
                array = new float[offset + 3];
            }
            array[offset] = this.x;
            array[offset + 1] = this.y;
            array[offset + 2] = this.z;
            return array;
        }

        public override string ToString() => 
            string.Concat(new object[] { "Viector3F(", this.x, ",", this.y, ",", this.z, ")" });

        public Vector3F transformDirection(Matrix4F m)
        {
            float x = this.x;
            float y = this.y;
            float z = this.z;
            float[] elements = m.elements;
            this.x = ((elements[0] * x) + (elements[4] * y)) + (elements[8] * z);
            this.y = ((elements[1] * x) + (elements[5] * y)) + (elements[9] * z);
            this.z = ((elements[2] * x) + (elements[6] * y)) + (elements[10] * z);
            this.normalize();
            return this;
        }
    }
}

