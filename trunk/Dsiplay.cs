﻿//=================================================================
// Display.cs
//=================================================================
// Copyright (C) 2011 S56A YT7PWR
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//=================================================================

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Windows;

namespace CWExpert
{
    #region structures

    struct Vertex
    {
        public Vector4 Position;
        public int Color;
    }

    struct DXRectangle
    {
        public int x1;
        public int x2;
        public int x3;
        public int x4;
        public int y1;
        public int y2;
        public int y3;
        public int y4;
    }

    struct VerticalString
    {
        public int pos_x;
        public int pos_y;
        public string label;
        public Color color;
    }

    struct HorizontalString
    {
        public int pos_x;
        public int pos_y;
        public string label;
        public Color color;
    }

    #endregion

    static class DirectX
    {
        #region Variable Declaration

        public static CWExpert MainForm;
        public const float CLEAR_FLAG = -999.999F;				// for resetting buffers
        public const int BUFFER_SIZE = 2048;
        public static float[] new_display_data;					// Buffer used to store the new data from the DSP for the display
        public static float[] new_waterfall_data;    			// Buffer used to store the new data from the DSP for the waterfall
        public static float[] current_display_data;				// Buffer used to store the current data for the display
        public static float[] current_waterfall_data;		    // Buffer used to store the current data for the scope
        public static float[] waterfall_display_data;            // Buffer for waterfall

        public static float[] average_buffer;					// Averaged display data buffer for Panadapter
        public static float[] average_waterfall_buffer;  		// Averaged display data buffer for Waterfall
        public static float[] peak_buffer;						// Peak hold display data buffer

        public static string background_image = ".\\picDisplay.png";

        #endregion

        #region Properties

        private static DisplayMode current_display_mode = DisplayMode.PANAFALL;
        public static DisplayMode CurrentDisplayMode // changes yt7pwr
        {
            get { return current_display_mode; }
            set
            {
                current_display_mode = value;
                if (average_on) ResetDisplayAverage();
                if (peak_on) ResetDisplayPeak();
            }
        }

        private static bool refresh_panadapter_grid = false;                 // yt7pwr
        public static bool RefreshPanadapterGrid
        {
            set { refresh_panadapter_grid = value; }
        }

        private static ColorSheme color_sheme = ColorSheme.enhanced;        // yt7pwr
        public static ColorSheme ColorSheme
        {
            get { return color_sheme; }

            set { color_sheme = value; }
        }

        private static bool reverse_waterfall = true;                      // yt7pwr
        public static bool ReverseWaterfall
        {
            get { return reverse_waterfall; }
            set { reverse_waterfall = value; }
        }

        public static bool smooth_line = false;                             // yt7pwr
        public static bool pan_fill = false;

        private static System.Drawing.Font pan_font = new System.Drawing.Font("Arial", 12);
        public static System.Drawing.Font PanFont
        {
            get { return pan_font; }
            set
            {
                pan_font = value;
                refresh_panadapter_grid = true;
                if (!MainForm.booting)
                {
                    if (panadapter_font != null)
                        panadapter_font.Dispose();
                    panadapter_font = new SlimDX.Direct3D9.Font(device, pan_font);
                }
            }
        }

        private static SlimDX.Direct3D9.Font panadapter_font = null;
        public static SlimDX.Direct3D9.Font PanadapterFont
        {
            get { return panadapter_font; }
            set
            {
                panadapter_font = value;
            }
        }

        private static Color pan_fill_color = Color.FromArgb(100, 0, 0, 127);
        public static Color PanFillColor
        {
            get { return pan_fill_color; }
            set { pan_fill_color = value; }
        }

        private static Color display_text_background = Color.FromArgb(255, 127, 127, 127);
        public static Color DisplayTextBackground
        {
            get { return display_text_background; }
            set { display_text_background = value; }
        }

        private static Color display_filter_color = Color.FromArgb(65, 255, 255, 255);
        public static Color DisplayFilterColor
        {
            get { return display_filter_color; }
            set { display_filter_color = value; }
        }

        private static bool show_horizontal_grid = true;
        public static bool Show_Horizontal_Grid
        {
            set
            {
                show_horizontal_grid = value;
                refresh_panadapter_grid = true;
            }
        }

        private static bool show_vertical_grid = true;
        public static bool Show_Vertical_Grid
        {
            set
            {
                show_vertical_grid = value;
                refresh_panadapter_grid = true;
            }
        }

        private static int waterfall_update_period = 50; // in ms
        public static int WaterfallUpdatePeriod
        {
            get { return waterfall_update_period; }
            set { waterfall_update_period = value; }
        }

        private static Color main_rx_zero_line_color = Color.LightSkyBlue;
        public static Color MainRXZeroLine
        {
            get { return main_rx_zero_line_color; }
            set
            {
                main_rx_zero_line_color = value;
            }
        }

        private static Color sub_rx_zero_line_color = Color.LightSkyBlue;
        public static Color SubRXZeroLine
        {
            get { return sub_rx_zero_line_color; }
            set
            {
                sub_rx_zero_line_color = value;
            }
        }

        private static Color main_rx_filter_color = Color.FromArgb(100, 0, 255, 0);  // green
        public static Color MainRXFilterColor
        {
            get { return main_rx_filter_color; }
            set
            {
                main_rx_filter_color = value;
            }
        }

        private static Color sub_rx_filter_color = Color.FromArgb(100, 0, 0, 255);  // blue
        public static Color SubRXFilterColor
        {
            get { return sub_rx_filter_color; }
            set
            {
                sub_rx_filter_color = value;
            }
        }

        private static bool sub_rx_enabled = false;
        public static bool SubRXEnabled
        {
            get { return sub_rx_enabled; }
            set
            {
                sub_rx_enabled = value;
            }
        }

        private static bool split_enabled = false;
        public static bool SplitEnabled
        {
            get { return split_enabled; }
            set
            {
                split_enabled = value;
            }
        }

        private static bool show_freq_offset = false;
        public static bool ShowFreqOffset
        {
            get { return show_freq_offset; }
            set
            {
                show_freq_offset = value;
            }
        }

        private static Color band_edge_color = Color.Red;
        public static Color BandEdgeColor
        {
            get { return band_edge_color; }
            set
            {
                band_edge_color = value;
            }
        }

        private static long losc_hz; // yt7pwr
        public static long LOSC
        {
            get { return losc_hz; }
            set { losc_hz = value; }
        }

        private static long vfoa_hz;
        public static long VFOA
        {
            get { return vfoa_hz; }
            set
            {
                vfoa_hz = value;
            }
        }

        private static long vfob_hz;
        public static long VFOB
        {
            get { return vfob_hz; }
            set
            {
                vfob_hz = value;
            }
        }

        private static int rit_hz;
        public static int RIT
        {
            get { return rit_hz; }
            set
            {
                rit_hz = value;
            }
        }

        private static int xit_hz;
        public static int XIT
        {
            get { return xit_hz; }
            set
            {
                xit_hz = value;
            }
        }

        private static int cw_pitch = 600;
        public static int CWPitch
        {
            get { return cw_pitch; }
            set { cw_pitch = value; }
        }

        private static int panadapter_H = 0;	// target height
        private static int panadapter_W = 0;	// target width
        private static Control panadapter_target = null;
        public static Control PanadapterTarget
        {
            get { return panadapter_target; }
            set
            {
                panadapter_target = value;
                panadapter_H = panadapter_target.Height;
                panadapter_W = panadapter_target.Width;
            }
        }

        private static int waterfall_H = 0;	// target height
        private static int waterfall_W = 0;	// target width
        private static Control waterfall_target = null;
        public static Control WaterfallTarget
        {
            get { return waterfall_target; }
            set
            {
                waterfall_target = value;
                waterfall_H = waterfall_target.Height;
                waterfall_W = waterfall_target.Width;
            }
        }

