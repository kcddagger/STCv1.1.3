using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using System.IO;
using SQLite;
using STC.Data;

namespace STC
{
    [Activity(Label = "STC", MainLauncher = true, Icon = "@mipmap/stc_launcher")]
    public class MainActivity : Activity
    {
        private TextView calculatorTop;
        private TextView calculatorMiddle;
        private TextView calculatorBottom;
        private TextView db_return;

        private string[] numbers = new string[2];
        private string @operator;
        private string zipCode = "";
        private string taxState = "";
        private string taxPercent = "";
        private string sTaxStr = "";
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource. This is what the user will see when the program opens.
            SetContentView(Resource.Layout.Main);

            //Prepares and installs the database on first load if the DB isn't set up.
            SQLite_Android.NewDatabase();

            //Initializes the text views to ready them to accept and show data.
            calculatorTop = FindViewById<TextView>(Resource.Id.calculator_top_view);
            calculatorMiddle = FindViewById<TextView>(Resource.Id.calculator_middle_view);
            calculatorBottom = FindViewById<TextView>(Resource.Id.calculator_bottom_view);
            db_return = FindViewById<TextView>(Resource.Id.db_return);

            //pulls up the zip code entry point for the user so the information can be 
            //passed to the search of the database and back to be displayed at the top of the screen.
            Button button = FindViewById<Button>(Resource.Id.Zip);
            button.Click += delegate
            {
                LayoutInflater layoutInflater = LayoutInflater.From(this);
                View view = layoutInflater.Inflate(Resource.Layout.Get_Zip, null);
                AlertDialog.Builder zipAlert = new AlertDialog.Builder(this);
                zipAlert.SetView(view);
                var zip_Code = view.FindViewById<EditText>(Resource.Id.editZip);
                zipAlert.SetCancelable(false);
                zipAlert.SetPositiveButton("Submit", delegate
                {
                    zipCode = zip_Code.Text;
                    Toast.MakeText(this, "Submit Input: " + zip_Code.Text, ToastLength.Short).Show();
                    GetTaxes();
                });
                zipAlert.SetNegativeButton("Cancel", delegate
                 {
                     zipAlert.Dispose();
                 });
                AlertDialog dialog = zipAlert.Create();
                dialog.Show();                
            };

            
        }

        //Gets the Sales tax information from the database via the API and formats it for display on the screen.
        private void GetTaxes()
        {            
            try
            {
                TaxRates taxRates = new TaxRates();
                taxRates = SalesTaxAPI.GetSalesTax(zipCode);
                sTaxStr = taxRates.EstimatedCombinedRate;
                taxState = taxRates.State;
                decimal taxPercentage = decimal.Parse(sTaxStr);
                //taxPercentage = (taxPercentage * 100);
                taxPercentage = Math.Round(taxPercentage, 4, MidpointRounding.ToEven);
                taxPercent = taxPercentage.ToString("p");


                db_return.Text = "State: " + taxState + "  Zip: " + zipCode + "  Tax %: " + taxPercent;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Error: {0}", e);

                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Error!");
                alert.SetMessage("Invalid Zipcode. Please try again.");
                alert.SetCancelable(true);
                Dialog dialog = alert.Create();
                dialog.Show();
            }
        }

        //Everything from here down is the actual calculator
        [Java.Interop.Export("ButtonClick")]
        public void ButtonClick(View v)
        {
            Button button = (Button)v;

            if ("0123456789.".Contains(button.Text))
            {
                AddDigitOrDecimalPoint(button.Text);
            }
            else if ("÷×+-".Contains(button.Text))
            {
                AddOperator(button.Text);
            }
            else if ("=" == button.Text)
            {
                Calculate();
            }
            else if ("+Tax" == button.Text)
            {
                AddTax();
            }
            else if ("-Tax" == button.Text)
            {
                MinusTax();
            }
            else
            {
                Erase();
            }
        }

        //Resets the calculator variables to null and blanks out the three output locations.
        private void Erase()
        {
            numbers[0] = numbers[1] = null;
            @operator = null;
            UpdateCalculatorTop();
            calculatorMiddle.Text = null;
            calculatorBottom.Text = null;            
        }

