using MatrixScanBubblesSample.Models;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixScanBubblesSample.Services
{
    public class SlideRepository
    {
        private const string UserContainer = "UserContainer";

        private readonly HashSet<string> PullList = new HashSet<string>
        {
            "339468930",
            "339469075",
            "339468331",
        };

        private readonly HashSet<string> PendingPulls = new HashSet<string>();

        private readonly Dictionary<string, List<string>> slides = new Dictionary<string, List<string>>
        {
            { "AA2-3-A",
                new List<string>
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
                    "339469202",
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
            },
            { "AA1-1-A",
                new List<string>
                {
                    "339472416",
                    "339468232",
                    "339468226",
                    "339468920",
                    "339471836",
                    "339469081",
                    "339471835",
                    "339471837",
                    "339471838",
                    "339468236",
                    "339471832",
                    "339469079",
                    "339469100",
                    "339468919",
                    "339468921",
                    "339468218",
                    "339468220",
                    "339468227",
                    "339468922",
                }
            },
            { "AA2-3-B",
                new List<string>
                {
                    "339472478",
                    "339472477",
                    "339470742",
                    "339470785",
                    "339469201",
                    "339469208",
                    "339469209",
                    "339469197",
                    "339468322",
                    "339468323",
                    "339468324",
                    "339468325",
                    "339468331",
                    "339468330",
                    "339468329",
                    "339468328",
                    "339468327",
                    "339468326",
                    "339468334",
                    "339468335",
                }
            },
            { UserContainer, new List<string>() },
        };

        public SlideRepository()
        {
            foreach (var item in PullList)
            {
                PendingPulls.Add(item);
            }
        }

        public bool IsSlide(string barcode)
        {
            return slides.SelectMany(x => x.Value).Contains(barcode);
        }

        public OneOf<Pulled, NeedsPull, Offset, UnknownSlide, ChitEmpty, Loading, PullComplete> GetPullInfoInChit(string barcode)
        {
            if (PullList.Contains(barcode) && PendingPulls.Contains(barcode) == false)
            {
                return new Pulled();
            }

            if (PendingPulls.Count == 0)
            {
                return new PullComplete();
            }

            if (PendingPulls.Contains(barcode))
            {
                return new NeedsPull();
            }

            var chitSlides = slides.Where(x => x.Value.Contains(barcode)).Select(x => x.Value).FirstOrDefault();
            var targetSlide = chitSlides.FirstOrDefault(x => PendingPulls.Contains(x));
            if (chitSlides == null)
            {
                return new UnknownSlide(); // Unknown slide somehow.
            }

            if (targetSlide == null)
            {
                return new ChitEmpty(); // Nothing left in this chit.
            }

            return new Offset(chitSlides.IndexOf(targetSlide) - chitSlides.IndexOf(barcode));
        }

        public bool IsChit(string barcode)
        {
            return barcode.StartsWith("AA");
        }

        public int GetChitPullCount(string barcode)
        {
            return slides.TryGetValue(barcode, out var chitSlides) ?
                chitSlides.Count(x => PendingPulls.Contains(x)) :
                0;
        }

        public void PullSlide(string barcode)
        {
            slides.Where(x => x.Value.Contains(barcode))
                .ToList()
                .ForEach(x => x.Value.Remove(barcode));
            slides[UserContainer].Add(barcode);
            _ = PendingPulls.Remove(barcode);
        }
    }
}
