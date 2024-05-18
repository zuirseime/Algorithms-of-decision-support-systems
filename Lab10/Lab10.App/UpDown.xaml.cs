using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab10.App;
/// <summary>
/// Interaction logic for UpDown.xaml
/// </summary>
public partial class UpDown : UserControl {
    public double Min {
        get { return (double)GetValue(MinProperty); }
        set { SetValue(MinProperty, value); }
    }

    public static readonly DependencyProperty MinProperty =
        DependencyProperty.Register("Min", typeof(double), typeof(UpDown), new PropertyMetadata(0d));

    public double Max {
        get { return (double)GetValue(MaxProperty); }
        set { SetValue(MaxProperty, value); }
    }

    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register("Max", typeof(double), typeof(UpDown), new PropertyMetadata(100d));

    public double Value {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double), typeof(UpDown), new PropertyMetadata(1d));

    public double Step {
        get { return (double)GetValue(StepProperty); }
        set { SetValue(StepProperty, value); }
    }

    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register("Step", typeof(double), typeof(UpDown), new PropertyMetadata(1d));

    public UpDown() {
        InitializeComponent();
        text.Text = Value.ToString();
    }

    private void text_PreviewKeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Up) {
            up.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(up, new object[] { true });
        }

        if (e.Key == Key.Down) {
            down.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(down, new object[] { true });
        }
    }

    private void text_PreviewKeyUp(object sender, KeyEventArgs e) {
        if (e.Key == Key.Up) {
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(up, new object[] { false });
        }

        if (e.Key == Key.Down) {
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(down, new object[] { false });
        }
    }

    private void text_TextChanged(object sender, TextChangedEventArgs e) {
        double value = 0d;
        if (!string.IsNullOrEmpty(text.Text))
            if (!double.TryParse(text.Text, out value))
                text.Text = Value.ToString();
        Value = Math.Round(Math.Clamp(value, Min, Max), 6);
        text.Text = Value.ToString();
        text.SelectionStart = text.Text.Length;
        ValueChanged?.Invoke(this, e);
    }

    private void up_Click(object sender, RoutedEventArgs e) {
        double value;
        if (!string.IsNullOrEmpty(text.Text))
            value = Convert.ToDouble(text.Text);
        else value = 0d;
        if (value < Max)
            text.Text = Convert.ToString(value + Step);
    }

    private void down_Click(object sender, RoutedEventArgs e) {
        double value;
        if (!string.IsNullOrEmpty(text.Text))
            value = Convert.ToDouble(text.Text);
        else value = 0d;
        if (value > Min)
            text.Text = Convert.ToString(value - Step);
    }

    public event EventHandler<EventArgs> ValueChanged;
}
