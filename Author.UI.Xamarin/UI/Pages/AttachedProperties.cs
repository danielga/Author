using System;
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
                propertyChanged: ProgressBarProgressChanged);

        public static BindableProperty AnimatedProgressAnimationTimeProperty =
           BindableProperty.CreateAttached("AnimatedProgressAnimationTime",
               typeof(int),
               typeof(ProgressBar),
               1000);

        public enum Easing
        {
            Linear,
            SinOut,
            SinIn,
            SinInOut,
            CubicIn,
            CubicOut,
            CubicInOut,
            BounceOut,
            BounceIn,
            SpringIn,
            SpringOut
        }

        public static BindableProperty AnimatedProgressEasingProperty =
           BindableProperty.CreateAttached("AnimatedProgressEasing",
               typeof(Easing),
               typeof(ProgressBar),
               Easing.Linear);

        public static double GetAnimatedProgress(BindableObject target) => (double)target.GetValue(AnimatedProgressProperty);
        public static void SetAnimatedProgress(BindableObject target, double value) => target.SetValue(AnimatedProgressProperty, value);

        public static int GetAnimatedProgressAnimationTime(BindableObject target) => (int)target.GetValue(AnimatedProgressAnimationTimeProperty);
        public static void SetAnimatedProgressAnimationTime(BindableObject target, int value) => target.SetValue(AnimatedProgressAnimationTimeProperty, value);

        public static Easing GetAnimatedProgressEasing(BindableObject target) => (Easing)target.GetValue(AnimatedProgressEasingProperty);
        public static void SetAnimatedProgressEasing(BindableObject target, Easing value) => target.SetValue(AnimatedProgressEasingProperty, value);

        private static Xamarin.Forms.Easing GetEasingFromEnum(Easing easing)
        {
            switch (easing)
            {
                case Easing.Linear:
                    return Xamarin.Forms.Easing.Linear;

                case Easing.SinOut:
                    return Xamarin.Forms.Easing.SinOut;

                case Easing.SinIn:
                    return Xamarin.Forms.Easing.SinIn;

                case Easing.SinInOut:
                    return Xamarin.Forms.Easing.SinInOut;

                case Easing.CubicIn:
                    return Xamarin.Forms.Easing.CubicIn;

                case Easing.CubicOut:
                    return Xamarin.Forms.Easing.CubicOut;

                case Easing.CubicInOut:
                    return Xamarin.Forms.Easing.CubicInOut;

                case Easing.BounceOut:
                    return Xamarin.Forms.Easing.BounceOut;

                case Easing.BounceIn:
                    return Xamarin.Forms.Easing.BounceIn;

                case Easing.SpringIn:
                    return Xamarin.Forms.Easing.SpringIn;

                case Easing.SpringOut:
                    return Xamarin.Forms.Easing.SpringOut;

                default:
                    return Xamarin.Forms.Easing.Linear;
            }
        }

        private static void ProgressBarProgressChanged(BindableObject obj, object oldVal, object newVal)
        {
            ProgressBar progressBar = (ProgressBar)obj;
            double newValue = (double)newVal;

            ViewExtensions.CancelAnimations(progressBar);

            uint animationTime = (uint)Math.Max(0, GetAnimatedProgressAnimationTime(progressBar));
            if (animationTime == 0)
            {
                progressBar.Progress = newValue;
            }
            else
            {
                Xamarin.Forms.Easing easing = GetEasingFromEnum(GetAnimatedProgressEasing(progressBar));
                progressBar.ProgressTo(newValue, animationTime, easing);
            }
        }
    }
}
