using System.Collections.Generic;

namespace MiniPowerPoint
{
    public class SlideManager
    {
        public List<Slide> Slides = new List<Slide>();
        public int CurrentIndex = 0;

        public Slide Current
        {
            get
            {
                if (Slides.Count == 0) return null;
                if (CurrentIndex < 0 || CurrentIndex >= Slides.Count) return null;
                return Slides[CurrentIndex];
            }
        }

        public void AddSlide()
        {
            Slides.Add(new Slide());
            CurrentIndex = Slides.Count - 1;
        }

        public void RemoveCurrent()
        {
            if (Slides.Count == 0) return;

            Slides.RemoveAt(CurrentIndex);

            if (Slides.Count == 0)
            {
                CurrentIndex = 0;
                return;
            }

            if (CurrentIndex >= Slides.Count)
                CurrentIndex = Slides.Count - 1;
        }
    }
}