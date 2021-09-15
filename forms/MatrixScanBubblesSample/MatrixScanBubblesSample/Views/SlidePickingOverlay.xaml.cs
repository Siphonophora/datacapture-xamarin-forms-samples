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
        public SlidePickingOverlay()
        {
            InitializeComponent();

            this.HeightRequest = 20;
            this.WidthRequest = 60;
        }
    }
}
