namespace Lab10.WinForms;

partial class Form1 {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
        if (disposing && (components != null)) {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
        tabControl1 = new TabControl();
        _home = new TabPage();
        _schedule = new TabPage();
        _workload = new TabPage();
        tabControl1.SuspendLayout();
        SuspendLayout();
        // 
        // tabControl1
        // 
        tabControl1.Controls.Add(_home);
        tabControl1.Controls.Add(_schedule);
        tabControl1.Controls.Add(_workload);
        tabControl1.Dock = DockStyle.Fill;
        tabControl1.Location = new Point(0, 0);
        tabControl1.Name = "tabControl1";
        tabControl1.SelectedIndex = 0;
        tabControl1.Size = new Size(800, 450);
        tabControl1.TabIndex = 0;
        // 
        // _home
        // 
        _home.Location = new Point(4, 24);
        _home.Name = "_home";
        _home.Padding = new Padding(3);
        _home.Size = new Size(792, 422);
        _home.TabIndex = 0;
        _home.Text = "Home";
        _home.UseVisualStyleBackColor = true;
        // 
        // _schedule
        // 
        _schedule.Location = new Point(4, 24);
        _schedule.Name = "_schedule";
        _schedule.Padding = new Padding(3);
        _schedule.Size = new Size(792, 422);
        _schedule.TabIndex = 1;
        _schedule.Text = "Schedule";
        _schedule.UseVisualStyleBackColor = true;
        // 
        // _workload
        // 
        _workload.Location = new Point(4, 24);
        _workload.Name = "_workload";
        _workload.Size = new Size(792, 422);
        _workload.TabIndex = 2;
        _workload.Text = "Workload";
        _workload.UseVisualStyleBackColor = true;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(tabControl1);
        Name = "Form1";
        Text = "Form1";
        tabControl1.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TabControl tabControl1;
    private TabPage _home;
    private TabPage _schedule;
    private TabPage _workload;
}
