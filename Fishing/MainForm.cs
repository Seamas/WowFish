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
        chkboxDebug.Checked = FishConfig.Instance.Debug;
        txbDistance.Text = FishConfig.Instance.Distance.ToString();
        txbThreshold.Text = FishConfig.Instance.Threshold.ToString();
        txbBobberpoints.Text = FishConfig.Instance.BobberPoints.ToString();
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
        FishConfig.Instance.Debug = chkboxDebug.Checked;

        if (double.TryParse(txbDistance.Text, out var distance))
        {
            FishConfig.Instance.Distance = distance;
        }
        else
        {
            MessageBox.Show("上钩跳动设置错误");
        }

        if (double.TryParse(txbThreshold.Text, out var threshold))
        {
            FishConfig.Instance.Threshold = threshold;
        }
        else
        {
            MessageBox.Show("颜色阈值设置错误");
        }

        if (int.TryParse(txbBobberpoints.Text, out var bobberpoints))
        {
            FishConfig.Instance.BobberPoints = bobberpoints;
        }
        else
        {
            MessageBox.Show("颜色点数设置错误");
        }
    }
}