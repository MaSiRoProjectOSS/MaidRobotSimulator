/**
 * @file CommonRateManager.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Manage rate event or moving average.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

using System;
using UnityEngine;


namespace MaidRobotSimulator.MaidRobotCafe
{
    public class CommonRateManager<DATA_TYPE>
    {
        private bool[] _success_flag_buffer;
        private bool[,] _success_flag_buffer_2D;
        private float[] _moving_average_buffer;
        private float[,] _moving_average_buffer_2D;

        private int _buffer_index;

        public CommonRateManager(int success_flag_buffer_length)
        {
            if (typeof(DATA_TYPE) == typeof(bool))
            {
                this._success_flag_buffer = new bool[success_flag_buffer_length];
                this._buffer_index = 0;
            }
            else
            {
                throw new Exception("wrong data type.");
            }
        }

        public CommonRateManager(int success_flag_buffer_length, int buffer_dimension)
        {
            if (typeof(DATA_TYPE) == typeof(bool))
            {
                this._success_flag_buffer_2D = new bool[success_flag_buffer_length, buffer_dimension];
                
                this._buffer_index = 0;
            }
            else
            {
                throw new Exception("wrong data type.");
            }
        }

        public CommonRateManager(int moving_average_buffer_length, float init_value)
        {
            if (typeof(DATA_TYPE) == typeof(float))
            {
                this._moving_average_buffer = new float[moving_average_buffer_length];
                for (int i = 0; i < moving_average_buffer_length; i++)
                {
                    this._moving_average_buffer[i] = init_value;
                }
                this._buffer_index = 0;
            }
            else
            {
                throw new Exception("wrong data type.");
            }
        }

        public CommonRateManager(int moving_average_buffer_length, float[] init_value)
        {
            if (typeof(DATA_TYPE) == typeof(float))
            {
                this._moving_average_buffer_2D = new float[moving_average_buffer_length, init_value.Length];
                for (int i = 0; i < moving_average_buffer_length; i++)
                {
                    for (int j = 0; j < init_value.Length; j++)
                    {
                        this._moving_average_buffer_2D[i, j] = init_value[j];
                    }
                }
                this._buffer_index = 0;
            }
            else
            {
                throw new Exception("wrong data type.");
            }
        }

        public void set_success_flag(bool flag)
        {
            if (typeof(DATA_TYPE) == typeof(bool))
            {
                this._success_flag_buffer[this._buffer_index] = flag;

                this._buffer_index++;
                if (this._buffer_index >= this._success_flag_buffer.Length)
                {
                    this._buffer_index = 0;
                }
            }
            else
            {
                throw new Exception("wrong data type.");
            }
        }

        public void set_success_flag(bool[] flag)
        {
            if (typeof(DATA_TYPE) == typeof(bool))
            {
                for (int i = 0; i < flag.Length; i++)
                {
                    this._success_flag_buffer_2D[this._buffer_index, i] = flag[i];
                }

                this._buffer_index++;
                if (this._buffer_index >= this._success_flag_buffer.GetLength(0))
                {
                    this._buffer_index = 0;
                }
            }
            else
            {
                throw new Exception("wrong data type.");
            }
        }

        public void set_moving_average_value(float value)
        {
            if (typeof(DATA_TYPE) == typeof(float))
            {
                this._moving_average_buffer[this._buffer_index] = value;

                this._buffer_index++;
                if (this._buffer_index >= this._moving_average_buffer.Length)
                {
                    this._buffer_index = 0;
                }
            }
            else
            {
                throw new Exception("wrong data type.");
            }
        }

        public void set_moving_average_value(float[] value)
        {
            if (typeof(DATA_TYPE) == typeof(float))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    this._moving_average_buffer_2D[this._buffer_index, i] = value[i];
                }

                this._buffer_index++;
                if (this._buffer_index >= this._moving_average_buffer_2D.GetLength(0))
                {
                    this._buffer_index = 0;
                }
            }
            else
            {
                throw new Exception("wrong data type.");
            }
        }

        public float get_success_rate_percent()
        {
            return this.get_success_rate() * CommonParameter.TO_PERCENT;
        }

        public float get_success_rate()
        {
            float return_value = 0;

            for (int i = 0; i < this._success_flag_buffer.Length; i++)
            {
                if (true == this._success_flag_buffer[i])
                {
                    return_value += 1.0f;
                }
            }

            return_value /= (float)this._success_flag_buffer.Length;

            return return_value;
        }

        public float get_moving_average_value()
        {
            float return_value = 0.0f;

            for (int i = 0; i < this._moving_average_buffer.Length; i++)
            {
                return_value += this._moving_average_buffer[i];
            }

            return_value /= (float)this._moving_average_buffer.Length;

            return return_value;
        }

        public float[] get_moving_average_values()
        {
            float[] return_value = new float[this._moving_average_buffer_2D.GetLength(1)];

            for (int i = 0; i < this._moving_average_buffer_2D.GetLength(1); i++)
            {
                for (int j = 0; j < this._moving_average_buffer_2D.GetLength(0); j++)
                {
                    return_value[i] += this._moving_average_buffer_2D[j, i];
                }
                return_value[i] /= (float)this._moving_average_buffer_2D.GetLength(0);
            }

            return return_value;
        }

        ~CommonRateManager()
        {

        }
    }
}
