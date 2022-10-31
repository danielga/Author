namespace Author.Views;

public static class AttachedProperties
{
    public static readonly BindableProperty AnimatedProgressProperty =
        BindableProperty.CreateAttached("AnimatedProgress",
            typeof(double),
            typeof(ProgressBar),
            0.0,
            propertyChanged: ProgressBarProgressChanged
        );

    public static readonly BindableProperty AnimatedProgressAnimationTimeProperty =
       BindableProperty.CreateAttached("AnimatedProgressAnimationTime",
           typeof(int),
           typeof(ProgressBar),
           1000
       );

    public static readonly BindableProperty AnimatedProgressEasingProperty =
       BindableProperty.CreateAttached("AnimatedProgressEasing",
           typeof(Easing),
           typeof(ProgressBar),
           Easing.Linear
       );

    public static double GetAnimatedProgress(BindableObject target) => (double)target.GetValue(AnimatedProgressProperty);
    public static void SetAnimatedProgress(BindableObject target, double value) => target.SetValue(AnimatedProgressProperty, value);

    public static int GetAnimatedProgressAnimationTime(BindableObject target) => (int)target.GetValue(AnimatedProgressAnimationTimeProperty);
    public static void SetAnimatedProgressAnimationTime(BindableObject target, int value) => target.SetValue(AnimatedProgressAnimationTimeProperty, value);

    public static Easing GetAnimatedProgressEasing(BindableObject target) => (Easing)target.GetValue(AnimatedProgressEasingProperty);
    public static void SetAnimatedProgressEasing(BindableObject target, Easing value) => target.SetValue(AnimatedProgressEasingProperty, value);

    private static void ProgressBarProgressChanged(BindableObject obj, object oldVal, object newVal)
    {
        if (obj is not ProgressBar progressBar || newVal is not double newValue)
            return;

        Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(progressBar);

        var animationTime = (uint)Math.Max(0, GetAnimatedProgressAnimationTime(progressBar));
        if (animationTime == 0)
            progressBar.Progress = newValue;
        else
            progressBar.ProgressTo(newValue, animationTime, GetAnimatedProgressEasing(progressBar));
    }
}
