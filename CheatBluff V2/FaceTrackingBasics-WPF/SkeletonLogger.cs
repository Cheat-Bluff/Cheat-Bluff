using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace FaceTrackingBasics
{
    class SkeletonLogger
    {
        private String filePath;

        public SkeletonLogger(String path)
        {
            filePath = path;
        }

        public void AppendSkeletonString(String text)
        {
            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("Timestamp,");

                    sb.Append("Head X,");
                    sb.Append("Head Y,");
                    sb.Append("Head Z,");
                    sb.Append("ShoulderCenter X,");
                    sb.Append("ShoulderCenter Y,");
                    sb.Append("ShoulderCenter Z,");
                    sb.Append("ShoulderLeft X,");
                    sb.Append("ShoulderLeft Y,");
                    sb.Append("ShoulderLeft Z,");
                    sb.Append("ShoulderRight X,");
                    sb.Append("ShoulderRight Y,");
                    sb.Append("ShoulderRight Z,");

                    sb.Append("ElbowLeft X,");
                    sb.Append("ElbowLeft Y,");
                    sb.Append("ElbowLeft Z,");
                    sb.Append("WristLeft X,");
                    sb.Append("WristLeft Y,");
                    sb.Append("WristLeft Z,");
                    sb.Append("HandLeft X,");
                    sb.Append("HandLeft Y,");
                    sb.Append("HandLeft Z,");

                    sb.Append("ElbowRight X,");
                    sb.Append("ElbowRight Y,");
                    sb.Append("ElbowRight Z,");
                    sb.Append("WristRight X,");
                    sb.Append("WristRight Y,");
                    sb.Append("WristRight Z,");
                    sb.Append("HandRight X,");
                    sb.Append("HandRight Y,");
                    sb.Append("HandRight Z");

                    sw.WriteLine(sb.ToString());
                    sw.WriteLine(text);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(text);
                }
            }
        }

        public void AppendFaceString(String text)
        {
            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine(text);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(text);
                }
            }
        }

        public void AppendSkeleton(Skeleton skeleton)
        {
            SkeletonPoint head = skeleton.Joints[JointType.Head].Position;
            SkeletonPoint shoulderCenter = skeleton.Joints[JointType.ShoulderCenter].Position;
            SkeletonPoint shoulderLeft = skeleton.Joints[JointType.ShoulderLeft].Position;
            SkeletonPoint shoulderRight = skeleton.Joints[JointType.ShoulderRight].Position;

            SkeletonPoint elbowLeft = skeleton.Joints[JointType.ElbowLeft].Position;
            SkeletonPoint wristLeft = skeleton.Joints[JointType.WristLeft].Position;
            SkeletonPoint handLeft = skeleton.Joints[JointType.HandLeft].Position;

            SkeletonPoint elbowRight = skeleton.Joints[JointType.ElbowRight].Position;
            SkeletonPoint wristRight = skeleton.Joints[JointType.WristRight].Position;
            SkeletonPoint handRight = skeleton.Joints[JointType.HandRight].Position;

            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append(head.X + "," + head.Y + "," + head.Z + ",");
            sb.Append(shoulderCenter.X + "," + shoulderCenter.Y + "," + shoulderCenter.Z + ",");
            sb.Append(shoulderLeft.X + "," + shoulderLeft.Y + "," + shoulderLeft.Z + ",");
            sb.Append(shoulderRight.X + "," + shoulderRight.Y + "," + shoulderRight.Z + ",");

            sb.Append(elbowLeft.X + "," + elbowLeft.Y + "," + elbowLeft.Z + ",");
            sb.Append(wristLeft.X + "," + wristLeft.Y + "," + wristLeft.Z + ",");
            sb.Append(handLeft.X + "," + handLeft.Y + "," + handLeft.Z + ",");

            sb.Append(elbowRight.X + "," + elbowRight.Y + "," + elbowRight.Z + ",");
            sb.Append(wristRight.X + "," + wristRight.Y + "," + wristRight.Z + ",");
            sb.Append(handRight.X + "," + handRight.Y + "," + handRight.Z);

            AppendSkeletonString(sb.ToString());
        }

        public void AppendFaceSkeleton(List<Point> facePts)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            for (int i = 0; i < facePts.Count-1; i++)
            {
                sb.Append(facePts[i].X + "," + facePts[i].Y + ",");
            }
            sb.Append(facePts[facePts.Count - 1].X + "," + facePts[facePts.Count - 1].Y);

            AppendFaceString(sb.ToString());
        }
    }
}
