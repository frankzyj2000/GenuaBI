using System.Web;
using System.Web.Optimization;

namespace GenuinaBI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/plugins/jQuery/jquery-2.1.4.js",
                        "~/Scripts/jquery.serialize-object.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/plugins/jQueryUI/jquery-ui-1.10.3.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js"));
                        //"~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/contextMenu").Include(
                        "~/Scripts/plugins/contextMenu/jquery.contextMenu.js"));
                        //"~/Scripts/plugins/contextMenu/jquery.ui.position.js"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                        "~/Scripts/plugins/toastr/toastr.js"));

            bundles.Add(new ScriptBundle("~/bundles/wysihtml5").Include(
                        "~/Scripts/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryValidate").Include(
                        "~/Scripts/plugins/jqueryValidate/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/accounting").Include(
                        "~/Scripts/plugins/accounting/accounting.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                        "~/Scripts/plugins/select2/select2.full.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/inputmask").Include(
                        "~/Scripts/plugins/input-mask/jquery.inputmask.js",
                        "~/Scripts/plugins/input-mask/jquery.inputmask.date.extensions.js",
                        "~/Scripts/plugins/input-mask/jquery.inputmask.extensions.js"));

            bundles.Add(new ScriptBundle("~/bundles/fastclick").Include(
                        "~/Scripts/plugins/fastclick/fastclick.js")); //fastclick.min.js has issues with IE 11

            bundles.Add(new ScriptBundle("~/bundles/slimscroll").Include(
                        "~/Scripts/plugins/slimScroll/jquery.slimscroll.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jvectormap").Include(
                        "~/Scripts/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                        "~/Scripts/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"));

            bundles.Add(new ScriptBundle("~/bundles/daterangepicker").Include(
                        "~/Scripts/plugins/moment/moment.js",
                        "~/Scripts/plugins/daterangepicker/daterangepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                        "~/Scripts/plugins/datepicker/bootstrap-datepicker.js"));

            bundles.Add(new ScriptBundle("~/bundles/colorpicker").Include(
                        "~/Scripts/plugins/colorpicker/bootstrap-colorpicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/timepicker").Include(
                        "~/Scripts/plugins/timepicker/bootstrap-timepicker.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/icheck").Include(
                        "~/Scripts/plugins/iCheck/icheck.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/fullcalendar").Include(
                        "~/Scripts/plugins/fullcalendar/fullcalendar.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                        "~/Scripts/plugins/datatables/JSZip-2.5.0/jszip.min.js",
                        "~/Scripts/plugins/datatables/pdfmake-0.1.18/build/pdfmake.js",
                        "~/Scripts/plugins/datatables/pdfmake-0.1.18/build/vfs_fonts.js",
                        "~/Scripts/plugins/datatables/DataTables-1.10.10/js/jquery.dataTables.js",
                        "~/Scripts/plugins/datatables/DataTables-1.10.10/js/dataTables.bootstrap.js",
                        "~/Scripts/plugins/datatables/Buttons-1.1.1/js/dataTables.buttons.js",
                        "~/Scripts/plugins/datatables/Buttons-1.1.1/js/buttons.bootstrap.js",
                        "~/Scripts/plugins/datatables/Buttons-1.1.1/js/buttons.colVis.js",
                        "~/Scripts/plugins/datatables/Buttons-1.1.1/js/buttons.flash.js",
                        "~/Scripts/plugins/datatables/Buttons-1.1.1/js/buttons.html5.js",
                        "~/Scripts/plugins/datatables/Buttons-1.1.1/js/buttons.print.js",
                        "~/Scripts/plugins/datatables/Responsive-2.0.0/js/dataTables.responsive.js"));

            bundles.Add(new ScriptBundle("~/bundles/knob").Include(
                        "~/Scripts/plugins/knob/jquery.knob.js"));

            bundles.Add(new ScriptBundle("~/bundles/sparkline").Include(
                        "~/Scripts/plugins/sparkline/jquery.sparkline.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/morris").Include(
                        "~/Scripts/plugins/morris/raphael-min.js",
                        "~/Scripts/plugins/morris/morris.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/app").Include(
            //            "~/Scripts/app.js")); //must use app.js instead of app.min.js as GenuinaBI has changed it

            //bundles.Add(new ScriptBundle("~/bundles/gbService").Include(
            //            "~/Scripts/pages/gbService.js")); //gbService is GenuinaBI javascript tools shared for all pages

            bundles.Add(new ScriptBundle("~/bundles/demo").Include(
                        "~/Scripts/demo.js"));

            bundles.Add(new ScriptBundle("~/bundles/dashboard2").Include(
                        "~/Scripts/pages/dashboard2.js"));

            bundles.Add(new ScriptBundle("~/bundles/dashboard").Include(
                        "~/Scripts/pages/dashboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/document").Include(
            "~/Scripts/pages/documentation.js"));

            bundles.Add(new ScriptBundle("~/bundles/chartjs").Include(
                        "~/Scripts/plugins/chartjs/Chart.js"));

            bundles.Add(new ScriptBundle("~/bundles/ioslider").Include(
                        "~/Scripts/plugins/ionslider/ion.rangeSlider.min.js")); /* Ion Slider */

            bundles.Add(new ScriptBundle("~/bundles/slider").Include(
                        "~/Scripts/plugins/bootstrap-slider/bootstrap-slider.js")); /* Bootstrap slider */

            bundles.Add(new ScriptBundle("~/bundles/pace").Include(
                        "~/Scripts/plugins/pace/pace.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/simon").Include(
                        "~/Scripts/plugins/simontabor/toggles.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jsPDF").Include(
            //            "~/Scripts/plugins/jsPDF/jspdf.js"));

            bundles.Add(new ScriptBundle("~/bundles/flotchart").Include(
                        "~/Scripts/plugins/flot/jquery.flot.js", /* Flot Chart*/
                        "~/Scripts/plugins/flot/jquery.flot.resize.min.js", /* Flot Resize Plugin - allows the chart to redraw when the window is resized*/
                        "~/Scripts/plugins/flot/jquery.flot.pie.min.js",    /* Flot Pie Plugin - also used to draw donut charts*/
                        "~/Scripts/plugins/flot/jquery.flot.categories.min.js")); /* Flot Categories Plugin - Used to draw bar charts*/

            /*****************************************************************************
             * Adding Styles
             *****************************************************************************/
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/toastr_css").Include(
                      "~/Scripts/plugins/toastr/toastr.css"));

            bundles.Add(new StyleBundle("~/Content/contextMenu_css").Include(
                      "~/Scripts/plugins/contextMenu/jquery.contextMenu.css"));
            
            bundles.Add(new StyleBundle("~/Content/morris_css").Include(
                      "~/Scripts/plugins/morris/morris.css"));

            bundles.Add(new StyleBundle("~/Content/jvectormap_css").Include(
                      "~/Scripts/plugins/jvectormap/jquery-jvectormap-1.2.2.css"));

            bundles.Add(new StyleBundle("~/Content/daterangepicker_css").Include(
                      "~/Scripts/plugins/daterangepicker/daterangepicker-bs3.css"));

            bundles.Add(new StyleBundle("~/Content/datatables_css").Include(
                      "~/Scripts/plugins/datatables/DataTables-1.10.10/css/dataTables.bootstrap.css",
                      "~/Scripts/plugins/datatables/Buttons-1.1.1/css/buttons.bootstrap.css",
                      "~/Scripts/plugins/datatables/Responsive-2.0.0/css/responsive.bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/icheck_css").Include(
                      "~/Scripts/plugins/iCheck/all.css",
                      "~/Scripts/plugins/iCheck/flat/blue.css"));

            bundles.Add(new StyleBundle("~/Content/fullcalendar_css").Include(
                      "~/Scripts/plugins/fullcalendar/fullcalendar.min.css"));

            bundles.Add(new StyleBundle("~/Content/datepicker_css").Include(
                      "~/Scripts/plugins/datepicker/datepicker3.css"));

            bundles.Add(new StyleBundle("~/Content/daterangepicker_css").Include(
                      "~/Scripts/plugins/daterangepicker/daterangepicker.css"));

            bundles.Add(new StyleBundle("~/Content/colorpicker_css").Include(
                      "~/Scripts/plugins/colorpicker/bootstrap-colorpicker.min.css"));

            bundles.Add(new StyleBundle("~/Content/timepicker_css").Include(
                      "~/Scripts/plugins/timepicker/bootstrap-timepicker.min.css"));

            bundles.Add(new StyleBundle("~/Content/select2_css").Include(
                      "~/Scripts/plugins/select2/select2.min.css"));

            bundles.Add(new StyleBundle("~/Content/wysihtml5_css").Include(
                      "~/Scripts/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css"));

            bundles.Add(new StyleBundle("~/Content/ionslider_css").Include(
                      "~/Scripts/plugins/ionslider/ion.rangeSlider.css", /*Ion Range Slider*/
                      "~/Scripts/plugins/ionslider/ion.rangeSlider.skinNice.css")); /* ion slider Nice */

            bundles.Add(new StyleBundle("~/Content/slider_css").Include(
                      "~/Scripts/plugins/bootstrap-slider/slider.css"));

            bundles.Add(new StyleBundle("~/Content/simon_css").Include(
                      "~/Scripts/plugins/simontabor/css/toggles.css",
                      "~/Scripts/plugins/simontabor/css/themes/toggles-light.css"));

            bundles.Add(new StyleBundle("~/Content/document_css").Include(
                      "~/Content/documentation.css"));
            /*****************************************************************************
             * TODO: AdminLTE Skins. Choose a skin from the css/skins 
             *       folder instead of downloading all of them to reduce the load.
             *****************************************************************************/
            bundles.Add(new StyleBundle("~/Content/app_css").Include(
                      "~/Content/AdminLTE.css",
                      "~/Content/skins/_all-skins.css")); 
        }
    }
}
