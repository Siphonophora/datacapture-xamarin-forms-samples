﻿/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using BarcodeCaptureSettingsSample.Models;
using BarcodeCaptureSettingsSample.Resources;
using Scandit.DataCapture.Core.UI.Style.Unified;

namespace BarcodeCaptureSettingsSample.ViewModels.Settings.ViewSettings.Overlay
{
    public class OverlaySettingsViewModel : BaseViewModel
    {
        private static SettingsManager settings = SettingsManager.Instance;

        private readonly IList<OverlaySettingsBrushItem> availableBrushes = new List<OverlaySettingsBrushItem>
        {
            new OverlaySettingsBrushItem(settings.DefaultBrush, AppResources.Brush_Defaults),
            // Transparent Red and Red colors used here
             new OverlaySettingsBrushItem(new Brush(fillColor: Xamarin.Forms.Color.FromHex("#33FF0000"),strokeColor: Xamarin.Forms.Color.FromHex("#FFFF0000"), strokeWidth: settings.DefaultBrush.StrokeWidth), AppResources.Brush_Red),
            // Transparent Green and Green colors used here
             new OverlaySettingsBrushItem(new Brush(fillColor: Xamarin.Forms.Color.FromHex("#3300FF00"),strokeColor: Xamarin.Forms.Color.FromHex("#FF00FF00"), strokeWidth: settings.DefaultBrush.StrokeWidth), AppResources.Brush_Green)
        };

        public IList<OverlaySettingsBrushItem> AvailableBrushes => availableBrushes;

        public OverlaySettingsBrushItem CurrentBrush
        {
            get
            {
                return this.GetSettingsBrush(settings.CurrentBrush) ?? this.AvailableBrushes[0];
            }
            set
            {
                Brush brush = this.GetSettingsBrush(value.Brush)?.Brush ?? this.AvailableBrushes[0].Brush;
                settings.CurrentBrush = brush;
            }
        }

        private OverlaySettingsBrushItem GetSettingsBrush(Brush brush)
        {
            return this.AvailableBrushes.FirstOrDefault(item => eq(item.Brush, brush));
        }

        private static bool eq(Brush first, Brush other)
        {
            if (first == null || other == null)
            {
                return false;
            }

            return first.FillColor == other.FillColor &&
                   first.StrokeColor == other.StrokeColor &&
                   first.StrokeWidth == other.StrokeWidth;
        }
    }

    public class OverlaySettingsBrushItem
    {
        public OverlaySettingsBrushItem(Brush brush, string displayText)
        {
            this.Brush = brush;
            this.DisplayText = displayText;
        }

        public Brush Brush { get; }

        public string DisplayText { get; }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}