        public static bool DisplayNotchFilter = false;
        private static int rx_display_notch_low_cut = -4000;        // yt7pwr
        public static int RXDisplayNotchLowCut
        {
            set { rx_display_notch_low_cut = value; }
        }

        private static int rx_display_notch_high_cut = 4000;        // yt7pwr
        public static int RXDisplayNotchHighCut
        {
            set { rx_display_notch_high_cut = value; }
        }

        private static int rx_display_low = 100;
        public static int RXDisplayLow
        {
            get { return rx_display_low; }
            set { rx_display_low = value; }
        }

        private static int rx_display_high = 4000;
        public static int RXDisplayHigh
        {
            get { return rx_display_high; }
            set { rx_display_high = value; }
        }

        private static float preamp_offset = 0.0f;
        public static float PreampOffset
        {
            get { return preamp_offset; }
            set { preamp_offset = value; }
        }

        private static float display_cal_offset;					// display calibration offset per volume setting in dB
        public static float DisplayCalOffset
        {
            get { return display_cal_offset; }
            set { display_cal_offset = value; }
        }

        private static int display_cursor_x;						// x-coord of the cursor when over the display
        public static int DisplayCursorX
        {
            get { return display_cursor_x; }
            set { display_cursor_x = value; }
        }

        private static int display_cursor_y;						// y-coord of the cursor when over the display
        public static int DisplayCursorY
        {
            get { return display_cursor_y; }
            set { display_cursor_y = value; }
        }

        private static int scope_time = 50;
        public static int ScopeTime
        {
            get { return scope_time; }
            set { scope_time = value; }
        }

        private static int sample_rate = 48000;
        public static int SampleRate
        {
            get { return sample_rate; }
            set { sample_rate = value; }
        }

        private static bool high_swr = false;
        public static bool HighSWR
        {
            get { return high_swr; }
            set { high_swr = value; }
        }

        private static bool mox = false;
        public static bool MOX
        {
            get { return mox; }
            set { mox = value; }
        }

        private static float max_x;								// x-coord of maxmimum over one display pass
        public static float MaxX
        {
            get { return max_x; }
            set { max_x = value; }
        }

        private static float scope_max_x;								// x-coord of maxmimum over one display pass
        public static float ScopeMaxX
        {
            get { return scope_max_x; }
            set { scope_max_x = value; }
        }

        private static float max_y;								// y-coord of maxmimum over one display pass
        public static float MaxY
        {
            get { return max_y; }
            set { max_y = value; }
        }

        private static float scope_max_y;								// y-coord of maxmimum over one display pass
        public static float ScopeMaxY
        {
            get { return scope_max_y; }
            set { scope_max_y = value; }
        }

        private static bool average_on = true;							// True if the Average button is pressed
        public static bool AverageOn
        {
            get { return average_on; }
            set
            {
                average_on = value;
                if (!average_on) ResetDisplayAverage();
            }
        }

        private static bool peak_on;							// True if the Peak button is pressed
        public static bool PeakOn
        {
            get { return peak_on; }
            set
            {
                peak_on = value;
                if (!peak_on) ResetDisplayPeak();
            }
        }

        public static bool scope_data_ready = false;
        private static bool data_ready = false;			// True when there is new display data ready from the DSP
        public static bool DataReady
        {
            get { return data_ready; }
            set { data_ready = value; }
        }

        private static bool waterfall_data_ready;	    // True when there is new display data ready from the DSP
        public static bool WaterfallDataReady
        {
            get { return waterfall_data_ready; }
            set { waterfall_data_ready = value; }
        }

        public static float display_avg_mult_old = 1 - (float)1 / 2;
        public static float display_avg_mult_new = (float)1 / 2;
        private static int display_avg_num_blocks = 2;
        public static int DisplayAvgBlocks
        {
            get { return display_avg_num_blocks; }
            set
            {
                display_avg_num_blocks = value;
                display_avg_mult_old = 1 - (float)1 / display_avg_num_blocks;
                display_avg_mult_new = (float)1 / display_avg_num_blocks;
            }
        }

        public static float waterfall_avg_mult_old = 1 - (float)1 / 18;
        public static float waterfall_avg_mult_new = (float)1 / 18;
        private static int waterfall_avg_num_blocks = 18;
        public static int WaterfallAvgBlocks
        {
            get { return waterfall_avg_num_blocks; }
            set
            {
                waterfall_avg_num_blocks = value;
                waterfall_avg_mult_old = 1 - (float)1 / waterfall_avg_num_blocks;
                waterfall_avg_mult_new = (float)1 / waterfall_avg_num_blocks;
            }
        }

        private static int spectrum_grid_max = 200;
        public static int SpectrumGridMax
        {
            get { return spectrum_grid_max; }
            set
            {
                spectrum_grid_max = value;
                refresh_panadapter_grid = true;
            }
        }

        private static int spectrum_grid_min = -200;
        public static int SpectrumGridMin
        {
            get { return spectrum_grid_min; }
            set
            {
                spectrum_grid_min = value;
                refresh_panadapter_grid = true;
            }
        }

        private static int spectrum_grid_step = 5;
        public static int SpectrumGridStep
        {
            get { return spectrum_grid_step; }
            set
            {
                spectrum_grid_step = value;
                refresh_panadapter_grid = true;
            }
        }

        private static Color grid_text_color = Color.Yellow;
        public static Color GridTextColor
        {
            get { return grid_text_color; }
            set
            {
                grid_text_color = value;
                refresh_panadapter_grid = true;
            }
        }

        private static Color grid_zero_color = Color.Red;
        public static Color GridZeroColor
        {
            get { return grid_zero_color; }
            set
            {
                grid_zero_color = value;
            }
        }

        private static Color grid_color = Color.FromArgb(100, Color.Blue);
        public static Color GridColor
        {
            get { return grid_color; }
            set
            {
                grid_color = Color.FromArgb(50, value);
                refresh_panadapter_grid = true;
            }
        }

        private static Pen data_line_pen = new Pen(new SolidBrush(Color.LightGreen), display_line_width);
        private static Color data_line_color = Color.LightGreen;
        public static Color DataLineColor
        {
            get { return data_line_color; }
            set
            {
                data_line_color = value;
                data_line_pen = new Pen(new SolidBrush(data_line_color), display_line_width);
            }
        }

        private static Color display_filter_tx_color = Color.Yellow;
        public static Color DisplayFilterTXColor
        {
            get { return display_filter_tx_color; }
            set
            {
                display_filter_tx_color = value;
            }
        }

        private static bool draw_tx_filter = false;
        public static bool DrawTXFilter
        {
            get { return draw_tx_filter; }
            set
            {
                draw_tx_filter = value;
            }
        }

        private static bool draw_tx_cw_freq = false;
        public static bool DrawTXCWFreq
        {
            get { return draw_tx_cw_freq; }
            set
            {
                draw_tx_cw_freq = value;
            }
        }

        private static Color display_background_color = Color.Black;
        public static Color DisplayBackgroundColor
        {
            get { return display_background_color; }
            set
            {
                display_background_color = value;
            }
        }

        private static Color waterfall_low_color = Color.Black;
        public static Color WaterfallLowColor
        {
            get { return waterfall_low_color; }
            set { waterfall_low_color = value; }
        }

        private static Color waterfall_mid_color = Color.Red;
        public static Color WaterfallMidColor
        {
            get { return waterfall_mid_color; }
            set { waterfall_mid_color = value; }
        }

        private static Color waterfall_high_color = Color.Yellow;
        public static Color WaterfallHighColor
        {
            get { return waterfall_high_color; }
            set { waterfall_high_color = value; }
        }

        private static float waterfall_high_threshold = -80.0F;
        public static float WaterfallHighThreshold
        {
            get { return waterfall_high_threshold; }
            set { waterfall_high_threshold = value; }
        }

        private static float waterfall_low_threshold = -110.0F;
        public static float WaterfallLowThreshold
        {
            get { return waterfall_low_threshold; }
            set { waterfall_low_threshold = value; }
        }

