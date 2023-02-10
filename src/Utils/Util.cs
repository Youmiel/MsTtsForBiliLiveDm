using MsTtsForBiliLiveDm.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace MsTtsForBiliLiveDm.Utils
{
    public class Util
    {
        public delegate void LogMethod(string content);
        public delegate int IntFunction(int num);

        public static LogMethod LogContent { get; set; }
        public static LogMethod DebugContent { get; set; }

        public static void RunInSTAThread(ThreadStart task)
        {
            Thread thread = new Thread(task);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public static T Clamp<T>(T value, T lower, T upper) where T : IComparable
        {
            if (lower.CompareTo(upper) >= 0)
                return lower;
            
            if (value.CompareTo(upper) > 0)
                return upper;

            if (value.CompareTo(lower) < 0)
                return lower;

            return value;
        }

        public static bool LimitKeyToNumbers(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
                return true;
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab || e.Key == Key.Enter || e.Key == Key.Escape)
                return true;
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                return true;

            e.Handled = true;
            return false;
        }

        public static void SyncTextToSlider(TextBox text, Slider slider, IntFunction op)
        {
            int sliderValue = (int)slider.Value;
            int textNum;
            if (int.TryParse(text.Text, out textNum))
            {
                textNum = op(textNum);
                if (textNum != sliderValue)
                    slider.Value = textNum;
            }
        }


        public static void SyncSliderToText(Slider slider, TextBox text, IntFunction op = null)
        {
            int sliderValue = (int)slider.Value;
            int textNum;
            if (int.TryParse(text.Text, out textNum))
            {
                if (sliderValue != textNum)
                    text.Text = sliderValue.ToString();
            }
            else
            {
                text.Text = sliderValue.ToString();
            }
        }

        public static string DictToString<K,V>(Dictionary<K,V> map)
        {
            StringBuilder sb = new StringBuilder("{\n");
            foreach(var entry in map)
            {
                sb.Append(entry.Key).Append(",").Append(entry.Value).Append("\n");
            }
            sb.Append("}");

            return sb.ToString();
        }

        public static SerializablePair<TKey, TValue> ToSerializablePair<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
        {
            return new SerializablePair<TKey, TValue> { Key = pair.Key, Value = pair.Value };
        }
    }
}
