using Lab6.Core.MatrixGame;
using System.Windows;

namespace Lab6.App {
    /// <summary>
    /// Interaction logic for Simulation.xaml
    /// </summary>
    public partial class Simulation : Window {
        //private List<Batch> batches;

        public Simulation(List<Batch> batches) {
            InitializeComponent();

            //this.batches = batches;
            log.ItemsSource = batches;
        }
    }
}
