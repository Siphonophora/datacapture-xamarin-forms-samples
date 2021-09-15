using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Forms.MatrixScanBubblesSample.Services
{
    public class SlideRepository
    {
        private readonly List<string> slides = new List<string>
        {
            "339469198",
            "339468221",
            "339470918",
            "339468215",
            "339468930",
            "339468233",
            "339468925",
            "339468230",
            "339469078",
            "339468222",
            "339468928",
            "339468929",
            "339468234",
            "339468216",
            "339469206",
            Target,
            "339468229",
            "339468927",
            "339470738",
            "339469075",
            "339468231",
            "339468228",
            "339468237",
            "339471831",
            "339468238",
        }

        public const string Target = "339469202";

        public bool IsSlide(string barcode)
        {
            return slides.Contains(barcode);
        }

        public bool GetLocationOffset(string barcode)
        {
            return slides.IndexOf(Target) - slides.IndexOf(barcode);
        }
    }
}
