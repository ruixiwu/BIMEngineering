using System;

namespace BIM.Lmv.Content.Geometry.Types
{
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
            x += v.x;
            y += v.y;
            z += v.z;
            return this;
        }

        public Vector3F addScalar(float s)
        {
            x += s;
            y += s;
            z += s;
            return this;
        }

        public Vector3F addScaledVector(Vector3F v, float s)
        {
            x += v.x*s;
            y += v.y*s;
            z += v.z*s;
            return this;
        }

        public Vector3F addVectors(Vector3F a, Vector3F b)
        {
            x = a.x + b.x;
            y = a.y + b.y;
            z = a.z + b.z;
            return this;
        }

        public Vector3F applyMatrix3(Matrix4F m)
        {
            var x = this.x;
            var y = this.y;
            var z = this.z;
            var elements = m.elements;
            this.x = elements[0]*x + elements[3]*y + elements[6]*z;
            this.y = elements[1]*x + elements[4]*y + elements[7]*z;
            this.z = elements[2]*x + elements[5]*y + elements[8]*z;
            return this;
        }

        public Vector3F applyMatrix4(Matrix4F m)
        {
            var x = this.x;
            var y = this.y;
            var z = this.z;
            var elements = m.elements;
            this.x = elements[0]*x + elements[4]*y + elements[8]*z + elements[12];
            this.y = elements[1]*x + elements[5]*y + elements[9]*z + elements[13];
            this.z = elements[2]*x + elements[6]*y + elements[10]*z + elements[14];
            return this;
        }

        public Vector3F applyProjection(Matrix4F m)
        {
            var x = this.x;
            var y = this.y;
            var z = this.z;
            var elements = m.elements;
            var num4 = 1f/(elements[3]*x + elements[7]*y + elements[11]*z + elements[15]);
            this.x = (elements[0]*x + elements[4]*y + elements[8]*z + elements[12])*num4;
            this.y = (elements[1]*x + elements[5]*y + elements[9]*z + elements[13])*num4;
            this.z = (elements[2]*x + elements[6]*y + elements[10]*z + elements[14])*num4;
            return this;
        }

        public Vector3F applyQuaternion(Vector4F q)
        {
            var x = this.x;
            var y = this.y;
            var z = this.z;
            var num4 = q.x;
            var num5 = q.y;
            var num6 = q.z;
            var w = q.w;
            var num8 = w*x + num5*z - num6*y;
            var num9 = w*y + num6*x - num4*z;
            var num10 = w*z + num4*y - num5*x;
            var num11 = -num4*x - num5*y - num6*z;
            this.x = num8*w + num11*-num4 + num9*-num6 - num10*-num5;
            this.y = num9*w + num11*-num5 + num10*-num4 - num8*-num6;
            this.z = num10*w + num11*-num6 + num8*-num5 - num9*-num4;
            return this;
        }

        public Vector3F ceil()
        {
            x = (float) Math.Ceiling(x);
            y = (float) Math.Ceiling(y);
            z = (float) Math.Ceiling(z);
            return this;
        }

        public Vector3F clamp(Vector3F min, Vector3F max)
        {
            if (x < min.x)
            {
                x = min.x;
            }
            else if (x > max.x)
            {
                x = max.x;
            }
            if (y < min.y)
            {
                y = min.y;
            }
            else if (y > max.y)
            {
                y = max.y;
            }
            if (z < min.z)
            {
                z = min.z;
            }
            else if (z > max.z)
            {
                z = max.z;
            }
            return this;
        }

        public Vector3F clampScalar(float minVal, float maxVal)
        {
            var min = new Vector3F();
            var max = new Vector3F();
            min.set(minVal, minVal, minVal);
            max.set(maxVal, maxVal, maxVal);
            return clamp(min, max);
        }

        public Vector3F clone()
        {
            return new Vector3F(x, y, z);
        }

        public Vector3F copy(Vector3F v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            return this;
        }

        public Vector3F cross(Vector3F v)
        {
            var x = this.x;
            var y = this.y;
            var z = this.z;
            this.x = y*v.z - z*v.y;
            this.y = z*v.x - x*v.z;
            this.z = x*v.y - y*v.x;
            return this;
        }

        public Vector3F crossVectors(Vector3F a, Vector3F b)
        {
            var x = a.x;
            var y = a.y;
            var z = a.z;
            var num4 = b.x;
            var num5 = b.y;
            var num6 = b.z;
            this.x = y*num6 - z*num5;
            this.y = z*num4 - x*num6;
            this.z = x*num5 - y*num4;
            return this;
        }

        public float distanceTo(Vector3F v)
        {
            return (float) Math.Sqrt(distanceToSquared(v));
        }

        public float distanceToSquared(Vector3F v)
        {
            var num = x - v.x;
            var num2 = y - v.y;
            var num3 = z - v.z;
            return num*num + num2*num2 + num3*num3;
        }

        public Vector3F divide(Vector3F v)
        {
            x /= v.x;
            y /= v.y;
            z /= v.z;
            return this;
        }

        public Vector3F divideScalar(float scalar)
        {
            if (!(scalar == 0f))
            {
                var num = 1f/scalar;
                x *= num;
                y *= num;
                z *= num;
            }
            else
            {
                x = 0f;
                y = 0f;
                z = 0f;
            }
            return this;
        }

        public float dot(Vector3F v)
        {
            return x*v.x + y*v.y + z*v.z;
        }

        public bool equals(Vector3F v)
        {
            return (v.x == x) && (v.y == y) && (v.z == z);
        }

        public Vector3F floor()
        {
            x = (float) Math.Floor(x);
            y = (float) Math.Floor(y);
            z = (float) Math.Floor(z);
            return this;
        }

        public Vector3F fromArray(float[] array, int offset = 0)
        {
            x = array[offset];
            y = array[offset + 1];
            z = array[offset + 2];
            return this;
        }

        public Vector3F fromAttribute(float[] attributeArray, int attributeItemSize, int index, int offset)
        {
            if (offset < 0)
            {
                offset = 0;
            }
            index = index*attributeItemSize + offset;
            x = attributeArray[index];
            y = attributeArray[index + 1];
            z = attributeArray[index + 2];
            return this;
        }

        public Vector3F getColumnFromMatrix(int index, Matrix4F matrix)
        {
            return setFromMatrixColumn(index, matrix);
        }

        public float getComponent(int index)
        {
            switch (index)
            {
                case 0:
                    return x;

                case 1:
                    return y;

                case 2:
                    return z;
            }
            throw new NotSupportedException("index is out of range: " + index);
        }

        public Vector3F getPositionFromMatrix(Matrix4F m)
        {
            return setFromMatrixPosition(m);
        }

        public Vector3F getScaleFromMatrix(Matrix4F m)
        {
            return setFromMatrixScale(m);
        }

        public float length()
        {
            return (float) Math.Sqrt(x*x + y*y + z*z);
        }

        public float lengthManhattan()
        {
            return Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
        }

        public float lengthSq()
        {
            return x*x + y*y + z*z;
        }

        public Vector3F lerp(Vector3F v, float alpha)
        {
            x += (v.x - x)*alpha;
            y += (v.y - y)*alpha;
            z += (v.z - z)*alpha;
            return this;
        }

        public Vector3F lerpVectors(Vector3F v1, Vector3F v2, float alpha)
        {
            subVectors(v2, v1).multiplyScalar(alpha).add(v1);
            return this;
        }

        public Vector3F max(Vector3F v)
        {
            if (x < v.x)
            {
                x = v.x;
            }
            if (y < v.y)
            {
                y = v.y;
            }
            if (z < v.z)
            {
                z = v.z;
            }
            return this;
        }

        public Vector3F min(Vector3F v)
        {
            if (x > v.x)
            {
                x = v.x;
            }
            if (y > v.y)
            {
                y = v.y;
            }
            if (z > v.z)
            {
                z = v.z;
            }
            return this;
        }

        public Vector3F multiply(Vector3F v, Vector3F w)
        {
            if (w != null)
            {
                return multiplyVectors(v, w);
            }
            x *= v.x;
            y *= v.y;
            z *= v.z;
            return this;
        }

        public Vector3F multiplyScalar(float scalar)
        {
            x *= scalar;
            y *= scalar;
            z *= scalar;
            return this;
        }

        public Vector3F multiplyVectors(Vector3F a, Vector3F b)
        {
            x = a.x*b.x;
            y = a.y*b.y;
            z = a.z*b.z;
            return this;
        }

        public Vector3F negate()
        {
            x = -x;
            y = -y;
            z = -z;
            return this;
        }

        public Vector3F normalize()
        {
            return divideScalar(length());
        }

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
            var v = new Vector3F();
            v.copy(this).projectOnVector(planeNormal);
            return sub(v);
        }

        public Vector3F projectOnVector(Vector3F vector)
        {
            var v = new Vector3F();
            v.copy(vector).normalize();
            var scalar = dot(v);
            return copy(v).multiplyScalar(scalar);
        }

        public Vector3F reflect(Vector3F normal)
        {
            var vectorf = new Vector3F();
            return sub(vectorf.copy(normal).multiplyScalar(2f*dot(normal)));
        }

        public Vector3F round()
        {
            x = (float) Math.Round(x);
            y = (float) Math.Round(y);
            z = (float) Math.Round(z);
            return this;
        }

        public Vector3F roundToZero()
        {
            x = x < 0f ? (float) Math.Ceiling(x) : (float) Math.Floor(x);
            y = y < 0f ? (float) Math.Ceiling(y) : (float) Math.Floor(y);
            z = z < 0f ? (float) Math.Ceiling(z) : (float) Math.Floor(z);
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
                    x = value;
                    break;

                case 1:
                    y = value;
                    break;

                case 2:
                    z = value;
                    break;

                default:
                    throw new NotSupportedException("index is out of range: " + index);
            }
        }

        public Vector3F setFromMatrixColumn(int index, Matrix4F matrix)
        {
            var num = index*4;
            var elements = matrix.elements;
            x = elements[num];
            y = elements[num + 1];
            z = elements[num + 2];
            return this;
        }

        public Vector3F setFromMatrixPosition(Matrix4F m)
        {
            x = m.elements[12];
            y = m.elements[13];
            z = m.elements[14];
            return this;
        }

        public Vector3F setFromMatrixScale(Matrix4F m)
        {
            var num = set(m.elements[0], m.elements[1], m.elements[2]).length();
            var num2 = set(m.elements[4], m.elements[5], m.elements[6]).length();
            var num3 = set(m.elements[8], m.elements[9], m.elements[10]).length();
            x = num;
            y = num2;
            z = num3;
            return this;
        }

        public Vector3F setLength(float l)
        {
            var num = length();
            if ((num != 0f) && (l != num))
            {
                multiplyScalar(l/num);
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
            x -= v.x;
            y -= v.y;
            z -= v.z;
            return this;
        }

        public Vector3F subScalar(float s)
        {
            x -= s;
            y -= s;
            z -= s;
            return this;
        }

        public Vector3F subVectors(Vector3F a, Vector3F b)
        {
            x = a.x - b.x;
            y = a.y - b.y;
            z = a.z - b.z;
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
            array[offset] = x;
            array[offset + 1] = y;
            array[offset + 2] = z;
            return array;
        }

        public override string ToString()
        {
            return string.Concat("Viector3F(", x, ",", y, ",", z, ")");
        }

        public Vector3F transformDirection(Matrix4F m)
        {
            var x = this.x;
            var y = this.y;
            var z = this.z;
            var elements = m.elements;
            this.x = elements[0]*x + elements[4]*y + elements[8]*z;
            this.y = elements[1]*x + elements[5]*y + elements[9]*z;
            this.z = elements[2]*x + elements[6]*y + elements[10]*z;
            normalize();
            return this;
        }
    }
}