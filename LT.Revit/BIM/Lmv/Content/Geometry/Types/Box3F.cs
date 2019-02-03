using BIM.Lmv.Common.TypeArray;

namespace BIM.Lmv.Content.Geometry.Types
{
    public class Box3F
    {
        public Vector3F max;
        public Vector3F min;

        public Box3F()
        {
            min = new Vector3F();
            max = new Vector3F();
        }

        public Box3F(Vector3F min, Vector3F max)
        {
            this.min = min;
            this.max = max;
        }

        public Box3F applyMatrix4(Matrix4F matrix)
        {
            Vector3F[] points =
            {
                new Vector3F(), new Vector3F(), new Vector3F(), new Vector3F(), new Vector3F(),
                new Vector3F(), new Vector3F(), new Vector3F()
            };
            points[0].set(min.x, min.y, min.z).applyMatrix4(matrix);
            points[1].set(min.x, min.y, max.z).applyMatrix4(matrix);
            points[2].set(min.x, max.y, min.z).applyMatrix4(matrix);
            points[3].set(min.x, max.y, max.z).applyMatrix4(matrix);
            points[4].set(max.x, min.y, min.z).applyMatrix4(matrix);
            points[5].set(max.x, min.y, max.z).applyMatrix4(matrix);
            points[6].set(max.x, max.y, min.z).applyMatrix4(matrix);
            points[7].set(max.x, max.y, max.z).applyMatrix4(matrix);
            makeEmpty();
            setFromPoints(points);
            return this;
        }

        public Vector3F center(Vector3F optionalTarget = null)
        {
            var vectorf = optionalTarget ?? new Vector3F();
            return vectorf.addVectors(min, max).multiplyScalar(0.5f);
        }

        public Vector3F clampPoint(Vector3F point, Vector3F optionalTarget)
        {
            var vectorf = optionalTarget ?? new Vector3F();
            return vectorf.copy(point).clamp(min, max);
        }

        public Box3F clone()
        {
            return new Box3F().copy(this);
        }

        public bool containsBox(Box3F box)
        {
            return (min.x <= box.min.x) && (box.max.x <= max.x) && (min.y <= box.min.y) && (box.max.y <= max.y) &&
                   (min.z <= box.min.z) && (box.max.z <= max.z);
        }

        public bool containsPoint(Vector3F point)
        {
            if ((point.x < min.x) || (point.x > max.x) || (point.y < min.y) || (point.y > max.y) || (point.z < min.z) ||
                (point.z > max.z))
            {
                return false;
            }
            return true;
        }

        public Box3F copy(Box3F box)
        {
            min.copy(box.min);
            max.copy(box.max);
            return this;
        }

        internal void copyToArray(Float32Array array, int offset)
        {
            array[offset] = min.x;
            array[offset + 1] = min.y;
            array[offset + 2] = min.z;
            array[offset + 3] = max.x;
            array[offset + 4] = max.y;
            array[offset + 5] = max.z;
        }

        public float distanceToPoint(Vector3F point)
        {
            var vectorf = new Vector3F();
            return vectorf.copy(point).clamp(min, max).sub(point).length();
        }

        public bool empty()
        {
            return (max.x < min.x) || (max.y < min.y) || (max.z < min.z);
        }

        public bool equals(Box3F box)
        {
            return box.min.@equals(min) && box.max.@equals(max);
        }

        public Box3F expandByPoint(Vector3F point)
        {
            min.min(point);
            max.max(point);
            return this;
        }

        public Box3F expandByScalar(float scalar)
        {
            min.addScalar(-scalar);
            max.addScalar(scalar);
            return this;
        }

        public Box3F expandByVector(Vector3F vector)
        {
            min.sub(vector);
            max.add(vector);
            return this;
        }

        public Vector3F getParameter(Vector3F point, Vector3F optionalTarget)
        {
            var vectorf = optionalTarget ?? new Vector3F();
            return vectorf.set((point.x - min.x)/(max.x - min.x), (point.y - min.y)/(max.y - min.y),
                (point.z - min.z)/(max.z - min.z));
        }

        public Box3F intersect(Box3F box)
        {
            min.max(box.min);
            max.min(box.max);
            return this;
        }

        public bool isIntersectionBox(Box3F box)
        {
            if ((box.max.x < min.x) || (box.min.x > max.x) || (box.max.y < min.y) || (box.min.y > max.y) ||
                (box.max.z < min.z) || (box.min.z > max.z))
            {
                return false;
            }
            return true;
        }

        public Box3F makeEmpty()
        {
            min.x = min.y = min.z = float.MaxValue;
            max.x = max.y = max.z = float.MinValue;
            return this;
        }

        public Box3F set(Vector3F min, Vector3F max)
        {
            this.min.copy(min);
            this.max.copy(max);
            return this;
        }

        internal Box3F setFromArray(Float32Array array, int offset)
        {
            min.x = array[offset];
            min.y = array[offset + 1];
            min.z = array[offset + 2];
            max.x = array[offset + 3];
            max.y = array[offset + 4];
            max.z = array[offset + 5];
            return this;
        }

        public Box3F setFromCenterAndSize(Vector3F center, Vector3F size)
        {
            var vectorf = new Vector3F();
            var v = vectorf.copy(size).multiplyScalar(0.5f);
            min.copy(center).sub(v);
            max.copy(center).add(v);
            return this;
        }

        public Box3F setFromPoints(Vector3F[] points)
        {
            makeEmpty();
            var length = points.Length;
            for (var i = 0; i < length; i++)
            {
                expandByPoint(points[i]);
            }
            return this;
        }

        public Vector3F size(Vector3F optionalTarget = null)
        {
            var vectorf = optionalTarget ?? new Vector3F();
            return vectorf.subVectors(max, min);
        }

        public Box3F translate(Vector3F offset)
        {
            min.add(offset);
            max.add(offset);
            return this;
        }

        public Box3F union(Box3F box)
        {
            min.min(box.min);
            max.max(box.max);
            return this;
        }
    }
}