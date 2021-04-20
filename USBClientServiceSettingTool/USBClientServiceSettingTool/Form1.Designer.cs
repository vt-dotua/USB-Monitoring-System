
namespace USBClientServiceSettingTool
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.ApplyButtton = new System.Windows.Forms.Button();
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.PortTextBox = new System.Windows.Forms.TextBox();
            this.IPLabel = new System.Windows.Forms.Label();
            this.PortLable = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ApplyButtton
            // 
            this.ApplyButtton.Location = new System.Drawing.Point(124, 348);
            this.ApplyButtton.Name = "ApplyButtton";
            this.ApplyButtton.Size = new System.Drawing.Size(181, 66);
            this.ApplyButtton.TabIndex = 0;
            this.ApplyButtton.Text = "Застосувати";
            this.ApplyButtton.UseVisualStyleBackColor = true;
            this.ApplyButtton.Click += new System.EventHandler(this.ApplyButtton_Click);
            // 
            // IPTextBox
            // 
            this.IPTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IPTextBox.Location = new System.Drawing.Point(36, 99);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(378, 30);
            this.IPTextBox.TabIndex = 1;
            // 
            // PortTextBox
            // 
            this.PortTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PortTextBox.Location = new System.Drawing.Point(36, 190);
            this.PortTextBox.Name = "PortTextBox";
            this.PortTextBox.Size = new System.Drawing.Size(378, 30);
            this.PortTextBox.TabIndex = 2;
            // 
            // IPLabel
            // 
            this.IPLabel.AutoSize = true;
            this.IPLabel.Location = new System.Drawing.Point(42, 70);
            this.IPLabel.Name = "IPLabel";
            this.IPLabel.Size = new System.Drawing.Size(91, 21);
            this.IPLabel.TabIndex = 3;
            this.IPLabel.Text = "Введіть IP";
            // 
            // PortLable
            // 
            this.PortLable.AutoSize = true;
            this.PortLable.Location = new System.Drawing.Point(42, 160);
            this.PortLable.Name = "PortLable";
            this.PortLable.Size = new System.Drawing.Size(115, 21);
            this.PortLable.TabIndex = 4;
            this.PortLable.Text = "Введіть порт";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 450);
            this.Controls.Add(this.PortLable);
            this.Controls.Add(this.IPLabel);
            this.Controls.Add(this.PortTextBox);
            this.Controls.Add(this.IPTextBox);
            this.Controls.Add(this.ApplyButtton);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(455, 497);
            this.MinimumSize = new System.Drawing.Size(455, 497);
            this.Name = "Form1";
            this.Text = "USB Client Service Setting Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ApplyButtton;
        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.TextBox PortTextBox;
        private System.Windows.Forms.Label IPLabel;
        private System.Windows.Forms.Label PortLable;
    }
}

