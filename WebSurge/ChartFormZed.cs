using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace WebSurge
{
    public partial class ChartFormZed : Form
    {
        private string Url;
        private IEnumerable<HttpRequestData> Results;
        private ChartTypes ChartType;
        public new Form ParentForm;
        public GraphSettings graphSettings;

        public ChartFormZed(IEnumerable<HttpRequestData> data, string url = null, ChartTypes chartType = ChartTypes.TimeTakenPerRequest, Form parentForm = null)
        {
            InitializeComponent();

            Url = url;
            Results = data;
            ChartType = chartType;
            ParentForm = null;

            Text = "WebSurge - Request Times Taken";
        }

        bool _isFirstPaint = true;
        protected override void OnPaint(PaintEventArgs e)
        {            
            if (!_isFirstPaint)
                return;

            _isFirstPaint = false;
            RenderChart();
        }

        private void RequestTimeTakenChart_Load(object sender, EventArgs e)
        {
            // moved to OnPaint() for better perceived performance
        }

        private void RenderChart()
        {
            if (ParentForm != null)
                ParentForm.Cursor = Cursors.WaitCursor;

            Cursor = Cursors.WaitCursor;

            if (ChartType == ChartTypes.TimeTakenPerRequest)
                RenderTimeTaken();
            else if (ChartType == ChartTypes.RequestsPerSecond)
                RenderRequestsPerSecond();
            else if (ChartType == ChartTypes.ResponseTimeDistribution)
                graphSettings = new DistributionGraphSettings();
                RenderResponseTimeDistribution(graphSettings as DistributionGraphSettings);
            if (ParentForm != null)
                ParentForm.Cursor = Cursors.Default;
        }

        private void RenderRequestsPerSecond()
        {
            ClearSeries();

            var parser = new ResultsParser();
            var reqs = parser.RequestsPerSecond(Results);

            var pane = Chart.GraphPane;

            pane.Title.Text = "Requests per Second";
            pane.Title.FontSpec.FontColor = Color.DarkBlue;
            pane.Title.FontSpec.Size = 14.25F;
            pane.Title.FontSpec.IsBold = true;


            pane.LineType = LineType.Normal;
            pane.XAxis.Title.Text = "second of test";
            pane.YAxis.Title.Text = "requests / second";
            pane.Chart.Fill = new Fill(Color.LightYellow, Color.PaleGoldenrod, 45.0F);
            Chart.IsShowPointValues = true;
            Chart.GraphPane.LineType = LineType.Normal;
            PointPairList series = new PointPairList();
                                   
            foreach (var req in reqs)
            {
                var point = new PointPair(req.Second, req.Requests);                
                series.Add(point);
            }

            var curve = pane.AddCurve("",series,Color.Green);
            
            curve.Line.Width = 4.0F;
            curve.Line.IsAntiAlias = true;
            curve.Line.Fill = new Fill(Color.White, Color.Green, 45F);
            curve.Symbol.Fill = new Fill(Color.LightYellow);
            curve.Symbol.Size = 4;

            // activate the cardinal spline smoothing
            curve.Line.IsSmooth = true;
            curve.Line.SmoothTension = 0.5F;

            // Force refresh of chart
            pane.AxisChange();
        }

        protected internal void RenderResponseTimeDistribution(DistributionGraphSettings settings)
        {
            ClearSeries();

            var parser = new ResultsParser();
            IEnumerable<DistributionResult> results = parser.TimeTakenDistribution(Results, settings.BinSizeMilliseconds, settings.ShowStats, 
                settings.MinX, settings.MaxX);

            var pane = Chart.GraphPane;

            pane.Title.Text = settings.Title;
            pane.Title.FontSpec.FontColor = Color.DarkBlue;
            pane.Title.FontSpec.Size = 14.25F;
            pane.Title.FontSpec.IsBold = true;

            pane.LineType = LineType.Normal;
            pane.XAxis.Title.Text = "Milli-seconds";
            pane.YAxis.Title.Text = "Occurrences";
            pane.Chart.Fill = new Fill(Color.LightYellow, Color.PaleGoldenrod, 45.0F);
            Chart.IsShowPointValues = true;

            int curveCount = 0;
            Color[] colorArray = { Color.Green, Color.Red, Color.Blue, Color.Black, Color.Yellow };

            foreach (DistributionResult result in results)
            {
                PointPairList pointsList;
                if(settings.BinSizeMilliseconds==1)
                {
                    pointsList = new PointPairList(
                    (from t in result.SegmentList
                     select Convert.ToDouble(t.Key)).ToArray(),
                    (from t in result.SegmentList
                     select Convert.ToDouble(t.Value)).ToArray()
                    );
                }
                else
                {
                    pointsList = new PointPairList(
                    (from t in result.SegmentList
                     select Convert.ToDouble(t.Key + (settings.BinSizeMilliseconds / 2))).ToArray(),
                    (from t in result.SegmentList
                     select Convert.ToDouble(t.Value)).ToArray()
                    );
                }
                    

                var curve = pane.AddCurve(string.Format("{0} ({1})", result.Url, result.HttpVerb),
                    pointsList, colorArray[curveCount], SymbolType.Circle);
                curve.Line.Width = 2.0F;
                curve.Line.IsSmooth = settings.IsSmooth;
                curve.Line.SmoothTension = settings.SmoothTension;
                curve.Line.IsAntiAlias = true;
                curve.Symbol.Size = 4;

                if (curveCount < (colorArray.Length - 1) )
                    curveCount++;
                else
                    curveCount = 0;
            }

            pane.AxisChange();
            Chart.Invalidate();
            Chart.Refresh();
        }

        public void RenderTimeTaken()
        {
            ClearSeries();

            var parser = new ResultsParser();            
            var times = parser.TimeLineDataForIndividualRequest(Results,Url);

            var pane = Chart.GraphPane;

            pane.Title.Text = "Time taken per request for\r\n" + Url;
            pane.Title.FontSpec.FontColor = Color.DarkBlue;
            pane.Title.FontSpec.Size = 14.25F;
            pane.Title.FontSpec.IsBold = true;
            
            
            pane.LineType = LineType.Normal;
            pane.XAxis.Title.Text = "Request number";
            pane.YAxis.Title.Text = "milli-seconds";
            pane.Chart.Fill = new Fill(Color.LightYellow, Color.PaleGoldenrod, 45.0F);
            Chart.IsShowPointValues = true;

            PointPairList series = new PointPairList();
            

            foreach (var req in times.Where( t=> !t.IsError ))
            {
                var pt = new PointPair(req.RequestNo, req.TimeTaken,req.TimeTaken.ToString());                
                series.Add(pt);
            }

            var curve = pane.AddCurve("Success", series, Color.Green, SymbolType.Circle);
            curve.Line.Width = 2.0F;
            curve.Line.IsAntiAlias = true;
            curve.Symbol.Fill = new Fill(Color.LightYellow);
            curve.Symbol.Size = 4;
            
            PointPairList series2 = new PointPairList();


            foreach (var req in times.Where( t=> t.IsError ))            
            {
                var point = new PointPair(req.TimeTaken,req.RequestNo);
                series2.Add(point);
            }

            var curve2 = pane.AddCurve("Errors", series2, Color.Red);
            curve2.Line.Width = 2.0F;
            curve2.Line.IsAntiAlias = true;
            curve2.Symbol.Fill = new Fill(Color.White);
            curve2.Symbol.Size = 4;
            
            // Force refresh of chart
            pane.AxisChange();
        }


        void ClearSeries()
        {
            Chart.GraphPane.CurveList.Clear();
            //for (int i = Chart.GraphPane.CurveList.Count -1 ; i > -1; i--)
            //{
            //    Chart.GraphPane.CurveList.RemoveAt(i);
            //}
        }

        private void DistributionGraphOptionsClick(object sender, EventArgs e)
        {
            DistributionGraphOptionsForm settingsForm = new DistributionGraphOptionsForm(this, graphSettings as DistributionGraphSettings);
            settingsForm.ShowDialog();
            //RenderResponseTimeDistribution(10, 0, 4000, 0, 5000, false, string.Empty, true, 0.5f);
        }

        private void Chart_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            //Add the extra menu entries that are specific to each graph
            if(ChartType== ChartTypes.ResponseTimeDistribution)
            {
                ToolStripMenuItem distrGraphOptionsItem = new ToolStripMenuItem();
                distrGraphOptionsItem.Name = "distrGraphOptionsItem";
                distrGraphOptionsItem.Tag = "distrGraphOptionsItem";
                distrGraphOptionsItem.Text = "Change Distribution Graph Options";
                ContextMenu customItemsMenu = new ContextMenu();
                distrGraphOptionsItem.Click += new System.EventHandler(DistributionGraphOptionsClick);
                menuStrip.Items.Add(distrGraphOptionsItem);
            }
        }
    }

    public enum ChartTypes
    {
        TimeTakenPerRequest,
        RequestsPerSecond,
        RequestsPerSecondPerUrl,
        ResponseTimeDistribution
    }

    public class GraphSettings
    {
        [Description("The title that appears at the top of the chart.")]
        [Category("Graph Header/Footer")]
        public string Title { get; set; }

        [DisplayName("Minimum X-axis value")]
        [Description("The minimum value of the X-Axis")]
        public int MinX { get; set; }

        [DisplayName("Maximum X-axis value")]
        [Description("The maximum value of the X-Axis")]
        public int MaxX { get; set; }

        //[DisplayName("Minimum Y-axis value")]
        //[Description("The minimum value of the Y-Axis")]
        //public int MinY { get; set; }

        //[DisplayName("Maximum Y-axis value")]
        //[Description("The maximum value of the Y-Axis")]
        //public int? MaxY { get; set; }

        [DisplayName("Smoothing Enabled")]
        [Description("Defines if any smoothing is applied to the resulting line chart")]
        public bool IsSmooth { get; set; }

        [DisplayName("Smoothing Tension")]
        [Description("Defines the smooth tension factor applied to the line chart (value should be between 0 and 1)")]
        public float SmoothTension { get; set; }

        public GraphSettings()
        {
            Title = string.Empty;
            MinX = 0;
            MaxX = 2147483647;
            //MinY = 0;
            //MaxY = 2147483647;
            IsSmooth = false;
            SmoothTension = 0f;
        }
    }

    public class DistributionGraphSettings : GraphSettings
    {
        [DisplayName("Bin Size (ms)")]
        [DescriptionAttribute("The class size (in milliseconds) to be used for grouping response times ")]
        [Category("Distribution Graph Settings")]
        public int BinSizeMilliseconds { get; set; }

        [DisplayName("Show statistics")]
        [Description("Display statistical data (average, median, variance etc.) at the footer of the graph")]
        [Category("Graph Header/Footer")]
        public bool ShowStats { get; set; }

        public DistributionGraphSettings() : base()
        {
            base.Title = "Distribution of response times";
            BinSizeMilliseconds = 1;
            ShowStats = false;
            IsSmooth = true;
            SmoothTension = 0.3f;
            BinSizeMilliseconds = 10;
        }
    }

}
