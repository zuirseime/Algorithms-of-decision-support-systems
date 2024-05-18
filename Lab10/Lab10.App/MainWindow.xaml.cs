﻿using Lab10.App.Pages;
using System.Windows;
using System.Windows.Controls;

namespace Lab10.App;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public static Page[] Pages => [new Home(), new Schedule(), new Workload()];

    public MainWindow() {
        InitializeComponent();
        _frame.Navigate(Pages[0]);
    }

    private void ChangePage(object sender, RoutedEventArgs e) {
        if (sender is Button button) {
            Navigate(int.Parse(button.Tag.ToString()!));
        }
    }

    private void Navigate(int index) {
        _frame.Navigate(Pages[index]);
    }
}