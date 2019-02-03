using System;
using BIM.Lmv.Common.Pack;
using BIM.Lmv.Types;

namespace BIM.Lmv.Content.Geometry.Types
{
    public class Transform
    {
        private static readonly Matrix4F _MatrixIdentity = new Matrix4F().identity();
        private Matrix4F _Result;
        private Matrix4F _ResultEx;
        public Matrix4F matrix;
        public Vector4F rotation;
        public float scale;
        public Vector3D translation;
        public TransformType type;

        public Transform Clone()
        {
            var transform = new Transform
            {
                type = type,
                scale = scale
            };
            if (rotation != null)
            {
                transform.rotation = rotation.Clone();
            }
            if (translation != null)
            {
                transform.translation = translation.Clone();
            }
            if (matrix != null)
            {
                transform.matrix = matrix.clone();
            }
            if (_Result != null)
            {
                transform._Result = _Result.clone();
            }
            if (_ResultEx != null)
            {
                transform._ResultEx = _ResultEx.clone();
            }
            return transform;
        }

        public static Transform GetAffineMatrix(Matrix4F matrix, Vector3D translation)
        {
            return new Transform
            {
                type = TransformType.AffineMatrix,
                matrix = matrix,
                translation = translation
            };
        }

        public static Transform GetIdentity()
        {
            return new Transform {type = TransformType.Identity};
        }

        public Matrix4F GetMatrix()
        {
            if (_Result == null)
            {
                _Result = new Matrix4F();
                switch (type)
                {
                    case TransformType.Translation:
                        _Result.makeTranslation((float) translation.x, (float) translation.y, (float) translation.z);
                        break;

                    case TransformType.RotationTranslation:
                        _Result.compose(translation, rotation, new Vector3F(1f, 1f, 1f));
                        break;

                    case TransformType.UniformScaleRotationTranslation:
                        _Result.compose(translation, rotation, new Vector3F(scale, scale, scale));
                        break;

                    case TransformType.AffineMatrix:
                        _Result.copy(matrix);
                        _Result.setPosition(translation);
                        break;

                    case TransformType.Identity:
                        _Result.identity();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _ResultEx = _Result.clone().setPosition(new Vector3F(0f, 0f, 0f));
            }
            return _Result;
        }

        public Matrix4F GetMatrixEx()
        {
            if (_ResultEx == null)
            {
                GetMatrix();
            }
            return _ResultEx;
        }

        public static Transform GetRotationTranslation(Vector4F rotation, Vector3D translation)
        {
            return new Transform
            {
                type = TransformType.RotationTranslation,
                rotation = rotation,
                translation = translation
            };
        }

        public static Transform GetTranslation(Vector3D translation)
        {
            return new Transform
            {
                type = TransformType.Translation,
                translation = translation
            };
        }

        public static Transform GetUniformScaleRotationTranslation(float scale, Vector4F rotation, Vector3D translation)
        {
            return new Transform
            {
                type = TransformType.UniformScaleRotationTranslation,
                scale = scale,
                rotation = rotation,
                translation = translation
            };
        }

        private static bool IsIdentity(Matrix4F m)
        {
            for (var i = 0; i < 0x10; i++)
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
            for (var i = 0; i < 0x10; i++)
            {
                if ((i != 12) && (i != 13) && (i != 14) && !(_MatrixIdentity.elements[i] == m.elements[i]))
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
                if (type == TransformType.Identity)
                {
                    type = t.type;
                    scale = t.scale;
                    rotation = t.rotation == null ? null : t.rotation.Clone();
                    translation = t.translation == null ? null : t.translation.Clone();
                    this.matrix = t.matrix == null ? null : t.matrix.clone();
                    _Result = t._Result == null ? null : t._Result.clone();
                    return this;
                }
                var matrix = GetMatrix();
                matrix.multiply(t.GetMatrix());
                if (IsIdentity(matrix))
                {
                    type = TransformType.Identity;
                }
                else if (IsTranslation(matrix))
                {
                    type = TransformType.Translation;
                    translation = new Vector3D(matrix.elements[12], matrix.elements[13], matrix.elements[14]);
                }
                else
                {
                    type = TransformType.AffineMatrix;
                    translation = new Vector3D(matrix.elements[12], matrix.elements[13], matrix.elements[14]);
                    this.matrix = matrix.clone().setPosition(new Vector3D(0.0, 0.0, 0.0));
                }
                _Result = null;
                _ResultEx = null;
            }
            return this;
        }

        public Vector3F OfPoint(Vector3F point)
        {
            if (type == TransformType.Identity)
            {
                return point;
            }
            return GetMatrix().transformPoint(point);
        }

        public Vector3F OfPointEx(Vector3F point)
        {
            if (type == TransformType.Identity)
            {
                return point;
            }
            return GetMatrixEx().transformPoint(point);
        }

        public Vector3F OfVector(Vector3F vector)
        {
            return GetMatrix().transformDirection(vector);
        }

        internal static Transform Read(PackFileStreamReader ptr)
        {
            Vector3D vectord;
            Vector4F vectorf2;
            var stream = ptr.stream;
            var num = stream.getUint8();
            var globalOffset = new Vector3F(0f, 0f, 0f);
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
                    var scale = stream.getFloat32();
                    vectorf2 = ptr.ReadQuaternionF();
                    vectord = ptr.ReadVector3D(globalOffset);
                    return GetUniformScaleRotationTranslation(scale, vectorf2, vectord);
                }
                case 3:
                {
                    var m = new Matrix4F();
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
            var stream = ptr.stream;
            var num = stream.getUint8();
            var globalOffset = new Vector3F(0f, 0f, 0f);
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
                    var scale = stream.getFloat32();
                    vectorf2 = ptr.ReadQuaternionF();
                    vectord = ptr.ReadVector3D(globalOffset);
                    return GetUniformScaleRotationTranslation(scale, vectorf2, vectord);
                }
                case 3:
                {
                    var m = new Matrix4F();
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
            type = TransformType.AffineMatrix;
            this.matrix = matrix;
            this.translation = translation;
        }

        public void SetIdentity()
        {
            type = TransformType.Identity;
        }

        public void SetRotationTranslation(Vector4F rotation, Vector3D translation)
        {
            type = TransformType.RotationTranslation;
            this.rotation = rotation;
            this.translation = translation;
        }

        public void SetTranslation(Vector3D translation)
        {
            type = TransformType.Translation;
            this.translation = translation;
        }

        public void SetUniformScaleRotationTranslation(float scale, Vector4F rotation, Vector3D translation)
        {
            type = TransformType.UniformScaleRotationTranslation;
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