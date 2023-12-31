/**
 * @file CommonStateMachine.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Common State Machine for Maid Robot Simulator.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using UnityEngine;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class CommonStateMachine<GENERIC_MODE> where GENERIC_MODE : struct
    {
        private GENERIC_MODE _mode;
        private GENERIC_MODE _mode_before;

        private float _time_interval = 0.0f;
        private bool _triggered_before = false;

        private float _elapsed_time_after_mode_changed = Mathf.Infinity;        /*!< elapsed time after mode changed */

        public CommonStateMachine(GENERIC_MODE mode_init)
        {
            this._mode = mode_init;
            this._mode_before = mode_init;
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public bool is_transition(GENERIC_MODE from_mode, GENERIC_MODE to_mode)
        {
            bool return_value = false;

            if (this._mode_before.Equals(from_mode) && this._mode.Equals(to_mode))
            {
                return_value = true;
            }

            return return_value;
        }

        public bool is_during(GENERIC_MODE mode)
        {
            bool return_value = false;

            if ( (this._mode_before.Equals(this._mode)) &&
                 (this._mode.Equals(mode)) )
            {
                return_value = true;
            }

            return return_value;
        }

        public void successive_switch_mode_and_update_elapsed_time(
            bool mode_check_flag, float mode_wait_time, float delta_time)
        {
            this.advance_mode();

            if (mode_check_flag)
            {
                this.successive_switch_mode(mode_wait_time);
            }

            this._elapsed_time_after_mode_changed += delta_time;
        }

        public void successive_switch_mode(float mode_wait_time)
        {
            GENERIC_MODE next_mode = this._mode;

            Type type = typeof(GENERIC_MODE);
            if (!type.IsEnum)
            {
                throw new ArgumentException("GENERIC_MODE must be an enumerated type");
            }

            if (this._elapsed_time_after_mode_changed > mode_wait_time)
            {
                var mode_list = Enum.GetValues(type);
                for (int i = 0; i < mode_list.Length; i++)
                {
                    var each_mode_list = mode_list.GetValue(i);
                    if (this._mode.Equals(each_mode_list))
                    {
                        if (i == mode_list.Length - 1)
                        {
                            next_mode = (GENERIC_MODE)mode_list.GetValue(0);
                        }
                        else
                        {
                            next_mode = (GENERIC_MODE)mode_list.GetValue(i + 1);
                        }
                    }
                }

                this._elapsed_time_after_mode_changed = 0.0f;
            }

            this._mode = next_mode;
        }

        public void switch_mode_successively()
        {
            GENERIC_MODE next_mode = this._mode;
            Type type = typeof(GENERIC_MODE);

            var mode_list = Enum.GetValues(type);
            for (int i = 0; i < mode_list.Length; i++)
            {
                var each_mode_list = mode_list.GetValue(i);
                if (this._mode.Equals(each_mode_list))
                {
                    if (i == mode_list.Length - 1)
                    {
                        next_mode = (GENERIC_MODE)mode_list.GetValue(0);
                    }
                    else
                    {
                        next_mode = (GENERIC_MODE)mode_list.GetValue(i + 1);
                    }
                }
            }

            this.update_mode(next_mode);
        }

        public GENERIC_MODE get_mode()
        {
            return this._mode;
        }

        public void set_mode(GENERIC_MODE mode)
        {
            this._mode = mode;
        }

        public GENERIC_MODE get_mode_before()
        {
            return this._mode_before;
        }

        public void set_mode_before(GENERIC_MODE mode_before)
        {
            this._mode_before = mode_before;
        }

        public void set_elapsed_time_after_mode_changed(float elapsed_time)
        {
            this._elapsed_time_after_mode_changed = elapsed_time;
        }

        public void update_mode(GENERIC_MODE mode)
        {
            this.advance_mode();
            this._mode = mode;
        }

        public void advance_mode()
        {
            this._mode_before = this._mode;
        }

        public float get_elapsed_time_after_mode_changed()
        {
            return this._elapsed_time_after_mode_changed;
        }

        public void advance_elapsed_time(float delta_time)
        {
            this._elapsed_time_after_mode_changed += delta_time;
        }

        public void reset_elapsed_time()
        {
            this._elapsed_time_after_mode_changed = 0.0f;
        }

        public void set_time_interval(float interval)
        {
            this._time_interval = interval;
        }

        public GENERIC_MODE triggered_switch_mode_by_elapsed_time(float delta_time)
        {
            this.advance_elapsed_time(delta_time);

            if (this._triggered_before)
            {
                this.switch_mode_successively();
                this._triggered_before = false;
            }
            else if (this._elapsed_time_after_mode_changed > this._time_interval)
            {
                this.switch_mode_successively();
                this.reset_elapsed_time();

                this._triggered_before = true;
            }

            return this._mode;
        }

        ~CommonStateMachine()
        {

        }

    }

}