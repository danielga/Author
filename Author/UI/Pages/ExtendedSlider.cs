using System;
using Xamarin.Forms;

namespace Author.UI.Pages
{
    public class ExtendedSlider : Slider
    {
        public static readonly BindableProperty StepValueProperty =
            BindableProperty.Create("StepValue", typeof(double), typeof(ExtendedSlider), 1.0);

        public double StepValue
        {
            get { return (double)GetValue(StepValueProperty); }
            set { SetValue(StepValueProperty, value); }
        }

        public ExtendedSlider()
        {
            ValueChanged += OnSliderValueChanged;
        }

        void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            Value = Math.Round(e.NewValue / StepValue) * StepValue;
        }
    }
}
