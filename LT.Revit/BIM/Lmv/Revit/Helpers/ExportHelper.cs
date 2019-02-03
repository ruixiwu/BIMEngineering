using System;
using Autodesk.Revit.DB;
using BIM.Lmv.Content.Geometry.Types;
using BIM.Lmv.Types;
using CameraInfo = BIM.Lmv.Types.CameraInfo;

namespace BIM.Lmv.Revit.Helpers
{
    internal static class ExportHelper
    {
        private static void AdjustCamera(CameraInfo camera, Box3F bounds)
        {
            var num = 2.0*Math.Atan(0.5)*180.0/3.1415926535897931;
            var position = camera.Position;
            var target = camera.Target;
            if (!bounds.empty())
            {
                var vectorf3 = bounds.center(null);
                var vectorf4 = bounds.size(null);
                var num2 = 0.5*
                           Math.Sqrt(
                               vectorf4.x*vectorf4.x + vectorf4.y*vectorf4.y + vectorf4.z*vectorf4.z);
                if (Math.Abs(num2) <= 1E-06)
                {
                    num2 = 1.0;
                }
                var num3 = Math.Max(1.0, 0.9 + num/100.0*0.5);
                num2 *= num3;
                var v = position.clone().sub(target).normalize();
                var num4 = num2/Math.Tan(0.017453292519943295*num*0.5);
                v.multiplyScalar((float) num4);
                var vectorf6 = vectorf3.clone().add(v);
                camera.Target = vectorf3;
                camera.Position = vectorf6;
            }
        }

        public static CameraInfo GetCameraInfo(View3D view, Autodesk.Revit.DB.CameraInfo cameraInfo,
            Vector3F boxMin, Vector3F boxMax)
        {
            var camera = new CameraInfo();
            var num = cameraInfo.HorizontalExtent/cameraInfo.VerticalExtent;
            var orientation = view.GetOrientation();
            camera.IsPerspective = view.IsPerspective;
            camera.Position = orientation.EyePosition.Convert();
            camera.Up = orientation.UpDirection.Convert();
            camera.Aspect = (float) num;
            camera.Fov = 0f;
            var num2 = view.get_Parameter(BuiltInParameter.VIEWER_TARGET_ELEVATION).AsDouble();
            if (Math.Abs(view.get_Parameter(BuiltInParameter.VIEWER_EYE_ELEVATION).AsDouble() - num2) <
                0.0001)
            {
                camera.Target = new Vector3F((boxMax.x - boxMin.x)/2f + boxMin.x,
                    (boxMax.y - boxMin.y)/2f + boxMin.y, (float) num2);
            }
            else
            {
                var num4 = (orientation.EyePosition.Z - num2)/orientation.ForwardDirection.Z;
                var num5 = orientation.EyePosition.X - num4*orientation.ForwardDirection.X;
                var num6 = orientation.EyePosition.Y - num4*orientation.ForwardDirection.Y;
                var num7 = num2;
                camera.Target = new Vector3F((float) num5, (float) num6, (float) num7);
                AdjustCamera(camera, new Box3F(boxMin, boxMax));
            }
            camera.OrthoScale = camera.Position.distanceTo(camera.Target);
            return camera;
        }

        public static SceneInfo GetSceneInfo(Document doc)
        {
            var activeProjectLocation = doc.ActiveProjectLocation;
            var info = new SceneInfo
            {
                DistanceUnit = "foot"
            };
            if (activeProjectLocation == null)
            {
                info.Longitude = 0.0;
                info.Latitude = 0.0;
                info.AngleToTrueNorth = 0.0;
                return info;
            }
            info.Longitude = activeProjectLocation.SiteLocation.Longitude*180.0/3.1415926535897931;
            info.Latitude = activeProjectLocation.SiteLocation.Latitude*180.0/3.1415926535897931;
            info.AngleToTrueNorth = activeProjectLocation.get_ProjectPosition(new XYZ(0.0, 0.0, 0.0)).Angle;
            return info;
        }
    }
}