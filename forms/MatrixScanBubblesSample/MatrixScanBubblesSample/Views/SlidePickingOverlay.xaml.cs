namespace MatrixScanBubblesSample.Views
{
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

        public SlidePickingOverlay(int offset)
        {
            InitializeComponent();

            if (offset == 0)
            {
                this.Label.Text = "This One";
            }
            else if (offset < 0)
            {
                this.Label.Text = $"{offset}{Down}";
            }
            else
            {
                this.Label.Text = $"{offset}{Up}";
            }
            this.HeightRequest = 20;
            this.WidthRequest = 100;
        }
    }
}
