using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using SAPbouiCOM.Framework;

namespace GtexExtremlyBigDocumentCreation
{
    [FormAttribute("GtexExtremlyBigDocumentCreation.ReserveInvoices", "ReserveInvoices.b1f")]
    class ReserveInvoices : UserFormBase
    {
        private readonly IRefresheshable _form;

        public ReserveInvoices(IRefresheshable form)
        {
            _form = form;
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("Item_0").Specific));
            this.Grid0.DoubleClickAfter += new SAPbouiCOM._IGridEvents_DoubleClickAfterEventHandler(this.Grid0_DoubleClickAfter);
            this.Grid0.ClickAfter += new SAPbouiCOM._IGridEvents_ClickAfterEventHandler(this.Grid0_ClickAfter);
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_1").Specific));
            this.Button0.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.Button0_PressedAfter);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
          
        }

        private SAPbouiCOM.Grid Grid0;

        private void OnCustomInitialize()
        {
            Grid0.DataTable.ExecuteQuery(DiManager.QueryHanaTransalte("SELECT DocEntry, DocNum, CardCode, Comments, DocDate FROM OPCH WHERE isIns = 'Y'"));
            Grid0.Item.Enabled = false;
        }

        private void Grid0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.Row == -1)
            {
                return;
            }
            Grid0.Rows.SelectedRows.Clear();
            Grid0.Rows.SelectedRows.Add(pVal.Row);
        }

        private SAPbouiCOM.Button Button0;

        private void Button0_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            Choose();
        }

        private void Choose()
        {
            if (Grid0.Rows.SelectedRows.Count < 1)
            {
                return;
            }

            var selectedRow = Grid0.Rows.SelectedRows;
            var z = selectedRow.Item(0, BoOrderType.ot_RowOrder);
            var docEntry = Grid0.DataTable.GetValue("DocEntry", z).ToString();
            var docNum = Grid0.DataTable.GetValue("DocNum", z).ToString();

            _form.RefreshClf(docEntry, docNum);

            SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm.Close();
        }

        private void Grid0_DoubleClickAfter(object sboObject, SBOItemEventArg pVal)
        {
            Choose();
        }
    }
}
