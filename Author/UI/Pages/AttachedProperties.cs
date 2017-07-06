using Xamarin.Forms;

namespace Author.UI.Pages
{
    public static class AttachedProperties
    {
        public static BindableProperty AnimatedProgressProperty =
            BindableProperty.CreateAttached("AnimatedProgress",
                typeof(double),
                typeof(ProgressBar),
                0.0,
                BindingMode.OneWay,
                propertyChanged: ProgressBarProgressChanged);

        // Specifically prepared for our timer ProgressBars:
        // - Takes 1 second to complete the animation in a linear progression.
        // - When the ProgressBar completes the cycle,
        // it is immediately set to 0 and progressed to the new value as before.
        static void ProgressBarProgressChanged(BindableObject obj, object oldVal, object newVal)
        {
            ProgressBar progressBar = (ProgressBar)obj;
            double oldValue = (double)oldVal, newValue = (double)newVal;

            ViewExtensions.CancelAnimations(progressBar);

            if (oldValue >= newValue)
                progressBar.Progress = 0;

            progressBar.ProgressTo(newValue, 1000, Easing.Linear);
        }
    }
}
