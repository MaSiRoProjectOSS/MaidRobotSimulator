/**
 * @file TrajectoryInterpolationManager.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Trajectory Interpolation Manager.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class TrajectoryInterpolationManager
    {
        private float _period_time = 1.0f;
        private float _elapsed_time = 0.0f;


        public TrajectoryInterpolationManager(float period_time)
        {
            this._period_time = period_time;
            this.initialize();
        }

        public void initialize()
        {
            this._elapsed_time = 0.0f;
        }

        public void set_period_time(float period_time)
        {
            this._period_time = period_time;
        }

        public void update_elapsed_time(float delta_time)
        {
            this._elapsed_time += delta_time;
        }


        public bool check_passing(float period_time)
        {
            bool result = this.check_time_over(period_time);
            if (result)
            {
                this.initialize();
            }
            return result;
        }

        public bool check_time_over(float period_time)
        {
            bool result = false;
            if (this._elapsed_time > period_time)
            {
                result = true;
            }
            return result;
        }

        public bool check_passing()
        {
            return this.check_passing(this._period_time);
        }

        public bool check_time_over()
        {
            return this.check_time_over(this._period_time);
        }

        public float get_elapsed_time()
        {
            return this._elapsed_time;
        }

        public float get_s_curve_interpolation()
        {
            float result = 1.0f;
            if (this._elapsed_time < this._period_time)
            {
                if (this._elapsed_time < 0.0f)
                {
                    result = 0.0f;
                }
                else
                {
                    float sin_data = Mathf.Sin(Mathf.PI / 2.0f * (this._elapsed_time / this._period_time));
                    result = sin_data * sin_data;
                }
            }
            return result;
        }

        public float get_sin_cycle()
        {
            float value = 1.0f;
            if (this._period_time > 0)
            {
                value = Mathf.Sin(Mathf.PI / 2 * (this.get_elapsed_time() / this._period_time));
            }
            return value;
        }

        public float get_cos_cycle()
        {
            float value = 0.0f;
            if (this._period_time > 0)
            {
                value = Mathf.Cos(Mathf.PI / 2 * (this.get_elapsed_time() / this._period_time));
            }
            return value;
        }

        ~TrajectoryInterpolationManager()
        {
        }
    }
}
