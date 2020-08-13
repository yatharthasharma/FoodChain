/*
 * @author Yathartha Sharma
 * @email yatharthasharma09@gmail.com
 * @create February 2019
 * @description
*/

using System;
using Parse;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using System.Threading.Tasks;

namespace FoodChain
{
    [Activity(Label = "RegisterActivity", Theme = "@style/AppTheme.NoActionBar")]
    public class RegisterActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_register);
            Button registerRegButton = FindViewById<Button>(Resource.Id.register_registerbutton);
            /*registerRegButton.Enabled = false;
            string namex = FindViewById<TextInputEditText>(Resource.Id.register_usernametext).Text;
            string emailx = FindViewById<TextInputEditText>(Resource.Id.register_emailtext).Text;
            string passwordx = FindViewById<TextInputEditText>(Resource.Id.register_passwordtext).Text;
            string reenterpasswordx = FindViewById<TextInputEditText>(Resource.Id.register_passwordmatchtext).Text;
            TextInputEditText xyz = FindViewById<TextInputEditText>(Resource.Id.register_usernametext);
            
            if (namex != null && emailx != null && passwordx != null && reenterpasswordx != null)
            {
                registerRegButton.Enabled = true;
            }*/
            registerRegButton.Click += async delegate
            {
                TextInputEditText name = FindViewById<TextInputEditText>(Resource.Id.register_nametext);
                TextInputEditText username = FindViewById<TextInputEditText>(Resource.Id.register_usernametext);
                TextInputEditText email = FindViewById<TextInputEditText>(Resource.Id.register_emailtext);
                TextInputEditText password = FindViewById<TextInputEditText>(Resource.Id.register_passwordtext);
                TextInputEditText reenterpassword = FindViewById<TextInputEditText>(Resource.Id.register_passwordmatchtext);
                if (name.Text.Length < 6)
                {
                    if (password.Text.Length < 6)
                    {
                        password.Error = Resources.GetString(Resource.String.register_passwordlength);
                        username.Error = Resources.GetString(Resource.String.register_usernamelength);
                    }
                    else
                    {
                        password.Error = null;
                        username.Error = Resources.GetString(Resource.String.register_usernamelength);
                    }
                }
                else
                {
                    if (password.Text.Length < 6)
                    {
                        password.Error = Resources.GetString(Resource.String.register_passwordlength);
                        username.Error = null;
                    }
                    /*else
                    {
                        password.Error = null;
                        username.Error = null;
                    }*/
                }
                if (name.Text == "" || email.Text == "" || password.Text == "" || reenterpassword.Text == "")
                {
                    if (name.Text == "")
                        name.Error = Resources.GetString(Resource.String.register_nameempty);
                    if (username.Text == "")
                        username.Error = Resources.GetString(Resource.String.register_usernameempty);
                    if (email.Text == "")
                        email.Error = Resources.GetString(Resource.String.register_emailempty);
                    if (password.Text == "")
                        password.Error = Resources.GetString(Resource.String.register_passwordempty);
                    if (reenterpassword.Text == "")
                        reenterpassword.Error = Resources.GetString(Resource.String.register_passwordmatchempty);
                }
                if (password.Text != reenterpassword.Text)
                    reenterpassword.Error = Resources.GetString(Resource.String.register_passwordmatchincorrect);
                if (name.Text != "" && email.Text != "" && password.Text != "" && reenterpassword.Text != "" && password != reenterpassword)
                {
                    name.Error = null;
                    username.Error = null;
                    email.Error = null;
                    password.Error = null;
                    reenterpassword.Error = null;
                    if (HelperFunctions.DoIHaveInternet())      // if internet is available
                    {
                        await RegisterUser(this, name.Text, username.Text, email.Text, password.Text);
                    }
                    else                // if no internet
                    {
                        HelperFunctions.UseAlertBox(this, "No Network Connectivity", "It seems that you do not have a working internet connection!");
                    }
                }
            };

        }
        public async Task RegisterUser(Context context, string name, string username, string email, string password)
        {
            AlertDialog dialog = null;
            try
            {
                var newUser = new ParseUser()
                {
                    Username = username,
                    Password = password,
                    Email = email
                };
                newUser["FullName"] = name;
                dialog = HelperFunctions.UseAlertBoxProgressBar(this, "Registering your details...");
                await newUser.SignUpAsync();
                HelperFunctions.HideAlertBoxProgressBar(dialog);
                HelperFunctions.UseAlertBox(context, "Registration Successful", "You are now successfully registered! Log in with your credentials to access the application.");
                Intent intent = new Intent(context, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.ClearTask);
                StartActivity(intent);
            }
            catch (System.InvalidOperationException)
            {
                HelperFunctions.HideAlertBoxProgressBar(dialog);
                HelperFunctions.UseAlertBox(context, "Unable to Register", "It seems that one or more fields are missing or empty.");
            }
            catch (ParseException p){                   // WARNING: Some parse error codes might not have been included - look at those
                if (p.Code == ParseException.ErrorCode.UsernameMissing)
                {
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                    HelperFunctions.UseAlertBox(context, "Username Missing", "The username is missing or empty.");
                }
                else if (p.Code == ParseException.ErrorCode.UsernameTaken)
                {
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                    HelperFunctions.UseAlertBox(context, "Username Taken", "The username has already been taken.");
                }
                else if (p.Code == ParseException.ErrorCode.EmailMissing)
                {
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                    HelperFunctions.UseAlertBox(context, "Email Missing", "The email is missing, and must be specified.");
                }
                else if (p.Code == ParseException.ErrorCode.EmailTaken)
                {
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                    HelperFunctions.UseAlertBox(context, "Email Taken", "Email has already been used.");
                }
                else if (p.Code == ParseException.ErrorCode.PasswordMissing)
                {
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                    HelperFunctions.UseAlertBox(context, "Password Missing", "The password is missing or empty.");
                }   
                else if (p.Code == ParseException.ErrorCode.InvalidEmailAddress)
                {
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                    HelperFunctions.UseAlertBox(context, "Invalid Email", "The email address format is invalid.");
                }
                else
                {
                    Console.WriteLine(p);
                    HelperFunctions.HideAlertBoxProgressBar(dialog);
                    HelperFunctions.UseAlertBox(context, "Unknown Parse error", "The database has encountered an unknown error. Please try signing up again later.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                HelperFunctions.HideAlertBoxProgressBar(dialog);
                HelperFunctions.UseAlertBox(context, "Unknown System error", "The system has encountered an unknown error. Please try signing up again later.");
            }
        }
    }
}