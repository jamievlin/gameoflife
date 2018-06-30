using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading; 
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<List<bool>> golData;
        private List<List<bool>> golData2;
        int numx = 100, numy = 100;
        int frame = 0; 

        DispatcherTimer baseTimer; 
        public MainWindow()
        {
            InitializeComponent();

            golData = new List<List<bool>>(Enumerable.Range(0, numx).Select(t =>
            {
                return new List<bool>(
                    Enumerable.Range(0, numy).Select(x => { return false; })
                    ); 
            }
            ));

            golData[5][4] = true;
            golData[5][5] = true;
            golData[5][6] = true;
            // golData[1][1] = true;

            baseTimer = new DispatcherTimer();
            baseTimer.Interval = new TimeSpan(0, 0, 1); 
            baseTimer.Tick += baseTimerTick;
        }

        private void baseTimerTick(object sender, EventArgs e)
        {
            updateGol(); 
            drawImage();

            frame++; 
            this.frameLabel.Content = string.Format("Frame {0}", frame);
            // frameLabel.InvalidateVisual();
            // this.InvalidateVisual(); 
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            baseTimer.Start();
            btnStep.IsEnabled = false; 
        }

        private void updateGol()
        {
            var newList = new List<List<bool>>(Enumerable.Range(0, numx).Select(i =>
            {
                return new List<bool>(
                    Enumerable.Range(0, numy).Select(j => { return golData[i][j]; })
                    );
            }
            ));

            for (int i = 0; i < numx; i++)
            {
                for (int j = 0; j < numy; j++)
                {
                    int neighborCount = getNeighborhoodCount(i, j);

                    if (golData[i][j])
                    {
                        if (neighborCount < 2 || neighborCount > 3)
                        {
                            newList[i][j] = false; 
                        }
                    }
                    else
                    {
                        if (neighborCount == 3)
                        {
                            newList[i][j] = true;
                        }
                    }
                }
            }

            golData = newList; 
        }

        private int getNeighborhoodCount(int i, int j)
        {

            var lookupList = new[] { 0, 1, -1 };

            var nonzeroCells =
                from f1 in lookupList
                from f2 in lookupList
                where !(f1 == 0 && f2 == 0)        // no center
                select new[] { f1 + i, f2 + j};

            var validcells =
                from cell in nonzeroCells
                where cell[0]  >= 0 && cell[1] >= 0        // only valid cells 
                where cell[0] < numx && cell[1] < numy
                select new[] { cell[0], cell[1] };

            var aliveCells =
                from cell in validcells
                where golData[cell[0]][cell[1]] == true
                select cell;
                
            return aliveCells.Count(); 
        }

        private void drawImage()
        {
            imgGridLabel.Children.Clear(); 
            int spacing = 10;

            int shiftx = 0;
            int shifty = 0;

            foreach (var sublist in golData)
            {
                shiftx = 0; 
                foreach (bool value in sublist)
                {
                    var gridData = new Rectangle();
                    gridData.Stroke = Brushes.Black;
                    gridData.StrokeThickness = 0.1; 

                    if (value)
                    {
                        gridData.Fill = Brushes.Black; 
                    }

                    gridData.Width = spacing;
                    gridData.Height = spacing;

                    Canvas.SetLeft(gridData, shiftx);
                    Canvas.SetTop(gridData, shifty);

                    imgGridLabel.Children.Add(gridData);

                    shiftx += spacing; 
                }

                shifty += spacing; 
            }
            // imgGridLabel.InvalidateVisual(); 
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            baseTimer.Stop();
            btnStep.IsEnabled = true; 
        }

        private void btnStep_Click(object sender, RoutedEventArgs e)
        {
            updateGol();
            drawImage();

            frame++;
            this.frameLabel.Content = string.Format("Frame {0}", frame);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.drawImage(); 
        }
    }
}
