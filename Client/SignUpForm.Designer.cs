namespace client_ppp
{
    partial class SignUpForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.username = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.Label();
            this.userText = new System.Windows.Forms.TextBox();
            this.passText = new System.Windows.Forms.TextBox();
            this.ok = new System.Windows.Forms.Button();
            this.back = new System.Windows.Forms.Button();
            this.email = new System.Windows.Forms.Label();
            this.emailText = new System.Windows.Forms.TextBox();
            this.SignUpLabel = new System.Windows.Forms.Label();
            this.erorrLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // username
            // 
            this.username.AutoSize = true;
            this.username.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.username.ForeColor = System.Drawing.Color.Black;
            this.username.Location = new System.Drawing.Point(160, 194);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(90, 23);
            this.username.TabIndex = 3;
            this.username.Text = "Username :";
            // 
            // Password
            // 
            this.Password.AutoSize = true;
            this.Password.BackColor = System.Drawing.Color.Transparent;
            this.Password.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Password.ForeColor = System.Drawing.Color.Black;
            this.Password.Location = new System.Drawing.Point(160, 227);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(86, 23);
            this.Password.TabIndex = 4;
            this.Password.Text = "Password :";
            // 
            // userText
            // 
            this.userText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userText.Location = new System.Drawing.Point(252, 195);
            this.userText.Name = "userText";
            this.userText.Size = new System.Drawing.Size(380, 26);
            this.userText.TabIndex = 5;
            // 
            // passText
            // 
            this.passText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.passText.Location = new System.Drawing.Point(252, 227);
            this.passText.Name = "passText";
            this.passText.Size = new System.Drawing.Size(380, 26);
            this.passText.TabIndex = 6;
            // 
            // ok
            // 
            this.ok.BackColor = System.Drawing.Color.White;
            this.ok.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ok.ForeColor = System.Drawing.Color.Black;
            this.ok.Location = new System.Drawing.Point(164, 337);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(204, 58);
            this.ok.TabIndex = 7;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = false;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // back
            // 
            this.back.BackColor = System.Drawing.Color.White;
            this.back.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back.ForeColor = System.Drawing.Color.Black;
            this.back.Location = new System.Drawing.Point(428, 337);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(204, 58);
            this.back.TabIndex = 8;
            this.back.Text = "Back";
            this.back.UseVisualStyleBackColor = false;
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // email
            // 
            this.email.AutoSize = true;
            this.email.BackColor = System.Drawing.Color.Transparent;
            this.email.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.email.ForeColor = System.Drawing.Color.Black;
            this.email.Location = new System.Drawing.Point(160, 263);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(59, 23);
            this.email.TabIndex = 9;
            this.email.Text = "Email :";
            // 
            // emailText
            // 
            this.emailText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.emailText.Location = new System.Drawing.Point(252, 260);
            this.emailText.Name = "emailText";
            this.emailText.Size = new System.Drawing.Size(380, 26);
            this.emailText.TabIndex = 10;
            // 
            // SignUpLabel
            // 
            this.SignUpLabel.AutoSize = true;
            this.SignUpLabel.Font = new System.Drawing.Font("Comic Sans MS", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignUpLabel.ForeColor = System.Drawing.Color.Black;
            this.SignUpLabel.Location = new System.Drawing.Point(324, 107);
            this.SignUpLabel.Name = "SignUpLabel";
            this.SignUpLabel.Size = new System.Drawing.Size(190, 67);
            this.SignUpLabel.TabIndex = 17;
            this.SignUpLabel.Text = "Sign up";
            // 
            // erorrLabel
            // 
            this.erorrLabel.AutoSize = true;
            this.erorrLabel.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.erorrLabel.ForeColor = System.Drawing.Color.White;
            this.erorrLabel.Location = new System.Drawing.Point(309, 301);
            this.erorrLabel.Name = "erorrLabel";
            this.erorrLabel.Size = new System.Drawing.Size(0, 23);
            this.erorrLabel.TabIndex = 19;
            this.erorrLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SignUpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(789, 419);
            this.Controls.Add(this.erorrLabel);
            this.Controls.Add(this.SignUpLabel);
            this.Controls.Add(this.emailText);
            this.Controls.Add(this.email);
            this.Controls.Add(this.back);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.passText);
            this.Controls.Add(this.userText);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.username);
            this.ForeColor = System.Drawing.Color.White;
            this.Location = new System.Drawing.Point(167, 9);
            this.Name = "SignUpForm";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label username;
        private System.Windows.Forms.Label Password;
        private System.Windows.Forms.TextBox userText;
        private System.Windows.Forms.TextBox passText;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Label email;
        private System.Windows.Forms.TextBox emailText;
        private System.Windows.Forms.Label SignUpLabel;
        private System.Windows.Forms.Label erorrLabel;
    }
}