using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class FreeDrawingMeshMaker : MonoBehaviour
    {
        public const string kObjectTypeRobot = "robot";
        public const string kObjectTypeCar = "car";
        public const string kObjectTypeAirplane = "airplane";

        public static Mesh MakeMesh(string type, string boneName, Spline spline)
        {
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(boneName) || spline == null) return null;

            Mesh mesh = null;

            float extrude = 0.35f;
            float bevel = 0.35f;

            // change the spline boundary if needed
            if (type.Equals(kObjectTypeCar))
            {
                switch (boneName)
                {
                    case FreeDrawingCarBone.kWheelFLBone:
                    case FreeDrawingCarBone.kWheelFRBone:
                    case FreeDrawingCarBone.kWheelRLBone:
                    case FreeDrawingCarBone.kWheelRRBone:
                        spline.ChangeBoundary(-.5f, .5f, .5f, -.5f);
                        break;
                    case FreeDrawingCarBone.kBodyMomentumBone:
                        spline.ChangeBoundary(-1.75f, 1.75f, 1.25f, -.25f);
                        extrude = 1.0f;
                        break;
                    default:
                        spline.ChangeBoundary(-.5f, .5f, .5f, -.5f);
                        break;
                }
            }
            else if (type.Equals(kObjectTypeRobot))
            {
                bevel = 0.1f;
                switch (boneName)
                {
                    case FreeDrawingRobotBone.kHeadBone:
                        spline.ChangeBoundary(-0.4f, 0.4f, 0.7f, -0.05f);
                        extrude = 0.425f;
                        break;
                    case FreeDrawingRobotBone.kBodyBone:
                        spline.ChangeBoundary(-0.5f, 0.5f, 0.35f, -0.65f);
                        extrude = 0.35f;
                        break;
                    case FreeDrawingRobotBone.kArmLBone:
                    case FreeDrawingRobotBone.kArmRBone:
                        spline.ChangeBoundary(-0.35f, 0.35f, 0.3f, -0.6f);
                        extrude = 0.25f;
                        break;
                    case FreeDrawingRobotBone.kLegLBone:
                    case FreeDrawingRobotBone.kLegRBone:
                        spline.ChangeBoundary(-.25f, .25f, 0.0f, -0.7f);
                        extrude = 0.25f;
                        break;
                    default:
                        spline.ChangeBoundary(-0.5f, 0.5f, 0.5f, 0.0f);
                        break;
                }
            }
            else if (type.Equals(kObjectTypeAirplane))
            {
                bevel = 0.1f;
                switch (boneName)
                {
                    case FreeDrawingAirplaneBone.kBodyBone:
                        spline.ChangeBoundary(-0.3f, 0.3f, 1.0f, -1.25f);
                        extrude = 0.6f;
                        break;
                    case FreeDrawingAirplaneBone.kWingBone:
                        spline.ChangeBoundary(-1.2f, 1.2f, 0.6f, -0.6f);
                        extrude = 0.2f;
                        break;
                    case FreeDrawingAirplaneBone.kTailBone:
                        spline.ChangeBoundary(-0.25f, 0.25f, 0.6f, 0.0f);
                        extrude = 0.2f;
                        break;
                    case FreeDrawingAirplaneBone.kPropellerBone:
                        spline.ChangeBoundary(-.5f, .5f, 0.5f, -0.5f);
                        extrude = 0.1f;
                        break;
                    default:
                        spline.ChangeBoundary(-0.5f, 0.5f, 0.5f, 0.0f);
                        break;
                }
            }

            if (type.Equals(kObjectTypeRobot) && (boneName.Equals(FreeDrawingRobotBone.kBodyBone)))
            {
                mesh = Triangulation.Triangulate3D(spline, 5, true, extrude, bevel);
            }
            else
            {
                // triangulate
                TriangulationContext context = new TriangulationContext(spline);
                context.Triangulate(true);
                MeshGenerationInfo info = MeshGeneration.Generate(context, 4, 0.5f, 5, extrude);
                MeshGeneration.Smooth(info);

                // get mesh
                mesh = MeshGeneration.GetMesh(info);
            }

            return mesh;
        }

        public static GameObject MakeGameObject(string type, string boneName, Spline spline)
        {
            Mesh mesh = MakeMesh(type, boneName, spline);

            GameObject parts = new GameObject(boneName);
            parts.AddComponent<MeshFilter>().mesh = mesh;
            parts.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Custom/Brushed Bumped Specular"));
            parts.AddComponent<Template3D>();
            return parts;
        }

        public static GameObject MakeGameObject(string name, Mesh mesh)
        {
            GameObject parts = new GameObject(name);
            parts.AddComponent<MeshFilter>().mesh = mesh;
            parts.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Custom/Brushed Bumped Specular"));
            parts.AddComponent<Template3D>();
            return parts;
        }

        /// <summary>
        /// More convenient method for making parts of the free-drawing object
        /// </summary>
        /// <param name="bone">Free drawing object</param>
        /// <param name="boneName">Bone name</param>
        /// <param name="spline">Spline</param>
        public static void MakeFreeDrawingParts(BoneObject bone, string boneName, Spline spline)
        {
            if (bone == null || spline == null)
                return;

            // Car
            if (bone.GetType().Equals(typeof(FreeDrawingCarBone)))
            {
                // Body
                if (boneName.Equals(FreeDrawingCarBone.kBodyMomentumBone))
                {
                    bone.SetAccessory(
                        FreeDrawingCarBone.kBodyMomentumBone,
                        MakeGameObject(kObjectTypeCar, FreeDrawingCarBone.kBodyMomentumBone, spline),
                        true);
                }
                // Wheels
                else if (boneName.Equals(FreeDrawingCarBone.kWheelFLBone) ||
                        boneName.Equals(FreeDrawingCarBone.kWheelFRBone) ||
                        boneName.Equals(FreeDrawingCarBone.kWheelRLBone) ||
                        boneName.Equals(FreeDrawingCarBone.kWheelRRBone))
                {
                    Mesh mesh = MakeMesh(kObjectTypeCar, FreeDrawingCarBone.kWheelFLBone, spline);

                    GameObject wheelFL = MakeGameObject(FreeDrawingCarBone.kWheelFLBone, mesh);
                    GameObject wheelFR = MakeGameObject(FreeDrawingCarBone.kWheelFRBone, mesh);
                    GameObject wheelRL = MakeGameObject(FreeDrawingCarBone.kWheelRLBone, mesh);
                    GameObject wheelRR = MakeGameObject(FreeDrawingCarBone.kWheelRRBone, mesh);

                    wheelFL.GetComponent<Template3D>().ResizeBrushTexture(256, 256);
                    wheelFR.GetComponent<Template3D>().ResizeBrushTexture(256, 256);
                    wheelRL.GetComponent<Template3D>().ResizeBrushTexture(256, 256);
                    wheelRR.GetComponent<Template3D>().ResizeBrushTexture(256, 256);

                    bone.SetAccessory(FreeDrawingCarBone.kWheelFLBone, wheelFL, true);
                    bone.SetAccessory(FreeDrawingCarBone.kWheelFRBone, wheelFR, true);
                    bone.SetAccessory(FreeDrawingCarBone.kWheelRLBone, wheelRL, true);
                    bone.SetAccessory(FreeDrawingCarBone.kWheelRRBone, wheelRR, true);
                }
            }
            // Robot
            else if (bone.GetType().Equals(typeof(FreeDrawingRobotBone)))
            {
                // Head
                if (boneName.Equals(FreeDrawingRobotBone.kHeadBone))
                {
                    GameObject head = MakeGameObject(kObjectTypeRobot, FreeDrawingCarBone.kHeadBone, spline);
                    head.GetComponent<Template3D>().ResizeBrushTexture(128, 128);
                    bone.SetAccessory(FreeDrawingRobotBone.kHeadBone, head, true);
                }
                // Body
                else if (boneName.Equals(FreeDrawingRobotBone.kBodyBone))
                {
                    GameObject body = MakeGameObject(kObjectTypeRobot, FreeDrawingCarBone.kBodyBone, spline);
                    body.GetComponent<Template3D>().ResizeBrushTexture(256, 256);
                    bone.SetAccessory(FreeDrawingRobotBone.kBodyBone, body, true);
                }
                // Arms
                else if (boneName.Equals(FreeDrawingRobotBone.kArmLBone) ||
                        boneName.Equals(FreeDrawingRobotBone.kArmRBone))
                {
                    Mesh mesh = MakeMesh(kObjectTypeRobot, FreeDrawingRobotBone.kArmLBone, spline);
                    GameObject armL = MakeGameObject(FreeDrawingRobotBone.kArmLBone, mesh);
                    GameObject armR = MakeGameObject(FreeDrawingRobotBone.kArmRBone, mesh);
                    armL.GetComponent<Template3D>().ResizeBrushTexture(128, 128);
                    armR.GetComponent<Template3D>().ResizeBrushTexture(128, 128);
                    bone.SetAccessory(FreeDrawingRobotBone.kArmLBone, armL, true);
                    bone.SetAccessory(FreeDrawingRobotBone.kArmRBone, armR, true);
                }
                // Legs
                else if (boneName.Equals(FreeDrawingRobotBone.kLegLBone) ||
                        boneName.Equals(FreeDrawingRobotBone.kLegRBone))
                {
                    Mesh mesh = MakeMesh(kObjectTypeRobot, FreeDrawingRobotBone.kLegLBone, spline);
                    GameObject legL = MakeGameObject(FreeDrawingRobotBone.kLegLBone, mesh);
                    GameObject legR = MakeGameObject(FreeDrawingRobotBone.kLegRBone, mesh);
                    legL.GetComponent<Template3D>().ResizeBrushTexture(128, 128);
                    legR.GetComponent<Template3D>().ResizeBrushTexture(128, 128);
                    bone.SetAccessory(FreeDrawingRobotBone.kLegLBone, legL, true);
                    bone.SetAccessory(FreeDrawingRobotBone.kLegRBone, legR, true);
                }
            }
            // Airplane
            else if (bone.GetType().Equals(typeof(FreeDrawingAirplaneBone)))
            {
                // Body
                if (boneName.Equals(FreeDrawingAirplaneBone.kBodyBone))
                {
                    GameObject body = MakeGameObject(kObjectTypeAirplane, FreeDrawingAirplaneBone.kBodyBone, spline);
                    body.GetComponent<Template3D>().ResizeBrushTexture(256, 256);
                    bone.SetAccessory(FreeDrawingAirplaneBone.kBodyBone, body, true);
                }
                // Wing
                else if (boneName.Equals(FreeDrawingAirplaneBone.kWingBone))
                {
                    GameObject wing = MakeGameObject(kObjectTypeAirplane, FreeDrawingAirplaneBone.kWingBone, spline);
                    wing.GetComponent<Template3D>().ResizeBrushTexture(256, 256);
                    bone.SetAccessory(FreeDrawingAirplaneBone.kWingBone, wing, true);
                }
                // Tail
                else if (boneName.Equals(FreeDrawingAirplaneBone.kTailBone))
                {
                    GameObject tail = MakeGameObject(kObjectTypeAirplane, FreeDrawingAirplaneBone.kTailBone, spline);
                    tail.GetComponent<Template3D>().ResizeBrushTexture(128, 128);
                    bone.SetAccessory(FreeDrawingAirplaneBone.kTailBone, tail, true);
                }
                // Propeller
                else if (boneName.Equals(FreeDrawingAirplaneBone.kPropellerBone))
                {
                    GameObject propeller = MakeGameObject(kObjectTypeAirplane, FreeDrawingAirplaneBone.kPropellerBone, spline);
                    propeller.GetComponent<Template3D>().ResizeBrushTexture(128, 128);
                    bone.SetAccessory(FreeDrawingAirplaneBone.kPropellerBone, propeller, true);
                }
            }
        }

        /// <summary>
        /// More (more!) convenient method for 3D authoring tool.
        /// </summary>
        /// <param name="bone">Free drawing object</param>
        /// <param name="step">Current step in step panel</param>
        /// <param name="spline">Spline</param>
        public static void MakeFreeDrawingParts(BoneObject bone, int step, Spline spline)
        {
            if (bone == null) return;

            string boneName = null;

            if (bone.GetType().Equals(typeof(FreeDrawingCarBone)))
            {
                if (step == 0) boneName = FreeDrawingCarBone.kBodyMomentumBone;
                else if (step == 1) boneName = FreeDrawingCarBone.kWheelFLBone;
            }
            else if (bone.GetType().Equals(typeof(FreeDrawingRobotBone)))
            {
                if (step == 0) boneName = FreeDrawingRobotBone.kHeadBone;
                else if (step == 1) boneName = FreeDrawingRobotBone.kBodyBone;
                else if (step == 2) boneName = FreeDrawingRobotBone.kArmLBone;
                else if (step == 3) boneName = FreeDrawingRobotBone.kLegLBone;
            }
            else if (bone.GetType().Equals(typeof(FreeDrawingAirplaneBone)))
            {
                if (step == 0) boneName = FreeDrawingAirplaneBone.kBodyBone;
                else if (step == 1) boneName = FreeDrawingAirplaneBone.kWingBone;
                else if (step == 2) boneName = FreeDrawingAirplaneBone.kTailBone;
                else if (step == 3) boneName = FreeDrawingAirplaneBone.kPropellerBone;
            }

            if (boneName != null) MakeFreeDrawingParts(bone, boneName, spline);
        }
    }
}