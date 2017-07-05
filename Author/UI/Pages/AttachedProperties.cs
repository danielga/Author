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
                propertyChanged: (b, o, n) => ProgressBarProgressChanged((ProgressBar)b, (double)n));

        // Specifically prepared for our timer ProgressBars:
        // - Takes 1 second to complete the animation in a linear progression.
        // - When the ProgressBar completes the cycle,
        // it is immediately set to 0 and progressed to the new value as before.
        static void ProgressBarProgressChanged(ProgressBar progressBar, double progress)
        {
            ViewExtensions.CancelAnimations(progressBar);

            if (progressBar.Progress == 1.0)
                progressBar.Progress = 0.0;

            progressBar.ProgressTo(progress, 1000, Easing.Linear);
        }
    }
}
