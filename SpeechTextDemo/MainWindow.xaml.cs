using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
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

namespace SpeechTextDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        SpeechSynthesizer synth;
        bool LoopState;//线程中循环运行的标志位
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (synth != null)
            {
                Button_Click_Stop(null,null);//如果不释放之前的资源，就会播放多个声音
            }
            synth = new SpeechSynthesizer();//Dispose()之后，虽然synth不为空，但是已经释放资源，所以每次都需要创建新实例
            LoopState = true;
            string text = SpeechTextTB.Text;//其他线程无法直接使用UI线程的变量，所以需要见SpeechTextTB.Text赋值给一个普通变量
            Task.Run(() =>
            {
                synth.SetOutputToDefaultAudioDevice();
                synth.SelectVoiceByHints(VoiceGender.Male);//不起作用
                PromptBuilder prompt = new PromptBuilder();

                PromptStyle style = new PromptStyle()
                {
                    Rate = PromptRate.ExtraFast,
                    Emphasis = PromptEmphasis.Strong,
                    Volume = PromptVolume.ExtraLoud,
                };

                //指定特殊的文本使用style
                prompt.StartStyle(style);
                prompt.AppendText(text);
                prompt.EndStyle();

                prompt.AppendText("来电话了");

                //循环播放声音
                while (LoopState)
                {
                    synth.Speak(prompt);
                }
            });

        }

        private void Button_Click_Pause(object sender, RoutedEventArgs e)
        {
            synth.Pause();
        }

        private void Button_Click_Resume(object sender, RoutedEventArgs e)
        {
            synth.Resume();
        }

        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            synth.Dispose();
            synth = null;
            LoopState = false;
        }
    }
}
