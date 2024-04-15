﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab7.App.Controls;

/// <summary>
/// Interaction logic for NumericUpDown.xaml
/// </summary>
public partial class NumericUpDown : UserControl {
    public double Min {
        get { return (double)GetValue(MinProperty); }
        set { SetValue(MinProperty, value); }
    }

    public static readonly DependencyProperty MinProperty =
        DependencyProperty.Register("Min", typeof(double), typeof(NumericUpDown), new PropertyMetadata(0d));

    public double Max {
        get { return (double)GetValue(MaxProperty); }
        set { SetValue(MaxProperty, value); }
    }

    public static readonly DependencyProperty MaxProperty =
        DependencyProperty.Register("Max", typeof(double), typeof(NumericUpDown), new PropertyMetadata(100d));

    public double Value {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown), new PropertyMetadata(1d));

    public double Step {
        get { return (double)GetValue(StepProperty); }
        set { SetValue(StepProperty, value); }
    }

    public static readonly DependencyProperty StepProperty =
        DependencyProperty.Register("Step", typeof(double), typeof(NumericUpDown), new PropertyMetadata(1d));

    public NumericUpDown() {
        InitializeComponent();
        this.text.Text = this.Value.ToString();
    }

    private void text_PreviewKeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Up) {
            this.up.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(this.up, new object[] { true });
        }

        if (e.Key == Key.Down) {
            this.down.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(this.down, new object[] { true });
        }
    }

    private void text_PreviewKeyUp(object sender, KeyEventArgs e) {
        if (e.Key == Key.Up) {
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(this.up, new object[] { false });
        }

        if (e.Key == Key.Down) {
            typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(this.down, new object[] { false });
        }
    }

    private void text_TextChanged(object sender, TextChangedEventArgs e) {
        double value = 0d;
        if (!string.IsNullOrEmpty(this.text.Text))
            if (!double.TryParse(this.text.Text, out value))
                this.text.Text = this.Value.ToString();
        this.Value = Math.Round(Math.Clamp(value, this.Min, this.Max), 6);
        this.text.Text = this.Value.ToString();
        this.text.SelectionStart = this.text.Text.Length;
    }

    private void up_Click(object sender, RoutedEventArgs e) {
        double value;
        if (!string.IsNullOrEmpty(this.text.Text))
            value = Convert.ToDouble(this.text.Text);
        else value = 0d;
        if (value < this.Max)
            text.Text = Convert.ToString(value + this.Step);
    }

    private void down_Click(object sender, RoutedEventArgs e) {
        double value;
        if (!string.IsNullOrEmpty(this.text.Text))
            value = Convert.ToDouble(this.text.Text);
        else value = 0d;
        if (value > this.Min)
            text.Text = Convert.ToString(value - this.Step);
    }
}
