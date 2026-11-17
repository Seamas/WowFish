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
        txbBaitDuration = new TextBox();
        label6 = new Label();
        txbBaitKey = new TextBox();
        label5 = new Label();
        chkboxUseBait = new CheckBox();
        btnApply = new Button();
        txbBobberpoints = new TextBox();
        label3 = new Label();
        txbDistance = new TextBox();
        label2 = new Label();
        txbThreshold = new TextBox();
        label1 = new Label();
        chkboxDebug = new CheckBox();
        richTextBoxLog = new RichTextBox();
        ((ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
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
        //
        // splitContainer1.Panel2
        //
        splitContainer1.Panel2.Controls.Add(richTextBoxLog);
        splitContainer1.Size = new Size(1200, 800);
        splitContainer1.SplitterDistance = 225;
        splitContainer1.TabIndex = 0;
        //
        // groupbox1
        //
        groupbox1.Controls.Add(txbFillKey);
        groupbox1.Controls.Add(label7);
        groupbox1.Controls.Add(chkboxUseFillKey);
        groupbox1.Controls.Add(txbBaitDuration);
        groupbox1.Controls.Add(label6);
        groupbox1.Controls.Add(txbBaitKey);
        groupbox1.Controls.Add(label5);
        groupbox1.Controls.Add(chkboxUseBait);
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
        groupbox1.Size = new Size(1200, 225);
        groupbox1.TabIndex = 0;
        groupbox1.TabStop = false;
        groupbox1.Text = "设置区";
        //
        // txbFillKey
        //
        txbFillKey.Location = new Point(430, 155);
        txbFillKey.Name = "txbFillKey";
        txbFillKey.Size = new Size(120, 30);
        txbFillKey.TabIndex = 15;
        //
        // label7
        //
        label7.AutoSize = true;
        label7.Location = new Point(330, 158);
        label7.Name = "label7";
        label7.Size = new Size(82, 24);
        label7.TabIndex = 14;
        label7.Text = "填充按键";
        //
        // chkboxUseFillKey
        //
        chkboxUseFillKey.AutoSize = true;
        chkboxUseFillKey.Location = new Point(85, 155);
        chkboxUseFillKey.Name = "chkboxUseFillKey";
        chkboxUseFillKey.Size = new Size(124, 28);
        chkboxUseFillKey.TabIndex = 13;
        chkboxUseFillKey.Text = "使用填充键";
        chkboxUseFillKey.UseVisualStyleBackColor = true;
        //
        // txbBaitDuration
        //
        txbBaitDuration.Location = new Point(760, 110);
        txbBaitDuration.Name = "txbBaitDuration";
        txbBaitDuration.Size = new Size(120, 30);
        txbBaitDuration.TabIndex = 12;
        //
        // label6
        //
        label6.AutoSize = true;
        label6.Location = new Point(610, 113);
        label6.Name = "label6";
        label6.Size = new Size(140, 24);
        label6.TabIndex = 11;
        label6.Text = "鱼饵持续(分钟)";
        //
        // txbBaitKey
        //
        txbBaitKey.Location = new Point(430, 110);
        txbBaitKey.Name = "txbBaitKey";
        txbBaitKey.Size = new Size(120, 30);
        txbBaitKey.TabIndex = 10;
        //
        // label5
        //
        label5.AutoSize = true;
        label5.Location = new Point(330, 113);
        label5.Name = "label5";
        label5.Size = new Size(82, 24);
        label5.TabIndex = 9;
        label5.Text = "鱼饵按键";
        //
        // chkboxUseBait
        //
        chkboxUseBait.AutoSize = true;
        chkboxUseBait.Location = new Point(85, 110);
        chkboxUseBait.Name = "chkboxUseBait";
        chkboxUseBait.Size = new Size(124, 28);
        chkboxUseBait.TabIndex = 8;
        chkboxUseBait.Text = "自动上鱼饵";
        chkboxUseBait.UseVisualStyleBackColor = true;
        //
        // btnApply
        //
        btnApply.Location = new Point(1000, 55);
        btnApply.Name = "btnApply";
        btnApply.Size = new Size(112, 34);
        btnApply.TabIndex = 7;
        btnApply.Text = "应用";
        btnApply.UseVisualStyleBackColor = true;
        btnApply.Click += btnApply_Click;
        //
        // txbBobberpoints
        //
        txbBobberpoints.Location = new Point(820, 55);
        txbBobberpoints.Name = "txbBobberpoints";
        txbBobberpoints.Size = new Size(120, 30);
        txbBobberpoints.TabIndex = 6;
        //
        // label3
        //
        label3.AutoSize = true;
        label3.Location = new Point(720, 58);
        label3.Name = "label3";
        label3.Size = new Size(82, 24);
        label3.TabIndex = 5;
        label3.Text = "颜色点数";
        //
        // txbDistance
        //
        txbDistance.Location = new Point(540, 55);
        txbDistance.Name = "txbDistance";
        txbDistance.Size = new Size(120, 30);
        txbDistance.TabIndex = 4;
        //
        // label2
        //
        label2.AutoSize = true;
        label2.Location = new Point(440, 58);
        label2.Name = "label2";
        label2.Size = new Size(82, 24);
        label2.TabIndex = 3;
        label2.Text = "上钩跳动";
        //
        // txbThreshold
        //
        txbThreshold.Location = new Point(270, 55);
        txbThreshold.Name = "txbThreshold";
        txbThreshold.Size = new Size(120, 30);
        txbThreshold.TabIndex = 2;
        //
        // label1
        //
        label1.AutoSize = true;
        label1.Location = new Point(170, 58);
        label1.Name = "label1";
        label1.Size = new Size(82, 24);
        label1.TabIndex = 1;
        label1.Text = "颜色阈值";
        //
        // chkboxDebug
        //
        chkboxDebug.AutoSize = true;
        chkboxDebug.Location = new Point(85, 55);
        chkboxDebug.Name = "chkboxDebug";
        chkboxDebug.Size = new Size(72, 28);
        chkboxDebug.TabIndex = 0;
        chkboxDebug.Text = "调试";
        chkboxDebug.UseVisualStyleBackColor = true;
        //
        // richTextBoxLog
        //
        richTextBoxLog.Dock = DockStyle.Fill;
        richTextBoxLog.Font = new Font("Consolas", 10F);
        richTextBoxLog.Location = new Point(0, 0);
        richTextBoxLog.Name = "richTextBoxLog";
        richTextBoxLog.ReadOnly = true;
        richTextBoxLog.Size = new Size(1200, 616);
        richTextBoxLog.TabIndex = 0;
        richTextBoxLog.Text = "";
        richTextBoxLog.WordWrap = true;
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(11F, 24F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 800);
        Controls.Add(splitContainer1);
        Name = "MainForm";
        Text = "WowFish - 钓鱼助手";
        Load += MainForm_Load;
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
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
    private CheckBox chkboxUseBait;
    private TextBox txbBaitKey;
    private Label label5;
    private TextBox txbBaitDuration;
    private Label label6;
    private CheckBox chkboxUseFillKey;
    private TextBox txbFillKey;
    private Label label7;
    private RichTextBox richTextBoxLog;
}
