using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MsTtsForBiliLiveDm.Plugin
{
    /// <summary>
    /// MsTtsConfigPanel.xaml 的交互逻辑
    /// </summary>
    public partial class MsTtsConfigPanel : UserControl, IPluginConfigPanel
    {
        public double RequiredPanelHeight => this.Height;

        public MsTtsConfigPanel()
        {
            InitializeComponent();

            this.VoiceTypeBox.Items.Clear();
            ICollection<MsVoiceType> voiceTypes = MsVoiceType.ALL_VOICE;
            foreach (MsVoiceType v in voiceTypes)
                this.VoiceTypeBox.Items.Add(v);
            this.VoiceTypeBox.SelectedIndex = 0;
        }

        public void ApplyToConfig(PluginConfig config)
        {
            config.VoiceType = (MsVoiceType)this.VoiceTypeBox.SelectedItem;
            config.Rate = (int)this.RateSlider.Value;
            config.Pitch = (int)this.PitchSlider.Value;
        }

        public void UpdateWithConfig(PluginConfig config)
        {
            this.VoiceTypeBox.SelectedItem = config.VoiceType;
            this.RateSlider.Value = config.Rate;
            this.PitchSlider.Value = config.Pitch;
        }

        private void RateText_KeyDown(object sender, KeyEventArgs e)
        {
            Util.SyncTextToSlider(this.RateText, this.RateSlider,
                (int num) => Util.Clamp(num, (int)this.RateSlider.Minimum, (int)this.RateSlider.Maximum));
        }

        private void PitchText_KeyDown(object sender, KeyEventArgs e)
        {
            Util.SyncTextToSlider(this.PitchText, this.PitchSlider,
                (int num) => Util.Clamp(num, (int)this.RateSlider.Minimum, (int)this.RateSlider.Maximum));
        }

        private void RateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Util.SyncSliderToText(this.RateSlider, this.RateText);
        }

        private void PitchSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Util.SyncSliderToText(this.PitchSlider, this.PitchText);
        }

        private void HelpLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://learn.microsoft.com/zh-cn/training/modules/transcribe-speech-input-text/1-introduction");
            }
            catch
            {
            }
        }
    }
}
