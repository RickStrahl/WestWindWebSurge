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
            for (int i = Chart.GraphPane.CurveList.Count -1 ; i > -1; i--)
            {
                Chart.GraphPane.CurveList.RemoveAt(i);
            }
        }

        private void Chart_Load(object sender, EventArgs e)
        {

        }
       
    }

    public enum ChartTypes
    {
        TimeTakenPerRequest,
        RequestsPerSecond,
        RequestsPerSecondPerUrl
    }


}
