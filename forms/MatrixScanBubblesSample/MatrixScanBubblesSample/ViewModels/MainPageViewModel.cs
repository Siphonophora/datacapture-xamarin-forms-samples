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
using System.Threading.Tasks;
using System.Windows.Input;
using MatrixScanBubblesSample.Models;
using MatrixScanBubblesSample.Services;
using MatrixScanBubblesSample.Views;
using Scandit.DataCapture.Barcode.Tracking.Capture.Unified;
using Scandit.DataCapture.Barcode.Tracking.Data.Unified;
using Scandit.DataCapture.Barcode.Tracking.UI.Unified;
using Scandit.DataCapture.Core.Capture.Unified;
using Scandit.DataCapture.Core.Common.Geometry.Unified;
using Scandit.DataCapture.Core.Data.Unified;
using Scandit.DataCapture.Core.Source.Unified;

using Xamarin.Essentials;
using Xamarin.Forms;

namespace MatrixScanBubblesSample.ViewModels
{
    public class MainPageViewModel : IBarcodeTrackingListener, IBarcodeTrackingBasicOverlayListener, IBarcodeTrackingAdvancedOverlayListener
    {
        public Func<TrackedBarcode, bool> ShouldHideOverlay;
        private readonly Scandit.DataCapture.Core.UI.Style.Unified.Brush highlightedBrush;
        private readonly IDictionary<int, View> overlays = new Dictionary<int, View>();
        private readonly int shelfCount = 4;
        private readonly int backRoom = 8;
        private readonly SlideRepository repository;

        public MainPageViewModel()
        {
            repository = new SlideRepository();
            this.InitializeScanner();
            this.SubscribeToAppMessages();

            this.highlightedBrush = new Scandit.DataCapture.Core.UI.Style.Unified.Brush(Color.Transparent, Color.Green, 2.0f);
            this.ToggleFreezeButton = new Command(() =>
            {
                if (this.BarcodeTracking.Enabled)
                {
                    this.BarcodeTracking.Enabled = false;
                    this.Camera.SwitchToDesiredStateAsync(FrameSourceState.Off);
                }
                else
                {
                    this.BarcodeTracking.Enabled = true;
                    this.Camera.SwitchToDesiredStateAsync(FrameSourceState.On);
                }
            });
        }

        public Camera Camera { get; private set; } = ScannerModel.Instance.CurrentCamera;

        public DataCaptureContext DataCaptureContext { get; private set; } = ScannerModel.Instance.DataCaptureContext;

        public BarcodeTracking BarcodeTracking { get; private set; } = ScannerModel.Instance.BarcodeTracking;

        public ICommand ToggleFreezeButton { get; private set; }

        public Task OnSleep()
        {
            return this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public async Task OnResumeAsync()
        {
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    await this.ResumeFrameSource();
                }
            }
            else
            {
                await this.ResumeFrameSource();
            }
        }

        private void SubscribeToAppMessages()
        {
            MessagingCenter.Subscribe(this, App.MessageKeys.OnResume, callback: async (App app) => await this.OnResumeAsync());
            MessagingCenter.Subscribe(this, App.MessageKeys.OnSleep, callback: async (App app) => await this.OnSleep());
        }

        private void InitializeScanner()
        {
            // Register self as a listener to get informed whenever a new barcode got recognized.
            this.BarcodeTracking.AddListener(this);
        }

        private Task ResumeFrameSource()
        {
            // Switch camera on to start streaming frames. The camera is started asynchronously and
            // will take some time to completely turn on.
            return this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        #region IBarcodeTrackingListener

        public void OnObservationStarted(BarcodeTracking barcodeTracking)
        { }

        public void OnObservationStopped(BarcodeTracking barcodeTracking)
        { }

        public void OnSessionUpdated(BarcodeTracking barcodeTracking, BarcodeTrackingSession session, IFrameData frameData)
        {
            // This method is called whenever objects are updated and it's the right place to react
            // to the tracking results.
            Device.InvokeOnMainThreadAsync(() =>
            {
                if (!this.BarcodeTracking.Enabled)
                {
                    return;
                }

                foreach (var identifier in session.RemovedTrackedBarcodes)
                {
                    this.overlays.Remove(identifier);
                }

                var filteredTrackedCodes = session.TrackedBarcodes.Values.Where(code => code != null && code.Barcode != null);
                foreach (var trackedCode in filteredTrackedCodes)
                {
                    if (this.overlays.TryGetValue(trackedCode.Identifier, out View stockOverlay))
                    {
                        stockOverlay.IsVisible = !this.ShouldHideOverlay?.Invoke(trackedCode) ?? true;
                    }
                }
            });
        }

        #endregion IBarcodeTrackingListener

        #region IBarcodeTrackingBasicOverlayListener

        public Scandit.DataCapture.Core.UI.Style.Unified.Brush BrushForTrackedBarcode(BarcodeTrackingBasicOverlay overlay, TrackedBarcode trackedBarcode)
        {
            return this.highlightedBrush;
        }

        public void OnTrackedBarcodeTapped(BarcodeTrackingBasicOverlay overlay, TrackedBarcode trackedBarcode)
        { }

        #endregion IBarcodeTrackingBasicOverlayListener

        #region IBarcodeTrackingAdvancedOverlay

        public View ViewForTrackedBarcode(BarcodeTrackingAdvancedOverlay overlay, TrackedBarcode trackedBarcode)
        {
            var identifier = trackedBarcode.Identifier;
            var barcode = trackedBarcode.Barcode.Data ?? "ERROR";
            if (!this.overlays.TryGetValue(identifier, out var stockOverlay))
            {
                if (repository.IsSlide(barcode))
                {
                    // Get the information you want to show from your back end system/database.
                    stockOverlay = new SlidePickingOverlay(barcode, repository.GetPullInfoInChit(barcode), () => repository.PullSlide(barcode));
                    this.overlays[identifier] = stockOverlay;
                    stockOverlay.IsVisible = !this.ShouldHideOverlay?.Invoke(trackedBarcode) ?? true;
                }
                else if (repository.IsChit(barcode))
                {
                    stockOverlay = new ChitOverlay(repository.GetChitPullCount(barcode));
                    this.overlays[identifier] = stockOverlay;
                    stockOverlay.IsVisible = true; // !this.ShouldHideOverlay?.Invoke(trackedBarcode) ?? true;
                }
            }
            return stockOverlay;
        }

        public Anchor AnchorForTrackedBarcode(BarcodeTrackingAdvancedOverlay overlay, TrackedBarcode trackedBarcode)
        {
            // The offset of our overlay will be calculated from the top center anchoring point.
            return Anchor.Center;
        }

        public PointWithUnit OffsetForTrackedBarcode(BarcodeTrackingAdvancedOverlay overlay, TrackedBarcode trackedBarcode)
        {
            // We set the offset's height to be equal of the 100 percent of our overlay. The minus
            // sign means that the overlay will be above the barcode.
            return new PointWithUnit(
                x: new FloatWithUnit(value: 0, unit: MeasureUnit.Fraction),
                y: new FloatWithUnit(value: 0, unit: MeasureUnit.Fraction));
        }

        #endregion IBarcodeTrackingAdvancedOverlay
    }
}
