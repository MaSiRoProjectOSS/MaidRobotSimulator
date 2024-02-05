/**
 * @file NeckController.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Control neck rotation.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class NeckController
    {
        private TrajectoryInterpolationManager _neck_calculate_manager;
        private TrajectoryInterpolationManager _neck_pose_manager;
        private TrajectoryInterpolationManager _timer_reset;
        private TrajectoryInterpolationManager _random_look_manager;
        private TrajectoryInterpolationManager _pose_sequence_manager;

        private float _old_sum_pose = 0.0f;
        private float _delta_pose = 0.0f;

        private float _speed_gain = CommonParameter.NECK_SPEED_INITIAL_GAIN;

        private SystemStructure.HEAD_UNIT_MODE _head_unit_mode = SystemStructure.HEAD_UNIT_MODE.LOOK_FORWARD;
        private SystemStructure.NECK_POSE_COMMAND _neck_pose_command = SystemStructure.NECK_POSE_COMMAND.STAY;
        private SystemStructure.NECK_POSE_MODE _neck_pose_mode = SystemStructure.NECK_POSE_MODE.RANDOM_MOVE;

        private SystemStructure.POSE_SEQUENCE _pose_sequence = SystemStructure.POSE_SEQUENCE.FIRST;

        private bool _flag_face_track_enable = false;
        private bool _mode_flag_arm_track = false;
        private bool _mode_flag_command_move = false;
        private bool _mode_flag_face_track = false;

        private bool _pose_reset_flag = false;

        private SystemStructure.ST_EULER_ANGLE _neck_input_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        private SystemStructure.ST_EULER_ANGLE _neck_current_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        private SystemStructure.ST_EULER_ANGLE _neck_target_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        private SystemStructure.ST_EULER_ANGLE _random_look_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        private SystemStructure.ST_EULER_ANGLE _random_start_look_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        private SystemStructure.ST_EULER_ANGLE _random_target_look_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        private SystemStructure.ST_EULER_ANGLE _final_reference_look_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        private SystemStructure.ST_EULER_ANGLE _target_look_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        private SystemStructure.ST_EULER_ANGLE _output_neck_face_track_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        private SystemStructure.ST_EULER_ANGLE _neck_arm_track_angle =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);
        
        private SystemStructure.ST_EULER_ANGLE _output_neck_face_track_angle_filter =
            new SystemStructure.ST_EULER_ANGLE(0.0f, 0.0f, 0.0f);

        private CommonRateManager<float> _output_neck_face_track_angle_average_manager;

        private float _stabilize_pitch = 0.0f;

        public NeckController()
        {
            this._neck_calculate_manager =
                new TrajectoryInterpolationManager(CommonParameter.NECK_CALCULATE_PERIOD);
            this._neck_pose_manager =
                new TrajectoryInterpolationManager(CommonParameter.NECK_POSE_RESET_WAIT_TIME);
            this._timer_reset =
                new TrajectoryInterpolationManager(CommonParameter.NECK_FACE_TRACK_START_WAIT_TIME);
            this._random_look_manager =
                new TrajectoryInterpolationManager(CommonParameter.NECK_RANDOM_LOOK_PERIOD);

            float[] neck_input_angle = new float[this._neck_input_angle.get_length()];
            CommonTransform.euler_angle_to_array(this._neck_input_angle, ref neck_input_angle);
            this._output_neck_face_track_angle_average_manager =
                new CommonRateManager<float>(CommonParameter.NECK_FACE_TRACK_AVERAGE_BUFFER_LENGTH,
                neck_input_angle);
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public bool calculate(float delta_time)
        {
            bool result = true;
            this._neck_calculate_manager.update_elapsed_time(delta_time);

            this._neck_current_angle = this._output_neck_face_track_angle_filter;

            this._resident(delta_time);

            this._neck_calculate_manager.set_period_time(CommonParameter.NECK_CALCULATE_PERIOD);
            if (true == this._neck_calculate_manager.check_passing())
            {
                this._mode_flag_face_track = false;
            }

            if (this._head_unit_mode == SystemStructure.HEAD_UNIT_MODE.FACE_TRACKING)
            {
                /* If there is face recognition information, */

                this._neck_calculate_manager.initialize();

                if (true == this._flag_face_track_enable)
                {
                    if ((Mathf.Abs(this._neck_input_angle.roll) < CommonParameter.QUARTER_CIRCLE_DEG) &&
                        (Mathf.Abs(this._neck_input_angle.pitch) < CommonParameter.QUARTER_CIRCLE_DEG) &&
                        (Mathf.Abs(this._neck_input_angle.yaw) < CommonParameter.QUARTER_CIRCLE_DEG) )
                    {
                        this._mode_flag_face_track = true;

                        this._output_neck_face_track_angle.yaw = this._neck_current_angle.yaw - this._neck_input_angle.yaw;

                        float calc_yaw_rad = this._neck_current_angle.yaw * Mathf.Deg2Rad;
                        float calc_roll = (this._neck_input_angle.pitch * Mathf.Sin(calc_yaw_rad) / CommonParameter.FACE_TRACKING_ROLL_FACTOR)
                            + (this._neck_input_angle.roll * Mathf.Cos(calc_yaw_rad));
                        float calc_pitch = this._neck_input_angle.pitch * Mathf.Cos(calc_yaw_rad);

                        this._output_neck_face_track_angle.pitch =
                            (this._neck_current_angle.pitch + calc_pitch)
                                * Mathf.Lerp(CommonParameter.QUARTER_CIRCLE_DEG, 0.0f,
                                    Mathf.InverseLerp(0.0f, CommonParameter.NECK_LIMIT_YAW_MAX, Mathf.Abs(this._neck_current_angle.yaw)))
                                / CommonParameter.QUARTER_CIRCLE_DEG;

                        this._output_neck_face_track_angle.roll = calc_roll - this._neck_current_angle.roll;
                    }
                }
            }

            return result;
        }

        public void set_target(float yaw, float pitch, float roll)
        {
            this._neck_target_angle.yaw = yaw;
            this._neck_target_angle.pitch = pitch;
            this._neck_target_angle.roll = roll;
        }

        public void reset()
        {
            set_target(0.0f, 0.0f, 0.0f);
            this._mode_flag_face_track = false;
            this._mode_flag_arm_track = false;
            this._timer_reset.initialize();
        }

        public void set_pose_reset_flag(bool flag)
        {
            this._pose_reset_flag = flag;
        }

        public bool get_pose_reset_flag()
        {
            return this._pose_reset_flag;
        }

        public void set_mode_flag_arm_track(bool flag)
        {
            this._mode_flag_arm_track = flag;
        }

        public void set_target_yaw(float yaw)
        {
            this._neck_target_angle.yaw = yaw;
        }

        public void set_neck_input_angle(SystemStructure.ST_EULER_ANGLE angle)
        {
            this._neck_input_angle.yaw = angle.yaw;
            this._neck_input_angle.pitch = angle.pitch;
            this._neck_input_angle.roll = angle.roll;
        }

        public void set_mode_flag_command_move(bool flag)
        {
            this._mode_flag_command_move = flag;
        }

        public void set_head_unit_mode(SystemStructure.HEAD_UNIT_MODE mode)
        {
            this._head_unit_mode = mode;
        }

        public SystemStructure.ST_EULER_ANGLE get_output_neck_face_track_angle()
        {
            /* These moving average for neck angles are to simulate the delay of servo motor. */
            float[] face_track_angle = new float[this._output_neck_face_track_angle.get_length()];
            CommonTransform.euler_angle_to_array(this._output_neck_face_track_angle, ref face_track_angle);
            this._output_neck_face_track_angle_average_manager.set_moving_average_value(
                face_track_angle);

            CommonTransform.array_to_euler_angle(
                this._output_neck_face_track_angle_average_manager.get_moving_average_values(),
                ref this._output_neck_face_track_angle_filter);

            this._output_neck_face_track_angle_filter.roll =
                Mathf.Clamp(
                    this._output_neck_face_track_angle_filter.roll,
                    CommonParameter.NECK_LIMIT_ROLL_OUTPUT_MIN, CommonParameter.NECK_LIMIT_ROLL_OUTPUT_MAX);

            return this._output_neck_face_track_angle_filter;
        }

        public SystemStructure.NECK_POSE_MODE get_neck_pose_mode()
        {
            return this._neck_pose_mode;
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        private void _setup()
        {
            this._random_look_manager.initialize();
        }

        private void _look_random()
        {
            float s_curve = this._random_look_manager.get_s_curve_interpolation();

            if (true == this._random_look_manager.check_time_over())
            {
                this._random_start_look_angle.yaw = this._neck_current_angle.yaw;
                this._random_start_look_angle.pitch = this._neck_current_angle.pitch;
                this._random_start_look_angle.roll = this._neck_current_angle.roll;

                this._neck_pose_mode = SystemStructure.NECK_POSE_MODE.RANDOM_MOVE;
                
                this._random_target_look_angle.yaw = Random.Range(
                    CommonParameter.TARGET_LOOK_YAW_RANDOM_RANGE_MIN, CommonParameter.TARGET_LOOK_YAW_RANDOM_RANGE_MAX);
                this._random_target_look_angle.pitch = Random.Range(
                    CommonParameter.TARGET_LOOK_PITCH_RANDOM_RANGE_MIN, CommonParameter.TARGET_LOOK_PITCH_RANDOM_RANGE_MAX);
                this._random_target_look_angle.roll = Random.Range(
                    CommonParameter.TARGET_LOOK_ROLL_RANDOM_RANGE_MIN, CommonParameter.TARGET_LOOK_ROLL_RANDOM_RANGE_MAX);

                this._random_look_manager.set_period_time(
                    Random.Range(
                        CommonParameter.LOOK_SEND_TIME_RANDOM_RANGE_MIN, CommonParameter.LOOK_SEND_TIME_RANDOM_RANGE_MAX));
                this._random_look_manager.initialize();
            }
            else
            {
                this._random_look_angle.yaw = Mathf.Clamp(
                    this._random_start_look_angle.yaw + this._random_target_look_angle.yaw * s_curve,
                    CommonParameter.RANDOM_LOOK_YAW_MIN, CommonParameter.RANDOM_LOOK_YAW_MAX);
                this._random_look_angle.roll = Mathf.Clamp(
                    this._random_start_look_angle.roll + this._random_target_look_angle.roll * s_curve,
                    CommonParameter.RANDOM_LOOK_ROLL_MIN, CommonParameter.RANDOM_LOOK_ROLL_MAX);
                this._random_look_angle.pitch = Mathf.Clamp(
                    this._random_start_look_angle.pitch + this._random_target_look_angle.pitch * s_curve,
                    CommonParameter.RANDOM_LOOK_PITCH_MIN, CommonParameter.RANDOM_LOOK_PITCH_MAX);
            }
        }

        private void _resident(float delta_time)
        {
            this._neck_pose_manager.update_elapsed_time(delta_time);
            this._timer_reset.update_elapsed_time(delta_time);
            this._random_look_manager.update_elapsed_time(delta_time);

            /* Reset if the neck doesn't move for a while when the mode is FACE_TRACK.  */
            if ( (this._neck_pose_mode != SystemStructure.NECK_POSE_MODE.FACE_TRACK) ||
                 (Mathf.Abs(this._neck_current_angle.yaw) > CommonParameter.NECK_YAW_RESET_LIMIT) )
            {
                if (true == this._neck_pose_manager.check_passing())
                {
                    float sum_pose = this._neck_current_angle.pitch + this._neck_current_angle.roll + this._neck_current_angle.yaw;
                    this._delta_pose = Mathf.Abs(sum_pose - this._old_sum_pose);
                    this._old_sum_pose = sum_pose;
                    if (this._delta_pose < CommonParameter.NECK_DELTA_POSE_LOW_LIMIT)
                    {
                        this.reset();
                        this._flag_face_track_enable = false;
                    }

                    this._neck_pose_manager.initialize();
                }
            }
            else
            {
                this._neck_pose_manager.initialize();
            }

            /* Wait for a while, and reset flag_face_track_enable */
            if (false == this._flag_face_track_enable)
            {
                if (true == this._timer_reset.check_time_over())
                {
                    this._flag_face_track_enable = true;
                }
            }

            this._pose_manager();

            if (true == this._mode_flag_command_move)
            {
                this._neck_pose_mode = SystemStructure.NECK_POSE_MODE.COMMAND_MOVING;
            }
            else
            {
                if ( (false == this._mode_flag_face_track) &&
                     (false == this._mode_flag_arm_track) )
                {
                    this._neck_pose_mode = SystemStructure.NECK_POSE_MODE.RANDOM_MOVE;
                }
                if ((true == this._mode_flag_face_track) && 
                    (false == this._mode_flag_arm_track) )
                {
                    this._neck_pose_mode = SystemStructure.NECK_POSE_MODE.FACE_TRACK;
                }
                if (true == this._mode_flag_arm_track)
                {
                    this._neck_pose_mode = SystemStructure.NECK_POSE_MODE.ARM_TRACK;
                }
            }

            if (SystemStructure.NECK_POSE_MODE.RANDOM_MOVE == this._neck_pose_mode)
            {
                this._speed_gain = CommonParameter.NECK_SPEED_RANDOM_MOVE_GAIN;
                this._look_random();
            }
            else
            {
                this._random_look_angle.yaw = 0.0f;
                this._random_look_angle.pitch = 0.0f;
                this._random_look_angle.roll = 0.0f;
                this._random_look_manager.initialize();
            }

            if (SystemStructure.NECK_POSE_MODE.COMMAND_MOVING == this._neck_pose_mode)
            {
                this._final_reference_look_angle.yaw = this._target_look_angle.yaw;
                this._final_reference_look_angle.pitch = this._target_look_angle.pitch + this._stabilize_pitch;
                this._final_reference_look_angle.roll = this._target_look_angle.roll;
            }

            if (SystemStructure.NECK_POSE_MODE.FACE_TRACK == this._neck_pose_mode)
            {
                this._speed_gain = CommonParameter.NECK_SPEED_FACE_TRACK_GAIN;
                this._final_reference_look_angle.yaw = this._target_look_angle.yaw + this._output_neck_face_track_angle.yaw;
                this._final_reference_look_angle.pitch = this._target_look_angle.pitch + this._output_neck_face_track_angle.pitch + this._stabilize_pitch;
                this._final_reference_look_angle.roll = this._target_look_angle.roll + this._output_neck_face_track_angle.roll;
            }
            else if (SystemStructure.NECK_POSE_MODE.ARM_TRACK == this._neck_pose_mode)
            {
                this._final_reference_look_angle.yaw = this._target_look_angle.yaw + this._neck_arm_track_angle.yaw;
                this._final_reference_look_angle.pitch = this._target_look_angle.pitch + this._neck_arm_track_angle.pitch + this._stabilize_pitch;
                this._final_reference_look_angle.roll = this._target_look_angle.roll + this._neck_arm_track_angle.roll;
            }
            else
            {
                this._final_reference_look_angle.yaw = this._target_look_angle.yaw + this._random_look_angle.yaw;
                this._final_reference_look_angle.pitch = this._target_look_angle.pitch + this._random_look_angle.pitch + this._stabilize_pitch;
                this._final_reference_look_angle.roll = this._target_look_angle.roll + this._random_look_angle.roll;
            }

            this._output_neck_face_track_angle.pitch += this._speed_gain * (this._final_reference_look_angle.pitch - this._output_neck_face_track_angle.pitch) * delta_time;
            this._output_neck_face_track_angle.yaw += this._speed_gain * (this._final_reference_look_angle.yaw - this._output_neck_face_track_angle.yaw) * delta_time;
            this._output_neck_face_track_angle.roll += this._speed_gain * (this._final_reference_look_angle.roll - this._output_neck_face_track_angle.roll) * delta_time;

            this._output_neck_face_track_angle.pitch = Mathf.Clamp(
                this._output_neck_face_track_angle.pitch,
                CommonParameter.NECK_LIMIT_PITCH_MIN, CommonParameter.NECK_LIMIT_PITCH_MAX);

            float roll_pitch_limit = 1.0f - Mathf.Clamp(
                Mathf.Abs(this._output_neck_face_track_angle.pitch),
                0.0f, CommonParameter.ROLL_PITCH_LIMIT)
                / CommonParameter.ROLL_PITCH_LIMIT;

            this._output_neck_face_track_angle.roll = Mathf.Clamp(
                this._output_neck_face_track_angle.roll,
                CommonParameter.NECK_LIMIT_ROLL_MIN * roll_pitch_limit, CommonParameter.NECK_LIMIT_ROLL_MAX * roll_pitch_limit);

            this._output_neck_face_track_angle.yaw = Mathf.Clamp(
                this._output_neck_face_track_angle.yaw,
                CommonParameter.NECK_LIMIT_YAW_MIN, CommonParameter.NECK_LIMIT_YAW_MAX);
        }

        private void _pose_manager()
        {
            if (this._neck_pose_command == SystemStructure.NECK_POSE_COMMAND.NOD)
            {
                this._nod();
            }
            if (_neck_pose_command == SystemStructure.NECK_POSE_COMMAND.INCLINE)
            {
                this._incline();
            }
        }

        private void _nod()
        {
            if (SystemStructure.POSE_SEQUENCE.FIRST == this._pose_sequence)
            {
                this._pose_sequence_manager.initialize();
                this._pose_sequence_manager.set_period_time(CommonParameter.NECK_NOD_PERIOD);

                this._pose_sequence = SystemStructure.POSE_SEQUENCE.SECOND;
            }
            if (SystemStructure.POSE_SEQUENCE.SECOND == this._pose_sequence)
            {
                this._target_look_angle.pitch = CommonParameter.NOD_TARGET_PITCH_FACTOR_1 *
                    (1.0f - this._pose_sequence_manager.get_cos_cycle()) / CommonParameter.NOD_TARGET_PITCH_FACTOR_2;

                if (true == this._pose_sequence_manager.check_passing())
                {
                    this._pose_sequence = SystemStructure.POSE_SEQUENCE.THIRD;
                    this._pose_sequence_manager.initialize();
                }
            }
            if (SystemStructure.POSE_SEQUENCE.THIRD == this._pose_sequence)
            {
                this._neck_pose_command = SystemStructure.NECK_POSE_COMMAND.STAY;
                this._pose_sequence = SystemStructure.POSE_SEQUENCE.FIRST;
            }
        }

        private void _incline()
        {
            if (SystemStructure.POSE_SEQUENCE.FIRST == this._pose_sequence)
            {
                this._pose_sequence_manager.initialize();
                this._pose_sequence_manager.set_period_time(CommonParameter.NECK_INCLINE_PERIOD);

                this._pose_sequence = SystemStructure.POSE_SEQUENCE.SECOND;
            }
            if (SystemStructure.POSE_SEQUENCE.SECOND == this._pose_sequence)
            {
                this._target_look_angle.roll = CommonParameter.INCLINE_TARGET_ROLL_ANGLE;
                if (true == this._pose_sequence_manager.check_passing())
                {
                    this._target_look_angle.roll = 0.0f;
                    this._pose_sequence = SystemStructure.POSE_SEQUENCE.THIRD;
                }
            }
            if (SystemStructure.POSE_SEQUENCE.THIRD == this._pose_sequence)
            {
                this._neck_pose_command = SystemStructure.NECK_POSE_COMMAND.STAY;
                this._pose_sequence = SystemStructure.POSE_SEQUENCE.FIRST;
            }
        }

        /*********************************************************
        * Destructor
        *********************************************************/
        ~NeckController()
        {
        }
    }
}
