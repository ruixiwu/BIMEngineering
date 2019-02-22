namespace BIM.Lmv.Content.Geometry.Types
{
    using BIM.Lmv.Common.TypeArray;
    using System;
    using System.Runtime.InteropServices;

    public class Box3F
    {
        public Vector3F max;
        public Vector3F min;

        public Box3F()
        {
            this.min = new Vector3F();
            this.max = new Vector3F();
        }

        public Box3F(Vector3F min, Vector3F max)
        {
            this.min = min;
            this.max = max;
        }

        public Box3F applyMatrix4(Matrix4F matrix)
        {
            Vector3F[] points = new Vector3F[] { new Vector3F(), new Vector3F(), new Vector3F(), new Vector3F(), new Vector3F(), new Vector3F(), new Vector3F(), new Vector3F() };
            points[0].set(this.min.x, this.min.y, this.min.z).applyMatrix4(matrix);
            points[1].set(this.min.x, this.min.y, this.max.z).applyMatrix4(matrix);
            points[2].set(this.min.x, this.max.y, this.min.z).applyMatrix4(matrix);
            points[3].set(this.min.x, this.max.y, this.max.z).applyMatrix4(matrix);
            points[4].set(this.max.x, this.min.y, this.min.z).applyMatrix4(matrix);
            points[5].set(this.max.x, this.min.y, this.max.z).applyMatrix4(matrix);
            points[6].set(this.max.x, this.max.y, this.min.z).applyMatrix4(matrix);
            points[7].set(this.max.x, this.max.y, this.max.z).applyMatrix4(matrix);
            this.makeEmpty();
            this.setFromPoints(points);
            return this;
        }

        public Vector3F center(Vector3F optionalTarget = null)
        {
            Vector3F vectorf = optionalTarget ?? new Vector3F();
            return vectorf.addVectors(this.min, this.max).multiplyScalar(0.5f);
        }

        public Vector3F clampPoint(Vector3F point, Vector3F optionalTarget)
        {
            Vector3F vectorf = optionalTarget ?? new Vector3F();
            return vectorf.copy(point).clamp(this.min, this.max);
        }

        public Box3F clone() => 
            new Box3F().copy(this);

        public bool containsBox(Box3F box) => 
            (((((this.min.x <= box.min.x) && (box.max.x <= this.max.x)) && ((this.min.y <= box.min.y) && (box.max.y <= this.max.y))) && (this.min.z <= box.min.z)) && (box.max.z <= this.max.z));

        public bool containsPoint(Vector3F point)
        {
            if (((((point.x < this.min.x) || (point.x > this.max.x)) || ((point.y < this.min.y) || (point.y > this.max.y))) || (point.z < this.min.z)) || (point.z > this.max.z))
            {
                return false;
            }
            return true;
        }

        public Box3F copy(Box3F box)
        {
            this.min.copy(box.min);
            this.max.copy(box.max);
            return this;
        }

        internal void copyToArray(Float32Array array, int offset)
        {
            array[offset] = this.min.x;
            array[offset + 1] = this.min.y;
            array[offset + 2] = this.min.z;
            array[offset + 3] = this.max.x;
            array[offset + 4] = this.max.y;
            array[offset + 5] = this.max.z;
        }

        public float distanceToPoint(Vector3F point)
        {
            Vector3F vectorf = new Vector3F();
            return vectorf.copy(point).clamp(this.min, this.max).sub(point).length();
        }

        public bool empty() => 
            (((this.max.x < this.min.x) || (this.max.y < this.min.y)) || (this.max.z < this.min.z));

        public bool equals(Box3F box) => 
            (box.min.equals(this.min) && box.max.equals(this.max));

        public Box3F expandByPoint(Vector3F point)
        {
            this.min.min(point);
            this.max.max(point);
            return this;
        }

        public Box3F expandByScalar(float scalar)
        {
            this.min.addScalar(-scalar);
            this.max.addScalar(scalar);
            return this;
        }

        public Box3F expandByVector(Vector3F vector)
        {
            this.min.sub(vector);
            this.max.add(vector);
            return this;
        }

        public Vector3F getParameter(Vector3F point, Vector3F optionalTarget)
        {
            Vector3F vectorf = optionalTarget ?? new Vector3F();
            return vectorf.set((point.x - this.min.x) / (this.max.x - this.min.x), (point.y - this.min.y) / (this.max.y - this.min.y), (point.z - this.min.z) / (this.max.z - this.min.z));
        }

        public Box3F intersect(Box3F box)
        {
            this.min.max(box.min);
            this.max.min(box.max);
            return this;
        }

        public bool isIntersectionBox(Box3F box)
        {
            if (((((box.max.x < this.min.x) || (box.min.x > this.max.x)) || ((box.max.y < this.min.y) || (box.min.y > this.max.y))) || (box.max.z < this.min.z)) || (box.min.z > this.max.z))
            {
                return false;
            }
            return true;
        }

        public Box3F makeEmpty()
        {
            this.min.x = this.min.y = this.min.z = float.MaxValue;
            this.max.x = this.max.y = this.max.z = float.MinValue;
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
            this.min.x = array[offset];
            this.min.y = array[offset + 1];
            this.min.z = array[offset + 2];
            this.max.x = array[offset + 3];
            this.max.y = array[offset + 4];
            this.max.z = array[offset + 5];
            return this;
        }

        public Box3F setFromCenterAndSize(Vector3F center, Vector3F size)
        {
            Vector3F vectorf = new Vector3F();
            Vector3F v = vectorf.copy(size).multiplyScalar(0.5f);
            this.min.copy(center).sub(v);
            this.max.copy(center).add(v);
            return this;
        }

        public Box3F setFromPoints(Vector3F[] points)
        {
            this.makeEmpty();
            int length = points.Length;
            for (int i = 0; i < length; i++)
            {
                this.expandByPoint(points[i]);
            }
            return this;
        }

        public Vector3F size(Vector3F optionalTarget = null)
        {
            Vector3F vectorf = optionalTarget ?? new Vector3F();
            return vectorf.subVectors(this.max, this.min);
        }

        public Box3F translate(Vector3F offset)
        {
            this.min.add(offset);
            this.max.add(offset);
            return this;
        }

        public Box3F union(Box3F box)
        {
            this.min.min(box.min);
            this.max.max(box.max);
            return this;
        }
    }
}

