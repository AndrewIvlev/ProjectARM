namespace ArmManipulatorApp
{
    using System.Windows;

    public class MyAnimationMod : DependencyObject
    {
        public static readonly DependencyProperty IterationProperty;

        public static readonly DependencyProperty CountProperty;

        static MyAnimationMod()
        {
            IterationProperty = DependencyProperty.Register("Iteration", typeof(int), typeof(MyAnimationMod));
            CountProperty = DependencyProperty.Register("Count", typeof(int), typeof(MyAnimationMod));
        }

        public int Iteration
        {
            get
            {
                return (int)GetValue(IterationProperty);
            }
            set
            {
                SetValue(IterationProperty, value);
            }
        }   
        
        public int Count
        {
            get
            {
                return (int)GetValue(CountProperty);
            }
            set
            {
                SetValue(CountProperty, value);
            }
        }
    }
}