        //Takes sales tax data that had been pulled from the database and the current total or sum entered in 
        //and calculates the sales tax and sub-total before displaying it in US currency format.
        private void MinusTax()
        {
            decimal gTotal;
            decimal sTax = decimal.Parse(sTaxStr);
            sTax = sTax + 1;
            decimal firstNumber = decimal.Parse(numbers[0]);
            firstNumber = Math.Round(firstNumber, 2, MidpointRounding.ToEven);
                        
            gTotal = (firstNumber / sTax);
            gTotal = Math.Round(gTotal, 2, MidpointRounding.ToEven);
            decimal totalTax = (firstNumber - gTotal);
            totalTax = Math.Round(totalTax, 2, MidpointRounding.ToEven);

            calculatorTop.Text = gTotal.ToString("c");
            calculatorMiddle.Text = totalTax.ToString("c");
            calculatorBottom.Text = firstNumber.ToString("c");
        }

        //Takes the sales tax data pulled from the database and the current total or entered sum 
        //and calculates the sales tax and grand total before displaying in US Currency format.
        private void AddTax()
        {
            decimal gTotal;
            decimal sTax = decimal.Parse(sTaxStr);
            sTax = sTax + 1;
            decimal firstNumber = decimal.Parse(numbers[0]);
            firstNumber = Math.Round(firstNumber, 2, MidpointRounding.ToEven);

            gTotal = (sTax * firstNumber);
            gTotal = Math.Round(gTotal, 2, MidpointRounding.ToEven);
            decimal totalTax = (gTotal - firstNumber);
            totalTax = Math.Round(totalTax, 2, MidpointRounding.ToEven);


            calculatorTop.Text = firstNumber.ToString("c");
            calculatorMiddle.Text = totalTax.ToString("c");
            calculatorBottom.Text = gTotal.ToString("c");
        }

        //This method handles that actual calculation of the two numbers utilizing the operator that the 
        //user has entered in when using as a standard calculator
        private void Calculate(string newOperator = null)
        {
            decimal? result = null;
            decimal? firstNumber = numbers[0] == null ? null : (decimal?)decimal.Parse(numbers[0]);
            decimal? secondNumber = numbers[1] == null ? null : (decimal?)decimal.Parse(numbers[1]);

            switch (@operator)
            {
                case "+":
                    result = firstNumber + secondNumber;
                    break;
                case "-":
                    result = firstNumber - secondNumber;
                    break;
                case "×":
                    result = firstNumber * secondNumber;
                    break;
                case "÷":
                    try
                    {
                        result = firstNumber / secondNumber;
                    }
                    catch (DivideByZeroException e)
                    {
                        Console.WriteLine("Error: {0}",e);

                        AlertDialog.Builder alert = new AlertDialog.Builder(this);
                        alert.SetTitle("Error!");
                        alert.SetMessage("Can not divide by 0.");
                        alert.SetCancelable(true);
                        Dialog dialog = alert.Create();    
                        dialog.Show();
                    }
                    break;
            }

            if (result != null)
            {
                numbers[0] = result.ToString();
                @operator = newOperator;
                numbers[1] = null;
                UpdateCalculatorTop();
            }
        }

        //Handles the operators the user enters in by triggering a calculation or 
        //the display of an operator depending on how many of the two numbers have data in them.
        private void AddOperator(string value)
        {
            if (numbers[1] != null)
            {
                Calculate(value);
            }
            else
            {
                @operator = value;
            }

            UpdateCalculatorTop();
        }

        //Handles adding the numbers the user enters in to the first or second number and prevents the use of multiple decimal points.
        private void AddDigitOrDecimalPoint(string value)
        {
            int index = @operator == null ? 0 : 1;

            if (value == "." && numbers[index].Contains("."))
            {
                return;
            }
            else
            {
                numbers[index] += value;
            }

            UpdateCalculatorTop();
        }

        //Updates the calculatorTop view to display new numbers being entered in or new operators
        private void UpdateCalculatorTop()
        {
            calculatorTop.Text = $"{numbers[0]} {@operator} {numbers[1]}";
        }
    }
}

