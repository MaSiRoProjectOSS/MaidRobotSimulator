/**
 * @file CommonImageProcessor.cs
 * @author Claude (claude.masiro@gmail.com)
 * @brief Common image processing for Maid Robot Simulator.
 *
 * @copyright Copyright (c) MaSiRo Project. 2023-.
 *
 */

#define USE_PARALLEL_JOB

using RosMessageTypes.Sensor;
using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace MaidRobotSimulator.MaidRobotCafe
{
    public class CommonImageProcessor : IDisposable
    {
        private string _image_structure_name;
        private Color32[] _original_pixels;
        private Color32[] _flipped_pixels;
        private byte[] _output_image_bytes;

        private bool _flip_image_job_scheduled = false;
        private JobHandle _flip_image_job_handle;

        private int _width_job;
        private int _height_job;
        private int _length_job;
        private NativeArray<Color32> _original_pixels_job;
        private NativeArray<Color32> _flipped_pixels_job;
        private NativeArray<byte> _output_image_bytes_job;

        private CommonRateManager<bool> _image_process_rate_manager;

        private MessageStructure.ST_SENSOR_MSGS_IMAGE _camera_image_structure;


        public CommonImageProcessor(RenderTexture eye_render_texture, Camera camera_object,
            string image_structure_name)
        {
            this._image_structure_name = image_structure_name;

            Texture2D screenshot = this.take_screenshot_from_camera(eye_render_texture, camera_object);

            /* initialize pixels */
            this._original_pixels = screenshot.GetPixels32();
            this._flipped_pixels = screenshot.GetPixels32();

            this._width_job = screenshot.width;
            this._height_job = screenshot.height;
            this._length_job = screenshot.GetPixels32().Length;

            this._output_image_bytes = new byte[screenshot.GetPixels32().Length * CommonParameter.COLOR_ELEMENTS_NUM];

            this._image_process_rate_manager = 
                new CommonRateManager<bool>(CommonParameter.IMAGE_PROCESS_SUCCESS_FLAG_BUFFER_LENGTH);

            this._camera_image_structure =
                new MessageStructure.ST_SENSOR_MSGS_IMAGE(
                    new MessageStructure.ST_STD_MSGS_HEADER(
                        0,
                        new MessageStructure.ST_TIME(0, 0),
                        this._image_structure_name
                        ),
                    (uint)screenshot.height,
                    (uint)screenshot.width,
                    CommonParameter.CAMERA_IMAGE_ENCODING,
                    0,
                    (uint)screenshot.width * (uint)CommonParameter.COLOR_ELEMENTS_NUM,
                    this._output_image_bytes);
        }

        /*********************************************************
         * Public functions
         *********************************************************/
        public Texture2D take_screenshot_from_camera(RenderTexture render_texture,
                Camera camera_object)
        {
            /* transform render_texture to Texture2D */
            Texture2D screenshot = new Texture2D(render_texture.width,
                    render_texture.height, TextureFormat.RGB24, false);
            camera_object.Render();
            RenderTexture.active = render_texture;
            screenshot.ReadPixels(new Rect(0, 0, render_texture.width,
                render_texture.height), 0, 0);
            screenshot.Apply();

            return screenshot;
        }

        public MessageStructure.ST_SENSOR_MSGS_IMAGE input_data_to_image_structure(
            RenderTexture render_texture, Texture2D screenshot)
        {
            this._get_RGB_bytes(screenshot);
            this._camera_image_structure.data = this._output_image_bytes;

            return this._camera_image_structure;
        }

        public float get_image_process_rate()
        {
            return this._image_process_rate_manager.get_success_rate_percent();
        }

        public void Dispose()
        {
            this._flip_image_job_handle.Complete();

            if (this._original_pixels_job.IsCreated)
            {
                this._original_pixels_job.Dispose();
            }
            if (this._flipped_pixels_job.IsCreated)
            {
                this._flipped_pixels_job.Dispose();
            }
            if (this._output_image_bytes_job.IsCreated)
            {
                this._output_image_bytes_job.Dispose();
            }
        }

        /*********************************************************
         * Private functions
         *********************************************************/
        private void _get_RGB_bytes(Texture2D texture)
        {
#if USE_PARALLEL_JOB
            if (false == this._check_image_size_is_valid(texture))
            {
                throw new Exception("wrong image size.");
            }
            else
            {
                if (true == this._flip_image_job_scheduled)
                {
                    if (true == this._flip_image_job_handle.IsCompleted)
                    {
                        this._flip_image_job_handle.Complete();

                        this._output_image_bytes = this._output_image_bytes_job.ToArray();

                        this._original_pixels_job.Dispose();
                        this._flipped_pixels_job.Dispose();
                        this._output_image_bytes_job.Dispose();

                        this._image_process_rate_manager.set_success_flag(true);
                    }
                    else
                    {
                        this._image_process_rate_manager.set_success_flag(false);
                    }
                }

                if ( (null == this._flip_image_job_handle) ||
                     (true == this._flip_image_job_handle.IsCompleted) )
                {
                    Color32[] original_pixels = texture.GetPixels32();
                    this._original_pixels_job =
                        new NativeArray<Color32>(original_pixels, Allocator.Persistent);
                    Color32[] flipped_pixels = new Color32[this._original_pixels.Length];
                    this._flipped_pixels_job =
                        new NativeArray<Color32>(flipped_pixels, Allocator.Persistent);
                    byte[] output_image_bytes = new byte[texture.GetPixels32().Length * CommonParameter.COLOR_ELEMENTS_NUM];
                    this._output_image_bytes_job =
                        new NativeArray<byte>(output_image_bytes, Allocator.Persistent);

                    var flip_image_job = new _flip_image_job()
                    {
                        width = this._width_job,
                        height = this._height_job,
                        length = this._length_job,
                        original_pixels = this._original_pixels_job,
                        flipped_pixels = this._flipped_pixels_job,
                        output_image_bytes = this._output_image_bytes_job
                    };

                    this._flip_image_job_handle = flip_image_job.Schedule();

                    JobHandle.ScheduleBatchedJobs();
                    this._flip_image_job_scheduled = true;
                }
            }

#else
            this._original_pixels = texture.GetPixels32();
            this._flipped_pixels = new Color32[this._original_pixels.Length];

            if (false == CommonParameter.CAMERA_DEBUG_DO_NOT_FLIP)
            {
                /* Flip image data array because somehow the image data is upside down. */
                this._flip_task(texture.width, texture.height);

                this._set_camera_image_bytes(this._flipped_pixels);
            }
            else
            {
                this._set_camera_image_bytes(this._original_pixels);
            }
#endif
        }

        private void _flip_task(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    this._flipped_pixels[x + (y * width)] =
                        this._original_pixels[x + ((height - 1 - y) * width)];
                }
            }
        }

        private void _set_camera_image_bytes(Color32[] pixels)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                this._output_image_bytes[i * CommonParameter.COLOR_ELEMENTS_NUM] =
                    pixels[i].r; /* R */
                this._output_image_bytes[i * CommonParameter.COLOR_ELEMENTS_NUM + 1] =
                    pixels[i].g; /* G */
                this._output_image_bytes[i * CommonParameter.COLOR_ELEMENTS_NUM + 2] =
                    pixels[i].b; /* B */
            }
        }

        private bool _check_image_size_is_valid(Texture2D texture)
        {
            bool return_value = (this._width_job == texture.width) &&
                                (this._height_job == texture.height) &&
                                (this._length_job == texture.GetPixels32().Length);

            return return_value;
        }

        /*********************************************************
         * Job System functions
         *********************************************************/
        private struct _flip_image_job : IJob
        {
            public int width;
            public int height;
            public int length;

            public NativeArray<Color32> flipped_pixels;
            public NativeArray<Color32> original_pixels;

            [WriteOnly]
            public NativeArray<byte> output_image_bytes;

            public void Execute()
            {
                if (false == CommonParameter.CAMERA_DEBUG_DO_NOT_FLIP)
                {
                    /* flip */
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            flipped_pixels[x + (y * width)] =
                                original_pixels[x + ((height - 1 - y) * width)];
                        }
                    }

                    /* image to bytes */
                    for (int i = 0; i < length; i++)
                    {
                        output_image_bytes[i * CommonParameter.COLOR_ELEMENTS_NUM] =
                            flipped_pixels[i].r; /* R */
                        output_image_bytes[i * CommonParameter.COLOR_ELEMENTS_NUM + 1] =
                            flipped_pixels[i].g; /* G */
                        output_image_bytes[i * CommonParameter.COLOR_ELEMENTS_NUM + 2] =
                            flipped_pixels[i].b; /* B */
                    }
                }
                else
                {
                    /* image to bytes */
                    for (int i = 0; i < length; i++)
                    {
                        output_image_bytes[i * CommonParameter.COLOR_ELEMENTS_NUM] =
                            original_pixels[i].r; /* R */
                        output_image_bytes[i * CommonParameter.COLOR_ELEMENTS_NUM + 1] =
                            original_pixels[i].g; /* G */
                        output_image_bytes[i * CommonParameter.COLOR_ELEMENTS_NUM + 2] =
                            original_pixels[i].b; /* B */
                    }
                }
            }
        }

        ~CommonImageProcessor()
        {
            this.Dispose();
        }
    }
}
