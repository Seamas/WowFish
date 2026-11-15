using WowScript;

namespace Fishing;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        chkboxDebug.Checked = FishConfig.Debug;
        txbDistance.Text = FishConfig.Distance.ToString();
        txbThreshold.Text = FishConfig.Threshold.ToString();
        txbBobberpoints.Text = FishConfig.BobberPoints.ToString();
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
        FishConfig.Debug = chkboxDebug.Checked;

        if (double.TryParse(txbDistance.Text, out var distance))
        {
            FishConfig.Distance = distance;
        }
        else
        {
            MessageBox.Show("上钩跳动设置错误");
        }

        if (double.TryParse(txbThreshold.Text, out var threshold))
        {
            FishConfig.Threshold = threshold;
        }
        else
        {
            MessageBox.Show("颜色阈值设置错误");
        }

        if (int.TryParse(txbBobberpoints.Text, out var bobberpoints))
        {
            FishConfig.BobberPoints = bobberpoints;
        }
        else
        {
            MessageBox.Show("颜色点数设置错误");
        }
    }
}