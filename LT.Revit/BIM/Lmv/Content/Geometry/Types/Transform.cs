namespace BIM.Lmv.Content.Geometry.Types
{
    using BIM.Lmv.Common.Pack;
    using BIM.Lmv.Types;
    using System;

    public class Transform
    {
        private static Matrix4F _MatrixIdentity = new Matrix4F().identity();
        private Matrix4F _Result;
        private Matrix4F _ResultEx;
        public Matrix4F matrix;
        public Vector4F rotation;
        public float scale;
        public Vector3D translation;
        public TransformType type;

        public Transform Clone()
        {
            Transform transform = new Transform {
                type = this.type,
                scale = this.scale
            };
            if (this.rotation != null)
            {
                transform.rotation = this.rotation.Clone();
            }
            if (this.translation != null)
            {
                transform.translation = this.translation.Clone();
            }
            if (this.matrix != null)
            {
                transform.matrix = this.matrix.clone();
            }
            if (this._Result != null)
            {
                transform._Result = this._Result.clone();
            }
            if (this._ResultEx != null)
            {
                transform._ResultEx = this._ResultEx.clone();
            }
            return transform;
        }

        public static Transform GetAffineMatrix(Matrix4F matrix, Vector3D translation) => 
            new Transform { 
                type = TransformType.AffineMatrix,
                matrix = matrix,
                translation = translation
            };

        public static Transform GetIdentity() => 
            new Transform { type = TransformType.Identity };

        public Matrix4F GetMatrix()
        {
            if (this._Result == null)
            {
                this._Result = new Matrix4F();
                switch (this.type)
                {
                    case TransformType.Translation:
                        this._Result.makeTranslation((float) this.translation.x, (float) this.translation.y, (float) this.translation.z);
                        break;

                    case TransformType.RotationTranslation:
                        this._Result.compose(this.translation, this.rotation, new Vector3F(1f, 1f, 1f));
                        break;

                    case TransformType.UniformScaleRotationTranslation:
                        this._Result.compose(this.translation, this.rotation, new Vector3F(this.scale, this.scale, this.scale));
                        break;

                    case TransformType.AffineMatrix:
                        this._Result.copy(this.matrix);
                        this._Result.setPosition(this.translation);
                        break;

                    case TransformType.Identity:
                        this._Result.identity();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                this._ResultEx = this._Result.clone().setPosition(new Vector3F(0f, 0f, 0f));
            }
            return this._Result;
        }

        public Matrix4F GetMatrixEx()
        {
            if (this._ResultEx == null)
            {
                this.GetMatrix();
            }
            return this._ResultEx;
        }

        public static Transform GetRotationTranslation(Vector4F rotation, Vector3D translation) => 
            new Transform { 
                type = TransformType.RotationTranslation,
                rotation = rotation,
                translation = translation
            };

        public static Transform GetTranslation(Vector3D translation) => 
            new Transform { 
                type = TransformType.Translation,
                translation = translation
            };

        public static Transform GetUniformScaleRotationTranslation(float scale, Vector4F rotation, Vector3D translation) => 
            new Transform { 
                type = TransformType.UniformScaleRotationTranslation,
                scale = scale,
                rotation = rotation,
                translation = translation
            };

        private static bool IsIdentity(Matrix4F m)
        {
            for (int i = 0; i < 0x10; i++)
            {
                if (!(_MatrixIdentity.elements[i] == m.elements[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsTranslation(Matrix4F m)
        {
            for (int i = 0; i < 0x10; i++)
            {
                if ((((i != 12) && (i != 13)) && (i != 14)) && !(_MatrixIdentity.elements[i] == m.elements[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public Transform Multiply(Transform t)
        {
            if (t.type != TransformType.Identity)
            {
                if (this.type == TransformType.Identity)
                {
                    this.type = t.type;
                    this.scale = t.scale;
                    this.rotation = t.rotation?.Clone();
                    this.translation = t.translation?.Clone();
                    this.matrix = t.matrix?.clone();
                    this._Result = t._Result?.clone();
                    return this;
                }
                Matrix4F matrix = this.GetMatrix();
                matrix.multiply(t.GetMatrix());
                if (IsIdentity(matrix))
                {
                    this.type = TransformType.Identity;
                }
                else if (IsTranslation(matrix))
                {
                    this.type = TransformType.Translation;
                    this.translation = new Vector3D((double) matrix.elements[12], (double) matrix.elements[13], (double) matrix.elements[14]);
                }
                else
                {
                    this.type = TransformType.AffineMatrix;
                    this.translation = new Vector3D((double) matrix.elements[12], (double) matrix.elements[13], (double) matrix.elements[14]);
                    this.matrix = matrix.clone().setPosition(new Vector3D(0.0, 0.0, 0.0));
                }
                this._Result = null;
                this._ResultEx = null;
            }
            return this;
        }

        public Vector3F OfPoint(Vector3F point)
        {
            if (this.type == TransformType.Identity)
            {
                return point;
            }
            return this.GetMatrix().transformPoint(point);
        }

        public Vector3F OfPointEx(Vector3F point)
        {
            if (this.type == TransformType.Identity)
            {
                return point;
            }
            return this.GetMatrixEx().transformPoint(point);
        }

        public Vector3F OfVector(Vector3F vector) => 
            this.GetMatrix().transformDirection(vector);

        internal static Transform Read(PackFileStreamReader ptr)
        {
            Vector3D vectord;
            Vector4F vectorf2;
            PackFileStream stream = ptr.stream;
            byte num = stream.getUint8();
            Vector3F globalOffset = new Vector3F(0f, 0f, 0f);
            switch (num)
            {
                case 0:
                    return GetTranslation(ptr.ReadVector3D(globalOffset));

                case 1:
                    vectorf2 = ptr.ReadQuaternionF();
                    vectord = ptr.ReadVector3D(globalOffset);
                    return GetRotationTranslation(vectorf2, vectord);

                case 2:
                {
                    float scale = stream.getFloat32();
                    vectorf2 = ptr.ReadQuaternionF();
                    vectord = ptr.ReadVector3D(globalOffset);
                    return GetUniformScaleRotationTranslation(scale, vectorf2, vectord);
                }
                case 3:
                {
                    Matrix4F m = new Matrix4F();
                    ptr.ReadMatrix3F(m);
                    vectord = ptr.ReadVector3D(globalOffset);
                    return GetAffineMatrix(m, vectord);
                }
                case 4:
                    return GetIdentity();
            }
            throw new NotSupportedException("transformType = " + num);
        }

        internal static Transform Read(PackFileStreamWriter ptr)
        {
            Vector3D vectord;
            Vector4F vectorf2;
            PackFileStream stream = ptr.stream;
            byte num = stream.getUint8();
            Vector3F globalOffset = new Vector3F(0f, 0f, 0f);
            switch (num)
            {
                case 0:
                    return GetTranslation(ptr.ReadVector3D(globalOffset));

                case 1:
                    vectorf2 = ptr.ReadQuaternionF();
                    vectord = ptr.ReadVector3D(globalOffset);
                    return GetRotationTranslation(vectorf2, vectord);

                case 2:
                {
                    float scale = stream.getFloat32();
                    vectorf2 = ptr.ReadQuaternionF();
                    vectord = ptr.ReadVector3D(globalOffset);
                    return GetUniformScaleRotationTranslation(scale, vectorf2, vectord);
                }
                case 3:
                {
                    Matrix4F m = new Matrix4F();
                    ptr.ReadMatrix3F(m);
                    vectord = ptr.ReadVector3D(globalOffset);
                    return GetAffineMatrix(m, vectord);
                }
                case 4:
                    return GetIdentity();
            }
            throw new NotSupportedException("transformType = " + num);
        }

        public void SetAffineMatrix(Matrix4F matrix, Vector3D translation)
        {
            this.type = TransformType.AffineMatrix;
            this.matrix = matrix;
            this.translation = translation;
        }

        public void SetIdentity()
        {
            this.type = TransformType.Identity;
        }

        public void SetRotationTranslation(Vector4F rotation, Vector3D translation)
        {
            this.type = TransformType.RotationTranslation;
            this.rotation = rotation;
            this.translation = translation;
        }

        public void SetTranslation(Vector3D translation)
        {
            this.type = TransformType.Translation;
            this.translation = translation;
        }

        public void SetUniformScaleRotationTranslation(float scale, Vector4F rotation, Vector3D translation)
        {
            this.type = TransformType.UniformScaleRotationTranslation;
            this.scale = scale;
            this.rotation = rotation;
            this.translation = translation;
        }

        internal static bool Write(PackFileStreamWriter ptw, Transform transform)
        {
            ptw.stream.Write((byte) transform.type);
            switch (transform.type)
            {
                case TransformType.Translation:
                    ptw.WriteVector3D(transform.translation);
                    break;

                case TransformType.RotationTranslation:
                    ptw.WriteQuaternionF(transform.rotation);
                    ptw.WriteVector3D(transform.translation);
                    break;

                case TransformType.UniformScaleRotationTranslation:
                    ptw.stream.Write(transform.scale);
                    ptw.WriteQuaternionF(transform.rotation);
                    ptw.WriteVector3D(transform.translation);
                    break;

                case TransformType.AffineMatrix:
                    ptw.WriteMatrix3F(transform.matrix);
                    ptw.WriteVector3D(transform.translation);
                    break;

                case TransformType.Identity:
                    break;

                default:
                    throw new NotSupportedException(transform.type.ToString());
            }
            return true;
        }
    }
}

