using System.ComponentModel;

namespace TimeManagementUtility
{
    public partial class Form1 : Form
    {
        private int _ticks;
        private int _maxTicks;
        private bool _isTimerRunning = false;
        BindingList<TimedTask> _items;

        public Form1()
        {
            InitializeComponent();
            _items = new BindingList<TimedTask>();
            listBox1.DataSource = _items;
            listBox1.DisplayMember = "DisplayValue";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Break");
            endBtn.Enabled = false;
            extendBtn.Enabled = false;
        }

        private void startTimer()
        {
            _ticks = 0;
            _maxTicks = 60 * Decimal.ToInt32(numericUpDown1.Value);
            setLabels();
            progressBar1.Maximum = _maxTicks;
            progressBar1.Value = _maxTicks;

            startBtn.Text = "Pause";
            progressBar1.Enabled = true;
            endBtn.Enabled = true;
            extendBtn.Enabled = true;

            timer.Start();
            _isTimerRunning = true;
            return;
        }

        private void clearTimer()
        {
            _ticks = 0;
            label1.Text = $"00:00";
            this.Text = "Finished";
            startBtn.Text = "Start";
            progressBar1.Value = 0;
            progressBar1.Enabled = false;
            endBtn.Enabled = false;
            extendBtn.Enabled = false;
            startBtn.Enabled = true;

            timer.Stop();
            _isTimerRunning = false;
            return;
        }

        private void togglePause(bool isRunning)
        {
            if (isRunning)
            {
                startBtn.Text = "Resume";

                timer.Stop();
                _isTimerRunning = false;
                return;
            }

            startBtn.Text = "Pause";

            timer.Start();
            _isTimerRunning = true;
            return;
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            if (!_isTimerRunning && _ticks == 0)
            {
                startTimer();
            }
            else
            {
                togglePause(_isTimerRunning);
            }
        }

        private static string elapsedMinutes(int ticks)
        {
            int minutes = ((ticks) / 60);
            return minutes == 60 ? "00" : (minutes < 10 ? $"0{minutes}" : minutes.ToString());
        }

        private static string elapsedSeconds(int ticks)
        {
            int seconds = (ticks % 60);
            return seconds == 60 ? "00" : (seconds < 10 ? $"0{seconds}" : seconds.ToString());
        }

        private string remainingMinutes()
        {
            int minutes = ((_maxTicks - _ticks) / 60);
            return minutes == 60 ? "00" : (minutes < 10 ? $"0{minutes}" : minutes.ToString());
        }

        private string remainingSeconds()
        {
            int seconds = (60 - (_ticks % 60));
            return seconds == 60 ? "00" : (seconds < 10 ? $"0{seconds}" : seconds.ToString());
        }

        private void setLabels()
        {

            string minutesText = remainingMinutes();
            string secondsText = remainingSeconds();
            label1.Text = $"{minutesText}:{secondsText}";
            this.Text = $"{minutesText}:{secondsText}";
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _ticks++;
            progressBar1.Value--;
            setLabels();

            if (_ticks == _maxTicks)
            {
                timer.Stop();
                _isTimerRunning = false;
                startBtn.Enabled = false;
            }
        }

        private void endBtn_Click(object sender, EventArgs e)
        {
            TimedTask timedTask = null;
            if (comboBox1.SelectedItem == null)
            {
                int taskIndex = comboBox1.Items.Add(comboBox1.Text);
                _items.Add(new TimedTask { Key = taskIndex, Ticks = _ticks, Name = comboBox1.Text });
                comboBox1.SelectedItem = comboBox1.Items[taskIndex];
            }
            else
            {
                timedTask = _items.FirstOrDefault(item => item.Key == comboBox1.SelectedIndex);
                if (timedTask != null)
                {
                    int index = _items.IndexOf(timedTask);
                    _items.RemoveAt(index);
                    _items.Add(TimedTask.CloneAndUpdate(timedTask, _ticks));
                } 
                else
                {
                    _items.Add(new TimedTask { Key = comboBox1.SelectedIndex, Ticks = _ticks, Name = comboBox1.Text });
                }
            }

            listBox1.Update();
            clearTimer();
        }

        private void extendBtn_Click(object sender, EventArgs e)
        {
            _maxTicks = _maxTicks + (60 * Decimal.ToInt32(numericUpDown1.Value));
            progressBar1.Maximum = _maxTicks;
            progressBar1.Value = _maxTicks - _ticks;
            if (!_isTimerRunning)
            {
                timer.Start();
                _isTimerRunning = true;
                startBtn.Enabled = true;
            }
        }

        private class TimedTask
        {
            public int Key { get; set; }
            public string Name { get; set; }
            public int Ticks { get; set; }
            public string DisplayValue 
            {
                get { return $"{elapsedMinutes(Ticks)}:{elapsedSeconds(Ticks)} : {Name}"; }
            }

            public static TimedTask CloneAndUpdate(TimedTask oldTimedTask, int ticks)
            {
                TimedTask timedTask = new TimedTask();
                timedTask.Key = oldTimedTask.Key;
                timedTask.Name = oldTimedTask.Name;
                timedTask.Ticks = oldTimedTask.Ticks + ticks;
                return timedTask;
            }
        }
    }
}