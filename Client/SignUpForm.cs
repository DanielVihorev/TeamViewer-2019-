using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace client_ppp
{
    public partial class SignUpForm : Form
    {
        public SignUpForm()
        {
            InitializeComponent();
        }

        /* check if user entered valid username password and email and let him sign up
        * input: object sender, EventArgs 
        * output: null
        */
        private void ok_Click(object sender, EventArgs e)
        {
            try
            {
                if (userText.Text != "" && passText.Text != "" && emailText.Text != "")
                {
                    if (userText.Text == passText.Text)
                    {
                        MessageBox.Show("Password cant be same as Username!!!");
                        return;
                    }

                    if (!IsValidEmail(emailText.Text))
                    {
                        MessageBox.Show("Invalid Email...\nPlease Try Again");
                        return;
                    }


                    string password = passText.Text;
                    erorrLabel.Text = string.Empty;
                    string uLen = userText.Text.Length.ToString();
                    string pLen = password.Length.ToString();
                    string eLen = emailText.Text.Length.ToString();
                    if (userText.Text.Length < 10)
                    {
                        uLen = "0" + userText.Text.Length.ToString();
                    }
                    if (password.Length < 10)
                    {
                        pLen = "0" + password.Length.ToString();
                    }
                    if (emailText.Text.Length < 10)
                    {
                        eLen = "0" + emailText.Text.Length.ToString();
                    }
                    
                    LogInScreen.MSG = Constants.SIGN_UP + uLen + userText.Text + pLen + password + eLen + emailText.Text;
                    LogInScreen.msgHandler();
                    if (LogInScreen.RecivedMSG == Constants.SUCCESS)
                    {
                        this.Hide();
                    }
                    else if (LogInScreen.RecivedMSG == Constants.USER_EXISTS)
                    {
                        MessageBox.Show("User already exists!\ntry another username...");
                    }
                    else if (LogInScreen.RecivedMSG == Constants.PASSWORD_INVALID)
                    {
                        MessageBox.Show("Invalid Password!!!\nPassword must contain at least 6 characters.\nat least one uppercase letter, one lowercase letter and one digit.(password only from letters and digits)\nPassword Example: Ben123");
                    }
                    else if (LogInScreen.RecivedMSG == Constants.USERNAME_INVALID)
                    {
                        MessageBox.Show("Invalid Username!!!\nUsername must contain at least 6 characters.\nall characters must be letters or digits.and first charecter must be a letter.\nUsername Example: Kfir235");
                    }

                }
                else
                {
                    //erorrLabel.Text = "Error";
                    MessageBox.Show("username or password or email cannot be empty!!!");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /* return to log in form
        * input: object sender, EventArgs 
        * output: null
        */
        private void back_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        /* check if given email is a valid email
        * input: string email
        * output: null
        */
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
