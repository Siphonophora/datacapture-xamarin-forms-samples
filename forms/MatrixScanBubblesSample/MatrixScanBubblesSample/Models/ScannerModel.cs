/*
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
using System.Threading;
using Scandit.DataCapture.Barcode.Capture.Unified;
using Scandit.DataCapture.Barcode.Data.Unified;
using Scandit.DataCapture.Barcode.Tracking.Capture.Unified;
using Scandit.DataCapture.Core.Capture.Unified;
using Scandit.DataCapture.Core.Source.Unified;

namespace MatrixScanBubblesSample.Models
{
    public class ScannerModel
    {
        private static readonly Lazy<ScannerModel> instance = new Lazy<ScannerModel>(() => new ScannerModel(), LazyThreadSafetyMode.PublicationOnly);

        private ScannerModel()
        {
            this.CameraSettings.PreferredResolution = VideoResolution.Uhd4k;
            this.CurrentCamera?.ApplySettingsAsync(this.CameraSettings);

            // Create data capture context using your license key and set the camera as the frame source.
            this.DataCaptureContext = DataCaptureContext.ForLicenseKey(Secrets.ScanditKeySecret.SCANDIT_LICENSE_KEY);
            this.DataCaptureContext.SetFrameSourceAsync(this.CurrentCamera);

            // The barcode tracking process is configured through barcode tracking settings and are
            // then applied to the barcode tracking instance that manages barcode tracking.
            this.BarcodeTrackingSettings = BarcodeTrackingSettings.Create(BarcodeTrackingScenario.A);

            // The settings instance initially has all types of barcodes (symbologies) disabled. For
            // the purpose of this sample we enable a very generous set of symbologies. In your own
            // app ensure that you only enable the symbologies that your app requires as every
            // additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>
            {
                Symbology.DataMatrix,
                Symbology.Code128
            };

            this.BarcodeTrackingSettings.EnableSymbologies(symbologies);

            this.BarcodeTracking = BarcodeTracking.Create(this.DataCaptureContext, this.BarcodeTrackingSettings);
        }

        public static ScannerModel Instance => instance.Value;

        #region DataCaptureContext

        public DataCaptureContext DataCaptureContext { get; private set; }

        #endregion DataCaptureContext

        #region CamerSettings

        public Camera CurrentCamera { get; private set; } = Camera.GetCamera(CameraPosition.WorldFacing);

        public CameraSettings CameraSettings { get; } = BarcodeTracking.RecommendedCameraSettings;

        #endregion CamerSettings

        #region BarcodeTracking

        public BarcodeTracking BarcodeTracking { get; private set; }

        public BarcodeTrackingSettings BarcodeTrackingSettings { get; private set; }

        #endregion BarcodeTracking
    }
}
