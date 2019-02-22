namespace BIM.Lmv.Revit.Helpers
{
    using Autodesk.Revit.DB;
    using BIM.Lmv.Content.Geometry.Types;
    using BIM.Lmv.Types;
    using System;

    internal static class ExportHelper
    {
        private static void AdjustCamera(BIM.Lmv.Types.CameraInfo camera, Box3F bounds)
        {
            double num = ((2.0 * Math.Atan(0.5)) * 180.0) / 3.1415926535897931;
            Vector3F position = camera.Position;
            Vector3F target = camera.Target;
            if (!bounds.empty())
            {
                Vector3F vectorf3 = bounds.center(null);
                Vector3F vectorf4 = bounds.size(null);
                double num2 = 0.5 * Math.Sqrt((double) (((vectorf4.x * vectorf4.x) + (vectorf4.y * vectorf4.y)) + (vectorf4.z * vectorf4.z)));
                if (Math.Abs(num2) <= 1E-06)
                {
                    num2 = 1.0;
                }
                double num3 = Math.Max((double) 1.0, (double) (0.9 + ((num / 100.0) * 0.5)));
                num2 *= num3;
                Vector3F v = position.clone().sub(target).normalize();
                double num4 = num2 / Math.Tan((0.017453292519943295 * num) * 0.5);
                v.multiplyScalar((float) num4);
                Vector3F vectorf6 = vectorf3.clone().add(v);
                camera.Target = vectorf3;
                camera.Position = vectorf6;
            }
        }

        public static BIM.Lmv.Types.CameraInfo GetCameraInfo(View3D view, Autodesk.Revit.DB.CameraInfo cameraInfo, Vector3F boxMin, Vector3F boxMax)
        {
            BIM.Lmv.Types.CameraInfo camera = new BIM.Lmv.Types.CameraInfo();
            double num = cameraInfo.HorizontalExtent / cameraInfo.VerticalExtent;
            ViewOrientation3D orientation = view.GetOrientation();
            camera.IsPerspective = view.IsPerspective;
            camera.Position = orientation.EyePosition.Convert();
            camera.Up = orientation.UpDirection.Convert();
            camera.Aspect = (float) num;
            camera.Fov = 0f;
            double num2 = view.get_Parameter(BuiltInParameter.VIEWER_TARGET_ELEVATION).AsDouble();
            if (Math.Abs((double) (view.get_Parameter(BuiltInParameter.VIEWER_EYE_ELEVATION).AsDouble() - num2)) < 0.0001)
            {
                camera.Target = new Vector3F(((boxMax.x - boxMin.x) / 2f) + boxMin.x, ((boxMax.y - boxMin.y) / 2f) + boxMin.y, (float) num2);
            }
            else
            {
                double num4 = (orientation.EyePosition.Z - num2) / orientation.ForwardDirection.Z;
                double num5 = orientation.EyePosition.X - (num4 * orientation.ForwardDirection.X);
                double num6 = orientation.EyePosition.Y - (num4 * orientation.ForwardDirection.Y);
                double num7 = num2;
                camera.Target = new Vector3F((float) num5, (float) num6, (float) num7);
                AdjustCamera(camera, new Box3F(boxMin, boxMax));
            }
            camera.OrthoScale = camera.Position.distanceTo(camera.Target);
            return camera;
        }

        public static SceneInfo GetSceneInfo(Document doc)
        {
            ProjectLocation activeProjectLocation = doc.ActiveProjectLocation;
            SceneInfo info = new SceneInfo {
                DistanceUnit = "foot"
            };
            if (activeProjectLocation == null)
            {
                info.Longitude = 0.0;
                info.Latitude = 0.0;
                info.AngleToTrueNorth = 0.0;
                return info;
            }
            info.Longitude = (activeProjectLocation.SiteLocation.Longitude * 180.0) / 3.1415926535897931;
            info.Latitude = (activeProjectLocation.SiteLocation.Latitude * 180.0) / 3.1415926535897931;
            info.AngleToTrueNorth = activeProjectLocation.get_ProjectPosition(new XYZ(0.0, 0.0, 0.0)).Angle;
            return info;
        }
    }
}

