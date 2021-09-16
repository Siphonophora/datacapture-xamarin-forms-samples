namespace MatrixScanBubblesSample.Views
{
    using MatrixScanBubblesSample.Models;
    using OneOf;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlidePickingOverlay : ContentView
    {
        private const string Down = "⬇";
        private const string Up = "⬆";
        private readonly string barcode;
        private readonly Action pull;
        private bool tapEnabled = false;

        public SlidePickingOverlay(
            string barcode,
            OneOf<Pulled, NeedsPull, Offset, UnknownSlide, ChitEmpty, Loading, PullComplete> data,
            Action pull)
        {
            if (string.IsNullOrWhiteSpace(barcode))
            {
                throw new ArgumentException($"'{nameof(barcode)}' cannot be null or whitespace.", nameof(barcode));
            }
            this.pull = pull ?? throw new ArgumentNullException(nameof(pull));
            this.barcode = barcode;

            InitializeComponent();

            this.HeightRequest = 20;
            this.WidthRequest = 100;

            data.Switch
                (
                    pulled => { this.Label.Text = "Pulled !"; },
                    inPull =>
                    {
                        this.Label.Text = "Pull";
                        this.HeightRequest = 100;
                        tapEnabled = true;
                    },
                    offset =>
                    {
                        if (offset.Value < 0)
                        {
                            this.Label.Text = $"{offset.Value}{Down}";
                        }
                        else
                        {
                            this.Label.Text = $"{offset.Value}{Up}";
                        }
                    },
                    unknown => { this.Label.Text = $"UNKNOWN SLIDE"; },
                    chitEmpty => { this.Label.Text = $"Chit Empty"; },
                    loading => { this.Label.Text = "..."; },
                    pullComplete => { this.Label.Text = "Pull Done"; }
                );
        }

        private void TapGestureRecognizerTapped(object sender, System.EventArgs e)
        {
            if (tapEnabled)
            {
                pull.Invoke();
                this.Label.Text = "Pulled !";
                // Fire and forget for now
                App.Current.MainPage.DisplayAlert("Pulled", $"Pulled {barcode}", "Ok");
            }
        }
    }
}
