/**
 * @file CommonTransform.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Common Transform for Maid Robot Simulator.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

#define UNITY_DRAW_ON_ANIMATOR_IK
//#define UNITY_DRAW_AT_LATE_UPDATE

using UnityEngine;
using UniHumanoid;
using System;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class CommonTransform
    {
        public CommonTransform()
        {

        }

        public static void euler_angle_to_array(
            SystemStructure.ST_EULER_ANGLE euler_angle,
            ref float[] array)
        {
            array[0] = euler_angle.roll;
            array[1] = euler_angle.pitch;
            array[2] = euler_angle.yaw;
        }

        public static void array_to_euler_angle(
            float[] array,
            ref SystemStructure.ST_EULER_ANGLE euler_angle)
        {
            euler_angle.roll = array[0];
            euler_angle.pitch = array[1];
            euler_angle.yaw = array[2];
        }

        public static void vector3_to_array(
            Vector3 vector3, ref float[] array)
        {
            array[0] = vector3.x;
            array[1] = vector3.y;
            array[2] = vector3.z;
        }

        public static void array_to_vector3(
            float[] array, ref Vector3 vector3)
        {
            vector3.x = array[0];
            vector3.y = array[1];
            vector3.z = array[2];
        }

        public static void quaternion_to_array(Quaternion q, ref float[] array)
        {
            array[0] = q.x;
            array[1] = q.y;
            array[2] = q.z;
            array[3] = q.w;
        }

        public static void array_to_quaternion(float[] array, ref Quaternion q)
        {
            q.x = array[0];
            q.y = array[1];
            q.z = array[2];
            q.w = array[3];
        }

        public static void update_orientation(Vector3 position_in, Quaternion rotation_in,
            ref Vector3 position_out, ref Quaternion rotation_out, ref Vector3 euler_angle_out)
        {
            position_out = CommonParameter.AXIS_UNITY_TO_ROBOT * position_in;
            position_out = Vector3.Scale(CommonParameter.AXIS_RIGHT_TO_LEFT, position_out);

            Quaternion temp_q;
            temp_q = CommonParameter.AXIS_REMOVE_OFFSET_Q * rotation_in;
            rotation_out.w = temp_q.w;
            rotation_out.x = -temp_q.z;
            rotation_out.y = temp_q.x;
            rotation_out.z = -temp_q.y;

            euler_angle_out.x = Mathf.Repeat(rotation_out.eulerAngles.x + CommonParameter.HALF_CIRCLE_DEG, CommonParameter.CIRCLE_DEG)
                 - CommonParameter.HALF_CIRCLE_DEG;
            euler_angle_out.y = Mathf.Repeat(rotation_out.eulerAngles.y + CommonParameter.HALF_CIRCLE_DEG, CommonParameter.CIRCLE_DEG)
                 - CommonParameter.HALF_CIRCLE_DEG;
            euler_angle_out.z = Mathf.Repeat(rotation_out.eulerAngles.z + CommonParameter.HALF_CIRCLE_DEG, CommonParameter.CIRCLE_DEG)
                 - CommonParameter.HALF_CIRCLE_DEG;
        }

        public static void transform_quaternion_to_euler_degree(
            Quaternion rotation_in, ref Vector3 euler_angle_out)
        {
            euler_angle_out.x = Mathf.Repeat(rotation_in.eulerAngles.x + CommonParameter.HALF_CIRCLE_DEG, CommonParameter.CIRCLE_DEG)
                 - CommonParameter.HALF_CIRCLE_DEG;
            euler_angle_out.y = Mathf.Repeat(rotation_in.eulerAngles.y + CommonParameter.HALF_CIRCLE_DEG, CommonParameter.CIRCLE_DEG)
                 - CommonParameter.HALF_CIRCLE_DEG;
            euler_angle_out.z = Mathf.Repeat(rotation_in.eulerAngles.z + CommonParameter.HALF_CIRCLE_DEG, CommonParameter.CIRCLE_DEG)
                 - CommonParameter.HALF_CIRCLE_DEG;
        }

        public static void transform_relative_rotation_from_unity_to_robot(Quaternion rotation_unity,
            ref Quaternion rotation_robot)
        {
            rotation_robot.w = rotation_unity.w;
            rotation_robot.x = -rotation_unity.z;
            rotation_robot.y = rotation_unity.x;
            rotation_robot.z = -rotation_unity.y;
        }

        public static void transform_relative_rotation_from_robot_to_unity(Quaternion rotation_robot,
            ref Quaternion rotation_unity)
        {
            rotation_unity.w = rotation_robot.w;
            rotation_unity.x = rotation_robot.y;
            rotation_unity.y = -rotation_robot.z;
            rotation_unity.z = -rotation_robot.x;
        }

        public static void transform_position_from_unity_to_robot(Vector3 position_unity, ref Vector3 position_robot)
        {
            position_robot = CommonParameter.AXIS_UNITY_TO_ROBOT * position_unity;
            position_robot = Vector3.Scale(CommonParameter.AXIS_RIGHT_TO_LEFT, position_robot);
        }

        public static void transform_position_from_robot_to_unity(Vector3 position_robot, ref Vector3 position_unity)
        {
            position_unity = Vector3.Scale(CommonParameter.AXIS_LEFT_TO_RIGHT, position_robot);
            position_unity = CommonParameter.AXIS_ROBOT_TO_UNITY * position_unity;
        }

        public static void draw_hip_rotation(Quaternion hip_rotation,
            Quaternion robot_rotation_unity_axis, ref GameObject robot_GameObject)
        {
#if UNITY_DRAW_ON_ANIMATOR_IK
            Animator animator = robot_GameObject.GetComponent<Animator>();
            draw_hip_rotation(hip_rotation, robot_rotation_unity_axis, ref animator);
#elif UNITY_DRAW_AT_LATE_UPDATE
            Humanoid humanoid_component = robot_GameObject.GetComponent<Humanoid>();
            draw_hip_rotation(hip_rotation, robot_rotation_unity_axis, ref humanoid_component);
#else
            throw new Exception("Abnormal draw timing.");
#endif
        }

        public static void draw_hip_rotation(Quaternion hip_rotation,
            Quaternion robot_rotation_unity_axis, ref Humanoid humanoid_component)
        {
            Quaternion relative_rotation_from_root = Quaternion.identity;
            transform_relative_rotation_from_robot_to_unity(hip_rotation, ref relative_rotation_from_root);

            humanoid_component.Hips.rotation =
                robot_rotation_unity_axis * relative_rotation_from_root;
        }

        public static void draw_hip_rotation(Quaternion hip_rotation,
            Quaternion robot_rotation_unity_axis, ref Animator animator)
        {
            Quaternion relative_rotation_from_root = Quaternion.identity;
            transform_relative_rotation_from_robot_to_unity(hip_rotation, ref relative_rotation_from_root);

            animator.SetBoneLocalRotation(HumanBodyBones.Hips,
                relative_rotation_from_root);
        }

        public static void draw_robot_bone_rotation(HumanBodyBones bone_name,
            Quaternion relative_rotation_robot, ref GameObject robot_GameObject)
        {
#if UNITY_DRAW_ON_ANIMATOR_IK
            Animator animator = robot_GameObject.GetComponent<Animator>();
            draw_robot_bone_rotation(bone_name, relative_rotation_robot, ref animator);
#elif UNITY_DRAW_AT_LATE_UPDATE
            Humanoid humanoid_component = robot_GameObject.GetComponent<Humanoid>();
            draw_robot_bone_rotation(bone_name, relative_rotation_robot, ref humanoid_component);
#else
            throw new Exception("Abnormal draw timing.");
#endif
        }

        public static void draw_robot_bone_rotation(HumanBodyBones bone_name,
            Quaternion relative_rotation_robot, ref Humanoid humanoid_component)
        {
            Quaternion relative_rotation_unity = Quaternion.identity;
            transform_relative_rotation_from_robot_to_unity(relative_rotation_robot, ref relative_rotation_unity);

            switch (bone_name)
            {
                case HumanBodyBones.Spine:
                    humanoid_component.Spine.rotation =
                        humanoid_component.Spine.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.Chest:
                    humanoid_component.Chest.rotation =
                        humanoid_component.Chest.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.UpperChest:
                    humanoid_component.UpperChest.rotation =
                        humanoid_component.UpperChest.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.Neck:
                    humanoid_component.Neck.rotation =
                        humanoid_component.Neck.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.Head:
                    humanoid_component.Head.rotation =
                        humanoid_component.Head.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightEye:
                    humanoid_component.RightEye.rotation =
                        humanoid_component.RightEye.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftEye:
                    humanoid_component.LeftEye.rotation =
                        humanoid_component.LeftEye.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightShoulder:
                    humanoid_component.RightShoulder.rotation =
                        humanoid_component.RightShoulder.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftShoulder:
                    humanoid_component.LeftShoulder.rotation =
                        humanoid_component.LeftShoulder.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightUpperArm:
                    humanoid_component.RightUpperArm.rotation =
                        humanoid_component.RightUpperArm.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftUpperArm:
                    humanoid_component.LeftUpperArm.rotation =
                        humanoid_component.LeftUpperArm.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightLowerArm:
                    humanoid_component.RightLowerArm.rotation =
                        humanoid_component.RightLowerArm.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftLowerArm:
                    humanoid_component.LeftLowerArm.rotation =
                        humanoid_component.LeftLowerArm.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightHand:
                    humanoid_component.RightHand.rotation =
                        humanoid_component.RightHand.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftHand:
                    humanoid_component.LeftHand.rotation =
                        humanoid_component.LeftHand.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightThumbProximal:
                    humanoid_component.RightThumbProximal.rotation =
                        humanoid_component.RightThumbProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftThumbProximal:
                    humanoid_component.LeftThumbProximal.rotation =
                        humanoid_component.LeftThumbProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightThumbIntermediate:
                    humanoid_component.RightThumbIntermediate.rotation =
                        humanoid_component.RightThumbIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftThumbIntermediate:
                    humanoid_component.LeftThumbIntermediate.rotation =
                        humanoid_component.LeftThumbIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightThumbDistal:
                    humanoid_component.RightThumbDistal.rotation =
                        humanoid_component.RightThumbDistal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftThumbDistal:
                    humanoid_component.LeftThumbDistal.rotation =
                        humanoid_component.LeftThumbDistal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightIndexProximal:
                    humanoid_component.RightIndexProximal.rotation =
                        humanoid_component.RightIndexProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftIndexProximal:
                    humanoid_component.LeftIndexProximal.rotation =
                        humanoid_component.LeftIndexProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightIndexIntermediate:
                    humanoid_component.RightIndexIntermediate.rotation =
                        humanoid_component.RightIndexIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftIndexIntermediate:
                    humanoid_component.LeftIndexIntermediate.rotation =
                        humanoid_component.LeftIndexIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightIndexDistal:
                    humanoid_component.RightIndexDistal.rotation =
                        humanoid_component.RightIndexDistal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftIndexDistal:
                    humanoid_component.LeftIndexDistal.rotation =
                        humanoid_component.LeftIndexDistal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightMiddleProximal:
                    humanoid_component.RightMiddleProximal.rotation =
                        humanoid_component.RightMiddleProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftMiddleProximal:
                    humanoid_component.LeftMiddleProximal.rotation =
                        humanoid_component.LeftMiddleProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightMiddleIntermediate:
                    humanoid_component.RightMiddleIntermediate.rotation =
                        humanoid_component.RightMiddleIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftMiddleIntermediate:
                    humanoid_component.LeftMiddleIntermediate.rotation =
                        humanoid_component.LeftMiddleIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightMiddleDistal:
                    humanoid_component.RightMiddleDistal.rotation =
                        humanoid_component.RightMiddleDistal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftMiddleDistal:
                    humanoid_component.LeftMiddleDistal.rotation =
                        humanoid_component.LeftMiddleDistal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightRingProximal:
                    humanoid_component.RightRingProximal.rotation =
                        humanoid_component.RightRingProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftRingProximal:
                    humanoid_component.LeftRingProximal.rotation =
                        humanoid_component.LeftRingProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightRingIntermediate:
                    humanoid_component.RightRingIntermediate.rotation =
                        humanoid_component.RightRingIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftRingIntermediate:
                    humanoid_component.LeftRingIntermediate.rotation =
                        humanoid_component.LeftRingIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightRingDistal:
                    humanoid_component.RightRingDistal.rotation =
                        humanoid_component.RightRingDistal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftRingDistal:
                    humanoid_component.LeftRingDistal.rotation =
                        humanoid_component.LeftRingDistal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightLittleProximal:
                    humanoid_component.RightLittleProximal.rotation =
                        humanoid_component.RightLittleProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftLittleProximal:
                    humanoid_component.LeftLittleProximal.rotation =
                        humanoid_component.LeftLittleProximal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightLittleIntermediate:
                    humanoid_component.RightLittleIntermediate.rotation =
                        humanoid_component.RightLittleIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftLittleIntermediate:
                    humanoid_component.LeftLittleIntermediate.rotation =
                        humanoid_component.LeftLittleIntermediate.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.RightLittleDistal:
                    humanoid_component.RightLittleDistal.rotation =
                        humanoid_component.RightLittleDistal.parent.rotation * relative_rotation_unity;
                    break;

                case HumanBodyBones.LeftLittleDistal:
                    humanoid_component.LeftLittleDistal.rotation =
                        humanoid_component.LeftLittleDistal.parent.rotation * relative_rotation_unity;
                    break;

                default:
                    break;
            }
        }

        public static void draw_robot_bone_rotation(HumanBodyBones bone_name,
            Quaternion relative_rotation_robot, ref Animator animator)
        {
            Quaternion relative_rotation_unity = Quaternion.identity;
            transform_relative_rotation_from_robot_to_unity(relative_rotation_robot, ref relative_rotation_unity);

            animator.SetBoneLocalRotation(bone_name, relative_rotation_unity);
        }

        ~CommonTransform()
        {

        }
    }

}