        private static float display_line_width = 1.0F;
        public static float DisplayLineWidth
        {
            get { return display_line_width; }
            set
            {
                display_line_width = value;
                data_line_pen = new Pen(new SolidBrush(data_line_color), display_line_width);
            }
        }

        #endregion

        #region Misc routine

        private static void UpdateDisplayPeak()
        {
            if (peak_buffer[0] == CLEAR_FLAG)
            {
                for (int i = 0; i < BUFFER_SIZE; i++)
                    peak_buffer[i] = current_display_data[i];
            }
            else
            {
                for (int i = 0; i < BUFFER_SIZE; i++)
                {
                    if (current_display_data[i] > peak_buffer[i])
                        peak_buffer[i] = current_display_data[i];
                    current_display_data[i] = peak_buffer[i];
                }
            }
        }

        public static void ResetDisplayAverage()
        {
            if (average_buffer != null)
            {
                average_buffer[0] = CLEAR_FLAG;	// set reset flag
                average_waterfall_buffer[0] = CLEAR_FLAG;
            }
        }

        public static void ResetDisplayPeak()
        {
            peak_buffer[0] = CLEAR_FLAG; // set reset flag
        }

        #endregion

        #region DirectX

        #region Variable Declaration

        private static SlimDX.Direct3D9.Device device = null;
        private static Texture PanadapterTexture = null;
        private static Texture WaterfallTexture = null;
        private static Sprite Panadapter_Sprite = null;
        private static Sprite Waterfall_Sprite = null;
        private static Rectangle Panadapter_texture_size;
        private static Rectangle Waterfall_texture_size;
        private static AutoResetEvent Panadapter_Event;
        private static AutoResetEvent Waterfall_Event;
        private static DXRectangle VFOArect;
        private static DXRectangle VFOBrect;
        private static VertexBuffer VerLine_vb = null;
        private static VertexBuffer HorLine_vb = null;
        private static VertexBuffer VerLines_vb = null;
        private static VertexBuffer HorLines_vb = null;
        private static VertexBuffer PanLine_vb = null;
        private static VertexBuffer ScopeLine_vb = null;
        private static VertexBuffer PanLine_vb_fill = null;
        private static Vertex[] PanLine_verts = null;
        private static Vertex[] ScopeLine_verts = null;
        private static Vertex[] PanLine_verts_fill = null;
        private static Vertex[] Phase_verts = null;
        private static VertexBuffer Phase_vb = null;      
        private static float[] waterfallX_data = null;
        private static float[] panadapterX_data = null;
        private static float[] panadapterX_scope_data = null;
        private static Bitmap waterfall_bmp = null;
        private static Device waterfall_dx_device = null;
        private static Point[] points = null;

        #endregion

        #region Routines

