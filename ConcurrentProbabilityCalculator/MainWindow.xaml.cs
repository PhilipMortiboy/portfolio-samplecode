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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace ConcurrentProbabilityCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Notes
        /*
         * Terminology:
         * > 'Service(s)' refers to applications that use the Decibel API
         * > 'API' refers to the Decibel API
         * 
         * Methodology:
         * > For a full details of the mathematically formulae and theories 
         *   used in this program, see the supporting documentation in 
         *   DecibelTech
         * 
         */
        #endregion

        #region Variables
        /// <summary>
        /// The total number of users that use the services over a period of time
        /// </summary>
        double active_users = 0;

        /// <summary>
        /// The total number of hours users spent using the services over a period of time
        /// </summary>
        double total_hours = 0;

        /// <summary>
        /// The amount of time the active_user and total_hours figures are measured over, in days
        /// </summary>
        double time_period = 0;

        /// <summary>
        /// The maximum amount of concurrent requests that API can support
        /// </summary>
        int max_concurrent_req = 0;

        /// <summary>
        /// The averge length of time the service takes to return a result from the API, in seconds
        /// </summary>
        double response_time = 0;

        /// <summary>
        /// The average length of a song, in seconds
        /// </summary>
        double song_time = 0;

        /// <summary>
        /// How often the API can be overloaded, i.e. take on more users than the concurrency limit, in seconds
        /// </summary>
        double overload_occurance = 0;

        /// <summary>
        /// How much traffic increases by during peak hour
        /// </summary>
        const double peak_mulitplier = 6.1;

        /// <summary>
        /// The total of all the traffic multipliers
        /// </summary>
        const double daily_total = 91.1;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            // Read the assumptions the user entered
            if (ReadAssumptions())
            {
                btn_start.IsEnabled = false;
                Thread thread = new Thread(() => RunCalculations());
                thread.Start();
            }
        }

        private void RunCalculations()
        {
            // First, find the probability of a single user using the API at any given second
            double p = CalcP();
            this.Dispatcher.Invoke(delegate { progress_bar.Value = 20; });

            // Find how many users can be supported on one instance
            double m = FindMaxUsers(p);
            this.Dispatcher.Invoke(delegate { 
                // Update the GUI with the results
                lbl_max_users.Content = m.ToString();
                lbl_request_prob.Content = p.ToString();
                btn_start.IsEnabled = true;
                progress_bar.Value = 100;
            });
        }

        /// <summary>
        /// Find the maximum amount of users supported by various single EC2 instances that can handled different amounts of concurrent users.
        /// Saves results to max_user_result variable
        /// </summary>
        /// <param name="p">The probability of a single user accessing the API at a given time</param>
        private double FindMaxUsers(double p)
        {
            // Loop through increasing values of k until x cannot be calculated anymore, then extrapolate the remaining results from the data obtained

            // The number of concurrent users (k)
            int k = 1;
            // The maximum number of users supported (n)
            double n = 0;
            // The acceptable probability of an overload, (k > n) occuring
            double x = 1 / overload_occurance;
            // A list to store the results of n in
            List<double> max_user_results = new List<double>();

            try
            {
                // Set amount to increase progress bar by for each pass of the loop
                double progressInc = (double)80 / max_concurrent_req;
                // While k is less than the maximum number of concurrent users supported
                while (k < max_concurrent_req)
                {
                    // Attempt to calculate n
                    n = CalcN(x, k, p);
                    // If n could not be found, exit the loop
                    if (n == 0)
                        break;
                    // Save n to the list of results
                    max_user_results.Add(n);
                    k++;
                    // Update the progress bar with the reusult
                    this.Dispatcher.Invoke(delegate { progress_bar.Value = progress_bar.Value + progressInc; });
                }
            }
            catch
            {
                MessageBox.Show("Calculation failed at " + k + " concurrent users.");
            }
            n = ExtrapolateResult(max_concurrent_req, max_user_results.ToArray());

            return n;
        }

        /// <summary>
        /// Read assumptions entered by user and save them to correct variables
        /// </summary>
        /// <returns>True if all assumptions were read successfully, false if not. 
        /// Displays an error message for any assumptions that could not be read</returns>
        private bool ReadAssumptions()
        {
            string errorMsg = "";
            try
            {
                active_users = double.Parse(txt_users.Text);
            }
            catch
            {
                errorMsg = errorMsg + "Please enter a valid number for the amount of active users.\n";
            }
            try
            {
                total_hours = double.Parse(txt_totalHours.Text);
            }
            catch
            {
                errorMsg = errorMsg + "Please enter a valid number for the total listening hours.\n";
            }
            try
            {
                max_concurrent_req = int.Parse(txt_concurrent_requests.Text);
            }
            catch
            {
                errorMsg = errorMsg + "Please enter a valid number for the max. concurrent requests. This must be a whole number\n";
            }
            try
            {
                response_time = double.Parse(txt_response_time.Text);
            }
            catch
            {
                errorMsg = errorMsg + "Please enter a valid number for the average response time.\n";
            }
            try
            {
                song_time = double.Parse(txt_song_time.Text);
            }
            catch
            {
                errorMsg = errorMsg + "Please enter a valid number for the average song time.\n";
            }
            try
            {
                overload_occurance = double.Parse(txt_overload.Text);
            }
            catch
            {
                errorMsg = errorMsg + "Please enter a valid value for the acceptable overload occurance.\n";
            }
            try
            {
                time_period = double.Parse(txt_totalDays.Text);
            }
            catch
            {
                errorMsg = errorMsg + "Please enter a valid value for the acceptable overload occurance.\n";
            }
            if (errorMsg != "")
            {
                ShowErrorMsg(errorMsg);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Display a error message to the user and log it in the error log
        /// </summary>
        /// <param name="errorMsg">The message to display</param>
        /// <param name="errorDetails">Additional details for the error log.
        /// These don't get displayed to the user - Optional</param>
        private void ShowErrorMsg(string errorMsg, string errorDetails = "")
        {
            //Note: add something to log errors
            MessageBox.Show(errorMsg);
        }

        #region Mathematical Functions

        /// <summary>
        /// Calculate the probability that a single user will use the API and any given second (P)
        /// </summary>
        /// <returns>The value of P</returns>
        private double CalcP()
        {
            // Calculate the total number of hours spent using the service at peak time for the time period given
            double y = (total_hours / daily_total) * peak_mulitplier;
            // Average this number out for 1 day
            double d = y / time_period;
            // Calculate the number of seconds used by each user every day at peak time
            double s = (d * 60 * 60) / active_users;
            // Calculate how much of this time is spent querying the API
            double r = (s / song_time) * response_time;
            // Find the probability of the API being queried by a single user at any given second
            double p = r / 3600;

            return p;
        }

        /// <summary>
        /// Calculate the n value in the equation x = p^k * C(n, k)
        /// </summary>
        /// <param name="x">The acceptable probability of a overload occuring</param>
        /// <param name="k">The maximum number of concurrent users</param>
        /// <param name="p">The probability of a single user accessing the API at any given second</param>
        /// <returns>The maximum number of users supported by one EC2 instance. 0 if k was too large to calculate p^k</returns>
        private double CalcN(double x, int k, double p)
        {
            // Find p^k
            double pK = Math.Pow(p, k);
            // Divide x by this to work out the bionomial coeff we are looking for, [y = C(n,k)]
            double y = x / pK;
            // If y could not be calculated, return 0
            if (double.IsPositiveInfinity(y))
                return 0;
            // Find an estimation for the value of n
            double n = EstN(y, k);

            return n;
        }

        /// <summary>
        /// Estimate the maximum number of users (n)
        /// </summary>
        /// <param name="y">The bionomial coeff to find</param>
        /// <param name="k">The number of concurrent users to support</param>
        /// <returns>An approximation for n</returns>
        private double EstN(double y, int k)
        {
            // The probability found
            double a = 0;
            // The previous probability found
            double p = 0;
            // The number of users supported
            int n = k + 1;

            // Incrementally try increasing values of n until the bionomial coeff, a, exceeds the maximum we are trying to find, y
            while(a < y)
            {
                // Save the previously calculated value of a
                p = a;
                // Find the bionomial coeff C(n,k)
                a = BionomialCoeff(n, k);
                // Move on to the next value of n
                n++;
            }
            
            // Work out whether p or a is closer to y
            if (ACloseThanB(y, a, p))
                return n;
            else
                return n - 1;
        }

        /// <summary>
        /// By extrapolating a result set, find the max number of users, (n) supported by the concurrency level, (k)
        /// </summary>
        /// <param name="k">The maximum number of concurrent users</param>
        /// <param name="results">The result set containing the values of n calculated for incrementing values of k</param>
        /// <returns>The maximum users supported for the given concurrency level</returns>
        private double ExtrapolateResult(double k, double[] results)
        {
            List<double> multiplier = new List<double>();
            // Find the average multiplier to get n from k
            for (int i = 1; i < (results.Length - 1); i++)
            {
                multiplier.Add(results[i + 1] - results[i]);
            }
            // Mulitply k by the average multiplier to find n
            double n = k * multiplier.Average();

            return n;
        }

        /// <summary>
        /// Calculate a bionomial coefficient - http://stackoverflow.com/a/12983878/3116013
        /// </summary>
        /// <param name="n">The n vale of the coefficient </param>
        /// <param name="k">The k value of the coefficient</param>
        /// <returns>The bionomial coefficient of the given values</returns>
        private double BionomialCoeff(double n, double k)
        {
            double sum = 0;
            for (double i = 0; i < k; i++)
            {
                sum += Math.Log10(n - i);
                sum -= Math.Log10(i + 1);
            }
            return (double)Math.Pow(10, sum);
        }

        /// <summary>
        /// Calculate the factorial of n
        /// </summary>
        /// <param name="n">The number to calculate a factorial of</param>
        /// <returns>n!</returns>
        private int Factorial(int n)
        {
            if (n <= 1)
                return 1;
            return n * Factorial(n - 1);
        }

        /// <summary>
        /// Determine if value a is closer to x than value b
        /// </summary>
        /// <param name="x">The value to find which number is closest to</param>
        /// <param name="a">A value</param>
        /// <param name="b">A value</param>
        /// <returns>The closest value to x</returns>
        private bool ACloseThanB(double x, double a, double b)
        {
            if (Math.Abs(x - a) < Math.Abs(x - b))
                return true;
            else
                return false;
        }

        #endregion
    }
}