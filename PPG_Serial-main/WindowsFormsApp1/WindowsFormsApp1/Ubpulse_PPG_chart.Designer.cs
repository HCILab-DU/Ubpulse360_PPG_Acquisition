namespace WindowsFormsApp1
{
    partial class Ubpulse_PPG_chart
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.PPG_rawdata = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.PPG_rawdata)).BeginInit();
            this.SuspendLayout();
            // 
            // PPG_rawdata
            // 
            chartArea1.Name = "ChartArea1";
            this.PPG_rawdata.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.PPG_rawdata.Legends.Add(legend1);
            this.PPG_rawdata.Location = new System.Drawing.Point(12, 116);
            this.PPG_rawdata.Name = "PPG_rawdata";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.PPG_rawdata.Series.Add(series1);
            this.PPG_rawdata.Size = new System.Drawing.Size(524, 322);
            this.PPG_rawdata.TabIndex = 0;
            this.PPG_rawdata.Text = "chart1";
            // 
            // Ubpulse_PPG_chart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.PPG_rawdata);
            this.Name = "Ubpulse_PPG_chart";
            this.Text = "Ubpulse_PPG_chart";
            ((System.ComponentModel.ISupportInitialize)(this.PPG_rawdata)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart PPG_rawdata;
    }
}