        public static bool DirectXInit()
        {
            if (!MainForm.booting)
            {
                try
                {
                    if (!MainForm.booting)
                    {
                        switch (current_display_mode)
                        {
                            case DisplayMode.PANADAPTER:
                            case DisplayMode.PANAFALL:
                                panadapter_target = (Control)MainForm.picPanadapter;
                                panadapter_W = panadapter_target.Width;
                                panadapter_H = panadapter_target.Height;
                                waterfall_target = (Control)MainForm.picWaterfall;
                                waterfall_H = waterfall_target.Height;
                                waterfall_W = waterfall_target.Width;
                                waterfallX_data = new float[waterfall_W];
                                panadapterX_scope_data = new float[waterfall_W];
                                break;
                            case DisplayMode.WATERFALL:
                                waterfall_target = (Control)MainForm.picWaterfall;
                                break;
                            default:
                                panadapter_H = panadapter_target.Height;
                                panadapter_W = panadapter_target.Width;
                                panadapterX_scope_data = new float[panadapter_W];
                                panadapter_target = (Control)MainForm.picPanadapter;
                                break;
                        }

                        panadapterX_data = new float[panadapter_W];
                        refresh_panadapter_grid = true;

                        average_buffer = new float[BUFFER_SIZE];	// initialize averaging buffer array
                        average_buffer[0] = CLEAR_FLAG;		// set the clear flag

                        average_waterfall_buffer = new float[BUFFER_SIZE];	// initialize averaging buffer array
                        average_waterfall_buffer[0] = CLEAR_FLAG;		// set the clear flag

                        peak_buffer = new float[BUFFER_SIZE];
                        peak_buffer[0] = CLEAR_FLAG;

                        new_display_data = new float[BUFFER_SIZE];
                        new_waterfall_data = new float[BUFFER_SIZE];
                        current_display_data = new float[BUFFER_SIZE];
                        current_waterfall_data = new float[BUFFER_SIZE];
                        waterfall_display_data = new float[BUFFER_SIZE];

                        for (int i = 0; i < BUFFER_SIZE; i++)
                        {
                            new_display_data[i] = -200.0f;
                            new_waterfall_data[i] = -200.0f;
                            current_display_data[i] = -200.0f;
                            current_waterfall_data[i] = -200.0f;
                            waterfall_display_data[i] = -200.0f;
                        }

                        PresentParameters presentParms = new PresentParameters();
                        presentParms.Windowed = true;
                        presentParms.SwapEffect = SwapEffect.Discard;
                        presentParms.BackBufferFormat = SlimDX.Direct3D9.Format.Unknown;
                        presentParms.BackBufferHeight = panadapter_target.Height;
                        presentParms.BackBufferWidth = panadapter_target.Width;
                        presentParms.BackBufferCount = 1;

                        try
                        {
                            device = new Device(new Direct3D(), 0, DeviceType.Hardware,
                                panadapter_target.Handle, CreateFlags.HardwareVertexProcessing |
                                CreateFlags.FpuPreserve | CreateFlags.Multithreaded, presentParms);

                            waterfall_dx_device = new Device(new Direct3D(), 0,
                                DeviceType.Hardware, waterfall_target.Handle,
                                CreateFlags.HardwareVertexProcessing | CreateFlags.FpuPreserve |
                                 CreateFlags.Multithreaded, presentParms);
                        }
                        catch (Direct3D9Exception ex)
                        {
                            MessageBox.Show("DirectX hardware init error!\n" + ex.ToString());

                            try
                            {
                                device = new Device(new Direct3D(), 0,
                                    DeviceType.Hardware,
                                    panadapter_target.Handle, CreateFlags.SoftwareVertexProcessing |
                                    CreateFlags.FpuPreserve, presentParms);

                                waterfall_dx_device = new Device(new Direct3D(), 0,
                                    DeviceType.Hardware, waterfall_target.Handle,
                                    CreateFlags.SoftwareVertexProcessing | CreateFlags.FpuPreserve, presentParms);

                            }
                            catch (Direct3D9Exception exe)
                            {
                                MessageBox.Show("DirectX software init error!\n" + exe.ToString());
                                return false;
                            }
                        }

                        var vertexElems = new[] {
                        new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                        new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                        VertexElement.VertexDeclarationEnd
                        };

                        var vertexDecl = new VertexDeclaration(device, vertexElems);
                        device.VertexDeclaration = vertexDecl;
                        var vertexDecl1 = new VertexDeclaration(waterfall_dx_device, vertexElems);
                        waterfall_dx_device.VertexDeclaration = vertexDecl1;

                        waterfall_bmp = new Bitmap(waterfall_target.Width, waterfall_target.Height,
                            System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                        panadapter_font = new SlimDX.Direct3D9.Font(device, pan_font);

                        if (File.Exists(background_image))
                        {
                            PanadapterTexture = Texture.FromFile(device, background_image, panadapter_target.Width, panadapter_target.Height,
                                1, Usage.None, Format.Unknown, Pool.Default, SlimDX.Direct3D9.Filter.Default, SlimDX.Direct3D9.Filter.Default, 0);

                            WaterfallTexture = Texture.FromFile(waterfall_dx_device, background_image, waterfall_target.Width, waterfall_target.Height,
                                1, Usage.None, Format.Unknown, Pool.Default, SlimDX.Direct3D9.Filter.Default, SlimDX.Direct3D9.Filter.Default, 0);
                            Panadapter_texture_size.Width = panadapter_target.Width;
                            Panadapter_texture_size.Height = panadapter_target.Height;
                            Panadapter_Sprite = new Sprite(device);

                            Waterfall_texture_size.Width = waterfall_target.Width;
                            Waterfall_texture_size.Height = waterfall_target.Height;
                            Waterfall_Sprite = new Sprite(waterfall_dx_device);
                        }
                        else
                        {
                            Panadapter_Sprite = null;
                            Waterfall_Sprite = null;
                        }

                        if (Panadapter_Event == null)
                            Panadapter_Event = new AutoResetEvent(true);
                        if (Waterfall_Event == null)
                            Waterfall_Event = new AutoResetEvent(true);

                        PanLine_vb = new VertexBuffer(device, panadapterX_data.Length * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                        PanLine_vb_fill = new VertexBuffer(device, panadapter_W * 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
                        PanLine_verts = new Vertex[panadapter_W];
                        PanLine_verts_fill = new Vertex[panadapter_W * 2];

                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("DirectX init general fault!\n" + ex.ToString());
                    return false;
                }
            }
            return false;
        }

        public static void DirectXRelease()
        {
            try
            {
                if (!MainForm.booting)
                {
                    waterfallX_data = null;
                    panadapterX_data = null;

                    new_display_data = null;
                    new_waterfall_data = null;
                    current_display_data = null;
                    current_waterfall_data = null;
                    waterfall_display_data = null;

                    average_buffer = null;
                    average_waterfall_buffer = null;

                    peak_buffer = null;
                    if (waterfall_bmp != null)
                        waterfall_bmp.Dispose();
                    waterfall_bmp = null;
                    if (Panadapter_Sprite != null)
                        Panadapter_Sprite.Dispose();
                    Panadapter_Sprite = null;
                    if (Waterfall_Sprite != null)
                        Waterfall_Sprite.Dispose();
                    Waterfall_Sprite = null;

                    if (PanadapterTexture != null)
                    {
                        PanadapterTexture.Dispose();
                        PanadapterTexture = null;
                    }

                    if (WaterfallTexture != null)
                    {
                        WaterfallTexture.Dispose();
                        WaterfallTexture = null;
                    }

                    if (Panadapter_Event != null)
                    {
                        Panadapter_Event.Close();
                        Panadapter_Event = null;
                    }

                    if (Waterfall_Event != null)
                    {
                        Waterfall_Event.Close();
                        Waterfall_Event = null;
                    }

                    if (VerLine_vb != null)
                    {
                        VerLine_vb.Dispose();
                        VerLine_vb = null;
                    }

                    if (VerLines_vb != null)
                    {
                        VerLines_vb.Dispose();
                        VerLines_vb = null;
                    }

                    if (HorLine_vb != null)
                    {
                        HorLine_vb.Dispose();
                        HorLine_vb = null;
                    }

                    if (HorLines_vb != null)
                    {
                        HorLines_vb.Dispose();
                        HorLines_vb = null;
                    }

                    if (PanLine_vb != null)
                    {
                        PanLine_vb.Dispose();
                        PanLine_vb.Dispose();
                    }

                    if (PanLine_vb_fill != null)
                    {
                        PanLine_vb_fill.Dispose();
                        PanLine_vb_fill.Dispose();
                    }

                    if (vertical_label != null)
                        vertical_label = null;

                    if (horizontal_label != null)
                        horizontal_label = null;

                    if (Phase_vb != null)
                    {
                        Phase_vb.Dispose();
                        Phase_vb.Dispose();
                    }

                    if (device != null)
                    {
                        device.Dispose();
                        device = null;
                    }

                    if (waterfall_dx_device != null)
                    {
                        waterfall_dx_device.Dispose();
                        waterfall_dx_device = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write("DX release error!" + ex.ToString());
            }
        }

        public static void Render_VFOA()  // yt7pwr
        {
            int low = rx_display_low;					// initialize variables
            int high = rx_display_high;
            int filter_low, filter_high;
            int[] step_list = { 10, 20, 25, 50 };
            int step_power = 1;
            int step_index = 0;
            int freq_step_size = 50;
            int filter_left = 0;
            int filter_right = 0;
            int notch_low = 0;
            int notch_high = 0;

                filter_low = 200;
                filter_high = 4000;

            // Calculate horizontal step size
            int width = high - low;
            while (width / freq_step_size > 10)
            {
                freq_step_size = step_list[step_index] * (int)Math.Pow(10.0, step_power);
                step_index = (step_index + 1) % 4;
                if (step_index == 0) step_power++;
            }

            int w_steps = width / freq_step_size;

            // calculate vertical step size
            int h_steps = (spectrum_grid_max - spectrum_grid_min) / spectrum_grid_step;
            double h_pixel_step = (double)panadapter_H / h_steps;

            if (!mox)
            {
                            // get filter screen coordinates
                            filter_left = (int)((float)(filter_low - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                            filter_right = (int)((float)(filter_high - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);

                            // make the filter display at least one pixel wide.
                            if (filter_left == filter_right) filter_right = filter_left + 1;

                            // draw Main RX 0Hz line
                            int main_rx_zero_line = (int)((float)(vfoa_hz + rit_hz - losc_hz - low) / (high - low) * panadapter_W);
                            RenderVerticalLine(device, main_rx_zero_line, panadapter_H, main_rx_zero_line_color);
                            VFOArect.x1 = filter_right;
                            VFOArect.y1 = (int)(pan_font.Size);
                            VFOArect.x2 = filter_right;
                            VFOArect.y2 = panadapter_H;
                            VFOArect.x3 = filter_left;
                            VFOArect.y3 = (int)(pan_font.Size);
                            VFOArect.x4 = filter_left;
                            VFOArect.y4 = panadapter_H;
                            RenderRectangle(device, VFOArect, main_rx_filter_color);

                            if (DisplayNotchFilter)
                            {
                                // get filter screen coordinates
                                if (filter_high < 0 && filter_low < 0)
                                {
                                    filter_left = (int)((float)(-notch_high - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                    filter_right = (int)((float)(-notch_low - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                }
                                else
                                {
                                    filter_left = (int)((float)(notch_low - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                    filter_right = (int)((float)(notch_high - low + vfoa_hz + rit_hz - losc_hz) / (high - low) * panadapter_W);
                                }

                                VFOArect.x1 = filter_right;
                                VFOArect.y1 = (int)(pan_font.Size);
                                VFOArect.x2 = filter_right;
                                VFOArect.y2 = panadapter_H;
                                VFOArect.x3 = filter_left;
                                VFOArect.y3 = (int)(pan_font.Size);
                                VFOArect.x4 = filter_left;
                                VFOArect.y4 = panadapter_H;
                                RenderRectangle(device, VFOArect, Color.FromArgb(main_rx_filter_color.A / 2, main_rx_filter_color.R / 2,
                                    main_rx_filter_color.G / 2, main_rx_filter_color.B / 2));
                            }
            }
        }

        private static void RenderRectangle(Device dev, DXRectangle rect, Color color)
        {
            Vertex[] verts = new Vertex[4];

            var vb = new VertexBuffer(dev, 4 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);

            verts[0] = new Vertex();
            verts[0].Color = color.ToArgb();
            verts[0].Position = new Vector4(rect.x1, rect.y1, 0.0f, 0.0f);
            verts[1] = new Vertex();
            verts[1].Color = color.ToArgb();
            verts[1].Position = new Vector4(rect.x2, rect.y2, 0.0f, 0.0f);
            verts[2] = new Vertex();
            verts[2].Color = color.ToArgb();
            verts[2].Position = new Vector4(rect.x3, rect.y3, 0.0f, 0.0f);
            verts[3] = new Vertex();
            verts[3].Color = color.ToArgb();
            verts[3].Position = new Vector4(rect.x4, rect.y4, 0.0f, 0.0f);

            vb.Lock(0, 0, LockFlags.None).WriteRange(verts, 0, 4);
            vb.Unlock();
            device.SetStreamSource(0, vb, 0, 20);
            device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

            vb.Dispose();
        }

        private static void RenderVerticalLines(Device dev, VertexBuffer vertex, int count)         // yt7pwr
        {
            dev.SetStreamSource(0, vertex, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineList, 0, count);
        }

        private static void RenderHorizontalLines(Device dev, VertexBuffer vertex, int count)        // yt7pwr
        {
            dev.SetStreamSource(0, vertex, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineList, 0, count);
        }

        private static void RenderVerticalLine(Device dev, int x, int y, Color color)                // yt7pwr
        {
            var vb = new VertexBuffer(dev, 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);

            vb.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { Color = color.ToArgb(), Position = new Vector4((float)x, (float)(pan_font.Size), 0.0f, 0.0f) },
                new Vertex() { Color = color.ToArgb(), Position = new Vector4((float)x, (float)y, 0.0f, 0.0f) }
                 });
            vb.Unlock();

            dev.SetStreamSource(0, vb, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineList, 0, 1);

            vb.Dispose();
        }

        private static void RenderHorizontalLine(Device dev, int x, int y, Color color)              // yt7pwr
        {
            var vb = new VertexBuffer(dev, 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);

            vb.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { Color = color.ToArgb(), Position = new Vector4((float)x, (float)y, 0.0f, 0.0f) },
                new Vertex() { Color = color.ToArgb(), Position = new Vector4((float)panadapter_W, (float)y, 0.0f, 0.0f) }
                 });
            vb.Unlock();

            dev.SetStreamSource(0, vb, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineList, 0, 1);

            vb.Dispose();
        }

        private static void RenderScopeLine(Device dev, int count)        // yt7pwr
        {
            for (int i = 0; i < count; i++)
            {
                ScopeLine_verts[i] = new Vertex();
                ScopeLine_verts[i].Color = data_line_color.ToArgb();
                ScopeLine_verts[i].Position = new Vector4(i, panadapterX_scope_data[i], 0.0f, 0.0f);
            }

            ScopeLine_vb.Lock(0, 0, LockFlags.None).WriteRange(ScopeLine_verts, 0, count);
            ScopeLine_vb.Unlock();

            dev.SetStreamSource(0, ScopeLine_vb, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineStrip, 0, count - 1);
        }

        private static void RenderPanadapterLine(Device dev)        // yt7pwr
        {
            if (pan_fill && (current_display_mode == DisplayMode.PANADAPTER || current_display_mode == DisplayMode.PANAFALL))
            {
                int j = 0;
                int i = 0;

                for (i = 0; i < panadapter_W * 2; i++)
                {
                    PanLine_verts_fill[i] = new Vertex();
                    PanLine_verts_fill[i].Color = pan_fill_color.ToArgb();
                    PanLine_verts_fill[i].Position = new Vector4(i / 2, panadapterX_data[j], 0.0f, 0.0f);
                    PanLine_verts_fill[i + 1] = new Vertex();
                    PanLine_verts_fill[i + 1].Color = pan_fill_color.ToArgb();
                    PanLine_verts_fill[i + 1].Position = new Vector4(i / 2, panadapter_H, 0.0f, 0.0f);
                    i++;
                    j++;
                }

                PanLine_vb_fill.Lock(0, 0, LockFlags.None).WriteRange(PanLine_verts_fill, 0, panadapter_W * 2);
                PanLine_vb_fill.Unlock();

                dev.SetStreamSource(0, PanLine_vb_fill, 0, 20);
                dev.DrawPrimitives(PrimitiveType.LineList, 0, panadapter_W);
            }

            for (int i = 0; i < panadapter_W; i++)
            {
                PanLine_verts[i] = new Vertex();
                PanLine_verts[i].Color = data_line_color.ToArgb();
                PanLine_verts[i].Position = new Vector4(i, panadapterX_data[i], 0.0f, 0.0f);
            }

            PanLine_vb.Lock(0, 0, LockFlags.None).WriteRange(PanLine_verts, 0, panadapter_W);
            PanLine_vb.Unlock();

            dev.SetStreamSource(0, PanLine_vb, 0, 20);
            dev.DrawPrimitives(PrimitiveType.LineStrip, 0, panadapter_W - 1);
        }

        private static int h_steps = 0;
        private static VerticalString[] vertical_label;
        private static int vgrid;
        private static HorizontalString[] horizontal_label;
        private static void RenderPanadapterGrid(int W, int H)      // yt7pwr
        {
            int low = rx_display_low;					// initialize variables
            int high = rx_display_high;
            int mid_w = W / 2;
            int[] step_list = { 10, 20, 25, 50 };
            int step_power = 1;
            int step_index = 0;
            int freq_step_size = 50;
            int y_range = spectrum_grid_max - spectrum_grid_min;
            int center_line_x = W / 2;

            // Calculate horizontal step size
            int width = high - low;
            while (width / freq_step_size > 10)
            {
                freq_step_size = step_list[step_index] * (int)Math.Pow(10.0, step_power);
                step_index = (step_index + 1) % 4;
                if (step_index == 0) step_power++;
            }

            // calculate vertical step size
            h_steps = 8;

            double vfo;
                vfo = losc_hz; //  +rit_hz;

            long vfo_round = ((long)(vfo / freq_step_size)) * freq_step_size;
            long vfo_delta = (long)(vfo - vfo_round);

            if (VerLines_vb == null)
                VerLines_vb = new VertexBuffer(device, 65 * 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            if (HorLines_vb == null)
                HorLines_vb = new VertexBuffer(device, h_steps * 2 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Default);
            if (vertical_label == null)
                vertical_label = new VerticalString[16 + 1];
            if (horizontal_label == null)
                horizontal_label = new HorizontalString[h_steps];

            double actual_fgrid = 0;

            vgrid = 0;
            vertical_label[0].label = actual_fgrid.ToString();
            vertical_label[0].label = "0";
            vertical_label[0].pos_x = 5;
            vertical_label[0].pos_y = 0;
            vertical_label[0].color = grid_text_color;

            VerLines_vb.Lock(0, 40, LockFlags.None).WriteRange(new[] {
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4(0.0f, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4(0.0f, (float)H, 0.0f, 0.0f) },
                    });
            VerLines_vb.Unlock();

            for (int i = 1; i < 12; i++)
            {
                vgrid += panadapter_W / 12;
                actual_fgrid += panadapter_W / 12;
                vertical_label[i].label = (i * 325).ToString();
                vertical_label[i].pos_x = vgrid;
                vertical_label[i].pos_y = 0;
                vertical_label[i].color = grid_text_color;
            }

            vgrid = 0;
            // Draw vertical lines
            for (int i = 1; i <= 65; i++)
            {
                vgrid += panadapter_W / 65;

                VerLines_vb.Lock(i * 40, 40, LockFlags.None).WriteRange(new[] {
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4((float)vgrid, (float)pan_font.Height, 0.0f, 0.0f) },
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4((float)vgrid, (float)H, 0.0f, 0.0f) },
                    });
                VerLines_vb.Unlock();

                RenderVerticalLine(device, vgrid, H, grid_color);
            }

            // Draw horizontal lines
            for (int i = 1; i < h_steps; i++)
            {
                int xOffset = 0;
                int num = spectrum_grid_max - i * spectrum_grid_step;
                int y = (int)((double)(spectrum_grid_max - num) * H / y_range); // +(int)pan_font.Size;
                if (show_horizontal_grid)
                {
                    HorLines_vb.Lock(i * 40, 40, LockFlags.None).WriteRange(new[] {
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4(0.0f, (float)y, 0.0f, 0.0f) },
                        new Vertex() { Color = grid_color.ToArgb(), Position = new Vector4((float)W, (float)y, 0.0f, 0.0f) },
                    });
                    HorLines_vb.Unlock();

                    RenderHorizontalLine(device, 0, y, grid_color);
                }

                // Draw horizontal line labels
                num = spectrum_grid_max - i * spectrum_grid_step;
                horizontal_label[i].label = num.ToString();
                if (horizontal_label[i].label.Length == 3) xOffset = 5;
                //int offset = (int)(label.Length*4.1);

                int x = 0;
                x = xOffset + 3;
                y -= 8;
                if (y + 9 < H)
                {
                    panadapter_font.DrawString(null, horizontal_label[i].label, x, y, grid_text_color.ToArgb());
                    horizontal_label[i].pos_x = x;
                    horizontal_label[i].pos_y = y;
                    horizontal_label[i].color = grid_text_color;
                }
            }
        }


        private static void ConvertDataForWaterfalll()
        {
            if (device == null) return;

        }

        private static HiPerfTimer timer_waterfall = new HiPerfTimer();
        private static float[] waterfall_data;
        unsafe static public bool RenderWaterfall(Graphics g, int W, int H)    // yt7pwr
        {
            if (waterfall_data == null || waterfall_data.Length < W)
                waterfall_data = new float[W];			                    // array of points to display
            float slope = 0.0F;						                        // samples to process per pixel
            int num_samples = 0;					                        // number of samples to process
            int start_sample_index = 0;				                        // index to begin looking at samples
            int low = 0;
            int high = 0;
            low = rx_display_low;
            high = rx_display_high;
            max_y = Int32.MinValue;
            int R = 0, G = 0, B = 0;	                                	// variables to save Red, Green and Blue component values
            int i = 0;

            if (MainForm.MRIsRunning)
            {
                int yRange = spectrum_grid_max - spectrum_grid_min;

                if (waterfall_data_ready)
                {
                    // get new data
                    fixed (void* rptr = &new_waterfall_data[0])
                    fixed (void* wptr = &current_waterfall_data[0])
                        CWExpert.memcpy(wptr, rptr, BUFFER_SIZE * sizeof(float));
                    waterfall_data_ready = false;
                }

                if (average_on)
                    UpdateDirectXDisplayWaterfallAverage();
                if (peak_on)
                    UpdateDisplayPeak();

                timer_waterfall.Stop();
                if (timer_waterfall.DurationMsec > waterfall_update_period)
                {
                    timer_waterfall.Start();
                    num_samples = (high - low);

                    start_sample_index = 0; // (BUFFER_SIZE >> 1) + (int)((low * BUFFER_SIZE) / Audio.SampleRate);
                    num_samples = 2048; // (int)((high - low) * BUFFER_SIZE / Audio.SampleRate);
                    start_sample_index = (start_sample_index + 2048) % 2048;
                    if ((num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                        num_samples = BUFFER_SIZE - start_sample_index;

                    slope = (float)num_samples / (float)W;
                    for (i = 0; i < W; i++)
                    {
                        float max = float.MinValue;
                        float dval = i * slope + start_sample_index;
                        int lindex = (int)Math.Floor(dval);
                        int rindex = (int)Math.Floor(dval + slope);

                        if (slope <= 1 || lindex == rindex)
                            max = current_waterfall_data[lindex] * ((float)lindex - dval + 1) +
                                current_waterfall_data[(lindex + 1) % 2048] * (dval - (float)lindex);
                        else
                        {
                            for (int j = lindex; j < rindex; j++)
                                if (current_waterfall_data[j % 2048] > max) max = current_waterfall_data[j % 2048];
                        }

                        max += display_cal_offset;
                        if (!mox) max += preamp_offset;

                        if (max > max_y)
                        {
                            max_y = max;
                            max_x = i;
                        }

                        waterfall_data[i] = max;
                    }

                    BitmapData bitmapData = waterfall_bmp.LockBits(
                            new Rectangle(0, 0, waterfall_bmp.Width, waterfall_bmp.Height),
                            ImageLockMode.ReadWrite,
                            waterfall_bmp.PixelFormat);

                    int pixel_size = 3;
                    byte* row = null;

                        if (reverse_waterfall)
                        {
                            // first scroll image up
                            int total_size = bitmapData.Stride * bitmapData.Height;		// find buffer size
                            CWExpert.memcpy(bitmapData.Scan0.ToPointer(),
                                new IntPtr((int)bitmapData.Scan0 + bitmapData.Stride).ToPointer(),
                                total_size - bitmapData.Stride);

                            row = (byte*)(bitmapData.Scan0.ToInt32() + total_size - bitmapData.Stride);
                        }
                        else
                        {
                            // first scroll image down
                            int total_size = bitmapData.Stride * bitmapData.Height;		// find buffer size
                             CWExpert.memcpy(new IntPtr((int)bitmapData.Scan0 + bitmapData.Stride).ToPointer(),
                                bitmapData.Scan0.ToPointer(),
                                total_size - bitmapData.Stride);

                            row = (byte*)(bitmapData.Scan0.ToInt32());
                        }

                        switch (color_sheme)
                        {
                            case (ColorSheme.original):                        // tre color only
                                {
                                    // draw new data
                                    for (i = 0; i < W; i++)	// for each pixel in the new line
                                    {
                                        if (waterfall_data[i] <= waterfall_low_threshold)		// if less than low threshold, just use low color
                                        {
                                            R = WaterfallLowColor.R;
                                            G = WaterfallLowColor.G;
                                            B = WaterfallLowColor.B;
                                        }
                                        else if (waterfall_data[i] >= WaterfallHighThreshold)// if more than high threshold, just use high color
                                        {
                                            R = WaterfallHighColor.R;
                                            G = WaterfallHighColor.G;
                                            B = WaterfallHighColor.B;
                                        }
                                        else // use a color between high and low
                                        {
                                            float percent = (waterfall_data[i] - waterfall_low_threshold) / (WaterfallHighThreshold - waterfall_low_threshold);
                                            if (percent <= 0.5)	// use a gradient between low and mid colors
                                            {
                                                percent *= 2;

                                                R = (int)((1 - percent) * WaterfallLowColor.R + percent * WaterfallMidColor.R);
                                                G = (int)((1 - percent) * WaterfallLowColor.G + percent * WaterfallMidColor.G);
                                                B = (int)((1 - percent) * WaterfallLowColor.B + percent * WaterfallMidColor.B);
                                            }
                                            else				// use a gradient between mid and high colors
                                            {
                                                percent = (float)(percent - 0.5) * 2;

                                                R = (int)((1 - percent) * WaterfallMidColor.R + percent * WaterfallHighColor.R);
                                                G = (int)((1 - percent) * WaterfallMidColor.G + percent * WaterfallHighColor.G);
                                                B = (int)((1 - percent) * WaterfallMidColor.B + percent * WaterfallHighColor.B);
                                            }
                                        }

                                        // set pixel color
                                        row[i * pixel_size + 0] = (byte)B;	// set color in memory
                                        row[i * pixel_size + 1] = (byte)G;
                                        row[i * pixel_size + 2] = (byte)R;
                                    }
                                }
                                break;

                            case (ColorSheme.enhanced): // SV1EIO
                                {
                                    // draw new data
                                    for (i = 0; i < W; i++)	// for each pixel in the new line
                                    {
                                        if (waterfall_data[i] <= waterfall_low_threshold)
                                        {
                                            R = WaterfallLowColor.R;
                                            G = WaterfallLowColor.G;
                                            B = WaterfallLowColor.B;
                                        }
                                        else if (waterfall_data[i] >= WaterfallHighThreshold)
                                        {
                                            R = 192;
                                            G = 124;
                                            B = 255;
                                        }
                                        else // value is between low and high
                                        {
                                            float range = WaterfallHighThreshold - waterfall_low_threshold;
                                            float offset = waterfall_data[i] - waterfall_low_threshold;
                                            float overall_percent = offset / range; // value from 0.0 to 1.0 where 1.0 is high and 0.0 is low.

                                            if (overall_percent < (float)2 / 9) // background to blue
                                            {
                                                float local_percent = overall_percent / ((float)2 / 9);
                                                R = (int)((1.0 - local_percent) * WaterfallLowColor.R);
                                                G = (int)((1.0 - local_percent) * WaterfallLowColor.G);
                                                B = (int)(WaterfallLowColor.B + local_percent * (255 - WaterfallLowColor.B));
                                            }
                                            else if (overall_percent < (float)3 / 9) // blue to blue-green
                                            {
                                                float local_percent = (overall_percent - (float)2 / 9) / ((float)1 / 9);
                                                R = 0;
                                                G = (int)(local_percent * 255);
                                                B = 255;
                                            }
                                            else if (overall_percent < (float)4 / 9) // blue-green to green
                                            {
                                                float local_percent = (overall_percent - (float)3 / 9) / ((float)1 / 9);
                                                R = 0;
                                                G = 255;
                                                B = (int)((1.0 - local_percent) * 255);
                                            }
                                            else if (overall_percent < (float)5 / 9) // green to red-green
                                            {
                                                float local_percent = (overall_percent - (float)4 / 9) / ((float)1 / 9);
                                                R = (int)(local_percent * 255);
                                                G = 255;
                                                B = 0;
                                            }
                                            else if (overall_percent < (float)7 / 9) // red-green to red
                                            {
                                                float local_percent = (overall_percent - (float)5 / 9) / ((float)2 / 9);
                                                R = 255;
                                                G = (int)((1.0 - local_percent) * 255);
                                                B = 0;
                                            }
                                            else if (overall_percent < (float)8 / 9) // red to red-blue
                                            {
                                                float local_percent = (overall_percent - (float)7 / 9) / ((float)1 / 9);
                                                R = 255;
                                                G = 0;
                                                B = (int)(local_percent * 255);
                                            }
                                            else // red-blue to purple end
                                            {
                                                float local_percent = (overall_percent - (float)8 / 9) / ((float)1 / 9);
                                                R = (int)((0.75 + 0.25 * (1.0 - local_percent)) * 255);
                                                G = (int)(local_percent * 255 * 0.5);
                                                B = 255;
                                            }
                                        }

                                        // set pixel color
                                        row[i * pixel_size + 0] = (byte)B;	// set color in memory
                                        row[i * pixel_size + 1] = (byte)G;
                                        row[i * pixel_size + 2] = (byte)R;
                                    }
                                }
                                break;

                            case (ColorSheme.SPECTRAN):
                                {
                                    // draw new data
                                    for (i = 0; i < W; i++)	// for each pixel in the new line
                                    {
                                        if (waterfall_data[i] <= waterfall_low_threshold)
                                        {
                                            R = 0;
                                            G = 0;
                                            B = 0;
                                        }
                                        else if (waterfall_data[i] >= WaterfallHighThreshold) // white
                                        {
                                            R = 240;
                                            G = 240;
                                            B = 240;
                                        }
                                        else // value is between low and high
                                        {
                                            float range = WaterfallHighThreshold - waterfall_low_threshold;
                                            float offset = waterfall_data[i] - waterfall_low_threshold;
                                            float local_percent = ((100.0f * offset) / range);

                                            if (local_percent < 5.0f)
                                            {
                                                R = G = 0;
                                                B = (int)local_percent * 5;
                                            }
                                            else if (local_percent < 11.0f)
                                            {
                                                R = G = 0;
                                                B = (int)local_percent * 5;
                                            }
                                            else if (local_percent < 22.0f)
                                            {
                                                R = G = 0;
                                                B = (int)local_percent * 5;
                                            }
                                            else if (local_percent < 44.0f)
                                            {
                                                R = G = 0;
                                                B = (int)local_percent * 5;
                                            }
                                            else if (local_percent < 51.0f)
                                            {
                                                R = G = 0;
                                                B = (int)local_percent * 5;
                                            }
                                            else if (local_percent < 66.0f)
                                            {
                                                R = G = (int)(local_percent - 50) * 2;
                                                B = 255;
                                            }
                                            else if (local_percent < 77.0f)
                                            {
                                                R = G = (int)(local_percent - 50) * 3;
                                                B = 255;
                                            }
                                            else if (local_percent < 88.0f)
                                            {
                                                R = G = (int)(local_percent - 50) * 4;
                                                B = 255;
                                            }
                                            else if (local_percent < 99.0f)
                                            {
                                                R = G = (int)(local_percent - 50) * 5;
                                                B = 255;
                                            }
                                        }

                                        // set pixel color
                                        row[i * pixel_size + 0] = (byte)B;	// set color in memory
                                        row[i * pixel_size + 1] = (byte)G;
                                        row[i * pixel_size + 2] = (byte)R;
                                    }
                                }
                                break;

                            case (ColorSheme.BLACKWHITE):
                                {
                                    // draw new data
                                    for (i = 0; i < W; i++)	// for each pixel in the new line
                                    {
                                        if (waterfall_data[i] <= waterfall_low_threshold)
                                        {
                                            R = 0;
                                            G = 0;
                                            B = 0;
                                        }
                                        else if (waterfall_data[i] >= WaterfallHighThreshold) // white
                                        {
                                            R = 255;
                                            G = 255;
                                            B = 255;
                                        }
                                        else // value is between low and high
                                        {
                                            float range = WaterfallHighThreshold - waterfall_low_threshold;
                                            float offset = waterfall_data[i] - waterfall_low_threshold;
                                            float overall_percent = offset / range; // value from 0.0 to 1.0 where 1.0 is high and 0.0 is low.
                                            float local_percent = ((100.0f * offset) / range);
//                                            float contrast = (MainForm.SetupForm.DisplayContrast / 100);
                                            R = (int)((local_percent / 100) * 255);
                                            G = R;
                                            B = R;
                                        }

                                        // set pixel color
                                        row[i * pixel_size + 0] = (byte)B;	// set color in memory
                                        row[i * pixel_size + 1] = (byte)G;
                                        row[i * pixel_size + 2] = (byte)R;
                                    }
                                    break;
                                }
                        }
                    waterfall_bmp.UnlockBits(bitmapData);
                }

                if (CurrentDisplayMode == DisplayMode.WATERFALL)
                    g.DrawImageUnscaled(waterfall_bmp, 0, 20);	// draw the image on the background	
                else
                    g.DrawImageUnscaled(waterfall_bmp, 0, 0);
            }
            return true;
        }

        public static void RenderDirectX()  // changes yt7pwr
        {
            try
            {
//                Panadapter_Event.WaitOne();

                if (!MainForm.pause_DisplayThread)
                {
                    if (device == null || waterfall_dx_device == null) return;

                    // setup data
                    switch (current_display_mode)
                    {
                        case DisplayMode.PANADAPTER:
                            ConvertDataForPanadapter();
                            break;
                        case DisplayMode.PANAFALL:
                            ConvertDataForPanadapter();
                            break;
                        case DisplayMode.WATERFALL:
                            break;
                        default:
                            ConvertDataForPanadapter();
                            break;
                    }

                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, display_background_color.ToArgb(), 0.0f, 0);
                    waterfall_dx_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, display_background_color.ToArgb(), 0.0f, 0);

                    if (Panadapter_Sprite != null)
                    {
                        Panadapter_Sprite.Begin(SpriteFlags.AlphaBlend);
                        Panadapter_Sprite.Draw(PanadapterTexture, Panadapter_texture_size, (Color4)Color.White);

                        Panadapter_Sprite.End();
                    }

                    //Begin the scene
                    device.BeginScene();
                    device.SetRenderState(RenderState.AlphaBlendEnable, true);
                    device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
                    device.SetRenderState(RenderState.DestinationBlend, Blend.DestinationAlpha);

                    switch (current_display_mode)
                    {
                        case DisplayMode.WATERFALL:
                            /*                        waterfall_dx_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, display_background_color.ToArgb(), 0.0f, 0);
                                                    Waterfall_Sprite.Begin(SpriteFlags.AlphaBlend);
                                                    Waterfall_Sprite.Draw(WaterfallTexture, Waterfall_texture_size, (Color4)Color.White);
                                                    Waterfall_Sprite.End();
                                                    waterfall_dx_device.BeginScene();
                                                    RenderVerticalLine(waterfall_dx_device, 100, 100, Color.Red);
                                                    waterfall_dx_device.EndScene();
                                                    waterfall_dx_device.Present();
                                                    RenderWaterfall();*/
                            break;
                        case DisplayMode.PANADAPTER:
                        case DisplayMode.PANAFALL:
                            /*                        if (current_display_mode == DisplayMode.PANAFALL)
                                                    {
                                                        waterfall_dx_device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, display_background_color.ToArgb(), 0.0f, 0);
                                                        Waterfall_Sprite.Begin(SpriteFlags.AlphaBlend);
                                                        Waterfall_Sprite.Draw(WaterfallTexture, Waterfall_texture_size, (Color4)Color.White);
                                                        Waterfall_Sprite.End();
                                                        waterfall_dx_device.BeginScene();
                                                        RenderVerticalLine(waterfall_dx_device, 100, 100, Color.Red);
                                                        waterfall_dx_device.EndScene();
                                                        waterfall_dx_device.Present();
                                                    }*/

                            if (MainForm.MRIsRunning)
                            {
                                if (refresh_panadapter_grid)
                                {
                                    vertical_label = null;
                                    horizontal_label = null;
                                    RenderPanadapterGrid(panadapter_W, panadapter_H);
                                    refresh_panadapter_grid = false;
                                }
                                else
                                {
                                    if (show_horizontal_grid)
                                        RenderHorizontalLines(device, HorLines_vb, h_steps);
                                    if (show_vertical_grid)
                                        RenderVerticalLines(device, VerLines_vb, 65);
                                }

                                for (int i = 0; i < 12; i++)
                                {
                                    panadapter_font.DrawString(null, vertical_label[i].label,
                                        vertical_label[i].pos_x, vertical_label[i].pos_y, vertical_label[i].color.ToArgb());
                                }

                                for (int i = 1; i < h_steps; i++)
                                {
                                    panadapter_font.DrawString(null, horizontal_label[i].label,
                                        horizontal_label[i].pos_x, horizontal_label[i].pos_y, horizontal_label[i].color.ToArgb());
                                }

//                                Render_VFOA();
                                RenderPanadapterLine(device);
                            }
                            break;
                    }

//                    RenderVerticalLine(device, display_cursor_x, panadapter_H, grid_text_color);

                    //End the scene
                    device.EndScene();
                    device.Present();
                }

                Panadapter_Event.Set();
            }
            catch (Exception ex)
            {
                Debug.Write("Error in RenderDirectX()\n" + ex.ToString());
            }
        }

        unsafe private static void ConvertDataForPanadapter()  // changes yt7pwr
        {
            try
            {
                if (panadapterX_data == null || panadapterX_data.Length != panadapter_W)
                    panadapterX_data = new float[panadapter_W];    			        // array of points to display
                if (waterfallX_data == null || waterfallX_data.Length != panadapter_W)
                    waterfallX_data = new float[panadapter_W];
                float slope = 0.0f;				        	            	// samples to process per pixel
                int num_samples = 0;					                    // number of samples to process
                int start_sample_index = 0;			        	            // index to begin looking at samples
                int Low = rx_display_low;
                int High = rx_display_high;
                int yRange = spectrum_grid_max - spectrum_grid_min;

                max_y = Int32.MinValue;

                if (data_ready)
                {
                    // get new data
                    fixed (void* rptr = &new_display_data[0])
                    fixed (void* wptr = &current_display_data[0])
                        CWExpert.memcpy(wptr, rptr, BUFFER_SIZE * sizeof(float));
                }

                DataReady = false;

                if (average_on)
                {
                    if (!UpdateDisplayPanadapterAverage())
                    {
                        average_buffer = null;
                        average_buffer = new float[BUFFER_SIZE];	// initialize averaging buffer array
                        average_buffer[0] = CLEAR_FLAG;		// set the clear flag   
                        Debug.Write("Reset display average buffer!");
                    }
                }

                if (peak_on)
                    UpdateDisplayPeak();

                num_samples = (High - Low);

                start_sample_index = 0; // (BUFFER_SIZE >> 1) + (int)((Low * BUFFER_SIZE) / sample_rate);
                num_samples = 2048; // (int)((High - Low) * BUFFER_SIZE / sample_rate);
                if (start_sample_index < 0) start_sample_index += 2048;
                if ((num_samples - start_sample_index) > (BUFFER_SIZE + 1))
                    num_samples = BUFFER_SIZE - start_sample_index;

                slope = (float)num_samples / (float)panadapter_W;
                for (int i = 0; i < panadapter_W; i++)
                {
                    float max = float.MinValue;
                    float dval = i * slope + start_sample_index;
                    int lindex = (int)Math.Floor(dval);
                    int rindex = (int)Math.Floor(dval + slope);

                    if (slope <= 1.0 || lindex == rindex)
                    {
                        max = current_display_data[lindex % 2048] * ((float)lindex - dval + 1) + current_display_data[(lindex + 1) % 2048] * (dval - (float)lindex);
                    }
                    else
                    {
                        for (int j = lindex; j < rindex; j++)
                            if (current_display_data[j % 2048] > max) max = current_display_data[j % 2048];
                    }

                    max += display_cal_offset;
                    if (!mox) max += preamp_offset;

                    if (max > max_y)
                    {
                        max_y = max;
                        max_x = i;
                    }

                    panadapterX_data[i] = (int)(Math.Floor((spectrum_grid_max - max) * panadapter_H / yRange));
                }

                panadapterX_data[0] = panadapterX_data[panadapter_W - 1];
                panadapterX_data[panadapter_W - 1] = panadapterX_data[panadapter_W - 2];
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }
        }

        private static bool UpdateDisplayPanadapterAverage()
        {
            try
            {
                double dttsp_osc = (losc_hz - vfoa_hz) * 1e6;
                if ((average_buffer[0] == CLEAR_FLAG) ||
                    float.IsNaN(average_buffer[0]))
                {
                    //Debug.WriteLine("Clearing average buf"); 
                    for (int i = 0; i < BUFFER_SIZE; i++)
                        average_buffer[i] = current_display_data[i];
                    return true;
                }

                float new_mult = 0.0f;
                float old_mult = 0.0f;
                new_mult = display_avg_mult_new;
                old_mult = display_avg_mult_old;

                for (int i = 0; i < BUFFER_SIZE; i++)
                    average_buffer[i] = current_display_data[i] =
                        (float)(current_display_data[i] * new_mult +
                        average_buffer[i] * old_mult);

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
        }

        public static void UpdateDirectXDisplayWaterfallAverage()
        {
            // Debug.WriteLine("last vfo: " + avg_last_ddsfreq + " vfo: " + DDSFreq); 
            if (average_waterfall_buffer[0] == CLEAR_FLAG)
            {
                //Debug.WriteLine("Clearing average buf"); 
                for (int i = 0; i < BUFFER_SIZE; i++)
                    average_waterfall_buffer[i] = current_waterfall_data[i];
            }

            float new_mult = 0.0f;
            float old_mult = 0.0f;

            new_mult = waterfall_avg_mult_new;
            old_mult = waterfall_avg_mult_old;

            for (int i = 0; i < BUFFER_SIZE; i++)
                average_waterfall_buffer[i] = current_waterfall_data[i] =
                    (float)(current_waterfall_data[i] * new_mult +
                    average_waterfall_buffer[i] * old_mult);
        }

        #endregion

        #endregion
    }
}