using LogDll;
using WowScript;

namespace Fishing;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        LogHelper.LogReceived += OnLogReceived;
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        var config = FishConfig.Instance;
        chkboxDebug.Checked = config.Debug;
        txbDistance.Text = config.Distance.ToString();
        txbThreshold.Text = config.Threshold.ToString();
        txbBobberpoints.Text = config.BobberPoints.ToString();
        chkboxUseBait.Checked = config.UseBait;
        txbBaitKey.Text = config.BaitKey;
        txbBaitDuration.Text = config.BaitDurationMinutes.ToString();
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
        var config = FishConfig.Instance;
        config.Debug = chkboxDebug.Checked;
        config.UseBait = chkboxUseBait.Checked;
        config.BaitKey = txbBaitKey.Text;

        if (double.TryParse(txbDistance.Text, out var distance))
        {
            config.Distance = distance;
        }
        else
        {
            MessageBox.Show("上钩跳动设置错误");
        }

        if (double.TryParse(txbThreshold.Text, out var threshold))
        {
            config.Threshold = threshold;
        }
        else
        {
            MessageBox.Show("颜色阈值设置错误");
        }

        if (int.TryParse(txbBobberpoints.Text, out var bobberpoints))
        {
            config.BobberPoints = bobberpoints;
        }
        else
        {
            MessageBox.Show("颜色点数设置错误");
        }

        if (int.TryParse(txbBaitDuration.Text, out var baitDuration))
        {
            config.BaitDurationMinutes = baitDuration;
        }
        else
        {
            MessageBox.Show("鱼饵持续时间设置错误");
        }

        config.Save();
        MessageBox.Show("配置已保存");
    }

    private void OnLogReceived(string message)
    {
        if (richTextBoxLog.InvokeRequired)
        {
            richTextBoxLog.BeginInvoke(new Action<string>(OnLogReceived), message);
            return;
        }

        richTextBoxLog.AppendText(message + Environment.NewLine);
        richTextBoxLog.SelectionStart = richTextBoxLog.TextLength;
        richTextBoxLog.ScrollToCaret();
    }
}
