using System.ComponentModel;

namespace Fishing;

partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
        splitContainer1 = new SplitContainer();
        groupbox1 = new GroupBox();
        btnApply = new Button();
        txbBobberpoints = new TextBox();
        label3 = new Label();
        txbDistance = new TextBox();
        label2 = new Label();
        txbThreshold = new TextBox();
        label1 = new Label();
        chkboxDebug = new CheckBox();
        ((ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.SuspendLayout();
        groupbox1.SuspendLayout();
        SuspendLayout();
        // 
        // splitContainer1
        // 
        splitContainer1.Dock = DockStyle.Fill;
        splitContainer1.Location = new Point(0, 0);
        splitContainer1.Name = "splitContainer1";
        splitContainer1.Orientation = Orientation.Horizontal;
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(groupbox1);
        splitContainer1.Size = new Size(1675, 986);
        splitContainer1.SplitterDistance = 211;
        splitContainer1.TabIndex = 0;
        // 
        // groupbox1
        // 
        groupbox1.Controls.Add(btnApply);
        groupbox1.Controls.Add(txbBobberpoints);
        groupbox1.Controls.Add(label3);
        groupbox1.Controls.Add(txbDistance);
        groupbox1.Controls.Add(label2);
        groupbox1.Controls.Add(txbThreshold);
        groupbox1.Controls.Add(label1);
        groupbox1.Controls.Add(chkboxDebug);
        groupbox1.Dock = DockStyle.Fill;
        groupbox1.Location = new Point(0, 0);
        groupbox1.Name = "groupbox1";
        groupbox1.Size = new Size(1675, 211);
        groupbox1.TabIndex = 0;
        groupbox1.TabStop = false;
        groupbox1.Text = "设置区";
        // 
        // btnApply
        // 
        btnApply.Location = new Point(1243, 54);
        btnApply.Name = "btnApply";
        btnApply.Size = new Size(112, 34);
        btnApply.TabIndex = 7;
        btnApply.Text = "应用";
        btnApply.UseVisualStyleBackColor = true;
        btnApply.Click += btnApply_Click;
        // 
        // txbBobberpoints
        // 
        txbBobberpoints.Location = new Point(992, 55);
        txbBobberpoints.Name = "txbBobberpoints";
        txbBobberpoints.Size = new Size(150, 30);
        txbBobberpoints.TabIndex = 6;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(877, 57);
        label3.Name = "label3";
        label3.Size = new Size(82, 24);
        label3.TabIndex = 5;
        label3.Text = "颜色点数";
        // 
        // txbDistance
        // 
        txbDistance.Location = new Point(667, 54);
        txbDistance.Name = "txbDistance";
        txbDistance.Size = new Size(150, 30);
        txbDistance.TabIndex = 4;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(552, 56);
        label2.Name = "label2";
        label2.Size = new Size(82, 24);
        label2.TabIndex = 3;
        label2.Text = "上钩跳动";
        // 
        // txbThreshold
        // 
        txbThreshold.Location = new Point(328, 53);
        txbThreshold.Name = "txbThreshold";
        txbThreshold.Size = new Size(150, 30);
        txbThreshold.TabIndex = 2;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(213, 55);
        label1.Name = "label1";
        label1.Size = new Size(82, 24);
        label1.TabIndex = 1;
        label1.Text = "颜色阈值";
        // 
        // chkboxDebug
        // 
        chkboxDebug.AutoSize = true;
        chkboxDebug.Location = new Point(85, 54);
        chkboxDebug.Name = "chkboxDebug";
        chkboxDebug.Size = new Size(72, 28);
        chkboxDebug.TabIndex = 0;
        chkboxDebug.Text = "调试";
        chkboxDebug.UseVisualStyleBackColor = true;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(11F, 24F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1675, 986);
        Controls.Add(splitContainer1);
        Name = "MainForm";
        Text = "MainForm";
        Load += MainForm_Load;
        splitContainer1.Panel1.ResumeLayout(false);
        ((ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        groupbox1.ResumeLayout(false);
        groupbox1.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private SplitContainer splitContainer1;
    private GroupBox groupbox1;
    private TextBox txbDistance;
    private Label label2;
    private TextBox txbThreshold;
    private Label label1;
    private CheckBox chkboxDebug;
    private TextBox txbBobberpoints;
    private Label label3;
    private Button btnApply;
}