using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraCharts;

namespace WebSurge
{
    public partial class ChartForm : Form
    {
        private string Url;
        private IEnumerable<HttpRequestData> Results;
        private ChartTypes ChartType;

        public ChartForm(IEnumerable<HttpRequestData> data, string url = null, ChartTypes chartType = ChartTypes.TimeTakenPerRequest)
        {
            InitializeComponent();

            Url = url;
            Results = data;
            ChartType = chartType;

            Text = "WebSurge - Request Times Taken";
        }

        private void RequestTimeTakenChart_Load(object sender, EventArgs e)
        {            
            if (ChartType == ChartTypes.TimeTakenPerRequest)
                RenderTimeTaken();
            else if (ChartType == ChartTypes.RequestsPerSecond)
                RenderRequestsPerSecond();
        }

        private void RenderRequestsPerSecond()
        {
            ClearSeries();

            var parser = new ResultsParser();
            var reqs = parser.RequestsPerSecond(Results);

            var series = new Series();
            series.ChangeView(ViewType.StackedLine);
            series.LegendText = null;
            series.View.Color = Color.Blue;


            foreach (var req in reqs)
            {
                var pt = new SeriesPoint(req.Second.ToString(), new double[] { req.Requests });                
                series.Points.Add(pt);
            }
            
            //Chart.Series.Remove(Chart.Series[0]);
            Chart.Series.Insert(0, series);


            XYDiagram diag = Chart.Diagram as XYDiagram;
            diag.AxisY.Title.Visible = true;
            diag.AxisY.Title.Text = "req/sec";
            diag.AxisX.Title.Visible = true;
            diag.AxisX.Title.Text = "second of test";



            Chart.Titles[0].Text = "Requests per Second over Time of Test Run";
            Chart.Titles[0].TextColor = Color.DarkBlue;
            Chart.Titles[0].Font = new Font(FontFamily.GenericSansSerif, 14.25F, FontStyle.Bold);
        }


        public void RenderTimeTaken()
        {
            ClearSeries();

            var parser = new ResultsParser();            
            var times = parser.TimeLineDataForIndividualRequest(Results,Url);
                               
            var series = new Series();
            series.ChangeView(ViewType.StackedLine);
            series.LegendText = "Success";
            series.View.Color = Color.Green;            


            foreach (var req in times.Where( t=> !t.IsError ))            
            {
                var pt = new SeriesPoint(req.RequestNo.ToString(), new double[] { req.TimeTaken });                
                pt.Tag = req.OrigId;

                series.Points.Add(pt);
            }

            
            //Chart.Series.Remove(Chart.Series[0]);
            Chart.Series.Insert(0,series);            


            var series2 = new Series();
            series2.ChangeView(ViewType.StackedLine);
            series2.LegendText = "Error";
            series2.View.Color = Color.Red;

            foreach (var req in times.Where( t=> t.IsError ))            
            {
                var pt = new SeriesPoint(req.RequestNo.ToString(), new double[] { req.TimeTaken });                
                pt.Tag = req.OrigId;

                series2.Points.Add(pt);
            }
            
            Chart.Series.Insert(0,series2);


            XYDiagram diag = Chart.Diagram as XYDiagram;
            diag.AxisY.Title.Visible = true;
            diag.AxisY.Title.Text = "milli-seconds";
            diag.AxisX.Title.Visible = true;
            diag.AxisX.Title.Text = "Request Number";



            Chart.Titles[0].Text = "Time taken per request for\r\n" + Url;
            Chart.Titles[0].TextColor = Color.DarkBlue;
            Chart.Titles[0].Font = new Font(FontFamily.GenericSansSerif, 14.25F, FontStyle.Bold);

        }


        void ClearSeries()
        {
            for (int i = Chart.Series.Count -1 ; i > -1; i--)
            {
                Chart.Series.RemoveAt(i);
            }
        }
       
    }

    public enum ChartTypes
    {
        TimeTakenPerRequest,
        RequestsPerSecond,
        RequestsPerSecondPerUrl
    }


}
