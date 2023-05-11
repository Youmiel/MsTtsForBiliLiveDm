using MsTtsForBiliLiveDm.MsTts;
using MsTtsForBiliLiveDm.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class ConfigWindow : IPluginConfigPanel
    {
        public delegate void ConfigApplyAction(PluginConfig config);

        private PluginConfig config = null;
        private ConfigApplyAction configApplyAsync = null;
        private CloseBehaviourEnum closeBehaviour;

        public CloseBehaviourEnum CloseBehaviour
        {
            get => this.closeBehaviour;
            set { 
                this.closeBehaviour = value;
                if (this.closeBehaviour == CloseBehaviourEnum.NONE || this.closeBehaviour == CloseBehaviourEnum.CROSS_CLOSE)
                {
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        this.CancelButton.Visibility = Visibility.Hidden;
                        this.CancelButton.IsEnabled = false;
                    });
                }
                else
                {
                    this.Dispatcher.Invoke((Action)delegate
                    {
                        this.CancelButton.Visibility = Visibility.Visible;
                        this.CancelButton.IsEnabled = true;
                    });
                }
            }
        }
        public ConfigApplyAction ConfigApplyAsync { get => this.configApplyAsync; set => this.configApplyAsync = value; }
        public double RequiredPanelHeight => this.Height;


        public ConfigWindow()
        {
            InitializeComponent();

            //this.config = new PluginConfig();
            this.closeBehaviour = CloseBehaviourEnum.CLOSE;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            switch (this.closeBehaviour)
            {
                case CloseBehaviourEnum.NONE:
                    e.Cancel = true;
                    break;
                case CloseBehaviourEnum.HIDE:
                    e.Cancel = true;
                    this.Visibility = Visibility.Hidden;
                    break;
                case CloseBehaviourEnum.CLOSE:
                    e.Cancel = false;
                    break;
                case CloseBehaviourEnum.CROSS_CLOSE:
                    e.Cancel = false;
                    break;
                default:
                    e.Cancel = false;
                    return;
            }
        }

        public void BehaveClose()
        {
            switch (this.closeBehaviour)
            {
                case CloseBehaviourEnum.NONE:
                    break;
                case CloseBehaviourEnum.HIDE:
                    this.Visibility = Visibility.Hidden;
                    break;
                case CloseBehaviourEnum.CLOSE:
                    this.Close();
                    break;
                case CloseBehaviourEnum.CROSS_CLOSE:
                    break;
                default:
                    return;
            }
        }

        public void BindConfig(PluginConfig config)
        {
            this.config = config;
            if (config != null)
                this.Dispatcher.Invoke((Action)delegate { this.UpdateWithConfig(config); });
        }

        public void UpdateWithConfig(PluginConfig config)
        {
            this.PortText.Text = config.Port.ToString();

            // TODO
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.config != null)
            {
                this.ApplyToConfig(this.config);
                //this.config.SaveAsync();

                // TODO
            }
            if (this.configApplyAsync != null)
                _ = Task.Run(delegate { this.configApplyAsync(this.config); });
            this.BehaveClose();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.config != null) this.UpdateWithConfig(this.config);
            this.BehaveClose();
        }

        private void PortText_KeyDown(object sender, KeyEventArgs e)
        {
            Util.LimitKeyToNumbers(sender, e);
        }

        public void ApplyToConfig(PluginConfig config)
        {
            int inputPort = int.Parse(this.PortText.Text);
            this.config.Port = inputPort;
        }
    }
}
