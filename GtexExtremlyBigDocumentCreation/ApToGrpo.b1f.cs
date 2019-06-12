using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using SAPbobsCOM;
using SAPbouiCOM;
using SAPbouiCOM.Framework;
using Application = SAPbouiCOM.Framework.Application;

namespace GtexExtremlyBigDocumentCreation
{
    [FormAttribute("GtexExtremlyBigDocumentCreation.ApToGrpo", "ApToGrpo.b1f")]
    class ApToGrpo : UserFormBase, IRefresheshable
    {
        public ApToGrpo()
        {
        }


        private Form grpoForm;
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_0").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("Item_2").Specific));
            this.EditText0.ChooseFromListBefore += new SAPbouiCOM._IEditTextEvents_ChooseFromListBeforeEventHandler(this.EditText0_ChooseFromListBefore);
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_3").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("Item_4").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_5").Specific));
            this.Button0.PressedAfter += new SAPbouiCOM._IButtonEvents_PressedAfterEventHandler(this.Button0_PressedAfter);
            this.OnCustomInitialize();

        }

        public void RefreshClf(string docEntry, string docNum)
        {
            grpoForm.DataSources.UserDataSources.Item("UD_0").Value = docEntry;
            grpoForm.DataSources.UserDataSources.Item("UD_0").ValueEx = docNum;
        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.VisibleAfter += new VisibleAfterHandler(this.Form_VisibleAfter);

        }

        private SAPbouiCOM.StaticText StaticText0;

        private void OnCustomInitialize()
        {

        }

        private SAPbouiCOM.EditText EditText0;

        private void EditText0_ChooseFromListBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = false;
            ReserveInvoices reserve = new ReserveInvoices(this);
            reserve.Show();
        }

        private void Form_VisibleAfter(SBOItemEventArg pVal)
        {
            grpoForm = SAPbouiCOM.Framework.Application.SBO_Application.Forms.ActiveForm;
        }

        private StaticText StaticText2;
        private EditText EditText1;
        private Button Button0;

        private void Button0_PressedAfter(object sboObject, SBOItemEventArg pVal)
        {
            if (string.IsNullOrEmpty(EditText0.Value) || string.IsNullOrWhiteSpace(EditText1.Value))
            {
                Application.SBO_Application.SetStatusBarMessage("შეავსეთ ველები",
                    BoMessageTime.bmt_Short, true);
            }
            Documents invoice = (Documents)DiManager.Company.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);
            invoice.GetByKey(int.Parse(EditText0.Value));

            Documents goodsReceiptPo = (Documents)DiManager.Company.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
            goodsReceiptPo.DocDate = DateTime.ParseExact(EditText1.Value, "yyyyMMdd",CultureInfo.InvariantCulture);

            for (int i = 0; i < invoice.Lines.Count; i++)
            {
                invoice.Lines.SetCurrentLine(i);
                goodsReceiptPo.Lines.BaseEntry = invoice.DocEntry;
                goodsReceiptPo.Lines.BaseLine = invoice.Lines.LineNum;
                goodsReceiptPo.Lines.BaseType = 18;
                goodsReceiptPo.CardCode = invoice.CardCode;
                goodsReceiptPo.Lines.Add();
            }

            var res = goodsReceiptPo.Add();
            if (res != 0)
            {
                Application.SBO_Application.SetStatusBarMessage(DiManager.Company.GetLastErrorDescription(),
                    BoMessageTime.bmt_Short, true);
            }
            else
            {
                var docentry = DiManager.Company.GetNewObjectKey();
                SAPbouiCOM.Framework.Application.SBO_Application.OpenForm(BoFormObjectEnum.fo_GoodsReceiptPO, "",
                    docentry);
            }
        }
    }
}